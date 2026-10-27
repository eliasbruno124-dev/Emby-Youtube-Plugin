using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Bridge;
using YoutubeExplode.Videos;

namespace Emby.YouTubePlugin
{
    // A single, fixed-client snapshot supplies media, HLS, video metadata and
    // original caption tracks. There is no executable or player-script fallback.
    internal static class YouTubeManagedResolver
    {
        internal const string Revision = "managed-6.6.3-snapshot-v1";
        internal const int MaximumOutputBytes = 8 * 1024 * 1024;
        private const int MaximumFormats = 1000;
        private const double MaximumDuration = 31 * 24 * 60 * 60;
        private static readonly Regex LanguagePattern = new("\\A[A-Za-z0-9][A-Za-z0-9_-]{0,79}\\z", RegexOptions.CultureInvariant);
        private static readonly Regex IdentityPattern = new("\\A[A-Za-z0-9_.-]{1,60}\\z", RegexOptions.CultureInvariant);
        private static readonly Regex InitialDataPattern = new(@"(?:var\s+)?ytInitialData\s*=\s*", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
        private static Func<HttpClient> _transportFactory = YouTubeDirectHttp.CreateResolverClient;

        internal static async Task<byte[]> ResolveAsync(string videoId, CancellationToken cancellationToken)
        {
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(TimeSpan.FromSeconds(60));
            using var guarded = new YouTubeManagedHttp(videoId, _transportFactory());
            using var http = new HttpClient(guarded) { Timeout = Timeout.InfiniteTimeSpan };
            var controller = new VideoController(http);
            var response = await controller.GetPlayerResponseAsync(videoId, cancellationToken: deadline.Token).ConfigureAwait(false);
            var result = ConvertSnapshot(videoId, response.Snapshot);
            var duration = result.TryGetValue("duration", out var durationValue) && durationValue is double seconds ? seconds : (double?)null;
            var chapters = ReadChapters(response.Snapshot, duration);
            if (chapters.Count == 0 && duration.HasValue && !guarded.Terminal)
            {
                // At most one optional metadata request, bounded independently.
                // Description timecodes alone are not proof of YouTube chapters.
                using var optional = CancellationTokenSource.CreateLinkedTokenSource(deadline.Token);
                optional.CancelAfter(TimeSpan.FromSeconds(4));
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.youtube.com/watch?v=" + videoId);
                    request.Headers.TryAddWithoutValidation("Accept", "text/html");
                    request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0");
                    using var watch = await http.SendAsync(request, optional.Token).ConfigureAwait(false);
                    string html = new UTF8Encoding(false, true).GetString(await watch.Content.ReadAsByteArrayAsync(optional.Token).ConfigureAwait(false));
                    var initial = ParseInitialData(html);
                    if (initial.HasValue) chapters = ReadChapters(initial.Value, duration);
                }
                catch (Exception error) when (error is not OutOfMemoryException)
                {
                    if (guarded.Terminal) throw new YouTubeManagedResolutionException(YouTubeManagedFailureKind.HTTP_DENIED);
                    deadline.Token.ThrowIfCancellationRequested();
                    // Missing/timeout optional chapter data is not deletion.
                }
            }
            if (chapters.Count > 0) result["chapters"] = chapters;
            deadline.Token.ThrowIfCancellationRequested();
            byte[] json = JsonSerializer.SerializeToUtf8Bytes(result);
            if (json.Length > MaximumOutputBytes) throw Failure();
            return json;
        }

        internal static Dictionary<string, object?> ConvertSnapshot(string videoId, JsonElement root)
        {
            if (root.ValueKind != JsonValueKind.Object || Text(Property(root, "videoDetails"), "videoId") != videoId
                || Text(Property(root, "playabilityStatus"), "status") != "OK") throw Failure();
            var player = new PlayerResponse(root);
            if (!string.IsNullOrEmpty(player.PreviewVideoId)) throw Failure();
            var details = Property(root, "videoDetails");
            var streaming = Property(root, "streamingData");
            var micro = Property(Property(root, "microformat"), "playerMicroformatRenderer");
            bool live = Boolean(details, "isLive") == true || Boolean(Property(micro, "liveBroadcastDetails"), "isLiveNow") == true;
            if (live) throw Failure();
            var result = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["id"] = videoId, ["live_status"] = live ? "is_live" : Boolean(details, "isLiveContent") == true ? "was_live" : "not_live",
                ["is_live"] = live, ["formats"] = ReadFormats(streaming),
                ["subtitles"] = new Dictionary<string, object?>(StringComparer.Ordinal),
                ["automatic_captions"] = new Dictionary<string, object?>(StringComparer.Ordinal)
            };
            if (TrySeconds(Text(details, "lengthSeconds"), out double duration)) result["duration"] = duration;
            if (Text(details, "shortDescription") is string description) result["description"] = Limit(description, 32768);
            if (Text(details, "title") is string title) result["title"] = Limit(title, 1024);
            if (CanonicalLong(Text(details, "viewCount"), allowZero: true) is long views) result["view_count"] = views;
            if (Text(streaming, "hlsManifestUrl") is string hls)
                result["hls_manifest_url"] = YouTubeDirectHttp.ValidateMediaUri(hls).OriginalString;
            AddCaptions(root, result);
            var chapters = ReadChapters(root, result.TryGetValue("duration", out var value) && value is double seconds ? seconds : null);
            if (chapters.Count > 0) result["chapters"] = chapters;
            return result;
        }

