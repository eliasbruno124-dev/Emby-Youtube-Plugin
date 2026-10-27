using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Persistence;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
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
            try
            {
                lock (Gates[(int)(id % Gates.Length)])
                {
                    var library = Plugin.ResolveService<ILibraryManager>();
                    var repository = Plugin.ResolveService<IItemRepository>();
                    var item = library?.GetItemById(id);
                    if (item == null || repository == null || !IsOwnedItem(item, videoId))
                        return;
                    var incoming = GetIncoming(item, videoId, out bool descriptionDerived);
                    if (incoming.Length == 0) return; // Absence is never a deletion request.
                    var existing = repository.GetChapters(item);
                    if (existing.Count > 4096) return;
                    var ordinary = existing.Where(c => c.MarkerType == MarkerType.Chapter).ToArray();
                    var oldHash = Fingerprint(ordinary);
                    var newHash = Fingerprint(incoming);
                    if (oldHash == newHash)
                    {
                        // Equivalent validated resolver data still upgrades the
                        // origin of our own description-derived chapter set.
                        // Never claim matching but externally owned chapters.
                        if (!descriptionDerived)
                        {
                            var matching = ReadState(id, videoId);
                            if (matching?.OwnedFingerprint == newHash && matching.SourceKind == "description")
                            {
                                matching.SourceKind = "resolver";
                                Store(Path.Combine(Plugin.DataPath!, "youtube-chapter-state",
                                    id.ToString(CultureInfo.InvariantCulture) + ".json"), matching);
                            }
                        }
                        return;
                    }
                    var root = Plugin.DataPath;
                    if (string.IsNullOrWhiteSpace(root)) return;
                    var directory = Path.Combine(root, "youtube-chapter-state");
                    Directory.CreateDirectory(directory);
                    if (new DirectoryInfo(directory).LinkTarget != null) return;
                    var path = Path.Combine(directory, id.ToString(CultureInfo.InvariantCulture) + ".json");
                    State? state = ReadState(id, videoId);
                    // A short-lived resolver cache expiring must not downgrade
                    // previously persisted resolver chapters to a description.
                    // Old ownership files have unknown origin, so preserve them.
                    if (descriptionDerived && ordinary.Length > 0 && state?.SourceKind != "description") return;
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
                    state.SourceKind = descriptionDerived ? "description" : "resolver";
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

        internal static void ApplyItemDto(BaseItem originalItem, BaseItemDto dto)
        {
            if (!YouTubeDirectPlayback.Enabled || originalItem.InternalId <= 0
                || !long.TryParse(dto.Id, NumberStyles.None, CultureInfo.InvariantCulture, out long id)
                || id != originalItem.InternalId) return;
            string? videoId = YouTubeImageProvider.TryGetVideoId(originalItem);
            if (string.IsNullOrEmpty(videoId) || !IsOwnedItem(originalItem, videoId)) return;
            // Both calls are local/cache-only. Never start or wait for a network
            // resolver flight while Emby's synchronous DTO service is executing.
            Synchronize(dto.Id, videoId);
            try
            {
                lock (Gates[(int)(id % Gates.Length)])
                {
                    var item = Plugin.ResolveService<ILibraryManager>()?.GetItemById(id);
                    var repository = Plugin.ResolveService<IItemRepository>();
                    if (item == null || repository == null || !IsOwnedItem(item, videoId)) return;
                    var existing = repository.GetChapters(item);
                    if (existing.Count > 4096) return;
                    var incoming = GetIncoming(item, videoId);
                    // Keep an already supplied DTO collection when neither the
                    // repository nor this plugin has new authoritative metadata.
                    var fallback = incoming.Length > 0 ? incoming : dto.Chapters?.ToArray() ?? Array.Empty<ChapterInfo>();
                    dto.Chapters = Preferred(existing, fallback, id, videoId).Select(CloneChapter).ToList();
                    ApplySources(dto, item, existing, id, videoId);
                }
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] Native item chapter selection unavailable (" + error.GetType().Name + ").");
            }
        }

        internal static void ApplyPreferredChapters(object response, string? itemId, string videoId)
        {
            if (!YouTubeDirectPlayback.Enabled || !long.TryParse(itemId, NumberStyles.None,
                    CultureInfo.InvariantCulture, out var id) || id <= 0)
                return;
            try
            {
                lock (Gates[(int)(id % Gates.Length)])
                {
                    var library = Plugin.ResolveService<ILibraryManager>();
                    var repository = Plugin.ResolveService<IItemRepository>();
                    var item = library?.GetItemById(id);
                    if (item == null || repository == null || !IsOwnedItem(item, videoId))
                        return;
                    var existing = repository.GetChapters(item);
                    if (existing.Count > 4096) return;
                    ApplySources(response, item, existing, id, videoId);
                }
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] Native playback chapter selection unavailable ("
                    + error.GetType().Name + ").");
            }
        }

        private static void ApplySources(object response, BaseItem item, List<ChapterInfo> existing, long id, string videoId)
        {
            var sources = response.GetType().GetProperty("MediaSources", BindingFlags.Public | BindingFlags.Instance)
                ?.GetValue(response) as IEnumerable;
            if (sources == null) return;
            foreach (var source in sources.Cast<object>().OfType<MediaSourceInfo>()
                .Where(source => string.Equals(source.Id, YouTubeDirectPlayback.SourceId(videoId), StringComparison.Ordinal)))
            {
                var fallback = source.Chapters?.Length > 0 ? source.Chapters : GetIncoming(item, videoId);
                source.Chapters = Preferred(existing, fallback, id, videoId).Select(CloneChapter).ToArray();
            }
        }

        private static IEnumerable<ChapterInfo> Preferred(List<ChapterInfo> existing, ChapterInfo[] fallback, long id, string videoId)
        {
            if (existing.Any(chapter => chapter.MarkerType == MarkerType.Chapter)) return existing;
            var state = ReadState(id, videoId);
            // An empty repository after plugin-owned chapters existed proves a
            // manual deletion. Mere emptiness before/after failed synchronization
            // does not: retain a fresh resolved source until persistence succeeds.
            if (state?.OwnedFingerprint is string fingerprint
                && fingerprint != Fingerprint(Array.Empty<ChapterInfo>())) return existing;
            return existing.Concat(fallback.Where(chapter => chapter.MarkerType == MarkerType.Chapter))
                .OrderBy(chapter => chapter.StartPositionTicks);
        }

        private static ChapterInfo[] GetIncoming(BaseItem item, string videoId) => GetIncoming(item, videoId, out _);

        private static ChapterInfo[] GetIncoming(BaseItem item, string videoId, out bool descriptionDerived)
        {
            var resolved = YouTubeDirectMetadata.GetChapters(videoId);
            descriptionDerived = resolved.Length == 0;
            if (resolved.Length > 0) return resolved;
            // A currently live item has no stable end boundary for a chapter
            // block; persisted plain descriptions otherwise need no new request.
            if (item.ExternalId?.StartsWith("LIVE_", StringComparison.Ordinal) == true) return Array.Empty<ChapterInfo>();
            return YouTubeDescriptionChapters.Read(item.Overview, item.RunTimeTicks);
        }

        private static bool IsOwnedItem(BaseItem item, string videoId)
        {
            if (videoId.Length != 11 || videoId.Any(c => c is not (>= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '-'))
                || !string.Equals(YouTubeImageProvider.TryGetVideoId(item), videoId, StringComparison.Ordinal)
                || !Uri.TryCreate(item.Path, UriKind.Absolute, out var uri) || uri.Scheme != "https" || uri.Port != 443
                || uri.UserInfo.Length != 0 || uri.Fragment.Length != 0 || item.Path.Any(char.IsWhiteSpace) || item.Path.Contains('\\')) return false;
            // Mapping.GetYouTubeWatchUrl persists the two player flags even in
            // native mode. They are part of our own stable item identity.
            bool watchQuery = uri.Query == "?v=" + videoId
                || uri.Query == "?v=" + videoId + "&playsinline=1&enablejsapi=1";
            return (uri.Host is "www.youtube.com" or "youtube.com") && uri.AbsolutePath == "/watch" && watchQuery
                || (uri.Host == "youtu.be" && uri.AbsolutePath == "/" + videoId && uri.Query.Length == 0);
        }

        private static State? ReadState(long id, string videoId)
        {
            var root = Plugin.DataPath;
            if (string.IsNullOrWhiteSpace(root)) return null;
            var directory = Path.Combine(root, "youtube-chapter-state");
            if (Directory.Exists(directory) && new DirectoryInfo(directory).LinkTarget != null)
                throw new InvalidDataException("Unsafe chapter state directory.");
            var path = Path.Combine(directory, id.ToString(CultureInfo.InvariantCulture) + ".json");
            if (!File.Exists(path)) return null;
            var info = new FileInfo(path);
            if (info.LinkTarget != null || info.Length > MaximumStateBytes)
                throw new InvalidDataException("Unsafe chapter state.");
            var state = JsonSerializer.Deserialize<State>(File.ReadAllBytes(path));
            if (state?.ItemId != id || state.VideoId != videoId || state.OwnedFingerprint is string fingerprint
                && (fingerprint.Length != 64 || fingerprint.Any(c => c is not (>= '0' and <= '9' or >= 'A' and <= 'F'))))
                throw new InvalidDataException("Invalid chapter ownership state.");
            return state;
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
            public string? SourceKind { get; set; }
            public ChapterInfo[] OriginalChapters { get; set; } = Array.Empty<ChapterInfo>();
        }
    }
}
