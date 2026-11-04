using System.Globalization;
using System.Net;
using System.Text;

namespace Emby.YouTubePlugin
{
    // Source captions use absolute video times. This is a format compatibility
    // boundary, not a seek/timeline mapper or proof of native client rendering.
    internal static class YouTubeCaptionNormalizer
    {
        internal const string CacheRevision = "plain-vtt-v3";
        internal const int MaximumPayloadBytes = 2 * 1024 * 1024;
        private const int MaximumLines = 100000;
        private const int MaximumCueCharacters = 64 * 1024;
        private const long MaximumTimestampMilliseconds = 7L * 24 * 60 * 60 * 1000;
        private static readonly UTF8Encoding StrictUtf8 = new(false, true);

        internal static bool TryNormalize(byte[]? payload, out byte[] normalized)
        {
            normalized = Array.Empty<byte>();
            if (payload == null || payload.Length == 0 || payload.Length > MaximumPayloadBytes)
                return false;
            string text;
            try { text = StrictUtf8.GetString(payload); }
            catch (DecoderFallbackException) { return false; }
            if (!SafeCharacters(text))
                return false;
            if (text[0] == '\uFEFF')
                text = text[1..];
            var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n');
            if (lines.Length < 4 || lines.Length > MaximumLines
                || !lines[0].StartsWith("WEBVTT", StringComparison.Ordinal)
                || (lines[0].Length > 6 && lines[0][6] != ' ' && lines[0][6] != '\t'))
                return false;
            var index = 0;
            while (index < lines.Length && lines[index].Length != 0)
            {
                // Mapped transport captions are not absolute source captions.
                // Silently dropping their map would change the timebase.
                if (lines[index].Length > 1024 || lines[index].Contains("-->", StringComparison.Ordinal)
                    || lines[index].StartsWith("X-TIMESTAMP-MAP", StringComparison.OrdinalIgnoreCase))
                    return false;
                index++;
            }
            if (index == lines.Length)
                return false;

            var output = new StringBuilder("WEBVTT\n\n");
            var cues = 0;
            long previousStart = -1;
            while (index < lines.Length)
            {
                while (index < lines.Length && lines[index].Length == 0) index++;
                if (index == lines.Length) break;
                var first = index;
                while (index < lines.Length && lines[index].Length != 0) index++;
                var end = index;
                if (lines[first] is "STYLE" or "REGION" or "NOTE"
                    || lines[first].StartsWith("NOTE ", StringComparison.Ordinal)
                    || lines[first].StartsWith("NOTE\t", StringComparison.Ordinal))
                    continue;

                var timing = lines[first].Contains("-->", StringComparison.Ordinal) ? first : first + 1;
                if (timing >= end || lines[first].Length > MaximumCueCharacters
                    || !TryTiming(lines[timing], out var startToken, out var endToken, out var startMs)
                    || startMs < previousStart)
                    return false;
                previousStart = startMs;
                var sourceText = new StringBuilder();
                for (var i = timing + 1; i < end; i++)
                {
                    if (lines[i].Contains("-->", StringComparison.Ordinal)
                        || sourceText.Length + lines[i].Length + 1 > MaximumCueCharacters)
                        return false;
                    if (i > timing + 1) sourceText.Append('\n');
                    sourceText.Append(lines[i]);
                }
                if (!TryPlainText(sourceText.ToString(), out var cueText))
                    return false;
                // YouTube ASR emits valid empty clearing windows, including
                // timing-only cues. Validate them above, then omit their empty
                // presentation; normal WebVTT text already ends at its own cue
                // endpoint. Never truncate another, possibly overlapping cue.
                if (cueText.Length == 0)
                    continue;
                // Do not merge overlapping ASR windows or deduplicate repeated
                // words: those operations can discard genuinely spoken content.
                if (output.Length + startToken.Length + endToken.Length + cueText.Length + 8 > MaximumPayloadBytes)
                    return false;
                output.Append(startToken).Append(" --> ").Append(endToken).Append('\n')
                    .Append(cueText).Append("\n\n");
                cues++;
            }
            var canonical = output.ToString();
            if (cues == 0 || StrictUtf8.GetByteCount(canonical) > MaximumPayloadBytes)
                return false;
            normalized = StrictUtf8.GetBytes(canonical);
            return true;
        }

        internal static bool IsCanonical(byte[] payload) =>
            TryNormalize(payload, out var normalized) && payload.AsSpan().SequenceEqual(normalized);

        private static bool TryTiming(string line, out string start, out string end, out long startMs)
        {
            start = end = string.Empty;
            startMs = 0;
            if (line.Length > 4096)
                return false;
            var separator = line.IndexOf("-->", StringComparison.Ordinal);
            if (separator <= 0 || line.IndexOf("-->", separator + 3, StringComparison.Ordinal) >= 0)
                return false;
            start = line[..separator].Trim();
            var after = line[(separator + 3)..].Trim();
            var whitespace = after.IndexOfAny(new[] { ' ', '\t' });
            end = whitespace < 0 ? after : after[..whitespace];
            // Geometry, regions and alignment are checked but not copied.
            return TryTimestamp(start, out startMs) && TryTimestamp(end, out var endMs) && endMs > startMs
                && (whitespace < 0 || ValidSettings(after[(whitespace + 1)..]));
        }

