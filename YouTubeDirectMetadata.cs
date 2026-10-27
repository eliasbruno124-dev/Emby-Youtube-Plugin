using MediaBrowser.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin
{
    // Only this allowlisted, in-memory projection survives beyond resolver data.
    // Never retain extraction JSON or source URL, header or cookie fields.
    internal static class YouTubeDirectMetadata
    {
        internal const int MaximumEntries = 128;
        internal const int MaximumBytes = 4 * 1024 * 1024;
        internal const int MaximumDescriptionCharacters = 32768;
        internal const int MaximumChapters = 256;
        internal const int MaximumChapterTitleCharacters = 256;
        internal static readonly TimeSpan Freshness = TimeSpan.FromMinutes(15);
        private const decimal MaximumDurationSeconds = 31 * 24 * 60 * 60;
        private static readonly MetadataCache Cache = new(() => DateTime.UtcNow);

        // Call only after a new extraction has passed the resolver's validation.
        // Reading a resolver cache entry must never renew this capture timestamp.
        internal static void Record(string videoId, JsonElement metadata) => Cache.Record(videoId, metadata);

        internal static bool TryGet(string videoId, out Snapshot metadata) => Cache.TryGet(videoId, out metadata);

        internal static ChapterInfo[] GetChapters(string videoId) =>
            TryGet(videoId, out var metadata) ? metadata.GetChapters() : Array.Empty<ChapterInfo>();

        internal sealed class Snapshot
        {
            private readonly Chapter[] _chapters;

            internal Snapshot(DateTime capturedUtc, string? description, long? views, long? likes,
                long? comments, long? durationTicks, Chapter[] chapters)
            {
                CapturedUtc = capturedUtc;
                Description = description;
                ViewCount = views;
                LikeCount = likes;
                CommentCount = comments;
                DurationTicks = durationTicks;
                _chapters = chapters;
                // Conservative UTF-16 payload plus object/collection overhead.
                SizeBytes = 512 + (description?.Length ?? 0) * 2
                    + chapters.Sum(chapter => 192 + chapter.Name.Length * 2);
            }

            internal DateTime CapturedUtc { get; }
            internal string? Description { get; }
            internal long? ViewCount { get; }
            internal long? LikeCount { get; }
            internal long? CommentCount { get; }
            internal long? DurationTicks { get; }
            internal int SizeBytes { get; }

            // Native DTOs are mutable. Never hand out the cache's backing data.
            internal ChapterInfo[] GetChapters() => _chapters.Select((chapter, index) => new ChapterInfo
            {
                StartPositionTicks = chapter.StartTicks,
                Name = chapter.Name,
                ChapterIndex = index,
                MarkerType = MarkerType.Chapter
            }).ToArray();
        }

        internal sealed class MetadataCache
        {
            private readonly object _gate = new();
            private readonly Dictionary<string, Snapshot> _entries = new(StringComparer.Ordinal);
            private readonly Func<DateTime> _utcNow;
            private int _bytes;

            internal MetadataCache(Func<DateTime> utcNow) => _utcNow = utcNow;

            internal void Record(string videoId, JsonElement metadata)
            {
                if (!IsVideoId(videoId) || metadata.ValueKind != JsonValueKind.Object
                    || !metadata.TryGetProperty("id", out var id) || id.ValueKind != JsonValueKind.String
                    || ReadString(id) != videoId) return;

                // Capture time is measured here, not taken from upload/release dates.
                var now = _utcNow();
                long? duration = ReadTime(metadata, "duration", allowZero: false);
                var snapshot = new Snapshot(now, ReadDescription(metadata),
                    ReadCount(metadata, "view_count"), ReadCount(metadata, "like_count"),
                    ReadCount(metadata, "comment_count"), duration, ReadChapters(metadata, duration));
                lock (_gate)
                {
                    // Parsing happens outside the lock. Use the current clock
                    // for eviction so an older concurrent parser cannot evict
                    // a newer entry as though the clock had moved backwards.
                    var cacheNow = _utcNow();
                    Prune(cacheNow);
                    if (now > cacheNow || cacheNow - now >= Freshness) return;
                    // Concurrent parsing must not replace a newer observation.
                    if (_entries.TryGetValue(videoId, out var previous) && previous.CapturedUtc > now) return;
                    Remove(videoId);
                    _entries.Add(videoId, snapshot);
                    _bytes += snapshot.SizeBytes;
                    while (_entries.Count > MaximumEntries || _bytes > MaximumBytes)
                        Remove(_entries.OrderBy(pair => pair.Value.CapturedUtc)
                            .ThenBy(pair => pair.Key, StringComparer.Ordinal).First().Key);
                }
            }

            internal bool TryGet(string videoId, out Snapshot metadata)
            {
                lock (_gate)
                {
                    Prune(_utcNow());
                    if (videoId != null && _entries.TryGetValue(videoId, out metadata!)) return true;
                    metadata = null!;
                    return false;
                }
            }

            private void Prune(DateTime now)
            {
                foreach (var pair in _entries.ToArray())
                    if (now < pair.Value.CapturedUtc || now - pair.Value.CapturedUtc >= Freshness)
                        Remove(pair.Key);
            }

            private void Remove(string videoId)
            {
                if (_entries.Remove(videoId, out var removed)) _bytes -= removed.SizeBytes;
            }
        }

        internal sealed record Chapter(long StartTicks, string Name);

        private static bool IsVideoId(string value) => value is { Length: 11 } && value.All(character =>
            character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '-');

        private static long? ReadCount(JsonElement metadata, string name) =>
            metadata.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number
            && value.TryGetInt64(out long count) && count >= 0 ? count : null;

        private static string? ReadDescription(JsonElement metadata)
        {
            if (!metadata.TryGetProperty("description", out var value) || value.ValueKind != JsonValueKind.String)
                return null;
            var description = ReadString(value);
            if (description == null) return null;
            // Explicitly empty is authoritative; missing/null still falls back
            // to the API rather than erasing a useful description.
            return LimitText(description.Replace("\r\n", "\n").Replace('\r', '\n'), MaximumDescriptionCharacters);
        }

        private static long? ReadTime(JsonElement metadata, string name, bool allowZero)
        {
            var seconds = ReadSeconds(metadata, name, allowZero);
            if (!seconds.HasValue) return null;
            var ticks = ToTicks(seconds.Value);
            return !allowZero && ticks == 0 ? null : ticks;
        }

        private static decimal? ReadSeconds(JsonElement metadata, string name, bool allowZero)
        {
            if (!metadata.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.Number
                || !value.TryGetDecimal(out decimal seconds) || seconds < 0
                || seconds > MaximumDurationSeconds || (!allowZero && seconds == 0)) return null;
            return seconds;
        }

        // Millisecond precision is sufficient for chapter seeks, stable across
        // floating-point JSON implementations, and far below Int64 overflow.
        private static long ToTicks(decimal seconds) =>
            (long)decimal.Round(seconds * 1000m, 0, MidpointRounding.AwayFromZero) * TimeSpan.TicksPerMillisecond;

        private static Chapter[] ReadChapters(JsonElement metadata, long? duration)
        {
            if (!duration.HasValue || !metadata.TryGetProperty("chapters", out var chapters)
                || chapters.ValueKind != JsonValueKind.Array || chapters.GetArrayLength() > 1024)
                return Array.Empty<Chapter>();

            var durationSeconds = ReadSeconds(metadata, "duration", allowZero: false)!.Value;
            var result = new List<Chapter>();
            foreach (var chapter in chapters.EnumerateArray())
            {
                if (chapter.ValueKind != JsonValueKind.Object) continue;
                var start = ReadSeconds(chapter, "start_time", allowZero: true);
                if (!start.HasValue || start >= durationSeconds || ToTicks(start.Value) >= duration) continue;
                if (chapter.TryGetProperty("end_time", out var endValue) && endValue.ValueKind != JsonValueKind.Null)
                {
                    var end = ReadSeconds(chapter, "end_time", allowZero: false);
                    if (!end.HasValue || end <= start || end > durationSeconds
                        || ToTicks(end.Value) <= ToTicks(start.Value)) continue;
                }
                if (!chapter.TryGetProperty("title", out var title) || title.ValueKind != JsonValueKind.String) continue;
                var text = ReadString(title) ?? string.Empty;
                var normalized = new StringBuilder(Math.Min(text.Length, MaximumChapterTitleCharacters));
                foreach (char character in text)
                {
                    if (normalized.Length == MaximumChapterTitleCharacters) break;
                    normalized.Append(char.IsControl(character) || char.IsWhiteSpace(character) ? ' ' : character);
                }
                var name = LimitText(normalized.ToString(), MaximumChapterTitleCharacters).Trim();
                if (name.Length == 0) continue; // No fabricated chapter names/times.
                result.Add(new Chapter(ToTicks(start.Value), name));
            }
            return result.OrderBy(chapter => chapter.StartTicks).GroupBy(chapter => chapter.StartTicks)
                .Select(group => group.First()).Take(MaximumChapters).ToArray();
        }

        private static string LimitText(string value, int maximum)
        {
            int length = Math.Min(value.Length, maximum);
            if (length > 0 && char.IsHighSurrogate(value[length - 1])) length--;
            return value.Substring(0, length);
        }

        private static string? ReadString(JsonElement value)
        {
            // Invalid escaped UTF-16 in an optional upstream field must not
            // turn otherwise playable extraction data into a playback failure.
            try { return value.GetString(); }
            catch (InvalidOperationException) { return null; }
        }
    }
}
