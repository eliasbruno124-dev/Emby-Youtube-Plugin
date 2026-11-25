using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Data;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin
{
    // Fills YouTube Music playlists when someone opens them. Emby never asks
    // a channel for a folder's items while a user browses; it only shows what
    // its last channel scan stored. A playlist folder therefore stays empty
    // until a user opens it (or the section listing it). A Harmony prefix on
    // Folder.GetItemsInternal sees that request, marks the playlists as
    // wanted and starts a channel refresh limited to that folder. From then
    // on every scan keeps the playlist current, until nobody opened it for
    // 30 days; then the next scan empties it again.
    //
    // Opening a section also marks its first 12 playlists, so they are
    // usually filled by the time one is tapped; marks that only came from a
    // section expire after 3 days and are dropped first. A playlist opened
    // before it was filled shows its videos when opened again a few seconds
    // later. The list lives in Emby's data folder: the plugin cache is wiped
    // on every plugin update, and an empty list would make the next scan
    // delete the videos (and watched state) of every filled playlist.
    internal static class YouTubeMusicBrowse
    {
        private static readonly string PlaylistPrefix = YouTubeMusicGenres.PlaylistType + YouTubeChannel.FolderSeparatorValue;
        private static readonly string SectionPrefix = YouTubeMusicGenres.SectionType + YouTubeChannel.FolderSeparatorValue;
        private static readonly TimeSpan KeepOpened = TimeSpan.FromDays(30);
        private static readonly TimeSpan KeepPrefetched = TimeSpan.FromDays(3);
        private static readonly TimeSpan TouchInterval = TimeSpan.FromHours(1);
        private const int SectionPrefetch = 12;
        // An opened playlist costs about 2 quota units a day (playlistItems
        // and videos.list, cached for a day).
        private const int MaxWanted = 300;
        private const string StateFile = "youtube-music-opened.json";

        internal sealed class Mark
        {
            public DateTime Last { get; set; }
            // Opened itself, not only listed in an opened section.
            public bool Opened { get; set; }
        }

        private static readonly object Sync = new();
        private static Dictionary<string, Mark>? _wanted; // playlist id -> mark
        // Playlists served with videos since Emby started. A marked playlist
        // that is opened again without having been served is loaded again
        // (its first load may have failed or waited behind a scan).
        private static readonly ConcurrentDictionary<string, byte> Served = new(StringComparer.Ordinal);
        private static bool _dirty;
        private static readonly ConcurrentDictionary<string, int> Pending = new(StringComparer.Ordinal);
        private static int _workerRunning;

        private static bool Alive(Mark mark, DateTime now) => now - mark.Last < (mark.Opened ? KeepOpened : KeepPrefetched);

        internal static bool IsWanted(string playlistId)
        {
            lock (Sync)
                return Load().TryGetValue(playlistId, out var mark) && Alive(mark, DateTime.UtcNow);
        }

        internal static void NoteServed(string playlistId) => Served[playlistId] = 0;

        internal static int Install(object harmony, Type harmonyMethodType)
        {
            try
            {
                var target = typeof(Folder).GetMethod("GetItemsInternal",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                    new[] { typeof(InternalItemsQuery), typeof(IDataContext), typeof(CancellationToken) }, null);
                var patch = harmony.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(method => method.Name == "Patch"
                        && method.GetParameters().Length >= 3
                        && typeof(MethodBase).IsAssignableFrom(method.GetParameters()[0].ParameterType)
                        && method.GetParameters().Any(parameter => parameter.Name == "prefix"));
                if (target == null || patch == null)
                {
                    YouTubeChannel.LogPublic("[YT] YouTube Music playlists cannot load on open (Folder.GetItemsInternal not found).");
                    return 0;
                }
                EmbyPatchSafety.RequirePatchable(new[] { target });
                var prefix = Activator.CreateInstance(harmonyMethodType,
                    typeof(YouTubeMusicBrowse).GetMethod(nameof(OpenPrefix), BindingFlags.NonPublic | BindingFlags.Static)!);
                var parameters = patch.GetParameters();
                var arguments = new object?[parameters.Length];
                arguments[0] = target;
                for (int i = 1; i < parameters.Length; i++)
                    if (parameters[i].Name == "prefix") arguments[i] = prefix;
                patch.Invoke(harmony, arguments);
                return 1;
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] YouTube Music playlists cannot load on open (" + error.GetType().Name + ").");
                return 0;
            }
        }

        // Runs for every folder listing in Emby: return at once unless a user
        // lists a YouTube Music section or playlist. Counting children for a
        // card (Limit = 0) is not opening the folder.
        private static void OpenPrefix(Folder __instance, InternalItemsQuery __0)
        {
            try
            {
                var externalId = __instance?.ExternalId;
                if (externalId == null || __0?.User == null || __0.Limit == 0) return;
                if (!externalId.StartsWith(PlaylistPrefix, StringComparison.Ordinal)
                    && !externalId.StartsWith(SectionPrefix, StringComparison.Ordinal)) return;
                if (Plugin.Instance?.Options.ShowMusicGenres != true) return;
                Opened(externalId);
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] YouTube Music open hook failed (" + error.GetType().Name + ").");
            }
        }

        internal static void Opened(string externalId)
        {
            var now = DateTime.UtcNow;
            List<string> ids;
            int depth;
            bool direct;
            if (externalId.StartsWith(PlaylistPrefix, StringComparison.Ordinal))
            {
                var id = externalId.Substring(PlaylistPrefix.Length);
                if (!YouTubeMusicCatalog.IsPlaylistId(id)) return;
                ids = new List<string> { id };
                depth = 2; // channel root, then this playlist
                direct = true;
            }
            else
            {
                var section = YouTubeMusicGenres.PeekSection(externalId.Substring(SectionPrefix.Length), Plugin.Instance?.Options);
                if (section == null || section.Videos) return;
                ids = section.Entries.Take(SectionPrefetch).Select(entry => entry.Id).ToList();
                depth = 3; // channel root, the section, its playlists
                direct = false;
            }

            var needsFill = false;
            lock (Sync)
            {
                var wanted = Load();
                foreach (var id in ids)
                {
                    var known = wanted.TryGetValue(id, out var mark) && Alive(mark!, now);
                    needsFill |= !known || !Served.ContainsKey(id);
                    if (!known)
                    {
                        wanted[id] = new Mark { Last = now, Opened = direct };
                        _dirty = true;
                    }
                    else if (direct && !mark!.Opened || now - mark!.Last > TouchInterval)
                    {
                        mark.Last = now;
                        mark.Opened |= direct;
                        _dirty = true;
                    }
                }
                if (wanted.Count > MaxWanted)
                    foreach (var old in wanted.OrderBy(pair => pair.Value.Opened).ThenBy(pair => pair.Value.Last)
                                 .Take(wanted.Count - MaxWanted).Select(pair => pair.Key).ToList())
                        wanted.Remove(old);
            }

            if (needsFill)
            {
                YouTubeChannel.LogPublic($"[YT] YouTube Music: loading {(depth == 2 ? "playlist" : "section")} {externalId} after it was opened.");
                Pending.AddOrUpdate(externalId, depth, (_, previous) => Math.Max(previous, depth));
            }
            if (needsFill || _dirty) StartWorker();
        }

        private static void StartWorker()
        {
            if (Interlocked.CompareExchange(ref _workerRunning, 1, 0) == 0)
                _ = Task.Run(RunAsync);
        }

        private static async Task RunAsync()
        {
            try
            {
                Save();
                while (true)
                {
                    // A single opened playlist first: someone is looking at it.
                    var next = Pending.OrderBy(pair => pair.Value).Select(pair => pair.Key).FirstOrDefault();
                    if (next == null || !Pending.TryRemove(next, out var depth)) break;
                    await ChannelRefreshInvoker.RefreshFolderAsync(next, depth).ConfigureAwait(false);
                    Save();
                }
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] YouTube Music loading failed (" + error.GetType().Name + ").");
            }
            finally
            {
                Volatile.Write(ref _workerRunning, 0);
                if (!Pending.IsEmpty) StartWorker();
            }
        }

        private static string? StatePath()
        {
            var root = Plugin.DataPath;
            return string.IsNullOrEmpty(root) ? null : Path.Combine(root, StateFile);
        }

        // Caller holds Sync.
        private static Dictionary<string, Mark> Load()
        {
            if (_wanted != null) return _wanted;
            var result = new Dictionary<string, Mark>(StringComparer.Ordinal);
            var path = StatePath();
            try
            {
                if (path != null && File.Exists(path))
                {
                    var stored = JsonSerializer.Deserialize<Dictionary<string, Mark>>(File.ReadAllText(path, Encoding.UTF8));
                    if (stored != null)
                        foreach (var pair in stored)
                        {
                            if (pair.Value == null || !YouTubeMusicCatalog.IsPlaylistId(pair.Key)) continue;
                            pair.Value.Last = DateTime.SpecifyKind(pair.Value.Last, DateTimeKind.Utc);
                            if (Alive(pair.Value, DateTime.UtcNow)) result[pair.Key] = pair.Value;
                        }
                }
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException or NotSupportedException)
            {
                // Keep the unreadable file for inspection instead of
                // overwriting it with the next save.
                YouTubeChannel.LogPublic("[YT] YouTube Music: opened playlists could not be read (" + error.GetType().Name + ").");
                if (path != null) try { File.Move(path, path + ".bad", overwrite: true); } catch { }
            }
            return _wanted = result;
        }

        private static void Save()
        {
            string json;
            lock (Sync)
            {
                if (!_dirty || _wanted == null) return;
                foreach (var expired in _wanted.Where(pair => !Alive(pair.Value, DateTime.UtcNow)).Select(pair => pair.Key).ToList())
                    _wanted.Remove(expired);
                json = JsonSerializer.Serialize(_wanted);
                _dirty = false;
            }
            string? temp = null;
            try
            {
                var path = StatePath();
                if (path == null) return;
                temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(temp, json, new UTF8Encoding(false));
                File.Move(temp, path, overwrite: true);
                temp = null;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                lock (Sync) _dirty = true;
                YouTubeChannel.LogPublic("[YT] YouTube Music: opened playlists could not be saved (" + error.GetType().Name + ").");
            }
            finally
            {
                if (temp != null) try { File.Delete(temp); } catch { }
            }
        }
    }
}
