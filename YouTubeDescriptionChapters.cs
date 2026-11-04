using MediaBrowser.Model.Entities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Emby.YouTubePlugin
{
    // These are explicitly written timestamp/title navigation markers from an
    // item's description, not proof that YouTube enabled automatic chapters.
    // No network, inferred titles, fabricated zero marker or free-text seeking.
    internal static class YouTubeDescriptionChapters
    {
        private const long MinimumChapterTicks = 10 * TimeSpan.TicksPerSecond;
        private const long MaximumDurationTicks = 31L * 24 * 60 * 60 * TimeSpan.TicksPerSecond;
        private static readonly Regex MarkerLine = new(
            @"\A[ \t]*(?<time>[0-9]{1,4}:[0-9]{2}(?::[0-9]{2})?)[ \t]+(?:[-–—][ \t]+)?(?<title>[^\r\n]+)\z",
            RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
        private static readonly Regex TimePrefix = new(@"\A[ \t]*[0-9]+:",
            RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));

        internal static ChapterInfo[] Read(string? description, long? durationTicks)
        {
            if (string.IsNullOrWhiteSpace(description) || description.Length > YouTubeDirectMetadata.MaximumDescriptionCharacters
                || durationTicks is not > 0 || durationTicks > MaximumDurationTicks)
                return Array.Empty<ChapterInfo>();
            var lines = description.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            if (lines.Length > 2048) return Array.Empty<ChapterInfo>();
            var chapters = new List<ChapterInfo>();
            bool ended = false;
            foreach (var line in lines)
            {
                var match = MarkerLine.Match(line);
                if (!match.Success)
                {
                    // A malformed timestamp line is not a gap we can silently
                    // remove from a chapter block. Multiple blocks are ambiguous.
                    if (TimePrefix.IsMatch(line)) return Array.Empty<ChapterInfo>();
                    if (chapters.Count > 0) ended = true;
                    continue;
                }
                if (ended || chapters.Count >= YouTubeDirectMetadata.MaximumChapters)
                    return Array.Empty<ChapterInfo>();
                var parts = match.Groups["time"].Value.Split(':');
                if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out int first)
                    || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int second)
                    || second > 59) return Array.Empty<ChapterInfo>();
                int last = 0;
                if (parts.Length == 3 && (!int.TryParse(parts[2], NumberStyles.None,
                        CultureInfo.InvariantCulture, out last) || last > 59))
                    return Array.Empty<ChapterInfo>();
                long seconds = parts.Length == 3 ? first * 3600L + second * 60L + last : first * 60L + second;
                long ticks = seconds * TimeSpan.TicksPerSecond;
                if (chapters.Count == 0 && ticks != 0 || ticks >= durationTicks
                    || chapters.Count > 0 && ticks - chapters[^1].StartPositionTicks < MinimumChapterTicks)
                    return Array.Empty<ChapterInfo>();
                var title = match.Groups["title"].Value.Trim();
                // Avoid treating linked timestamps or markup as authoritative
                // labels. Channel overview descriptions are already plain text.
                if (title.Length == 0 || title.Length > YouTubeDirectMetadata.MaximumChapterTitleCharacters
                    || title.Any(char.IsControl) || title.Contains('<') || title.Contains('>')
                    || title.Contains("://", StringComparison.Ordinal))
                    return Array.Empty<ChapterInfo>();
                chapters.Add(new ChapterInfo
                {
                    Name = title, StartPositionTicks = ticks, ChapterIndex = chapters.Count,
                    MarkerType = MarkerType.Chapter
                });
            }
            return chapters.Count >= 3 && durationTicks - chapters[^1].StartPositionTicks >= MinimumChapterTicks
                ? chapters.ToArray() : Array.Empty<ChapterInfo>();
        }
    }
}