        private static bool ValidSettings(string settings)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var token in settings.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var colon = token.IndexOf(':');
                if (colon <= 0 || colon == token.Length - 1) return false;
                var name = token[..colon];
                var value = token[(colon + 1)..];
                if (!seen.Add(name)) return false;
                var components = value.Split(',');
                bool Percentage(string number) => number.EndsWith('%')
                    && number.Length <= 12 && number[..^1].All(c => c is >= '0' and <= '9' or '.')
                    && decimal.TryParse(number[..^1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var percent)
                    && percent is >= 0 and <= 100;
                switch (name)
                {
                    case "vertical":
                        if (value is not ("rl" or "lr")) return false;
                        break;
                    case "align":
                        if (value is not ("start" or "center" or "middle" or "end" or "left" or "right")) return false;
                        break;
                    case "size":
                        if (!Percentage(value)) return false;
                        break;
                    case "position":
                        if (components.Length > 2 || !Percentage(components[0])
                            || (components.Length == 2 && components[1] is not ("line-left" or "center" or "line-right" or "auto")))
                            return false;
                        break;
                    case "line":
                        if (components.Length > 2
                            || (components[0] != "auto" && !Percentage(components[0])
                                && (!int.TryParse(components[0], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var line)
                                    || line is < -10000 or > 10000))
                            || (components.Length == 2 && components[1] is not ("start" or "center" or "end")))
                            return false;
                        break;
                    case "region":
                        if (value.Length > 128 || !value.All(c => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z'
                            or >= '0' and <= '9' or '-' or '_' or '.')) return false;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        private static bool TryTimestamp(string value, out long milliseconds)
        {
            milliseconds = 0;
            var parts = value.Split(':');
            if (parts.Length is not (2 or 3)) return false;
            var seconds = parts[^1].Split('.');
            if (seconds.Length != 2 || seconds[0].Length != 2 || seconds[1].Length != 3
                || parts[^2].Length != 2 || (parts.Length == 3 && parts[0].Length is < 2 or > 3))
                return false;
            static bool Number(string token, out int number)
            {
                number = 0;
                return token.All(character => character is >= '0' and <= '9')
                    && int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out number);
            }
            if (!Number(parts[^2], out var minutes) || minutes > 59
                || !Number(seconds[0], out var secs) || secs > 59 || !Number(seconds[1], out var ms))
                return false;
            var hours = 0;
            if (parts.Length == 3 && !Number(parts[0], out hours)) return false;
            milliseconds = (hours * 3600L + minutes * 60L + secs) * 1000 + ms;
            return milliseconds <= MaximumTimestampMilliseconds;
        }

        private static bool TryPlainText(string source, out string text)
        {
            text = string.Empty;
            var stripped = new StringBuilder(source.Length);
            for (var index = 0; index < source.Length; index++)
            {
                var character = source[index];
                if (character != '<')
                {
                    stripped.Append(character);
                    continue;
                }
                var close = source.IndexOf('>', index + 1);
                if (close < 0)
                    return false;
                var tag = source[(index + 1)..close];
                if (tag.Length > 1024 || tag.Contains('<') || tag.Contains('\n'))
                    return false;
                if (tag.Length > 0 && tag[0] is >= '0' and <= '9')
                {
                    if (!TryTimestamp(tag, out _)) return false;
                }
                else
                {
                    var content = tag.StartsWith('/') ? tag[1..] : tag;
                    var boundary = content.IndexOfAny(new[] { '.', ' ', '\t', '/' });
                    var name = boundary < 0 ? content : content[..boundary];
                    if (name is "br")
                        stripped.Append('\n');
                    else if (name is not ("b" or "i" or "u" or "c" or "v" or "lang" or "ruby" or "rt" or "font"))
                        return false;
                }
                index = close;
            }
            // Decode entities only after tags have been removed, so escaped
            // literal markup stays visible text, never an active tag.
            var decoded = WebUtility.HtmlDecode(stripped.ToString())
                .Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
            if (!SafeCharacters(decoded))
                return false;
            var escaped = new StringBuilder(decoded.Length);
            var meaningful = false;
            foreach (var line in decoded.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (meaningful) escaped.Append('\n');
                foreach (var character in line)
                {
                    switch (character)
                    {
                        case '&': escaped.Append("&amp;"); break;
                        case '<': escaped.Append("&lt;"); break;
                        case '>': escaped.Append("&gt;"); break;
                        default: escaped.Append(character); break;
                    }
                }
                meaningful = true;
            }
            text = escaped.ToString();
            // Empty is valid cue content, not invalid markup. The caller omits
            // it and still rejects an entire track with no visible-text cues.
            return true;
        }

        private static bool SafeCharacters(string text) => text.All(character =>
            (!char.IsControl(character) || character is '\t' or '\r' or '\n')
            && character is not (>= '\u202A' and <= '\u202E')
            && character is not (>= '\u2066' and <= '\u2069'));
    }
}
