using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Emby.YouTubePlugin
{
    internal sealed class YouTubeNoHlsVariantException : InvalidOperationException
    {
        internal YouTubeNoHlsVariantException() : base("No HLS variant within the configured height limit.") { }
    }

    internal sealed record YouTubeHlsSelection(
        string Content, Uri VideoUri, Uri? AudioUri, int Width, int Height,
        int Bitrate, float? FrameRate, string VideoCodec, string AudioCodec,
        int? AudioChannels, string? AudioLanguage)
    {
        internal string? VideoContent { get; init; }
        internal IReadOnlyList<YouTubeHlsAudioTrack> AudioTracks { get; init; } = Array.Empty<YouTubeHlsAudioTrack>();
    }

    internal sealed record YouTubeHlsAudioTrack(string Key, string Content, string Name,
        string? Language, int? Channels, bool IsDefault);

    // Parses a bounded upstream master, never arbitrary client input. The output
    // has one video variant. Real alternate audio renditions are represented
    // separately so native selection maps only the requested language.
    internal static class YouTubeDirectManifest
    {
        internal const int MaximumMasterBytes = 512 * 1024;

        internal static YouTubeHlsSelection Select(
            string text, Uri masterUri, int maximumHeight, Func<string, Uri> validateUri,
            string? requiredVideoCodec = null, int? maximumBitrate = null, string? requiredAudioCodec = null)
        {
            if (text.Length > MaximumMasterBytes || !text.TrimStart('\uFEFF', ' ', '\r', '\n').StartsWith("#EXTM3U", StringComparison.Ordinal))
                throw new InvalidOperationException("Invalid HLS master.");
            maximumHeight = Math.Clamp(maximumHeight, 144, 2160);
            var lines = text.Replace("\r", "", StringComparison.Ordinal).Split('\n').Select(x => x.Trim()).ToArray();
            var audio = new List<Dictionary<string, string>>();
            var variants = new List<(Dictionary<string, string> Attributes, Uri Uri, int Width, int Height, int Bitrate)>();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                // This path deliberately has no DRM/key or variable-substitution
                // support; silently dropping these would make a broken source.
                if (line.StartsWith("#EXT-X-SESSION-KEY:", StringComparison.Ordinal)
                    || line.StartsWith("#EXT-X-KEY:", StringComparison.Ordinal)
                    || line.StartsWith("#EXT-X-DEFINE:", StringComparison.Ordinal))
                    throw new InvalidOperationException("Unsupported HLS master feature.");
                if (line.StartsWith("#EXT-X-MEDIA:", StringComparison.Ordinal))
                {
                    var attrs = ParseAttributes(line.Substring(13));
                    if (Value(attrs, "TYPE") == "AUDIO")
                    {
                        if (Value(attrs, "URI").Length > 0) Resolve(Value(attrs, "URI"), masterUri, validateUri);
                        audio.Add(attrs);
                    }
                }
                else if (line.StartsWith("#EXT-X-STREAM-INF:", StringComparison.Ordinal))
                {
                    var attrs = ParseAttributes(line.Substring(18));
                    while (++i < lines.Length && lines[i].Length == 0) { }
                    if (i >= lines.Length || lines[i].StartsWith('#'))
                        throw new InvalidOperationException("Missing HLS variant URI.");
                    var uri = Resolve(lines[i], masterUri, validateUri);
                    var dimensions = Value(attrs, "RESOLUTION").Split('x');
                    if (dimensions.Length != 2
                        || !int.TryParse(dimensions[0], NumberStyles.None, CultureInfo.InvariantCulture, out var width)
                        || !int.TryParse(dimensions[1], NumberStyles.None, CultureInfo.InvariantCulture, out var height)
                        || width < 1 || width > 16384 || height < 1 || height > 16384
                        || !int.TryParse(Value(attrs, "BANDWIDTH"), NumberStyles.None, CultureInfo.InvariantCulture, out var bitrate)
                        || bitrate <= 0)
                        throw new InvalidOperationException("Missing HLS video dimensions or bitrate.");
                    variants.Add((attrs, uri, width, height, bitrate));
                }
            }

            // Runtime compatibility is decided before quality ranking. The
            // chosen variant still undergoes full codec/audio validation below;
            // a malformed preferred candidate must not fall through to another.
            var eligible = variants.Where(v => v.Height <= maximumHeight)
                .Where(v => requiredVideoCodec == null || Value(v.Attributes, "CODECS").Split(',')
                    .Any(codec => VideoCodec(codec.Trim()) == requiredVideoCodec))
                .Where(v => requiredAudioCodec == null || Value(v.Attributes, "CODECS").Split(',')
                    .Any(codec => AudioCodec(codec.Trim()) == requiredAudioCodec))
                .OrderByDescending(v => v.Height)
                .ThenByDescending(v => Value(v.Attributes, "CODECS").Contains("avc1", StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(v => FrameRate(v.Attributes) is > 0 and <= YouTubeDirectQualityPolicy.MaximumFrameRate)
                .ThenByDescending(v => FrameRate(v.Attributes) ?? 0)
                .ThenByDescending(v => v.Bitrate).ToList();
            if (eligible.Count == 0) throw new YouTubeNoHlsVariantException();

            // Validate the preferred candidate before applying a bandwidth
            // ceiling. A cheap lower rendition must never hide its corruption.
            var preferred = BuildSelection(eligible[0], audio, masterUri, validateUri);
            if (!maximumBitrate.HasValue) return preferred;
            var affordable = eligible.Where(v => v.Bitrate <= maximumBitrate.Value).ToList();
            var selected = affordable.Count > 0 ? affordable[0]
                : eligible.OrderBy(v => v.Bitrate).ThenBy(v => v.Height).First();
            return BuildSelection(selected, audio, masterUri, validateUri);
        }

        private static YouTubeHlsSelection BuildSelection(
            (Dictionary<string, string> Attributes, Uri Uri, int Width, int Height, int Bitrate) selected,
            List<Dictionary<string, string>> audio, Uri masterUri, Func<string, Uri> validateUri)
        {
            var codecs = Value(selected.Attributes, "CODECS").Split(',').Select(x => x.Trim()).ToArray();
            var videoCodecs = codecs.Select(VideoCodec).Where(x => x != null).Distinct().ToArray();
            var audioCodecs = codecs.Select(AudioCodec).Where(x => x != null).Distinct().ToArray();
            if (videoCodecs.Length != 1 || audioCodecs.Length != 1
                || codecs.Any(x => VideoCodec(x) == null && AudioCodec(x) == null))
                throw new InvalidOperationException("HLS video/audio codec information is incomplete or unsupported.");

            Dictionary<string, string>? rendition = null;
            Uri? audioUri = null;
            var group = Value(selected.Attributes, "AUDIO");
            if (group.Length > 0)
            {
                rendition = audio.Where(a => Value(a, "GROUP-ID") == group)
                    .OrderByDescending(a => YouTubeAudioTracks.HasExplicitOriginalLabel(Value(a, "NAME")))
                    .ThenByDescending(a => Value(a, "DEFAULT") == "YES")
                    .ThenByDescending(a => Value(a, "AUTOSELECT") == "YES")
                    .ThenBy(a => Value(a, "NAME"), StringComparer.Ordinal).FirstOrDefault();
                if (rendition == null || Value(rendition, "NAME").Length == 0 || Value(rendition, "URI").Length == 0)
                    throw new InvalidOperationException("HLS variant references an unavailable audio rendition.");
                audioUri = Resolve(Value(rendition, "URI"), masterUri, validateUri);
            }
            // No AUDIO group is valid only when the codecs explicitly declare
            // an in-band audio codec. The codec check above enforces that.
            var output = new StringBuilder("#EXTM3U\n#EXT-X-VERSION:6\n");
            if (rendition != null)
            {
                output.Append("#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=").Append(Quote(group))
                    .Append(",NAME=").Append(Quote(Value(rendition, "NAME")))
                    .Append(",DEFAULT=YES,AUTOSELECT=YES,URI=").Append(Quote(audioUri!.AbsoluteUri));
                foreach (var key in new[] { "LANGUAGE", "ASSOC-LANGUAGE", "CHANNELS" })
                    if (Value(rendition, key).Length > 0) output.Append(',').Append(key).Append('=').Append(Quote(Value(rendition, key)));
                output.Append('\n');
            }
            output.Append("#EXT-X-STREAM-INF:BANDWIDTH=").Append(selected.Bitrate.ToString(CultureInfo.InvariantCulture))
                .Append(",RESOLUTION=").Append(selected.Width).Append('x').Append(selected.Height)
                .Append(",CODECS=").Append(Quote(Value(selected.Attributes, "CODECS")));
            if (group.Length > 0) output.Append(",AUDIO=").Append(Quote(group));
            if (FrameRate(selected.Attributes) is { } frameRate)
                output.Append(",FRAME-RATE=").Append(frameRate.ToString(CultureInfo.InvariantCulture));
            var range = Value(selected.Attributes, "VIDEO-RANGE");
            if (range.Length > 0)
            {
                if (range is not ("SDR" or "PQ" or "HLG")) throw new InvalidOperationException("Unsupported HLS video range.");
                output.Append(",VIDEO-RANGE=").Append(range);
            }
            if (int.TryParse(Value(selected.Attributes, "AVERAGE-BANDWIDTH"), NumberStyles.None, CultureInfo.InvariantCulture, out var average) && average > 0)
                output.Append(",AVERAGE-BANDWIDTH=").Append(average.ToString(CultureInfo.InvariantCulture));
            output.Append('\n').Append(selected.Uri.AbsoluteUri).Append('\n');

            int? channels = null;
            if (rendition != null && int.TryParse(Value(rendition, "CHANNELS").Split('/')[0], NumberStyles.None, CultureInfo.InvariantCulture, out var count) && count is > 0 and <= 32)
                channels = count;
            var tracks = new List<YouTubeHlsAudioTrack>();
            string? videoContent = null;
            if (rendition != null)
            {
                var groupTracks = audio.Where(a => Value(a, "GROUP-ID") == group)
                    .OrderByDescending(a => YouTubeAudioTracks.HasExplicitOriginalLabel(Value(a, "NAME")))
                    .ThenByDescending(a => Value(a, "DEFAULT") == "YES")
                    .ThenByDescending(a => Value(a, "AUTOSELECT") == "YES")
                    .ThenBy(a => Value(a, "LANGUAGE"), StringComparer.Ordinal)
                    .ThenBy(a => Value(a, "NAME"), StringComparer.Ordinal).ToArray();
                foreach (var track in groupTracks)
                {
                    string name = Value(track, "NAME"), language = Value(track, "LANGUAGE");
                    if (name.Length == 0 || Value(track, "URI").Length == 0)
                        throw new InvalidOperationException("HLS audio rendition has no stable identity or input.");
                    var uri = Resolve(Value(track, "URI"), masterUri, validateUri);
                    string key = "hls:" + group + "\n" + language + "\n" + name;
                    int? trackChannels = int.TryParse(Value(track, "CHANNELS").Split('/')[0], NumberStyles.None,
                        CultureInfo.InvariantCulture, out var trackCount) && trackCount is > 0 and <= 32 ? trackCount : null;
                    // The parent's total BANDWIDTH is a conservative bound for
                    // this audio-only wrapper, not an invented audio measurement.
                    var content = "#EXTM3U\n#EXT-X-VERSION:6\n#EXT-X-STREAM-INF:BANDWIDTH="
                        + selected.Bitrate.ToString(CultureInfo.InvariantCulture) + ",CODECS="
                        + Quote(codecs.Single(codec => AudioCodec(codec) != null)) + "\n" + uri.AbsoluteUri + "\n";
                    tracks.Add(new(key, content, YouTubeAudioTracks.Clean(name),
                        language.Length == 0 ? null : language, trackChannels, Value(track, "DEFAULT") == "YES"));
                }
                var collisions = tracks.GroupBy(track => YouTubeAudioTracks.Index(track.Key))
                    .Where(items => items.Count() > 1).Select(items => items.Key).ToHashSet();
                tracks = tracks.Where(track => !collisions.Contains(YouTubeAudioTracks.Index(track.Key)))
                    .Take(YouTubeAudioTracks.MaximumTracks).ToList();
                if (tracks.Count == 0) throw new InvalidOperationException("HLS audio track identities are ambiguous.");
                // Exactly one native default; DEFAULT is not proof of Original.
                tracks = tracks.Select((track, index) => track with { IsDefault = index == 0 }).ToList();
                videoContent = "#EXTM3U\n#EXT-X-VERSION:6\n#EXT-X-STREAM-INF:BANDWIDTH="
                    + selected.Bitrate.ToString(CultureInfo.InvariantCulture) + ",RESOLUTION="
                    + selected.Width.ToString(CultureInfo.InvariantCulture) + "x" + selected.Height.ToString(CultureInfo.InvariantCulture)
                    + ",CODECS=" + Quote(codecs.Single(codec => VideoCodec(codec) != null))
                    + (FrameRate(selected.Attributes) is { } rate ? ",FRAME-RATE=" + rate.ToString(CultureInfo.InvariantCulture) : "")
                    + (range.Length > 0 ? ",VIDEO-RANGE=" + range : "") + "\n" + selected.Uri.AbsoluteUri + "\n";
            }
            return new YouTubeHlsSelection(output.ToString(), selected.Uri, audioUri,
                selected.Width, selected.Height, selected.Bitrate, FrameRate(selected.Attributes),
                videoCodecs[0]!, audioCodecs[0]!, channels,
                rendition == null || Value(rendition, "LANGUAGE").Length == 0 ? null : Value(rendition, "LANGUAGE"))
            {
                VideoContent = videoContent, AudioTracks = tracks
            };
        }

        internal static Dictionary<string, string> ParseAttributes(string input)
        {
            var values = new Dictionary<string, string>(StringComparer.Ordinal);
            var i = 0;
            while (i < input.Length)
            {
                var start = i;
                while (i < input.Length && (input[i] is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '-')) i++;
                if (i == start || i >= input.Length || input[i++] != '=') throw new InvalidOperationException("Invalid HLS attributes.");
                var key = input.Substring(start, i - start - 1);
                string value;
                if (i < input.Length && input[i] == '"')
                {
                    start = ++i;
                    while (i < input.Length && input[i] != '"') i++;
                    if (i >= input.Length) throw new InvalidOperationException("Unterminated HLS attribute.");
                    value = input.Substring(start, i++ - start);
                }
                else
                {
                    start = i;
                    while (i < input.Length && input[i] != ',') i++;
                    value = input.Substring(start, i - start);
                }
                if (!values.TryAdd(key, value) || value.Any(c => char.IsControl(c))) throw new InvalidOperationException("Unsafe or duplicate HLS attribute.");
                if (i < input.Length && input[i++] != ',') throw new InvalidOperationException("Invalid HLS attribute separator.");
                if (i == input.Length && input.EndsWith(',')) throw new InvalidOperationException("Incomplete HLS attributes.");
            }
            return values;
        }

        private static Uri Resolve(string value, Uri master, Func<string, Uri> validate) =>
            validate(new Uri(master, value).AbsoluteUri);
        private static string Value(IReadOnlyDictionary<string, string> attrs, string key) => attrs.TryGetValue(key, out var value) ? value : string.Empty;
        private static float? FrameRate(IReadOnlyDictionary<string, string> attrs) =>
            float.TryParse(Value(attrs, "FRAME-RATE"), NumberStyles.Float, CultureInfo.InvariantCulture, out var fps) && fps > 0 && fps <= 240 ? fps : null;
        private static string Quote(string value) => '"' + SafeToken(value) + '"';
        private static string SafeToken(string value) => value.Any(c => char.IsControl(c) || c == '"')
            ? throw new InvalidOperationException("Unsafe HLS attribute value.") : value;
        internal static string? VideoCodec(string value) => value.ToLowerInvariant() switch
        {
            var s when s.StartsWith("avc1") || s.StartsWith("avc3") || s == "h264" => "h264",
            var s when s.StartsWith("vp09") || s == "vp9" => "vp9",
            var s when s.StartsWith("av01") || s == "av1" => "av1",
            var s when s.StartsWith("hvc1") || s.StartsWith("hev1") || s == "hevc" => "hevc",
            _ => null
        };
        internal static string? AudioCodec(string value) => value.ToLowerInvariant() switch
        {
            var s when s.StartsWith("mp4a.40.") || s == "aac" => "aac",
            "opus" => "opus",
            "ac-3" or "ac3" => "ac3",
            "ec-3" or "eac3" => "eac3",
            _ => null
        };
    }
}
