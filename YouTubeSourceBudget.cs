using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Startup/restart source selection only. This observes one server-to-CDN
    // route, not the client link, and neither performs continuous ABR nor
    // guarantees stall-free playback. No signed URLs are logged or persisted.
    internal static class YouTubeSourceBudget
    {
        internal const int MaximumPlaylistBytes = YouTubeDirectHttp.MaximumMediaPlaylistBytes;
        internal const int MaximumPlaylistSegments = 8192;
        internal const int MaximumCacheEntries = 64;
        internal const int MaximumPendingProbes = 16;
        internal const int MaximumParallelProbes = 2;
        internal const int ProbeDeadlineMilliseconds = 1800;
        internal const int FallbackMaximumHeight = 480;
        internal const double ThroughputHeadroom = 0.60;
        internal static readonly TimeSpan SuccessLifetime = TimeSpan.FromSeconds(90);
        internal static readonly TimeSpan FailureLifetime = TimeSpan.FromSeconds(15);

        private sealed record Observation(int? Bitrate, string Reason, long ExpiresAtMilliseconds);
        private static readonly object CacheLock = new();
        private static readonly Dictionary<string, Task<Observation>> Cache = new(StringComparer.Ordinal);
        private static readonly SemaphoreSlim ProbeSlots = new(MaximumParallelProbes, MaximumParallelProbes);
        private static int _pendingProbes;
        // Monotonic TTLs: a wall-clock/NTP correction cannot prolong fallback.
        private static Func<long> _clockMilliseconds = () => Environment.TickCount64;
        private static Func<Uri, IReadOnlyDictionary<string, string>, int, CancellationToken, Task<byte[]>> _fetchPlaylist =
            YouTubeDirectHttp.GetMediaPlaylistBytesAsync;
        private static Func<Uri, IReadOnlyDictionary<string, string>, CancellationToken, Task<YouTubeMediaProbe>> _probeMedia =
            YouTubeDirectHttp.ProbeMediaAsync;

        internal static async Task<YouTubeHlsSelection> SelectAsync(string masterText, Uri masterUri,
            IReadOnlyDictionary<string, string> headers, int maximumHeight, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (masterUri == null) throw new InvalidOperationException("Invalid YouTube source URI.");
            masterUri = YouTubeDirectHttp.ValidateMediaUri(masterUri.OriginalString);
            var safeHeaders = YouTubeDirectHttp.CopyHeaders(headers);
            maximumHeight = YouTubeDirectQualityPolicy.ClampHeight(maximumHeight);

            // First validate the preferred AVC candidate without hiding bad
            // audio metadata behind a cheaper/AAC-only choice. The complete
            // master parser also validates every variant URI before ranking.
            YouTubeDirectManifest.Select(masterText, masterUri, maximumHeight, YouTubeDirectHttp.ValidateMediaUri, "h264");
            var preferred = YouTubeDirectManifest.Select(masterText, masterUri, maximumHeight,
                YouTubeDirectHttp.ValidateMediaUri, "h264", requiredAudioCodec: "aac");
            var lowest = YouTubeDirectManifest.Select(masterText, masterUri, maximumHeight,
                YouTubeDirectHttp.ValidateMediaUri, "h264", maximumBitrate: 0, requiredAudioCodec: "aac");
            var fallbackHeight = Math.Min(maximumHeight, FallbackMaximumHeight);
            YouTubeHlsSelection fallback;
            try
            {
                fallback = YouTubeDirectManifest.Select(masterText, masterUri, fallbackHeight,
                    YouTubeDirectHttp.ValidateMediaUri, "h264", requiredAudioCodec: "aac");
            }
            catch (YouTubeNoHlsVariantException) { fallback = lowest; }

            cancellationToken.ThrowIfCancellationRequested();
            var observation = await GetObservation(preferred.VideoUri, safeHeaders)
                .WaitAsync(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return observation.Bitrate is { } bitrate
                ? YouTubeDirectManifest.Select(masterText, masterUri, maximumHeight,
                    YouTubeDirectHttp.ValidateMediaUri, "h264",
                    bitrate, "aac")
                : fallback;
        }

        private static Task<Observation> GetObservation(Uri videoUri, IReadOnlyDictionary<string, string> headers)
        {
            var key = CacheKey(videoUri, headers);
            lock (CacheLock)
            {
                var now = _clockMilliseconds();
                foreach (var expired in Cache.Where(pair => pair.Value.IsCompletedSuccessfully
                    && pair.Value.Result.ExpiresAtMilliseconds <= now).Select(pair => pair.Key).ToArray()) Cache.Remove(expired);
                if (Cache.TryGetValue(key, out var cached)) return cached;
                if (Cache.Count >= MaximumCacheEntries)
                {
                    var oldest = Cache.Where(pair => pair.Value.IsCompletedSuccessfully)
                        .OrderBy(pair => pair.Value.Result.ExpiresAtMilliseconds).FirstOrDefault();
                    if (oldest.Key == null) return Task.FromResult(Fallback("capacity"));
                    Cache.Remove(oldest.Key);
                }
                Task<Observation> task;
                if (Volatile.Read(ref _pendingProbes) >= MaximumPendingProbes) task = Task.FromResult(Fallback("capacity"));
                else
                {
                    Interlocked.Increment(ref _pendingProbes);
                    task = MeasureAsync(videoUri, headers);
                }
                Cache.Add(key, task);
                return task;
            }
        }

        private static async Task<Observation> MeasureAsync(Uri videoUri, IReadOnlyDictionary<string, string> headers)
        {
            bool acquired = false;
            using var deadline = new CancellationTokenSource(ProbeDeadlineMilliseconds);
            try
            {
                // Publish the task in the cache before a synchronous test or
                // unusually fast transport completes. No caller owns this
                // shared deadline; canceling one waiter cannot poison another.
                await Task.Yield();
                await ProbeSlots.WaitAsync(deadline.Token).ConfigureAwait(false);
                acquired = true;
                var bytes = await _fetchPlaylist(videoUri, headers, MaximumPlaylistBytes, deadline.Token)
                    .WaitAsync(deadline.Token).ConfigureAwait(false);
                if (bytes.Length > MaximumPlaylistBytes) return Fallback("playlist-limit");
                var segment = ParseProbeSegment(new UTF8Encoding(false, true).GetString(bytes), videoUri);
                deadline.Token.ThrowIfCancellationRequested();
                var sample = await _probeMedia(segment, headers, deadline.Token)
                    .WaitAsync(deadline.Token).ConfigureAwait(false);
                if (sample.ReceivedBytes < YouTubeDirectHttp.MinimumProbeBytes
                    || sample.ReceivedBytes > YouTubeDirectHttp.MaximumProbeBytes
                    || sample.Elapsed <= TimeSpan.Zero) return Fallback("insufficient-media");
                // The minimum interval avoids treating an instantaneous cache
                // burst as unlimited bandwidth. BANDWIDTH includes the chosen
                // audio rendition; retain 40% headroom for normal variation.
                double rate = sample.ReceivedBytes * 8.0 / Math.Max(0.1, sample.Elapsed.TotalSeconds) * ThroughputHeadroom;
                int budget = (int)Math.Min(int.MaxValue, Math.Max(1, Math.Floor(rate)));
                return new Observation(budget, "measured", _clockMilliseconds() + (long)SuccessLifetime.TotalMilliseconds);
            }
            catch (UnsupportedProbePlaylistException) { return Fallback("unsupported-playlist"); }
            catch (OperationCanceledException) { return Fallback("timeout"); }
            catch (Exception) { return Fallback("probe-unavailable"); }
            finally
            {
                if (acquired) ProbeSlots.Release();
                Interlocked.Decrement(ref _pendingProbes);
            }
        }

        private static Observation Fallback(string reason)
        {
            // Only fixed categories reach this message, never exception text,
            // headers, video ids, playlist contents or signed request URLs.
            try { YouTubeChannel.LogPublic("[YT] Source budget fallback (" + reason
                + "): at most 480p when available; otherwise lowest compatible source."); }
            catch (Exception) { }
            return new Observation(null, reason, _clockMilliseconds() + (long)FailureLifetime.TotalMilliseconds);
        }

        private static string CacheKey(Uri uri, IReadOnlyDictionary<string, string> headers)
        {
            // A source route, not a signed URL or per-segment key. Different
            // videos/seeks on the same CDN route share a short observation.
            var route = string.Join('/', uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).Take(3));
            var identity = uri.Host.ToLowerInvariant() + "/" + route + "\n"
                + string.Join("\n", headers.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(pair => pair.Key.ToLowerInvariant() + ":" + pair.Value));
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
        }

        internal static Uri ParseProbeSegment(string text, Uri playlistUri)
        {
            if (text.Length > MaximumPlaylistBytes) throw new UnsupportedProbePlaylistException();
            var lines = text.TrimStart('\uFEFF').Replace("\r", "", StringComparison.Ordinal)
                .Split('\n').Select(line => line.Trim()).ToArray();
            if (lines.Length == 0 || lines[0] != "#EXTM3U") throw new UnsupportedProbePlaylistException();
            Uri? first = null;
            bool durationPending = false, targetDuration = false, ended = false;
            int segments = 0;
            foreach (var line in lines.Skip(1))
            {
                if (line.Length == 0) continue;
                if (line.StartsWith("#EXTINF:", StringComparison.Ordinal))
                {
                    if (ended || durationPending || !double.TryParse(line.Substring(8).Split(',')[0],
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var duration)
                        || !double.IsFinite(duration) || duration <= 0 || duration > 3600)
                        throw new UnsupportedProbePlaylistException();
                    durationPending = true;
                    continue;
                }
                if (line.StartsWith("#EXT-X-TARGETDURATION:", StringComparison.Ordinal))
                {
                    if (targetDuration || !int.TryParse(line.Substring(22), NumberStyles.None,
                        CultureInfo.InvariantCulture, out var target) || target <= 0 || target > 3600)
                        throw new UnsupportedProbePlaylistException();
                    targetDuration = true;
                    continue;
                }
                if (line == "#EXT-X-ENDLIST")
                {
                    if (durationPending || ended) throw new UnsupportedProbePlaylistException();
                    ended = true;
                    continue;
                }
                if (line == "#EXT-X-DISCONTINUITY" || line == "#EXT-X-INDEPENDENT-SEGMENTS"
                    || line.StartsWith("#EXT-X-VERSION:", StringComparison.Ordinal)
                    || line.StartsWith("#EXT-X-MEDIA-SEQUENCE:", StringComparison.Ordinal)
                    || line.StartsWith("#EXT-X-DISCONTINUITY-SEQUENCE:", StringComparison.Ordinal)
                    || line.StartsWith("#EXT-X-PLAYLIST-TYPE:", StringComparison.Ordinal)
                    || line.StartsWith("#EXT-X-PROGRAM-DATE-TIME:", StringComparison.Ordinal)) continue;
                if (line.StartsWith("#EXT", StringComparison.Ordinal)) throw new UnsupportedProbePlaylistException();
                if (line.StartsWith('#')) continue;
                if (ended || !durationPending || ++segments > MaximumPlaylistSegments) throw new UnsupportedProbePlaylistException();
                Uri segment;
                try { segment = YouTubeDirectHttp.ValidateMediaUri(new Uri(playlistUri, line).AbsoluteUri); }
                catch (Exception) { throw new UnsupportedProbePlaylistException(); }
                first ??= segment;
                durationPending = false;
            }
            if (!targetDuration || durationPending || first == null) throw new UnsupportedProbePlaylistException();
            return first;
        }

        private sealed class UnsupportedProbePlaylistException : InvalidOperationException
        {
            internal UnsupportedProbePlaylistException() : base("Unsupported YouTube probe playlist.") { }
        }
    }
}
