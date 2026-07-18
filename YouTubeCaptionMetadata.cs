using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Text.Json;

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
        private static readonly HttpClient Client = new(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        })
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        private static readonly ConcurrentDictionary<string, CacheEntry> Cache = new(StringComparer.Ordinal);

        static YouTubeCaptionMetadata()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/149 Safari/537.36");
            Client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("de-DE,de;q=0.9,en;q=0.7");
        }

        public static async Task<IReadOnlyList<YouTubeCaptionTrackMetadata>> GetTracksAsync(string videoId)
        {
            if (Cache.TryGetValue(videoId, out var cached)
                && cached.ExpiresUtc > DateTime.UtcNow)
            {
                return cached.Tracks;
            }

            try
            {
                var url = $"https://www.youtube.com/watch?v={Uri.EscapeDataString(videoId)}&hl=de";
                var html = await Client.GetStringAsync(url).ConfigureAwait(false);
                var json = ExtractPlayerResponseJson(html);
                var tracks = string.IsNullOrEmpty(json)
                    ? Array.Empty<YouTubeCaptionTrackMetadata>()
                    : ParseTracks(json);

                Cache[videoId] = new CacheEntry(tracks, DateTime.UtcNow.Add(SuccessTtl));
                YouTubeChannel.LogPublic(
                    $"[YT] YouTube caption metadata discovered for {videoId}: {tracks.Count} track(s), no caption payload downloaded.");
                return tracks;
            }
            catch (Exception ex)
            {
                var tracks = Array.Empty<YouTubeCaptionTrackMetadata>();
                Cache[videoId] = new CacheEntry(tracks, DateTime.UtcNow.Add(FailureTtl));
                YouTubeChannel.LogPublic($"[YT] YouTube caption metadata lookup failed for {videoId}: {ex.Message}");
                return tracks;
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
            DateTime ExpiresUtc);
    }
}
