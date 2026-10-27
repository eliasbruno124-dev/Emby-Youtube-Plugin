using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // A server-private transport capability, not an Emby API or a general URL
    // proxy. FFmpeg sees a seekable object; only bounded byte slices are fetched.
    internal static class YouTubeDirectRangeProxy
    {
        private const int ChunkBytes = 4 * 1024 * 1024;
        // Each selectable audio language owns a capability, but only the
        // selected one performs IO. Bound metadata separately from the
        // unchanged request slots and chunk buffers.
        private const int MaximumEntries = 1024;
        private const long MaximumFileBytes = 1L << 40;
        private static readonly object Gate = new();
        private static readonly SemaphoreSlim RequestSlots = new(16, 16);
        private static readonly Regex VideoId = new("\\A[A-Za-z0-9_-]{11}\\z", RegexOptions.CultureInvariant);
        private static readonly Regex FormatId = new("\\A[A-Za-z0-9_.-]{1,80}\\z", RegexOptions.CultureInvariant);
        private static readonly Regex CapabilityPath = new("\\A/media/[0-9a-f]{64}\\z", RegexOptions.CultureInvariant);
        private static Server? _server;
        private static bool _shutdown;

        // Private seams permit linked-source tests to replace only upstream IO;
        // they are not configurable or reachable through any HTTP request.
        private static Func<Uri, IReadOnlyDictionary<string, string>, long, int, long, CancellationToken, Task<byte[]>> _fetchRange =
            YouTubeDirectHttp.GetRangeBytesAsync;
        private static Func<string, CancellationToken, Task<YouTubeResolvedVideo>> _resolve = YouTubeStreamResolver.ResolveAsync;
        private static Action<string> _invalidate = YouTubeStreamResolver.Invalidate;

        internal static Task<string> RegisterAsync(YouTubeResolvedVideo video, JsonElement format,
            IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var snapshot = ValidateSelection(video, format, headers);
            lock (Gate)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_shutdown) throw Failure();
                _server ??= Server.Start();
                return Task.FromResult(_server.Register(snapshot));
            }
        }

        internal static bool IsOwnedUri(string value)
        {
            lock (Gate) return _server?.Owns(value) == true;
        }

        internal static void Shutdown()
        {
            Server? previous;
            lock (Gate) { _shutdown = true; previous = _server; _server = null; }
            previous?.Stop();
        }

        private static Snapshot ValidateSelection(YouTubeResolvedVideo video, JsonElement format,
            IReadOnlyDictionary<string, string> headers)
        {
            if (video == null || !VideoId.IsMatch(video.VideoId ?? "")
                || Text(video.Metadata, "id") != video.VideoId || Text(format, "protocol") != "https")
                throw Failure();
            string id = Text(format, "format_id");
            if (!FormatId.IsMatch(id) || !format.TryGetProperty("filesize", out var size)
                || !size.TryGetInt64(out long length) || length <= 0 || length > MaximumFileBytes)
                throw Failure();
            var uri = YouTubeDirectHttp.ValidateMediaUri(Text(format, "url"));
            long? declaredLength = null;
            long? expiry = null;
            foreach (string pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var pieces = pair.Split('=', 2);
                string name = Uri.UnescapeDataString(pieces[0]);
                if (name == "clen")
                {
                    if (pieces[0] != "clen" || declaredLength.HasValue || pieces.Length != 2
                        || !Decimal(pieces[1], out long value) || pieces[1][0] == '0') throw Failure();
                    declaredLength = value;
                }
                else if (name == "expire")
                {
                    if (pieces[0] != "expire" || expiry.HasValue || pieces.Length != 2 || !Decimal(pieces[1], out long value)) throw Failure();
                    expiry = value;
                }
            }
            if (declaredLength != length) throw Failure();
            if (!video.Metadata.TryGetProperty("formats", out var formats) || formats.ValueKind != JsonValueKind.Array
                || formats.GetArrayLength() > 1000) throw Failure();
            var matches = formats.EnumerateArray().Where(f => Text(f, "format_id") == id).ToArray();
            if (matches.Length != 1 || matches[0].GetRawText() != format.GetRawText()) throw Failure();
            var identity = new Identity(video.VideoId, id, length, Text(format, "vcodec"), Text(format, "acodec"),
                Text(format, "ext"), Text(format, "language"), Integer(format, "width"), Integer(format, "height"),
                Integer(format, "audio_channels"), Integer(format, "asr"));
            if (identity.VideoCodec.Length > 128 || identity.AudioCodec.Length > 128 || identity.Language.Length > 80
                || identity.Extension is not ("mp4" or "m4a" or "webm")
                || (identity.VideoCodec is "" or "none") && (identity.AudioCodec is "" or "none")) throw Failure();
            DateTime expires;
            try { expires = expiry.HasValue ? DateTimeOffset.FromUnixTimeSeconds(expiry.Value).UtcDateTime.AddSeconds(-120) : video.ExpiresUtc; }
            catch (ArgumentOutOfRangeException) { throw Failure(); }
            return new Snapshot(identity, uri, new ReadOnlyDictionary<string, string>(YouTubeDirectHttp.CopyHeaders(headers)), expires);
        }

        private static string Text(JsonElement value, string name) => value.ValueKind == JsonValueKind.Object
            && value.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String ? property.GetString() ?? "" : "";

        private static int? Integer(JsonElement value, string name)
        {
            // yt-dlp explicitly emits null for dimensions on audio-only formats
            // and audio properties on video-only formats. These are unknown,
            // not malformed integers; TryGetInt32 itself throws on JSON null.
            if (!value.TryGetProperty(name, out var property) || property.ValueKind == JsonValueKind.Null) return null;
            if (property.ValueKind != JsonValueKind.Number || !property.TryGetInt32(out int number)) throw Failure();
            return number;
        }

        private static bool Decimal(string value, out long number)
        {
            number = 0;
            return value.Length is > 0 and <= 19 && value.All(c => c is >= '0' and <= '9')
                && long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out number);
        }

        private static bool ParseRange(string? value, long length, out long start, out long end)
        {
            start = 0; end = length - 1;
            if (value == null) return true;
            if (value.Length > 128 || !value.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase) || value.Contains(',')) return false;
            var pieces = value.Substring(6).Split('-');
            if (pieces.Length != 2) return false;
            if (pieces[0].Length == 0)
            {
                if (!Decimal(pieces[1], out long suffix) || suffix == 0) return false;
                start = suffix >= length ? 0 : length - suffix;
                return true;
            }
            if (!Decimal(pieces[0], out start) || start >= length) return false;
            if (pieces[1].Length == 0) return true;
            if (!Decimal(pieces[1], out long requestedEnd) || requestedEnd < start) return false;
            end = Math.Min(requestedEnd, length - 1);
            return true;
        }

        private static async Task<byte[]> ReadChunkAsync(Server server, Entry entry, RequestState request,
            long offset, int count, CancellationToken cancellationToken)
        {
            Snapshot snapshot;
            lock (entry.Sync) snapshot = entry.Current;
            if (snapshot.ExpiresUtc <= DateTime.UtcNow)
            {
                if (request.Renewed) throw Failure();
                request.Renewed = true;
                snapshot = await RenewAsync(server, entry, snapshot, cancellationToken).ConfigureAwait(false);
            }
            try
            {
                return await _fetchRange(snapshot.Uri, snapshot.Headers, offset, count, snapshot.Identity.Length,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (YouTubeDirectHttp.RangeForbiddenException error) when (!request.Renewed && !error.RateLimited)
            {
                request.Renewed = true;
                snapshot = await RenewAsync(server, entry, snapshot, cancellationToken).ConfigureAwait(false);
                return await _fetchRange(snapshot.Uri, snapshot.Headers, offset, count, snapshot.Identity.Length,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<Snapshot> RenewAsync(Server server, Entry entry, Snapshot stale, CancellationToken cancellationToken)
        {
            Task<Snapshot> renewal;
            lock (entry.Sync)
            {
                if (!ReferenceEquals(stale, entry.Current) && entry.Current.ExpiresUtc > DateTime.UtcNow) return entry.Current;
                if (entry.Renewal == null || entry.Renewal.IsCompleted && entry.RenewalRetryAfterUtc <= DateTime.UtcNow)
                {
                    entry.RenewalRetryAfterUtc = DateTime.UtcNow.AddSeconds(15);
                    entry.Renewal = Task.Run(() => RefreshAsync(server, entry, stale));
                    _ = entry.Renewal.ContinueWith(task => { _ = task.Exception; }, CancellationToken.None,
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                }
                renewal = entry.Renewal;
            }
            return await renewal.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Snapshot> RefreshAsync(Server server, Entry entry, Snapshot stale)
        {
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(server.StopToken);
            deadline.CancelAfter(TimeSpan.FromSeconds(95));
            _invalidate(entry.Identity.VideoId);
            var resolved = await _resolve(entry.Identity.VideoId, deadline.Token).WaitAsync(deadline.Token).ConfigureAwait(false);
            if (!resolved.Metadata.TryGetProperty("formats", out var formats) || formats.ValueKind != JsonValueKind.Array) throw Failure();
            var selected = formats.EnumerateArray().Where(f => Text(f, "format_id") == entry.Identity.FormatId).ToArray();
            if (selected.Length != 1) throw Failure();
            var headers = YouTubeDirectHttp.ReadHeaders(resolved.Metadata.TryGetProperty("http_headers", out var global) ? global : default);
            foreach (var header in YouTubeDirectHttp.ReadHeaders(selected[0].TryGetProperty("http_headers", out var local) ? local : default))
                headers[header.Key] = header.Value;
            var refreshed = ValidateSelection(resolved, selected[0], headers);
            if (refreshed.Identity != entry.Identity || refreshed.ExpiresUtc <= DateTime.UtcNow) throw Failure();
            deadline.Token.ThrowIfCancellationRequested();
            lock (entry.Sync)
            {
                if (!ReferenceEquals(entry.Current, stale) && entry.Current.ExpiresUtc >= refreshed.ExpiresUtc
                    && entry.Current.ExpiresUtc > DateTime.UtcNow)
                    return entry.Current;
                entry.Current = refreshed;
                return refreshed;
            }
        }

        private static InvalidOperationException Failure() => new("YouTube local media transport is unavailable.");

        private sealed record Identity(string VideoId, string FormatId, long Length, string VideoCodec,
            string AudioCodec, string Extension, string Language, int? Width, int? Height, int? Channels, int? SampleRate);
        private sealed record Snapshot(Identity Identity, Uri Uri, IReadOnlyDictionary<string, string> Headers, DateTime ExpiresUtc);
        private sealed class RequestState { internal bool Renewed; }
        private sealed class Entry
        {
            internal Entry(Snapshot snapshot, string token)
            { Current = snapshot; Identity = snapshot.Identity; Token = token; }
            internal object Sync { get; } = new();
            internal Snapshot Current;
            internal Identity Identity { get; }
            internal string Token { get; }
            internal DateTime CreatedUtc { get; } = DateTime.UtcNow;
            internal DateTime LastAccessUtc = DateTime.UtcNow;
            internal int ActiveRequests;
            internal Task<Snapshot>? Renewal;
            internal DateTime RenewalRetryAfterUtc;
        }

        private sealed class Server
        {
            private readonly object _gate = new();
            private readonly HttpListener _listener;
            private readonly CancellationTokenSource _stop = new();
            private readonly Dictionary<string, Entry> _entries = new(StringComparer.Ordinal);
            private readonly HashSet<HttpListenerContext> _contexts = new();
            private readonly string _origin;
            private bool _stopped;
            private Server(HttpListener listener, int port)
            { _listener = listener; _origin = "http://127.0.0.1:" + port.ToString(CultureInfo.InvariantCulture); StopToken = _stop.Token; }
            internal CancellationToken StopToken { get; }

            internal static Server Start()
            {
                for (int attempt = 0; attempt < 8; attempt++)
                {
                    var reservation = new TcpListener(IPAddress.Loopback, 0);
                    int port;
                    try { reservation.Start(1); port = ((IPEndPoint)reservation.LocalEndpoint).Port; }
                    finally { reservation.Stop(); }
                    var listener = new HttpListener { IgnoreWriteExceptions = false };
                    listener.Prefixes.Add("http://127.0.0.1:" + port.ToString(CultureInfo.InvariantCulture) + "/");
                    try
                    {
                        listener.Start();
                        var server = new Server(listener, port);
                        _ = server.AcceptAsync();
                        return server;
                    }
                    catch (HttpListenerException) { listener.Close(); }
                    catch (Exception) { listener.Close(); throw Failure(); }
                }
                throw Failure();
            }

            internal string Register(Snapshot snapshot)
            {
                lock (_gate)
                {
                    if (_stopped) throw Failure();
                    Prune();
                    var existing = _entries.Values.FirstOrDefault(e => e.Identity == snapshot.Identity);
                    if (existing != null)
                    {
                        lock (existing.Sync)
                            if (snapshot.ExpiresUtc > existing.Current.ExpiresUtc) existing.Current = snapshot;
                        existing.LastAccessUtc = DateTime.UtcNow;
                        return _origin + "/media/" + existing.Token;
                    }
                    if (_entries.Count >= MaximumEntries)
                        throw Failure(); // A paused player can still own an idle capability.
                    string token;
                    do { token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant(); }
                    while (_entries.ContainsKey(token));
                    _entries.Add(token, new Entry(snapshot, token));
                    return _origin + "/media/" + token;
                }
            }

            internal bool Owns(string value)
            {
                lock (_gate)
                {
                    if (_stopped || value == null || !value.StartsWith(_origin + "/media/", StringComparison.Ordinal)) return false;
                    string path = value.Substring(_origin.Length);
                    if (!CapabilityPath.IsMatch(path) || !_entries.TryGetValue(path.Substring(7), out var entry)) return false;
                    if (entry.ActiveRequests == 0 && entry.CreatedUtc.AddHours(12) <= DateTime.UtcNow)
                    { _entries.Remove(entry.Token); return false; }
                    return true;
                }
            }

            private void Prune()
            {
                foreach (var entry in _entries.Values.Where(e => e.ActiveRequests == 0
                    && e.Renewal?.IsCompleted != false && e.CreatedUtc.AddHours(12) <= DateTime.UtcNow).ToArray())
                    _entries.Remove(entry.Token);
            }

            private async Task AcceptAsync()
            {
                try
                {
                    while (!StopToken.IsCancellationRequested)
                    {
                        var context = await _listener.GetContextAsync().ConfigureAwait(false);
                        if (!RequestSlots.Wait(0)) { Reject(context, 429); continue; }
                        lock (_gate)
                        {
                            if (_stopped) { RequestSlots.Release(); Reject(context, 503); continue; }
                            _contexts.Add(context);
                        }
                        // HandleAsync owns its context and catches every failure;
                        // there is no unbounded queue of tasks or buffered bodies.
                        _ = HandleAsync(context);
                    }
                }
                catch (Exception) { Stop(); }
            }

            private async Task HandleAsync(HttpListenerContext context)
            {
                Entry? entry = null;
                bool committed = false;
                using var deadline = CancellationTokenSource.CreateLinkedTokenSource(StopToken);
                deadline.CancelAfter(TimeSpan.FromHours(6));
                try
                {
                    var request = context.Request;
                    if (request.RemoteEndPoint == null || !IPAddress.IsLoopback(request.RemoteEndPoint.Address)
                        || request.Headers["Host"] != _origin.Substring(7) || request.Url?.Host != "127.0.0.1"
                        || request.HasEntityBody || request.Headers["Origin"] != null)
                    { Reject(context, 400); return; }
                    if (request.HttpMethod is not ("GET" or "HEAD")) { Reject(context, 405); return; }
                    string path = request.RawUrl ?? "";
                    lock (_gate)
                    {
                        Prune();
                        if (!_stopped && CapabilityPath.IsMatch(path) && _entries.TryGetValue(path.Substring(7), out entry))
                        { entry.ActiveRequests++; entry.LastAccessUtc = DateTime.UtcNow; }
                        else entry = null;
                    }
                    if (entry == null) { Reject(context, 404); return; }
                    bool head = request.HttpMethod == "HEAD";
                    string? range = head || request.Headers["If-Range"] != null ? null : request.Headers["Range"];
                    if (!ParseRange(range, entry.Identity.Length, out long offset, out long end))
                    {
                        context.Response.Headers["Content-Range"] = "bytes */" + entry.Identity.Length.ToString(CultureInfo.InvariantCulture);
                        Reject(context, 416); return;
                    }
                    long remaining = end - offset + 1;
                    var state = new RequestState();
                    byte[]? first = head ? null : await ReadChunkAsync(this, entry, state, offset,
                        (int)Math.Min(ChunkBytes, remaining), deadline.Token).ConfigureAwait(false);
                    if (first != null && first.Length != Math.Min(ChunkBytes, remaining)) throw Failure();
                    var response = context.Response;
                    response.StatusCode = range == null ? 200 : 206;
                    response.KeepAlive = false;
                    response.SendChunked = false;
                    response.ContentLength64 = remaining;
                    response.ContentType = (entry.Identity.VideoCodec is "" or "none" ? "audio/" : "video/")
                        + (entry.Identity.Extension == "webm" ? "webm" : "mp4");
                    response.Headers["Accept-Ranges"] = "bytes";
                    response.Headers["Cache-Control"] = "no-store";
                    response.Headers["X-Content-Type-Options"] = "nosniff";
                    if (range != null) response.Headers["Content-Range"] = "bytes " + offset.ToString(CultureInfo.InvariantCulture)
                        + "-" + end.ToString(CultureInfo.InvariantCulture) + "/" + entry.Identity.Length.ToString(CultureInfo.InvariantCulture);
                    if (!head)
                    {
                        while (remaining > 0)
                        {
                            int count = (int)Math.Min(ChunkBytes, remaining);
                            var bytes = first ?? await ReadChunkAsync(this, entry, state, offset, count, deadline.Token).ConfigureAwait(false);
                            first = null;
                            if (bytes.Length != count) throw Failure();
                            committed = true;
                            // Native encoding can deliberately stop reading while
                            // paused or throttled. Retain only this bounded slice;
                            // shutdown and the six-hour request deadline still abort.
                            await response.OutputStream.WriteAsync(bytes.AsMemory(), deadline.Token).AsTask()
                                .WaitAsync(deadline.Token).ConfigureAwait(false);
                            bytes = Array.Empty<byte>();
                            offset += count; remaining -= count;
                        }
                    }
                    response.Close();
                }
                catch (YouTubeDirectHttp.RangeBusyException) { if (!committed) Reject(context, 503); else Abort(context); }
                catch (Exception) { if (!committed && !StopToken.IsCancellationRequested) Reject(context, 502); else Abort(context); }
                finally
                {
                    lock (_gate)
                    {
                        if (entry != null) { entry.ActiveRequests--; entry.LastAccessUtc = DateTime.UtcNow; }
                        _contexts.Remove(context);
                    }
                    RequestSlots.Release();
                }
            }

            private static void Reject(HttpListenerContext context, int status)
            {
                try
                {
                    context.Response.StatusCode = status;
                    context.Response.KeepAlive = false;
                    context.Response.ContentLength64 = 0;
                    context.Response.Headers["Cache-Control"] = "no-store";
                    context.Response.Close();
                }
                catch (Exception) { Abort(context); }
            }

            private static void Abort(HttpListenerContext context)
            { try { context.Response.Abort(); } catch (Exception) { } }

            internal void Stop()
            {
                HttpListenerContext[] contexts;
                lock (_gate)
                {
                    if (_stopped) return;
                    _stopped = true;
                    contexts = _contexts.ToArray();
                    _entries.Clear();
                }
                try { _stop.Cancel(); } catch (Exception) { }
                try { _listener.Abort(); _listener.Close(); } catch (Exception) { }
                foreach (var context in contexts) Abort(context);
            }
        }
    }
}
