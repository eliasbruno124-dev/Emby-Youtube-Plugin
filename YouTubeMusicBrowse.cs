using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Data;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin
{
    // Fills YouTube Music playlists when someone opens them. Emby never asks
    // a channel for a folder's items while a user browses; it only shows what
    // its last channel scan stored. A Harmony prefix on Folder.GetItemsInternal
    // sees a user open a playlist: if the playlist has no stored videos yet,
    // the prefix stores them right then (ChannelRefreshInvoker.
    // FillFolderDirectAsync, about a second) and lets Emby read the folder
    // through a fresh database connection, so the first tap already shows the
    // videos. Opening a section stores its first 12 playlists in the
    // background. Every channel scan then keeps opened playlists current,
    // until nobody opened them for 30 days (3 days for playlists that were
    // only listed in an opened section); then the next scan empties them.
    //
    // The list of opened playlists lives in Emby's data folder: the plugin
    // cache is wiped on every plugin update, and an empty list would make the
    // next scan delete the videos (and watched state) of every playlist.
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
        // Playlist -> until when it is not asked again: the Data API did not
        // hand it out (deleted, private, error; failed requests are not
        // cached, so every scan would pay again).
        private static readonly ConcurrentDictionary<string, DateTime> Failed = new(StringComparer.Ordinal);
        // Playlist -> until when a tap does not load it again: it had no
        // playable video (all private, deleted or age restricted).
        private static readonly ConcurrentDictionary<string, DateTime> Empty = new(StringComparer.Ordinal);
        private static readonly TimeSpan FailedPause = TimeSpan.FromHours(6);
        // How long a user's request waits for its playlist to be stored.
        private static readonly TimeSpan OpenWait = TimeSpan.FromSeconds(20);
        // A section is pre-loaded at most this often.
        private static readonly TimeSpan SectionInterval = TimeSpan.FromMinutes(2);
        private static readonly ConcurrentDictionary<string, DateTime> SectionLoaded = new(StringComparer.Ordinal);
        // One fill per folder at a time; playlist id -> fills running now.
        private static readonly ConcurrentDictionary<long, SemaphoreSlim> FolderLocks = new();
        private static readonly ConcurrentDictionary<string, int> Filling = new(StringComparer.Ordinal);
        // Set while this plugin stores a playlist itself, so its own provider
        // call is told apart from a channel scan reaching the same folder.
        private static readonly AsyncLocal<bool> OwnFill = new();
        private static readonly ConcurrentQueue<(long Section, string[] Ids)> Queue = new();
        private static DateTime _lastReadError;
        private static bool _dirty;
        private static int _workerRunning;

        private static bool Alive(Mark mark, DateTime now) => now - mark.Last < (mark.Opened ? KeepOpened : KeepPrefetched);

        // Null while the list cannot be read: the caller must then keep what
        // Emby stored instead of answering "not wanted" (an empty playlist).
        internal static bool? IsWanted(string playlistId)
        {
            lock (Sync)
            {
                var wanted = Load();
                if (wanted == null) return null;
                return wanted.TryGetValue(playlistId, out var mark) && Alive(mark, DateTime.UtcNow);
            }
        }

        internal static bool StateKnown
        {
            get { lock (Sync) return Load() != null; }
        }

        internal static bool RecentlyFailed(string playlistId) =>
            Failed.TryGetValue(playlistId, out var until) && DateTime.UtcNow < until;

        internal static void NoteFailed(string playlistId)
        {
            Failed[playlistId] = DateTime.UtcNow + FailedPause;
            YouTubeChannel.LogPublic($"[YT] YouTube Music: playlist {playlistId} is not available from the YouTube API; asked again in {FailedPause.TotalHours:0} hours.");
        }

        internal static void NoteEmpty(string playlistId) => Empty[playlistId] = DateTime.UtcNow + FailedPause;

        internal static void NoteServed(string playlistId)
        {
            Failed.TryRemove(playlistId, out _);
            Empty.TryRemove(playlistId, out _);
        }

        private static bool NothingToLoad(string playlistId) => RecentlyFailed(playlistId)
            || Empty.TryGetValue(playlistId, out var until) && DateTime.UtcNow < until;

        // A channel scan that reaches a playlist while this plugin is storing
        // it must not store it a second time (duplicate videos).
        internal static bool FillingElsewhere(string playlistId) =>
            !OwnFill.Value && Filling.TryGetValue(playlistId, out var running) && running > 0;

        internal static int Install(object harmony, Type harmonyMethodType)
        {
            var count = 0;
            try
            {
                var patch = harmony.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(method => method.Name == "Patch"
                        && method.GetParameters().Length >= 3
                        && typeof(MethodBase).IsAssignableFrom(method.GetParameters()[0].ParameterType)
                        && method.GetParameters().Any(parameter => parameter.Name == "prefix"));
                if (patch == null) throw new MissingMethodException("Harmony", "Patch");

                void Add(MethodInfo target, string callback)
                {
                    EmbyPatchSafety.RequirePatchable(new[] { target });
                    var prefix = Activator.CreateInstance(harmonyMethodType,
                        typeof(YouTubeMusicBrowse).GetMethod(callback, BindingFlags.NonPublic | BindingFlags.Static)!);
                    var parameters = patch.GetParameters();
                    var arguments = new object?[parameters.Length];
                    arguments[0] = target;
                    for (int i = 1; i < parameters.Length; i++)
                        if (parameters[i].Name == "prefix") arguments[i] = prefix;
                    patch.Invoke(harmony, arguments);
                    count++;
                }

                // The apps list a folder through GET /Items?ParentId=... . Its
                // handler opens a database read transaction that holds back
                // writes until the request ends, so a playlist can only be
                // stored for the same request before that transaction starts.
                var service = FindType("Emby.Api.UserLibrary.ItemsService");
                var requestType = service?.Assembly.GetType("Emby.Api.UserLibrary.GetItems", false);
                var get = requestType == null ? null : service!.GetMethod("Get",
                    BindingFlags.Public | BindingFlags.Instance, null, new[] { requestType }, null);
                if (get != null) Add(get, nameof(RequestPrefix));
                else YouTubeChannel.LogPublic("[YT] YouTube Music: Items endpoint not found; playlists load in the background when opened.");

                // Any other way into a folder (and the endpoint above if it
                // could not be hooked): load in the background.
                var folder = typeof(Folder).GetMethod("GetItemsInternal",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                    new[] { typeof(InternalItemsQuery), typeof(IDataContext), typeof(CancellationToken) }, null);
                if (folder != null) Add(folder, nameof(OpenPrefix));
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] YouTube Music playlists cannot load on open (" + error.GetType().Name + ").");
            }
            return count;
        }

        private static Type? FindType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType(fullName, false);
                    if (type != null) return type;
                }
                catch { }
            }
            return null;
        }

        // GET /Items: runs before Emby opens its read transaction. Stores an
        // empty playlist right away (about a second), so this very request
        // already lists its videos.
        private static void RequestPrefix(object __0)
        {
            try
            {
                if (__0 is not MediaBrowser.Controller.Api.BaseItemsRequest request
                    || string.IsNullOrEmpty(request.ParentId) || request.Limit == 0) return;
                if (Plugin.Instance?.Options.ShowMusicGenres != true) return;
                var library = Plugin.ResolveService<ILibraryManager>();
                if (library == null) return;
                var internalId = library.GetInternalId(request.ParentId);
                if (internalId <= 0 || library.GetItemById(internalId) is not Folder folder) return;
                var externalId = folder.ExternalId;
                if (externalId == null) return;
                if (externalId.StartsWith(SectionPrefix, StringComparison.Ordinal))
                {
                    SectionOpened(folder, externalId);
                    return;
                }
                if (!externalId.StartsWith(PlaylistPrefix, StringComparison.Ordinal)) return;
                var id = externalId.Substring(PlaylistPrefix.Length);
                if (!YouTubeMusicCatalog.IsPlaylistId(id) || !MarkWanted(new[] { id }, opened: true)) return;
                if (NothingToLoad(id) || HasVideos(folder)) return;
                if (!Task.Run(() => FillAsync(folder, id)).Wait(OpenWait))
                    YouTubeChannel.LogPublic($"[YT] YouTube Music: playlist {id} still loading after {OpenWait.TotalSeconds:0} s; it shows up when opened again.");
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] YouTube Music request hook failed (" + error.GetType().Name + ").");
            }
        }

        // Runs for every folder listing in Emby: return at once unless a user
        // lists a YouTube Music section or playlist. Counting children for a
        // card (Limit = 0) is not opening the folder. Never waits here: the
        // caller holds a read transaction that would block the store.
        private static void OpenPrefix(Folder __instance, InternalItemsQuery __0)
        {
            try
            {
                var externalId = __instance?.ExternalId;
                if (externalId == null || __0?.User == null || __0.Limit == 0) return;
                if (!externalId.StartsWith(PlaylistPrefix, StringComparison.Ordinal)
                    && !externalId.StartsWith(SectionPrefix, StringComparison.Ordinal)) return;
                if (Plugin.Instance?.Options.ShowMusicGenres != true) return;
                if (externalId.StartsWith(SectionPrefix, StringComparison.Ordinal))
                {
                    SectionOpened(__instance!, externalId);
                    return;
                }
                var id = externalId.Substring(PlaylistPrefix.Length);
                if (!YouTubeMusicCatalog.IsPlaylistId(id) || !MarkWanted(new[] { id }, opened: true)) return;
                if (NothingToLoad(id) || HasVideos(__instance!)) return;
                _ = Task.Run(() => FillAsync(__instance!, id));
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] YouTube Music open hook failed (" + error.GetType().Name + ").");
            }
        }

        // Marks playlists as wanted. False while the list cannot be read.
        private static bool MarkWanted(IEnumerable<string> ids, bool opened)
        {
            var now = DateTime.UtcNow;
            lock (Sync)
            {
                var wanted = Load();
                if (wanted == null) return false;
                foreach (var id in ids)
                {
                    if (!wanted.TryGetValue(id, out var mark) || !Alive(mark, now))
                    {
                        wanted[id] = new Mark { Last = now, Opened = opened };
                        _dirty = true;
                    }
                    else if (opened && !mark.Opened || now - mark.Last > TouchInterval)
                    {
                        mark.Last = now;
                        mark.Opened |= opened;
                        _dirty = true;
                    }
                }
                if (wanted.Count > MaxWanted)
                    foreach (var old in wanted.OrderBy(pair => pair.Value.Opened).ThenBy(pair => pair.Value.Last)
                                 .Take(wanted.Count - MaxWanted).Select(pair => pair.Key).ToList())
                        wanted.Remove(old);
            }
            if (_dirty) StartWorker();
            return true;
        }

        private static void SectionOpened(Folder section, string externalId)
        {
            var entries = YouTubeMusicGenres.PeekSection(externalId.Substring(SectionPrefix.Length), Plugin.Instance?.Options);
            if (entries == null || entries.Videos) return;
            var ids = entries.Entries.Take(SectionPrefetch).Select(entry => entry.Id).ToArray();
            if (!MarkWanted(ids, opened: false)) return;
            var now = DateTime.UtcNow;
            if (SectionLoaded.TryGetValue(externalId, out var last) && now - last < SectionInterval) return;
            SectionLoaded[externalId] = now;
            Queue.Enqueue((section.InternalId, ids));
            StartWorker();
        }

        private static bool HasVideos(BaseItem folder)
        {
            var library = Plugin.ResolveService<ILibraryManager>();
            return library == null || library.GetItemList(new InternalItemsQuery
            {
                ParentIds = new[] { folder.InternalId },
                IsFolder = false,
                Limit = 1
            }).Length > 0;
        }

        // Stores one playlist folder now, unless it already has videos.
        internal static async Task FillAsync(BaseItem folder, string id)
        {
            var gate = FolderLocks.GetOrAdd(folder.InternalId, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync().ConfigureAwait(false);
            Filling.AddOrUpdate(id, 1, (_, running) => running + 1);
            try
            {
                if (HasVideos(folder) || NothingToLoad(id)) return;
                var started = DateTime.UtcNow;
                OwnFill.Value = true;
                if (!await ChannelRefreshInvoker.FillFolderDirectAsync(folder, CancellationToken.None).ConfigureAwait(false))
                    await ChannelRefreshInvoker.RefreshFolderAsync(folder.ExternalId, 2).ConfigureAwait(false);
                RemoveDuplicates(folder);
                YouTubeChannel.LogPublic($"[YT] YouTube Music: playlist {id} loaded in {(DateTime.UtcNow - started).TotalSeconds:0.0} s.");
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic($"[YT] YouTube Music: playlist {id} could not be loaded ({error.GetBaseException().GetType().Name}: {error.GetBaseException().Message}).");
            }
            finally
            {
                OwnFill.Value = false;
                Filling.AddOrUpdate(id, 0, (_, running) => Math.Max(0, running - 1));
                gate.Release();
            }
        }

        // A channel scan that had read the playlist just before this fill
        // started can store the same videos once more. Keep the oldest copy.
        private static void RemoveDuplicates(BaseItem folder)
        {
            var library = Plugin.ResolveService<ILibraryManager>();
            if (library == null) return;
            var extra = library.GetItemList(new InternalItemsQuery { ParentIds = new[] { folder.InternalId }, IsFolder = false })
                .Where(item => !string.IsNullOrEmpty(item.ExternalId))
                .GroupBy(item => item.ExternalId, StringComparer.Ordinal)
                .SelectMany(group => group.OrderBy(item => item.InternalId).Skip(1))
                .ToList();
            foreach (var item in extra)
                library.DeleteItem(item, new MediaBrowser.Controller.Library.DeleteOptions { DeleteFileLocation = false, DeleteFromExternalProvider = false }, folder, false);
            if (extra.Count > 0)
                YouTubeChannel.LogPublic($"[YT] YouTube Music: removed {extra.Count} duplicate videos from {folder.ExternalId}.");
        }

        private static void StartWorker()
        {
            if (Interlocked.CompareExchange(ref _workerRunning, 1, 0) == 0)
                _ = Task.Run(RunAsync);
        }

        // Saves the list and pre-loads opened sections, one playlist at a
        // time, so a tapped playlist never waits behind a whole section.
        private static async Task RunAsync()
        {
            try
            {
                Save();
                var library = Plugin.ResolveService<ILibraryManager>();
                while (library != null && Queue.TryDequeue(out var job))
                {
                    var wanted = new HashSet<string>(job.Ids, StringComparer.Ordinal);
                    var folders = library.GetItemList(new InternalItemsQuery { ParentIds = new[] { job.Section }, IsFolder = true })
                        .Where(item => item.ExternalId?.StartsWith(PlaylistPrefix, StringComparison.Ordinal) == true
                            && wanted.Contains(item.ExternalId.Substring(PlaylistPrefix.Length)))
                        .OrderBy(item => Array.IndexOf(job.Ids, item.ExternalId!.Substring(PlaylistPrefix.Length)))
                        .ToList();
                    foreach (var folder in folders)
                        await FillAsync(folder, folder.ExternalId!.Substring(PlaylistPrefix.Length)).ConfigureAwait(false);
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
                if (!Queue.IsEmpty) StartWorker();
            }
        }

        private static string? StatePath()
        {
            var root = Plugin.DataPath;
            return string.IsNullOrEmpty(root) ? null : Path.Combine(root, StateFile);
        }

        // Caller holds Sync. Null when the file exists but cannot be read
        // right now (tried again on the next call).
        private static Dictionary<string, Mark>? Load()
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
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            {
                if (DateTime.UtcNow - _lastReadError > TimeSpan.FromMinutes(10))
                {
                    _lastReadError = DateTime.UtcNow;
                    YouTubeChannel.LogPublic("[YT] YouTube Music: opened playlists cannot be read right now (" + error.GetType().Name + "); stored music folders are kept.");
                }
                return null;
            }
            catch (Exception error) when (error is JsonException or NotSupportedException)
            {
                // Damaged beyond use: keep it for inspection, start empty.
                YouTubeChannel.LogPublic("[YT] YouTube Music: opened playlists file is damaged (" + error.GetType().Name + "); moved aside.");
                if (path != null) try { File.Move(path, path + ".bad", overwrite: true); } catch { }
                result.Clear();
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
