using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace Emby.YouTubePlugin
{
    internal sealed record YouTubeCaptionTrackMetadata(
        string LanguageCode,
        string DisplayName,
        string Kind);

    // Reads caption-track metadata only. Caption payloads and timed-text URLs
    // are deliberately discarded; playback remains entirely inside YouTube.
    internal static class YouTubeCaptionMetadata
    {
        private static readonly TimeSpan SuccessTtl = TimeSpan.FromHours(6);
        private static readonly TimeSpan FailureTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);
        private const int MaxCacheEntries = 512;
        private static readonly SemaphoreSlim FetchGate = new(4, 4);
        private static readonly HttpClient Client = new(
            YouTubeHttpClientFactory.CreateHandler(
                allowAutoRedirect: true,
                automaticDecompression: DecompressionMethods.All))
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan
        };
        private static readonly ConcurrentDictionary<string, CacheEntry> Cache = new(StringComparer.Ordinal);
        private static readonly ConcurrentDictionary<string, Lazy<Task<IReadOnlyList<YouTubeCaptionTrackMetadata>>>> InFlight =
            new(StringComparer.Ordinal);

        static YouTubeCaptionMetadata()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/149 Safari/537.36");
            Client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("de-DE,de;q=0.9,en;q=0.7");
        }

        public static void Prefetch(string videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId) || TryGetFresh(videoId, out _))
                return;

            var pending = InFlight.GetOrAdd(
                videoId,
                id => new Lazy<Task<IReadOnlyList<YouTubeCaptionTrackMetadata>>>(
                    () => FetchAndCacheAsync(id),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            // FetchAndCacheAsync handles and records every failure internally, so
            // this fire-and-forget task cannot surface an unobserved exception.
            _ = pending.Value;
        }

        public static IReadOnlyList<YouTubeCaptionTrackMetadata> GetCachedTracks(string videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                return Array.Empty<YouTubeCaptionTrackMetadata>();

            if (TryGetFresh(videoId, out var fresh))
                return fresh;

            // An expired successful result is still a better non-blocking hint
            // than hiding every track while its background refresh is running.
            Cache.TryGetValue(videoId, out var stale);
            Prefetch(videoId);
            return stale?.Tracks ?? Array.Empty<YouTubeCaptionTrackMetadata>();
        }

        public static async Task<IReadOnlyList<YouTubeCaptionTrackMetadata>> GetTracksWithBudgetAsync(
            string videoId,
            TimeSpan maxWait)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                return Array.Empty<YouTubeCaptionTrackMetadata>();
            if (TryGetFresh(videoId, out var fresh))
                return fresh;

            Prefetch(videoId);
            if (InFlight.TryGetValue(videoId, out var pending))
            {
                try
                {
                    return await pending.Value.WaitAsync(maxWait).ConfigureAwait(false);
                }
                catch (TimeoutException)
                {
                    // Playback continues with stale/empty metadata while the
                    // shared background request warms the next response.
                }
            }

            Cache.TryGetValue(videoId, out var stale);
            return stale?.Tracks ?? Array.Empty<YouTubeCaptionTrackMetadata>();
        }

        private static bool TryGetFresh(
            string videoId,
            out IReadOnlyList<YouTubeCaptionTrackMetadata> tracks)
        {
            if (Cache.TryGetValue(videoId, out var cached)
                && cached.ExpiresUtc > DateTime.UtcNow)
            {
                tracks = cached.Tracks;
                return true;
            }

            tracks = Array.Empty<YouTubeCaptionTrackMetadata>();
            return false;
        }

        private static async Task<IReadOnlyList<YouTubeCaptionTrackMetadata>> FetchAndCacheAsync(string videoId)
        {
            var now = DateTime.UtcNow;

            try
            {
                var url = $"https://www.youtube.com/watch?v={Uri.EscapeDataString(videoId)}&hl=de";
                using var timeout = new CancellationTokenSource(RequestTimeout);
                await FetchGate.WaitAsync(timeout.Token).ConfigureAwait(false);
                string html;
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    using var response = await Client.SendAsync(
                            request,
                            HttpCompletionOption.ResponseHeadersRead,
                            timeout.Token)
                        .ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    html = await response.Content.ReadAsStringAsync(timeout.Token).ConfigureAwait(false);
                }
                finally
                {
                    FetchGate.Release();
                }
                var json = ExtractPlayerResponseJson(html);
                if (string.IsNullOrEmpty(json))
                    throw new InvalidDataException("YouTube player response was not present in the watch page.");

                var tracks = ParseTracks(json);

                now = DateTime.UtcNow;
                Cache[videoId] = new CacheEntry(tracks, now, now.Add(SuccessTtl));
                TrimCache(now);
                YouTubeChannel.LogPublic(
                    $"[YT] YouTube caption metadata discovered for {videoId}: {tracks.Count} track(s), no caption payload downloaded.");
                return tracks;
            }
            catch (OperationCanceledException)
            {
                now = DateTime.UtcNow;
                var tracks = PreserveStaleTracks(videoId, now);
                TrimCache(now);
                YouTubeChannel.LogPublic($"[YT] YouTube caption metadata lookup timed out for {videoId}.");
                return tracks;
            }
            catch (Exception ex)
            {
                now = DateTime.UtcNow;
                var tracks = PreserveStaleTracks(videoId, now);
                TrimCache(now);
                YouTubeChannel.LogPublic($"[YT] YouTube caption metadata lookup failed for {videoId}: {ex.Message}");
                return tracks;
            }
            finally
            {
                InFlight.TryRemove(videoId, out _);
            }
        }

        private static IReadOnlyList<YouTubeCaptionTrackMetadata> PreserveStaleTracks(
            string videoId,
            DateTime now)
        {
            var tracks = Cache.TryGetValue(videoId, out var previous)
                ? previous.Tracks
                : Array.Empty<YouTubeCaptionTrackMetadata>();
            Cache[videoId] = new CacheEntry(tracks, previous?.CachedUtc ?? now, now.Add(FailureTtl));
            return tracks;
        }

        private static void TrimCache(DateTime now)
        {
            if (Cache.Count <= MaxCacheEntries)
                return;

            foreach (var expired in Cache
                         .Where(entry => entry.Value.ExpiresUtc <= now)
                         .Select(entry => entry.Key)
                         .ToArray())
            {
                Cache.TryRemove(expired, out _);
            }

            var overflow = Cache.Count - MaxCacheEntries;
            if (overflow <= 0)
                return;

            foreach (var oldest in Cache
                         .OrderBy(entry => entry.Value.CachedUtc)
                         .Take(overflow)
                         .Select(entry => entry.Key)
                         .ToArray())
            {
                Cache.TryRemove(oldest, out _);
            }
        }

        private static IReadOnlyList<YouTubeCaptionTrackMetadata> ParseTracks(string json)
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("captions", out var captions)
                || !captions.TryGetProperty("playerCaptionsTracklistRenderer", out var renderer)
                || !renderer.TryGetProperty("captionTracks", out var captionTracks)
                || captionTracks.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<YouTubeCaptionTrackMetadata>();
            }

            var result = new List<YouTubeCaptionTrackMetadata>();
            foreach (var track in captionTracks.EnumerateArray())
            {
                if (!track.TryGetProperty("languageCode", out var languageElement))
                    continue;

                var languageCode = languageElement.GetString();
                if (string.IsNullOrWhiteSpace(languageCode))
                    continue;

                var displayName = ReadText(track, "name") ?? languageCode;
                var kind = track.TryGetProperty("kind", out var kindElement)
                    ? kindElement.GetString() ?? string.Empty
                    : string.Empty;

                // YouTube exposes automatic speech-recognition tracks in the
                // watch-page metadata but not in the IFrame player's selectable
                // caption track list. Advertising them to Emby creates a selector
                // entry that the real player cannot activate.
                if (string.Equals(kind, "asr", StringComparison.OrdinalIgnoreCase))
                    continue;

                result.Add(new YouTubeCaptionTrackMetadata(languageCode, displayName, kind));
            }

            return result;
        }

        private static string? ReadText(JsonElement parent, string propertyName)
        {
            if (!parent.TryGetProperty(propertyName, out var value))
                return null;
            if (value.TryGetProperty("simpleText", out var simpleText))
                return simpleText.GetString();
            if (!value.TryGetProperty("runs", out var runs) || runs.ValueKind != JsonValueKind.Array)
                return null;

            return string.Concat(
                runs.EnumerateArray()
                    .Select(run => run.TryGetProperty("text", out var text) ? text.GetString() : string.Empty));
        }

        private static string? ExtractPlayerResponseJson(string html)
        {
            const string marker = "ytInitialPlayerResponse";
            var markerIndex = html.IndexOf(marker, StringComparison.Ordinal);
            if (markerIndex < 0)
                return null;

            var start = html.IndexOf('{', markerIndex + marker.Length);
            if (start < 0)
                return null;

            var depth = 0;
            var inString = false;
            var escaped = false;
            for (var i = start; i < html.Length; i++)
            {
                var character = html[i];
                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (character == '\\')
                    {
                        escaped = true;
                    }
                    else if (character == '"')
                    {
                        inString = false;
                    }
                    continue;
                }

                if (character == '"')
                {
                    inString = true;
                }
                else if (character == '{')
                {
                    depth++;
                }
                else if (character == '}' && --depth == 0)
                {
                    return html.Substring(start, i - start + 1);
                }
            }

            return null;
        }

        private sealed record CacheEntry(
            IReadOnlyList<YouTubeCaptionTrackMetadata> Tracks,
            DateTime CachedUtc,
            DateTime ExpiresUtc);
    }
}
