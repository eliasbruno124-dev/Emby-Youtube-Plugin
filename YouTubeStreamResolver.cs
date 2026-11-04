using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Extraction is independent of format selection: video-only HLS remains
    // video-only until the playback layer supplies its verified audio group.
    internal static class YouTubeStreamResolver
    {
        private const int MaximumJsonBytes = 8 * 1024 * 1024;
        private const int MaximumCacheBytes = 16 * 1024 * 1024;
        private static readonly object Gate = new();
        private static readonly SemaphoreSlim ResolutionSlots = new(2, 2);
        private static readonly Dictionary<string, CacheEntry> Cache = new(StringComparer.Ordinal);
        private static readonly Dictionary<string, Flight> Flights = new(StringComparer.Ordinal);
        private static Func<string, CancellationToken, Task<byte[]>> _acquire = YouTubeManagedResolver.ResolveAsync;
        private static Func<DateTime> _utcNow = () => DateTime.UtcNow;
        private static long _cacheSequence;
        private static int _cacheBytes;
        private static readonly Regex VideoIdPattern = new("\\A[A-Za-z0-9_-]{11}\\z", RegexOptions.CultureInvariant);
        private static readonly Regex PathExpiryPattern = new("/expire/([0-9]{1,12})(?:/|$)", RegexOptions.CultureInvariant);

        internal static async Task<YouTubeResolvedVideo> ResolveAsync(string videoId, CancellationToken cancellationToken)
        {
            if (videoId == null || !VideoIdPattern.IsMatch(videoId)) throw Failure("Invalid YouTube video ID.");
            cancellationToken.ThrowIfCancellationRequested();
            var options = Plugin.Instance?.Options;
            if (options?.DirectPlaybackEnabled != true) throw Failure("Direct YouTube playback is disabled.");
            string key = videoId + "\n" + YouTubeManagedResolver.Revision;
            Flight flight;
            Task<YouTubeResolvedVideo>? invalidated = null;
            lock (Gate)
            {
                PruneCache(_utcNow());
                if (Cache.TryGetValue(key, out var cached))
                {
                    cached.LastAccess = ++_cacheSequence;
                    if (cached.Result == null) throw Failure("YouTube resolution is temporarily unavailable.");
                    return cached.Result;
                }
                if (!Flights.TryGetValue(key, out flight!))
                {
                    if (YouTubeRequestBackoff.IsBlocked())
                        throw Failure("YouTube has limited requests. New playback requests are temporarily paused.");
                    if (Flights.Count >= 10) throw Failure("YouTube resolver queue is full.");
                    flight = new Flight(videoId);
                    Flights.Add(key, flight);
                    flight.Task = Task.Run(() => RunFlightAsync(key, flight));
                    _ = flight.Task.ContinueWith(task => { _ = task.Exception; },
                        CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                }
                if (!flight.CacheAllowed) invalidated = flight.Task;
                else flight.Waiters++;
            }
            if (invalidated != null)
            {
                // Keep the old flight in capacity accounting, but never give a
                // post-invalidation caller its stale snapshot. The old flight's
                // own deadline bounds this wait; caller cancellation is separate.
                try { await invalidated.WaitAsync(cancellationToken).ConfigureAwait(false); }
                catch (Exception) { cancellationToken.ThrowIfCancellationRequested(); }
                return await ResolveAsync(videoId, cancellationToken).ConfigureAwait(false);
            }
            try
            {
                return await flight.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException("YouTube resolution canceled.", cancellationToken);
                throw Failure("YouTube resolution timed out.");
            }
            finally
            {
                lock (Gate)
                {
                    flight.Waiters--;
                    if (flight.Waiters == 0 && !flight.Task.IsCompleted) TryCancel(flight.Deadline);
                }
            }
        }

        internal static void Invalidate(string videoId)
        {
            if (videoId == null || !VideoIdPattern.IsMatch(videoId)) return;
            lock (Gate)
            {
                foreach (var key in Cache.Where(pair => pair.Value.VideoId == videoId).Select(pair => pair.Key).ToArray())
                    RemoveCache(key);
                foreach (var flight in Flights.Values.Where(value => value.VideoId == videoId))
                    flight.CacheAllowed = false;
            }
        }

        private static async Task<YouTubeResolvedVideo> RunFlightAsync(
            string key, Flight flight)
        {
            bool acquired = false;
            try
            {
                await ResolutionSlots.WaitAsync(flight.Deadline.Token).ConfigureAwait(false);
                acquired = true;
                if (YouTubeRequestBackoff.IsBlocked())
                    throw Failure("YouTube request cooldown is active.");
                var json = await _acquire(flight.VideoId, flight.Deadline.Token).ConfigureAwait(false);
                flight.Deadline.Token.ThrowIfCancellationRequested();
                var result = ParseResult(flight.VideoId, json, _utcNow());
                lock (Gate)
                {
                    if (flight.CacheAllowed && !flight.Deadline.IsCancellationRequested)
                    {
                        AddCache(key, new CacheEntry(flight.VideoId, result, result.ExpiresUtc, json.Length));
                        YouTubeDirectMetadata.Record(flight.VideoId, result.Metadata);
                    }
                }
                return result;
            }
            catch (OperationCanceledException)
            {
                throw Failure("YouTube resolution canceled or timed out.");
            }
            catch (Exception error)
            {
                string code = error is YouTubeManagedResolutionException managed ? managed.Kind + "/" + managed.Stage : "INVALID_RESPONSE/SNAPSHOT";
                YouTubeChannel.LogPublic("[YT] Managed resolver failed (" + code + ").");
                lock (Gate)
                {
                    if (flight.CacheAllowed && !flight.Deadline.IsCancellationRequested)
                        AddCache(key, new CacheEntry(flight.VideoId, null, _utcNow().AddSeconds(15), 0));
                }
                throw Failure("YouTube media resolution failed.");
            }
            finally
            {
                if (acquired) ResolutionSlots.Release();
                lock (Gate)
                {
                    Flights.Remove(key);
                    flight.Deadline.Dispose();
                }
            }
        }

        private static YouTubeResolvedVideo ParseResult(string videoId, byte[] json, DateTime now)
        {
            if (json == null || json.Length == 0 || json.Length > MaximumJsonBytes) throw Failure("YouTube metadata limit exceeded.");
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 48 });
            var metadata = document.RootElement;
            if (metadata.ValueKind != JsonValueKind.Object
                || StringValue(metadata, "id") != videoId
                || !metadata.TryGetProperty("formats", out var formats)
                || formats.ValueKind != JsonValueKind.Array || formats.GetArrayLength() > 1000)
                throw Failure("Invalid YouTube managed result.");
            var liveStatus = StringValue(metadata, "live_status");
            if (liveStatus != "not_live" && liveStatus != "was_live")
                throw Failure("Live YouTube playback is not supported by this resolver.");
            if (metadata.TryGetProperty("duration", out var duration)
                && duration.ValueKind != JsonValueKind.Null
                && (duration.ValueKind != JsonValueKind.Number || !duration.TryGetDouble(out double seconds) || !double.IsFinite(seconds) || seconds <= 0 || seconds > 31 * 24 * 60 * 60))
                throw Failure("Invalid YouTube duration.");

            DateTime expiry = now.AddMinutes(5);
            bool hasMedia = false;
            if (metadata.TryGetProperty("hls_manifest_url", out var hls))
            {
                if (hls.ValueKind != JsonValueKind.String) throw Failure("Invalid YouTube HLS metadata.");
                expiry = Earlier(expiry, UrlExpiry(YouTubeDirectHttp.ValidateMediaUri(hls.GetString()!), now));
                hasMedia = true;
            }
            var identities = new HashSet<string>(StringComparer.Ordinal);
            foreach (var format in formats.EnumerateArray())
            {
                string? protocol = StringValue(format, "protocol");
                if (protocol != "https") throw Failure("Invalid YouTube media protocol.");
                string? url = StringValue(format, "url");
                if (url == null) throw Failure("Invalid YouTube media URL.");
                var mediaUri = YouTubeDirectHttp.ValidateMediaUri(url);
                string? identity = StringValue(format, "format_id");
                if (identity == null || identity.Length is 0 or > 80 || identity.Any(ch => !((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9')) && ch is not ('_' or '.' or '-'))
                    || !identities.Add(identity) || !format.TryGetProperty("filesize", out var size)
                    || size.ValueKind != JsonValueKind.Number || !size.TryGetInt64(out long length) || length <= 0)
                    throw Failure("Invalid YouTube media identity.");
                YouTubeManagedResolver.ValidateLength(mediaUri, length);
                expiry = Earlier(expiry, UrlExpiry(mediaUri, now));
                if (StringValue(format, "manifest_url") is string manifest)
                    expiry = Earlier(expiry, UrlExpiry(YouTubeDirectHttp.ValidateMediaUri(manifest), now));
                if (format.TryGetProperty("http_headers", out var headers)) YouTubeDirectHttp.ReadHeaders(headers);
                hasMedia = true;
            }
            foreach (string name in new[] { "subtitles", "automatic_captions" })
            {
                if (!metadata.TryGetProperty(name, out var languages) || languages.ValueKind == JsonValueKind.Null) continue;
                if (languages.ValueKind != JsonValueKind.Object) throw Failure("Invalid YouTube captions.");
                int languageCount = 0;
                foreach (var language in languages.EnumerateObject())
                {
                    if (++languageCount > 512 || language.Value.ValueKind != JsonValueKind.Array
                        || language.Value.GetArrayLength() > 32) throw Failure("YouTube caption limit exceeded.");
                    if (language.Name == "live_chat") continue;
                    foreach (var caption in language.Value.EnumerateArray())
                    {
                        if (StringValue(caption, "url") is string url)
                            expiry = Earlier(expiry, UrlExpiry(YouTubeDirectHttp.ValidateCaptionUri(url), now));
                        if (caption.TryGetProperty("http_headers", out var headers)) YouTubeDirectHttp.ReadHeaders(headers);
                    }
                }
            }
            if (!hasMedia || expiry <= now.AddSeconds(5)) throw Failure("YouTube media URLs are unavailable or expired.");
            if (metadata.TryGetProperty("http_headers", out var topHeaders)) YouTubeDirectHttp.ReadHeaders(topHeaders);
            return new YouTubeResolvedVideo(videoId, metadata.Clone(), expiry);
        }

        private static DateTime UrlExpiry(Uri uri, DateTime now)
        {
            long? earliest = null;
            foreach (string pair in YouTubeManagedResolver.RawQuery(uri).Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = pair.Split('=', 2);
                if (Uri.UnescapeDataString(parts[0]) == "expire")
                {
                    if (parts.Length != 2 || parts[0] != "expire" || earliest.HasValue || parts[1].Length is 0 or > 12 || parts[1][0] == '0'
                        || !long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out long value))
                        throw Failure("Invalid YouTube URL expiry.");
                    earliest = earliest.HasValue ? Math.Min(earliest.Value, value) : value;
                }
            }
            foreach (Match match in PathExpiryPattern.Matches(uri.AbsolutePath))
            {
                if (!long.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out long value))
                    throw Failure("Invalid YouTube URL expiry.");
                earliest = earliest.HasValue ? Math.Min(earliest.Value, value) : value;
            }
            return earliest.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(earliest.Value).UtcDateTime.AddSeconds(-120)
                : now.AddSeconds(60);
        }

        private static string? StringValue(JsonElement element, string property) =>
            element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value)
                && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

        private static DateTime Earlier(DateTime first, DateTime second) => first < second ? first : second;

        private static void AddCache(string key, CacheEntry entry)
        {
            RemoveCache(key);
            entry.LastAccess = ++_cacheSequence;
            Cache.Add(key, entry);
            _cacheBytes += entry.Bytes;
            while (Cache.Count > 128 || _cacheBytes > MaximumCacheBytes)
                RemoveCache(Cache.OrderBy(pair => pair.Value.LastAccess).First().Key);
        }

        private static void PruneCache(DateTime now)
        {
            foreach (var key in Cache.Where(pair => pair.Value.ExpiresUtc <= now).Select(pair => pair.Key).ToArray())
                RemoveCache(key);
        }

        private static void RemoveCache(string key)
        {
            if (Cache.TryGetValue(key, out var entry)) { Cache.Remove(key); _cacheBytes -= entry.Bytes; }
        }

        private static void TryCancel(CancellationTokenSource source)
        {
            try { source.Cancel(); } catch (ObjectDisposedException) { }
        }

        private static InvalidOperationException Failure(string message) => new(message);

        private sealed class Flight
        {
            internal Flight(string videoId) { VideoId = videoId; Deadline.CancelAfter(TimeSpan.FromSeconds(90)); }
            internal string VideoId { get; }
            internal CancellationTokenSource Deadline { get; } = new();
            internal Task<YouTubeResolvedVideo> Task { get; set; } = null!;
            internal int Waiters { get; set; }
            internal bool CacheAllowed { get; set; } = true;
        }

        private sealed class CacheEntry
        {
            internal CacheEntry(string videoId, YouTubeResolvedVideo? result, DateTime expiresUtc, int bytes)
            { VideoId = videoId; Result = result; ExpiresUtc = expiresUtc; Bytes = bytes; }
            internal string VideoId { get; }
            internal YouTubeResolvedVideo? Result { get; }
            internal DateTime ExpiresUtc { get; }
            internal int Bytes { get; }
            internal long LastAccess { get; set; }
        }
    }
}
