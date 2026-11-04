using System.Security.Cryptography;
using System.Text.Json;

namespace Emby.YouTubePlugin;

internal sealed record YouTubeRootLayout(string Revision, string Epoch);

// A task completion is not a materialization witness: Emby can swallow errors.
// Persist only a current provider-root observation that matches actual DB roots.
internal sealed class YouTubeRootMaterialization : IDisposable
{
    internal sealed record RootAttempt(YouTubeRootLayout Layout, long Generation);
    private sealed record Observation(RootAttempt Attempt, string[] Ids);
    private sealed class Stamp
    {
        public int Schema { get; set; }
        public string Revision { get; set; } = "";
        public string RootFingerprint { get; set; } = "";
    }
    private readonly object _sync = new();
    private readonly Func<YouTubeRootLayout?> _capture;
    private readonly Func<CancellationToken, Task<string[]>> _readRoots;
    private readonly Func<CancellationToken, Task<bool>> _refresh;
    private readonly Func<string?> _stampPath;
    private readonly Action<string> _log;
    private readonly Func<YouTubeRootLayout, string[], bool>? _acceptExisting;
    private readonly CancellationTokenSource _lifetime;
    private readonly CancellationToken _token;
    private YouTubeRootLayout? _requested;
    private long _generation, _attempted = -1, _issuedRequest, _appliedRequest, _reconciledRequest = -1;
    private bool _pending, _force, _disposed;
    private Task? _worker;
    private Observation? _observation;

    internal YouTubeRootMaterialization(Func<YouTubeRootLayout?> capture,
        Func<CancellationToken, Task<string[]>> readRoots, Func<CancellationToken, Task<bool>> refresh,
        Func<string?> stampPath, Action<string> log, CancellationToken lifetime = default,
        Func<YouTubeRootLayout, string[], bool>? acceptExisting = null)
    {
        _capture = capture; _readRoots = readRoots; _refresh = refresh;
        _stampPath = stampPath; _log = log; _acceptExisting = acceptExisting;
        _lifetime = CancellationTokenSource.CreateLinkedTokenSource(lifetime);
        _token = _lifetime.Token;
    }

    internal Task RequestAsync(string reason, bool force = false)
    {
        long ticket;
        lock (_sync)
        {
            if (_disposed) return Task.CompletedTask;
            ticket = ++_issuedRequest;
        }
        var current = _capture();
        lock (_sync)
        {
            if (_disposed) return Task.CompletedTask;
            // Capture can block outside this lock. A late older capture must
            // never overwrite a newer policy notification already accepted.
            if (ticket < _appliedRequest) return _worker ?? Task.CompletedTask;
            _appliedRequest = ticket;
            if (current == null)
            {
                _requested = null; _generation++;
                _pending = false; _force = false;
                return _worker ?? Task.CompletedTask;
            }
            if (_requested?.Epoch != current.Epoch)
            {
                _requested = current;
                _generation++;
            }
            _pending = true;
            _force |= force;
            return _worker ??= Task.Run(RunAsync);
        }
    }

    internal async Task<bool> EnsureCurrentAsync(string reason)
    {
        await RequestAsync(reason).ConfigureAwait(false);
        if (_token.IsCancellationRequested) return false;
        var current = _capture();
        if (current == null) return false;
        var actual = await _readRoots(_token).ConfigureAwait(false);
        var confirmed = _capture();
        lock (_sync)
            return !_disposed && !_token.IsCancellationRequested
                && _requested?.Epoch == current.Epoch && confirmed?.Epoch == current.Epoch
                && MatchesStamp(current.Revision, Fingerprint(actual));
    }

    internal RootAttempt? BeginRoot()
    {
        var current = _capture();
        lock (_sync)
            return !_disposed && current != null && _requested?.Epoch == current.Epoch
                ? new RootAttempt(current, _generation) : null;
    }

    internal void ObserveRoot(RootAttempt attempt, IEnumerable<string> expectedIds)
    {
        var current = _capture();
        var ids = expectedIds.Distinct(StringComparer.Ordinal).OrderBy(id => id, StringComparer.Ordinal).ToArray();
        if (ids.Any(id => string.IsNullOrEmpty(id) || id == "msg")) return;
        lock (_sync)
        {
            if (!_disposed && !_token.IsCancellationRequested && current?.Epoch == attempt.Layout.Epoch
                && attempt.Generation == _generation && _requested?.Epoch == current.Epoch)
                _observation = new Observation(attempt, ids);
        }
    }

    private async Task RunAsync()
    {
        while (true)
        {
            YouTubeRootLayout layout;
            long generation;
            bool force;
            lock (_sync)
            {
                if (_disposed || !_pending || _requested == null)
                { _worker = null; return; }
                _pending = false;
                layout = _requested; generation = _generation;
                force = _force; _force = false;
                if (_attempted == generation) continue;
                _attempted = generation;
            }
            try { await ApplyAsync(layout, generation, force).ConfigureAwait(false); }
            catch (OperationCanceledException) when (_token.IsCancellationRequested) { }
            catch (Exception error) when (error is not OutOfMemoryException)
            { _log("[YT] Root materialization not verified (" + error.GetType().Name + "); no success stamp. Retry on a new policy revision or next startup."); }
            finally { Reconcile(generation); }
        }
    }

