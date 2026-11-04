using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin;

// Public admin-channel identities only. Never serialize configuration, keys,
// personal profiles or a union of unproven historical library rows.
internal static class YouTubeAdminProvenance
{
    internal const string FileName = "youtube-admin-channel-provenance-v1.json";
    private const int MaxMappings = 1024;
    private const int MaxBytes = 128 * 1024;
    private static readonly object Sync = new();
    private sealed record State(string Revision, Dictionary<string, string> Mappings);
    private static State? _state;
    private sealed class Entry
    {
        public string Handle { get; set; } = "";
        public string ChannelId { get; set; } = "";
    }
    private sealed class Document
    {
        public int Schema { get; set; }
        public string Revision { get; set; } = "";
        public Entry[] Mappings { get; set; } = Array.Empty<Entry>();
    }
    internal static void Reset() => Volatile.Write(ref _state, null);
    private static string Normalize(string term) => term.StartsWith("@", StringComparison.Ordinal) ? term.ToLowerInvariant() : term;
    private static string[] Terms(PluginConfiguration config) => YouTubeChannel.ConfiguredTerms(config.SavedItems)
        .Where(term => YouTubeChannel.IsCanonicalHandle(term) || YouTubeChannel.IsCanonicalChannelId(term))
        .Select(Normalize).Distinct(StringComparer.Ordinal).OrderBy(term => term, StringComparer.Ordinal).ToArray();
    internal static string Revision(PluginConfiguration config) => Convert.ToHexString(SHA256.HashData(
        Encoding.UTF8.GetBytes("admin-channel-identities-v1\n" + string.Join("\n", Terms(config))))).ToLowerInvariant();
    private static bool Current(string revision) => Plugin.Instance?.Options is { } config && Revision(config) == revision;
    private static string? StoragePath => string.IsNullOrWhiteSpace(Plugin.DataPath) ? null : Path.Combine(Plugin.DataPath, FileName);

    internal static bool Allows(string legacyId, PluginConfiguration config)
    {
        if (!legacyId.StartsWith("channel_x_", StringComparison.Ordinal)) return false;
        var state = Volatile.Read(ref _state);
        return state != null && state.Revision == Revision(config)
            && state.Mappings.Values.Contains(legacyId.Substring("channel_x_".Length), StringComparer.Ordinal);
    }

    internal static string? KnownChannel(PluginConfiguration config, string handle)
    {
        var state = Volatile.Read(ref _state);
        return state != null && state.Revision == Revision(config)
            && state.Mappings.TryGetValue(Normalize(handle), out var id) ? id : null;
    }

    internal static string IdentityRevision(PluginConfiguration config)
    {
        var state = Volatile.Read(ref _state);
        var revision = Revision(config);
        return state?.Revision != revision ? revision : Convert.ToHexString(SHA256.HashData(
            JsonSerializer.SerializeToUtf8Bytes(state.Mappings.OrderBy(pair => pair.Key, StringComparer.Ordinal).ToArray()))).ToLowerInvariant();
    }

    internal static void Load(PluginConfiguration config)
    {
        if (YouTubePersonalRequestContext.Current != null) return;
        var revision = Revision(config);
        lock (Sync)
        {
            Volatile.Write(ref _state, null);
            try
            {
                var path = StoragePath;
                if (path == null || !File.Exists(path)) return;
                var info = new FileInfo(path);
                if (info.Length > MaxBytes || (info.Attributes & FileAttributes.ReparsePoint) != 0) return;
                var bytes = File.ReadAllBytes(path);
                if (bytes.Length > MaxBytes) return;
                var document = JsonSerializer.Deserialize<Document>(bytes, new JsonSerializerOptions { MaxDepth = 8 });
                if (document == null || document.Schema != 1 || document.Revision != revision
                    || document.Mappings == null || document.Mappings.Length > MaxMappings) return;
                var terms = Terms(config).ToHashSet(StringComparer.Ordinal);
                var mappings = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var entry in document.Mappings)
                {
                    if (entry == null || entry.Handle == null || entry.Handle.Length > 512
                        || !YouTubeChannel.IsCanonicalHandle(entry.Handle) || Normalize(entry.Handle) != entry.Handle
                        || !terms.Contains(entry.Handle) || entry.ChannelId == null
                        || !YouTubeChannel.IsCanonicalChannelId(entry.ChannelId)
                        || !mappings.TryAdd(entry.Handle, entry.ChannelId)) return;
                }
                if (Current(revision)) Volatile.Write(ref _state, new State(revision, mappings));
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            { YouTubeChannel.LogPublic("[YT] Admin channel identity load unavailable (" + error.GetType().Name + ")."); }
        }
    }