        private static List<Dictionary<string, object?>> ReadFormats(JsonElement streaming)
        {
            var formats = new List<Dictionary<string, object?>>();
            var identities = new HashSet<string>(StringComparer.Ordinal);
            int considered = 0;
            foreach (string field in new[] { "formats", "adaptiveFormats" })
            {
                var array = Property(streaming, field);
                if (array.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null) continue;
                if (array.ValueKind != JsonValueKind.Array || array.GetArrayLength() > MaximumFormats) throw Failure();
                foreach (var entry in array.EnumerateArray())
                {
                    if (++considered > MaximumFormats) throw Failure();
                    if (entry.ValueKind != JsonValueKind.Object) continue;
                    try
                    {
                        var data = new PlayerResponse.StreamData(entry);
                        if (string.IsNullOrEmpty(Text(entry, "url")) || !string.IsNullOrEmpty(data.Signature)) continue;
                        if (data.Itag is not int itag || itag <= 0 || data.ContentLength is not long length || length <= 0) continue;
                        var uri = YouTubeDirectHttp.ValidateMediaUri(data.Url ?? throw Failure());
                        ValidateLength(uri, length);
                        string? container = data.Container;
                        string? videoCodec = data.VideoCodec, audioCodec = data.AudioCodec;
                        if (data.Codecs == "unknown") continue; // Do not invent a codec profile.
                        if (container is not ("mp4" or "webm" or "m4a") ||
                            string.IsNullOrEmpty(videoCodec) && string.IsNullOrEmpty(audioCodec)) continue;
                        if ((videoCodec?.Length ?? 0) > 128 || (audioCodec?.Length ?? 0) > 128) throw Failure();
                        string id = itag.ToString(CultureInfo.InvariantCulture);
                        if (Text(Property(entry, "audioTrack"), "id") is string trackId)
                        {
                            if (!IdentityPattern.IsMatch(trackId)) throw Failure();
                            id += "." + trackId;
                        }
                        // DRC and original audio can share an itag and language.
                        // Preserve the actual server-provided discriminator across
                        // renewal instead of conflating the two representations.
                        if (Boolean(entry, "isDrc") == true) id += ".drc";
                        if (!identities.Add(id))
                        {
                            // No stable discriminator: exclude every colliding
                            // representation, not just whichever happened to be last.
                            formats.RemoveAll(item => (string?)item["format_id"] == id);
                            continue;
                        }
                        var format = new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["format_id"] = id, ["protocol"] = "https", ["url"] = uri.OriginalString,
                            ["filesize"] = length, ["ext"] = container,
                            ["vcodec"] = string.IsNullOrEmpty(videoCodec) ? "none" : videoCodec,
                            ["acodec"] = string.IsNullOrEmpty(audioCodec) ? "none" : audioCodec
                        };
                        if (data.Bitrate is long bitrate && bitrate > 0) format["tbr"] = bitrate / 1000d;
                        if (data.VideoWidth is int width && width is > 0 and <= 32768) format["width"] = width;
                        if (data.VideoHeight is int height && height is > 0 and <= 32768) format["height"] = height;
                        if (data.VideoFramerate is int fps && fps is > 0 and <= 1000) format["fps"] = fps;
                        if (data.AudioLanguageCode is string language && LanguagePattern.IsMatch(language)) format["language"] = language;
                        if (data.IsAudioLanguageDefault is bool isDefault) format["language_preference"] = isDefault ? 1 : 0;
                        if (Integer(entry, "audioChannels") is int channels && channels is > 0 and <= 64) format["audio_channels"] = channels;
                        if (CanonicalLong(Text(entry, "audioSampleRate"), allowZero: false) is long rate && rate <= 768000)
                            format["asr"] = (int)rate;
                        formats.Add(format);
                    }
                    catch (Exception error) when (error is InvalidOperationException or ArgumentException or FormatException or JsonException)
                    {
                        // A malformed, unselected representation does not make
                        // an independent validated HLS manifest unusable.
                    }
                }
            }
            return formats;
        }

        internal static void ValidateLength(Uri uri, long length)
        {
            long? value = null;
            foreach (string pair in RawQuery(uri).Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var pieces = pair.Split('=', 2);
                string name = Uri.UnescapeDataString(pieces[0]);
                if (name == "range") throw Failure();
                if (name != "clen") continue;
                if (pieces[0] != "clen" || value.HasValue || pieces.Length != 2 ||
                    CanonicalLong(pieces[1], allowZero: false) is not long parsed) throw Failure();
                value = parsed;
            }
            if (value != length) throw Failure();
        }

        // Uri.Query canonicalizes escaped unreserved characters. Identity
        // checks must inspect the spelling actually supplied by the server.
        internal static string RawQuery(Uri uri)
        {
            int offset = uri.OriginalString.IndexOf('?');
            return offset < 0 ? "" : uri.OriginalString[(offset + 1)..];
        }

        private static void AddCaptions(JsonElement root, Dictionary<string, object?> target)
        {
            var tracks = Property(Property(Property(root, "captions"), "playerCaptionsTracklistRenderer"), "captionTracks");
            if (tracks.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null) return;
            if (tracks.ValueKind != JsonValueKind.Array || tracks.GetArrayLength() > 512) throw Failure();
            int accepted = 0;
            foreach (var track in tracks.EnumerateArray())
            {
                if (track.ValueKind != JsonValueKind.Object || ++accepted > 512) throw Failure();
                var data = new PlayerResponse.ClosedCaptionTrackData(track);
                string? language = data.LanguageCode;
                if (language == null || !LanguagePattern.IsMatch(language) || string.IsNullOrEmpty(data.Url)) continue;
                var uri = YouTubeDirectHttp.ValidateCaptionUri(data.Url);
                if (uri.AbsolutePath != "/api/timedtext") continue;
                var query = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                bool duplicate = false;
                foreach (string pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = pair.Split('=', 2);
                    if (!query.TryAdd(Uri.UnescapeDataString(parts[0]), parts.Length == 2 ? Uri.UnescapeDataString(parts[1]) : "")) duplicate = true;
                }
                if (duplicate || query.ContainsKey("tlang") || query.TryGetValue("lang", out var actualLanguage)
                    && !actualLanguage.Equals(language, StringComparison.OrdinalIgnoreCase)) continue;
                if (!data.IsAutoGenerated && query.TryGetValue("kind", out var kind) && kind == "asr") continue;
                string url = YoutubeExplode.Utils.UrlEx.SetQueryParameter(uri.OriginalString, "fmt", "vtt");
                YouTubeDirectHttp.ValidateCaptionUri(url);
                var group = (Dictionary<string, object?>)target[data.IsAutoGenerated ? "automatic_captions" : "subtitles"]!;
                string key = language + (data.IsAutoGenerated ? "-orig" : "");
                if (!group.ContainsKey(key)) group[key] = new[] { new Dictionary<string, object?>
                { ["ext"] = "vtt", ["url"] = url, ["name"] = Limit(data.LanguageName ?? language, 256) } };
            }
        }

        internal static List<Dictionary<string, object?>> ReadChapters(JsonElement root, double? duration)
        {
            var found = new SortedDictionary<long, string>();
            if (!duration.HasValue || !double.IsFinite(duration.Value) || duration.Value <= 0 || duration.Value > MaximumDuration)
                return new List<Dictionary<string, object?>>();
            int nodes = 0;
            bool exceeded = false;
            void Visit(JsonElement value, int depth, bool chapterPanel)
            {
                if (++nodes > 20000 || depth > 40) { exceeded = true; return; }
                if (value.ValueKind == JsonValueKind.Array)
                { foreach (var item in value.EnumerateArray()) Visit(item, depth + 1, chapterPanel); return; }
                if (value.ValueKind != JsonValueKind.Object) return;
                string? targetId = Text(value, "targetId");
                bool knownPanel = chapterPanel || targetId is "engagement-panel-macro-markers-description-chapters" or "engagement-panel-macro-markers-auto-chapters";
                foreach (var property in value.EnumerateObject())
                {
                    var marker = property.Value;
                    if (property.Name == "chapterRenderer" && marker.ValueKind == JsonValueKind.Object
                        && Long(marker, "timeRangeStartMillis") is long millis)
                        Add(millis, Title(Property(marker, "title")));
                    else if (knownPanel && property.Name == "macroMarkersListItemRenderer" && marker.ValueKind == JsonValueKind.Object)
                    {
                        var watch = Property(Property(marker, "onTap"), "watchEndpoint");
                        if (Long(watch, "startTimeSeconds") is long seconds && seconds <= long.MaxValue / 1000)
                            Add(seconds * 1000, Title(Property(marker, "title")));
                    }
                    Visit(marker, depth + 1, knownPanel);
                }
            }
            void Add(long milliseconds, string? name)
            {
                if (found.Count >= 256 || milliseconds < 0 || milliseconds / 1000d >= duration.Value || string.IsNullOrWhiteSpace(name)) return;
                string cleaned = Limit(new string(name.Select(c => char.IsControl(c) || char.IsWhiteSpace(c) ? ' ' : c).ToArray()), 256).Trim();
                if (cleaned.Length > 0) found.TryAdd(milliseconds, cleaned);
            }
            Visit(root, 0, false);
            if (exceeded) return new List<Dictionary<string, object?>>();
            return found.Select(pair => new Dictionary<string, object?>
            { ["start_time"] = pair.Key / 1000d, ["title"] = pair.Value }).ToList();
        }

        internal static JsonElement? ParseInitialData(string html)
        {
            if (html.Length > YouTubeManagedHttp.MaximumResponseBytes) return null;
            var match = InitialDataPattern.Match(html);
            if (!match.Success) return null;
            int start = match.Index + match.Length;
            if (start >= html.Length || html[start] != '{') return null;
            int depth = 0; bool quoted = false, escaped = false;
            for (int i = start; i < html.Length; i++)
            {
                char ch = html[i];
                if (quoted)
                {
                    if (escaped) escaped = false;
                    else if (ch == '\\') escaped = true;
                    else if (ch == '"') quoted = false;
                    continue;
                }
                if (ch == '"') { quoted = true; continue; }
                if (ch == '{') { if (++depth > 48) return null; }
                else if (ch == '}' && --depth == 0)
                {
                    using var document = JsonDocument.Parse(html.AsMemory(start, i - start + 1), new JsonDocumentOptions { MaxDepth = 48 });
                    return document.RootElement.Clone();
                }
            }
            return null;
        }

        private static string? Title(JsonElement value)
        {
            if (Text(value, "simpleText") is string simple) return simple;
            var runs = Property(value, "runs");
            if (runs.ValueKind != JsonValueKind.Array || runs.GetArrayLength() > 32) return null;
            return string.Concat(runs.EnumerateArray().Select(run => Text(run, "text") ?? ""));
        }
        private static JsonElement Property(JsonElement value, string name) => value.ValueKind == JsonValueKind.Object && value.TryGetProperty(name, out var found) ? found : default;
        private static string? Text(JsonElement value, string name) => Property(value, name) is var found && found.ValueKind == JsonValueKind.String ? found.GetString() : null;
        private static bool? Boolean(JsonElement value, string name) => Property(value, name).ValueKind switch { JsonValueKind.True => true, JsonValueKind.False => false, _ => null };
        private static int? Integer(JsonElement value, string name) => Property(value, name) is var found && found.ValueKind == JsonValueKind.Number && found.TryGetInt32(out int number) ? number : null;
        private static long? Long(JsonElement value, string name) => Property(value, name) is var found && found.ValueKind == JsonValueKind.Number && found.TryGetInt64(out long number) ? number : null;
        private static long? CanonicalLong(string? value, bool allowZero) => value != null && value.Length is > 0 and <= 19
            && value.All(ch => ch is >= '0' and <= '9') && (value.Length == 1 || value[0] != '0')
            && long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out long number) && (allowZero || number > 0) ? number : null;
        private static bool TrySeconds(string? value, out double seconds)
        { seconds = 0; return double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out seconds) && double.IsFinite(seconds) && seconds > 0 && seconds <= MaximumDuration; }
        private static string Limit(string value, int maximum)
        { int length = Math.Min(value.Length, maximum); if (length > 0 && char.IsHighSurrogate(value[length - 1])) length--; return value[..length]; }
        private static YouTubeManagedResolutionException Failure() => new(YouTubeManagedFailureKind.INVALID_RESPONSE);
    }
}