    private void Reconcile(long completedGeneration)
    {
        if (_token.IsCancellationRequested) return;
        try
        {
            var current = _capture();
            lock (_sync)
            {
                // Admin identity discovery may change the stable revision during
                // the scan itself. Reconcile once per external notification;
                // unstable input must never create an automatic refresh loop.
                if (!_disposed && current != null && _generation == completedGeneration
                    && _requested?.Epoch != current.Epoch && _reconciledRequest != _appliedRequest)
                {
                    _reconciledRequest = _appliedRequest;
                    _requested = current; _generation++; _pending = true;
                }
            }
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        { _log("[YT] Root revision changed while reconciling; awaiting a new notification."); }
    }

    private bool Current(YouTubeRootLayout layout, long generation)
    {
        var current = _capture();
        lock (_sync)
            return !_disposed && !_token.IsCancellationRequested && _generation == generation
                && _requested?.Epoch == layout.Epoch && current?.Epoch == layout.Epoch;
    }

    private async Task ApplyAsync(YouTubeRootLayout layout, long generation, bool force)
    {
        _token.ThrowIfCancellationRequested();
        if (!Current(layout, generation)) return;
        var actual = await _readRoots(_token).ConfigureAwait(false);
        if (!Current(layout, generation)) return;
        if (!force && MatchesStamp(layout.Revision, Fingerprint(actual)))
        { _log("[YT] Root materialization already verified for current policy."); return; }
        bool adopt = !force && StampAbsent() && _acceptExisting?.Invoke(layout, actual) == true;
        if (!Current(layout, generation)) return;
        if (!adopt)
        {
            lock (_sync) _observation = null;
            _log("[YT] Root materialization differs from current policy/format; requesting registered YouTube refresh.");
            if (!await _refresh(_token).ConfigureAwait(false)) return;
            _token.ThrowIfCancellationRequested();
            actual = await _readRoots(_token).ConfigureAwait(false);
            if (!Current(layout, generation)) return;
            Observation? observed;
            lock (_sync) observed = _observation;
            if (observed == null || observed.Attempt.Generation != generation
                || observed.Attempt.Layout.Epoch != layout.Epoch
                || !actual.ToHashSet(StringComparer.Ordinal).SetEquals(observed.Ids))
            { _log("[YT] Root refresh ended without a matching current materialization; no success stamp."); return; }
        }
        else
        {
            // Adoption is a read-only shape proof, not a provider rebuild.
            // Confirm the same committed rows still exist before recording it.
            var confirmed = await _readRoots(_token).ConfigureAwait(false);
            if (!Current(layout, generation) || Fingerprint(confirmed) != Fingerprint(actual)
                || _acceptExisting?.Invoke(layout, confirmed) != true) return;
        }
        var path = _stampPath();
        if (path == null) return;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(new Stamp
        { Schema = 1, Revision = layout.Revision, RootFingerprint = Fingerprint(actual) });
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            { stream.Write(bytes); stream.Flush(flushToDisk: true); }
            if (!Current(layout, generation)) return;
            lock (_sync)
            {
                if (_disposed || _token.IsCancellationRequested || _generation != generation
                    || _requested?.Epoch != layout.Epoch) return;
                if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) return;
                File.Move(temporary, path, overwrite: true);
            }
            _log("[YT] Root materialization verified and recorded for current policy.");
        }
        finally
        {
            try { File.Delete(temporary); }
            catch (Exception error) when (error is not OutOfMemoryException) { }
        }
    }

    private bool StampAbsent()
    {
        try
        {
            var path = _stampPath();
            if (path == null) return false;
            _ = File.GetAttributes(path);
            return false;
        }
        catch (FileNotFoundException) { return true; }
        catch (Exception error) when (error is not OutOfMemoryException) { return false; }
    }

    private bool MatchesStamp(string revision, string fingerprint)
    {
        try
        {
            var path = _stampPath();
            if (path == null || !File.Exists(path)) return false;
            var info = new FileInfo(path);
            if (info.Length > 4096 || (info.Attributes & FileAttributes.ReparsePoint) != 0) return false;
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length > 4096) return false;
            var stamp = JsonSerializer.Deserialize<Stamp>(bytes, new JsonSerializerOptions { MaxDepth = 4 });
            return stamp?.Schema == 1 && stamp.Revision == revision && stamp.RootFingerprint == fingerprint;
        }
        catch (Exception error) when (error is not OutOfMemoryException) { return false; }
    }

    internal static string Fingerprint(IEnumerable<string> ids) => Convert.ToHexString(SHA256.HashData(
        JsonSerializer.SerializeToUtf8Bytes(ids.Distinct(StringComparer.Ordinal).OrderBy(id => id, StringComparer.Ordinal).ToArray()))).ToLowerInvariant();

    public void Dispose()
    {
        Task? worker;
        lock (_sync)
        {
            if (_disposed) return;
            _disposed = true; _pending = false;
            worker = _worker;
        }
        _lifetime.Cancel();
        if (worker == null) _lifetime.Dispose();
        else _ = worker.ContinueWith(_ => _lifetime.Dispose(), CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }
}