    internal static bool Record(PluginConfiguration snapshot, string handle, string channelId, CancellationToken cancellation = default)
    {
        if (YouTubePersonalRequestContext.Current != null || !YouTubeChannel.IsCanonicalHandle(handle)
            || handle.Length > 512 || !YouTubeChannel.IsCanonicalChannelId(channelId)) return false;
        var revision = Revision(snapshot);
        var normalized = Normalize(handle);
        if (!Terms(snapshot).Contains(normalized, StringComparer.Ordinal)) return false;
        lock (Sync)
        {
            cancellation.ThrowIfCancellationRequested();
            if (!Current(revision)) return false;
            var previous = Volatile.Read(ref _state);
            var mappings = previous?.Revision == revision
                ? new Dictionary<string, string>(previous.Mappings, StringComparer.Ordinal)
                : new Dictionary<string, string>(StringComparer.Ordinal);
            if (!mappings.ContainsKey(normalized) && mappings.Count >= MaxMappings) return false;
            mappings[normalized] = channelId;
            var next = new State(revision, mappings);
            cancellation.ThrowIfCancellationRequested();
            if (!Current(revision)) return false;
            Volatile.Write(ref _state, next);
            Persist(next, cancellation);
            return true;
        }
    }

    private static void Persist(State state, CancellationToken cancellation)
    {
        string? temporary = null;
        try
        {
            var path = StoragePath;
            if (path == null) return;
            if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) return;
            var bytes = JsonSerializer.SerializeToUtf8Bytes(new Document
            {
                Schema = 1,
                Revision = state.Revision,
                Mappings = state.Mappings.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                    .Select(pair => new Entry { Handle = pair.Key, ChannelId = pair.Value }).ToArray()
            });
            if (bytes.Length > MaxBytes) return;
            temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            { stream.Write(bytes); stream.Flush(flushToDisk: true); }
            cancellation.ThrowIfCancellationRequested();
            if (Current(state.Revision)) File.Move(temporary, path, overwrite: true);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception error) when (error is not OutOfMemoryException)
        { YouTubeChannel.LogPublic("[YT] Admin channel identities not persisted (" + error.GetType().Name + ")."); }
        finally
        {
            if (temporary != null)
                try { File.Delete(temporary); }
                catch (Exception error) when (error is not OutOfMemoryException) { }
        }
    }

    internal static Task BootstrapAsync(CancellationToken cancellation = default) => BootstrapAsync(
        async (key, handle, token) => (await YouTubeApi.GetChannelDetailsAsync(key, handle, true, token).ConfigureAwait(false)).id,
        TimeSpan.FromSeconds(30), cancellation);

    internal static async Task BootstrapAsync(Func<string, string, CancellationToken, Task<string?>> resolve,
        TimeSpan timeout, CancellationToken cancellation = default)
    {
        if (YouTubePersonalRequestContext.Current != null || Plugin.Instance?.Options is not { } config) return;
        var snapshot = YouTubeChannelProvenance.Copy(config);
        Load(snapshot);
        var revision = Revision(snapshot);
        var state = Volatile.Read(ref _state);
        var missing = YouTubeChannel.ConfiguredTerms(snapshot.SavedItems).Where(YouTubeChannel.IsCanonicalHandle)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(handle => state?.Revision != revision || !state.Mappings.ContainsKey(Normalize(handle)))
            .Take(128).ToArray();
        if (missing.Length == 0 || string.IsNullOrWhiteSpace(snapshot.ApiKey))
        {
            Report(revision);
            return;
        }
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        deadline.CancelAfter(timeout);
        var token = deadline.Token;
        async Task Resolve(string handle)
        {
            try
            {
                var channelId = await resolve(snapshot.ApiKey, handle, token).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();
                if (channelId != null) Record(snapshot, handle, channelId, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            catch (Exception error) when (error is not OutOfMemoryException)
            { YouTubeChannel.LogPublic("[YT] Admin channel identity resolution unavailable (" + error.GetType().Name + ")."); }
        }
        try { await Task.WhenAll(missing.Select(Resolve)).WaitAsync(token).ConfigureAwait(false); }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        Report(revision);
    }

    private static void Report(string revision)
    {
        var final = Volatile.Read(ref _state);
        var available = final?.Revision == revision ? final.Mappings.Count : 0;
        YouTubeChannel.LogPublic("[YT] Verified admin handle identities available: " + available + "; unresolved configured handles stay hidden.");
    }
}
