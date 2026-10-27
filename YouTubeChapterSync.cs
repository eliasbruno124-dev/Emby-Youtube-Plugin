using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Persistence;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin
{
    // Only the exact validated channel item is touched. Native intro/credit
    // markers and chapters edited outside this plugin are never replaced.
    internal static class YouTubeChapterSync
    {
        private static readonly object[] Gates = Enumerable.Range(0, 16).Select(_ => new object()).ToArray();
        private static readonly object StateAdmissionGate = new();
        private const int MaximumItems = 4096;
        private const int MaximumStateBytes = 256 * 1024;

        internal static void Synchronize(string? itemId, string videoId)
        {
            if (!YouTubeDirectPlayback.Enabled || !long.TryParse(itemId, NumberStyles.None,
                    CultureInfo.InvariantCulture, out var id) || id <= 0)
                return;
            var incoming = YouTubeDirectMetadata.GetChapters(videoId);
            if (incoming.Length == 0) return; // An unavailable extraction is not a deletion request.
            try
            {
                lock (Gates[(int)(id % Gates.Length)])
                {
                    var library = Plugin.ResolveService<ILibraryManager>();
                    var repository = Plugin.ResolveService<IItemRepository>();
                    var item = library?.GetItemById(id);
                    if (item == null || repository == null
                        || !string.Equals(YouTubeImageProvider.TryGetVideoId(item), videoId, StringComparison.Ordinal))
                        return;
                    var existing = repository.GetChapters(item);
                    if (existing.Count > 4096) return;
                    var ordinary = existing.Where(c => c.MarkerType == MarkerType.Chapter).ToArray();
                    var oldHash = Fingerprint(ordinary);
                    var newHash = Fingerprint(incoming);
                    if (oldHash == newHash) return;
                    var root = Plugin.DataPath;
                    if (string.IsNullOrWhiteSpace(root)) return;
                    var directory = Path.Combine(root, "youtube-chapter-state");
                    Directory.CreateDirectory(directory);
                    if (new DirectoryInfo(directory).LinkTarget != null) return;
                    var path = Path.Combine(directory, id.ToString(CultureInfo.InvariantCulture) + ".json");
                    State? state = null;
                    if (File.Exists(path))
                    {
                        var info = new FileInfo(path);
                        if (info.LinkTarget != null || info.Length > MaximumStateBytes) return;
                        state = JsonSerializer.Deserialize<State>(File.ReadAllBytes(path));
                        if (state?.VideoId != videoId || state.ItemId != id) return;
                    }
                    // Without an ownership record, existing ordinary chapters
                    // are user/provider data. A later edit, including deleting
                    // every ordinary chapter, also wins. A rollback-only record
                    // with no owned fingerprint has not yet claimed any chapters.
                    if (state?.OwnedFingerprint is string ownedFingerprint)
                    {
                        if (ownedFingerprint != oldHash) return;
                    }
                    else if (ordinary.Length != 0) return;
                    var merged = existing.Where(c => c.MarkerType != MarkerType.Chapter)
                        .Concat(incoming).OrderBy(c => c.StartPositionTicks).ToList();
                    state ??= new State { ItemId = id, VideoId = videoId, OriginalChapters = existing.ToArray() };
                    // Persist the initial per-item rollback before the first DB
                    // write. No whole-library rewrite or metadata scan is used.
                    if (!File.Exists(path))
                    {
                        // Different item stripes can arrive concurrently. Count
                        // and admit new ownership records under one bounded lock.
                        lock (StateAdmissionGate)
                        {
                            if (Directory.EnumerateFiles(directory, "*.json").Take(MaximumItems).Count() >= MaximumItems)
                                return;
                            Store(path, state);
                        }
                    }
                    // Local locks cannot serialize an external metadata editor.
                    // Recheck after the rollback flush rather than knowingly
                    // replacing chapters changed while the backup was written.
                    if (Fingerprint(repository.GetChapters(item)) != Fingerprint(existing)) return;
                    repository.SaveChapters(id, merged);
                    state.OwnedFingerprint = newHash;
                    Store(path, state);
                }
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                // SaveChapters can succeed before the ownership-state flush
                // fails, so do not claim the repository was left unchanged.
                YouTubeChannel.LogPublic("[YT] Native chapter synchronization incomplete ("
                    + error.GetType().Name + ").");
            }
        }

        internal static void ApplyPreferredChapters(object response, string? itemId, string videoId)
        {
            if (!YouTubeDirectPlayback.Enabled || !long.TryParse(itemId, NumberStyles.None,
                    CultureInfo.InvariantCulture, out var id) || id <= 0)
                return;
            try
            {
                var sources = response.GetType().GetProperty("MediaSources", BindingFlags.Public | BindingFlags.Instance)
                    ?.GetValue(response) as IEnumerable;
                if (sources == null) return;
                var owned = sources.Cast<object>().OfType<MediaSourceInfo>()
                    .Where(source => string.Equals(source.Id, YouTubeDirectPlayback.SourceId(videoId), StringComparison.Ordinal))
                    .ToArray();
                if (owned.Length == 0) return;
                lock (Gates[(int)(id % Gates.Length)])
                {
                    var library = Plugin.ResolveService<ILibraryManager>();
                    var repository = Plugin.ResolveService<IItemRepository>();
                    var item = library?.GetItemById(id);
                    if (item == null || repository == null
                        || !string.Equals(YouTubeImageProvider.TryGetVideoId(item), videoId, StringComparison.Ordinal))
                        return;
                    var existing = repository.GetChapters(item);
                    if (existing.Count > 4096) return;
                    // Source-provider metadata must not shadow a manual edit or
                    // deletion that Synchronize deliberately preserved in Emby.
                    // Empty is authoritative too; keep native marker entries.
                    foreach (var source in owned)
                        source.Chapters = existing.Select(CloneChapter).ToArray();
                }
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] Native playback chapter selection unavailable ("
                    + error.GetType().Name + ").");
            }
        }

        private static ChapterInfo CloneChapter(ChapterInfo chapter) => new()
        {
            StartPositionTicks = chapter.StartPositionTicks,
            Name = chapter.Name,
            MarkerType = chapter.MarkerType,
            ChapterIndex = chapter.ChapterIndex,
            ImagePath = chapter.ImagePath,
            ImageTag = chapter.ImageTag,
            ImageDateModified = chapter.ImageDateModified
        };

        internal static string Fingerprint(IEnumerable<ChapterInfo> chapters)
        {
            // Include image/marker identity so an external edit cannot silently
            // become plugin-owned just because its title and position match.
            var value = chapters.Select(c => new
            {
                c.StartPositionTicks, c.Name, c.MarkerType, c.ChapterIndex,
                c.ImagePath, c.ImageTag, c.ImageDateModified
            }).ToArray();
            return Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(value)));
        }

        private static void Store(string path, State state)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(state);
            if (bytes.Length > MaximumStateBytes) throw new InvalidDataException("Chapter state exceeds limit.");
            var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var file = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    file.Write(bytes);
                    file.Flush(true);
                }
                if (File.Exists(path) && new FileInfo(path).LinkTarget != null)
                    throw new InvalidDataException("Unsafe chapter state.");
                File.Move(temporary, path, true);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }

        private sealed class State
        {
            public long ItemId { get; set; }
            public string VideoId { get; set; } = "";
            public string? OwnedFingerprint { get; set; }
            public ChapterInfo[] OriginalChapters { get; set; } = Array.Empty<ChapterInfo>();
        }
    }
}
