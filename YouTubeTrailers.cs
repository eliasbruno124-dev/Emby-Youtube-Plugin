using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using System.Globalization;
using System.Reflection;

namespace Emby.YouTubePlugin
{
    // Movie and series metadata lists trailers as YouTube watch URLs. Clients
    // with a bundled player (LG webOS, Tizen) cannot open them. With
    // NativeTrailersEnabled each URL becomes a path-less local trailer of its
    // owner whose only playable source is the native YouTube source, so every
    // client uses Emby's own PlaybackInfo and transcoding.
    //
    // Path-less on purpose: Emby's owner validation deletes stale children
    // only when they have a path, and a placeholder static source is never
    // probed. Rows are created lazily per owner on the LocalTrailers request.
    internal static class YouTubeTrailers
    {
        internal const string ProviderKey = "YouTubeTrailer";
        internal const int MaximumPerOwner = 5;
        private static readonly ExtraType[] TrailerTypes = { ExtraType.Trailer };
        // Creation and cleanup share one gate, so a disabled option can never
        // race with a request that is still materializing rows.
        private static readonly object Gate = new();
        private static int _cleanupQueued;

        internal static bool Enabled =>
            Plugin.Instance?.Options.NativeTrailersEnabled == true && YouTubeDirectPlayback.Enabled;

        internal static bool IsOwner(BaseItem? item) =>
            item is Movie or Series && !item.ExtraType.HasValue;

        internal static string? GetPluginVideoId(BaseItem? item)
        {
            if (item is not Trailer || item.ExtraType != ExtraType.Trailer || !string.IsNullOrEmpty(item.Path))
                return null;
            var id = item.GetProviderId(ProviderKey);
            return IsVideoId(id) ? id : null;
        }

        internal static string[] GetVideoIds(IEnumerable<string>? urls)
        {
            var result = new List<string>();
            if (urls == null) return Array.Empty<string>();
            foreach (var url in urls)
            {
                var id = ParseVideoId(url);
                if (id != null && !result.Contains(id, StringComparer.Ordinal)) result.Add(id);
                if (result.Count == MaximumPerOwner) break;
            }
            return result.ToArray();
        }

        internal static string? ParseVideoId(string? url)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)
                || uri.Scheme is not ("http" or "https")) return null;
            var host = uri.Host.ToLowerInvariant();
            var path = uri.AbsolutePath;
            string? id = null;
            if (host is "youtu.be" or "www.youtu.be")
                id = path.Trim('/');
            else if (host is "youtube.com" or "www.youtube.com" or "m.youtube.com"
                or "youtube-nocookie.com" or "www.youtube-nocookie.com")
            {
                if (path == "/watch")
                {
                    foreach (var part in uri.Query.TrimStart('?').Split('&'))
                        if (part.StartsWith("v=", StringComparison.Ordinal))
                        {
                            id = Uri.UnescapeDataString(part.Substring(2));
                            break;
                        }
                }
                else
                {
                    foreach (var prefix in new[] { "/embed/", "/v/", "/shorts/" })
                        if (path.StartsWith(prefix, StringComparison.Ordinal))
                        {
                            id = path.Substring(prefix.Length).Split('/')[0];
                            break;
                        }
                }
            }
            return IsVideoId(id) ? id : null;
        }

        private static bool IsVideoId(string? id) =>
            id is { Length: 11 } && id.All(YouTubeDirectPlayback.IsVideoIdCharacter);

        // Brings the owner's plugin trailers in line with its metadata. Real
        // local trailers win: then no plugin rows exist for that owner. The
        // extras query covers all versions sharing the owner's presentation key.
        internal static void Ensure(BaseItem owner)
        {
            if (!IsOwner(owner)) return;
            var library = Plugin.ResolveService<ILibraryManager>();
            if (library == null) return;
            lock (Gate)
            {
                var wanted = Enabled ? GetVideoIds(owner.RemoteTrailers) : Array.Empty<string>();
                var extras = owner.GetExtras(TrailerTypes);
                var ours = extras.Where(item => GetPluginVideoId(item) != null).ToArray();
                if (extras.Length > ours.Length) wanted = Array.Empty<string>();
                // Each version manages only its own rows; another version's
                // metadata must never remove them. Its rows still count as
                // present, so the grouped view gets no duplicate video.
                var own = ours.Where(item => item.ParentId == owner.InternalId).ToArray();
                var elsewhere = new HashSet<string>(ours.Where(item => item.ParentId != owner.InternalId)
                    .Select(item => GetPluginVideoId(item)!), StringComparer.Ordinal);
                if (wanted.Length == 0 && own.Length == 0) return;

                var kept = new HashSet<string>(StringComparer.Ordinal);
                int removed = 0;
                foreach (var trailer in own)
                {
                    if (wanted.Contains(GetPluginVideoId(trailer)!, StringComparer.Ordinal)
                        && kept.Add(GetPluginVideoId(trailer)!)) continue;
                    Delete(library, trailer, owner);
                    removed++;
                }

                var created = new List<BaseItem>();
                for (int i = 0; i < wanted.Length; i++)
                {
                    if (kept.Contains(wanted[i]) || elsewhere.Contains(wanted[i])) continue;
                    var trailer = new Trailer
                    {
                        // Owner-specific: the same video under two movies must
                        // not share one item identity or its user data. Not
                        // GetNewItemId: it may lowercase the key, and YouTube
                        // IDs differ by case.
                        Id = library.GetNewItemIdFromName(
                            "youtube-trailer|" + owner.Id.ToString("N") + "|" + wanted[i], typeof(Trailer)),
                        Name = i == 0 ? "Trailer" : "Trailer " + (i + 1).ToString(CultureInfo.InvariantCulture),
                        ExtraType = ExtraType.Trailer,
                        IsLocked = true,
                        DateCreated = DateTimeOffset.UtcNow
                    };
                    trailer.SetProviderId(ProviderKey, wanted[i]);
                    trailer.SetParent(owner);
                    created.Add(trailer);
                }
                if (created.Count > 0)
                    library.CreateItems(created, owner, null, library.GetCollectionFolders(owner),
                        triggerItemAdded: false, CancellationToken.None);
                if (created.Count > 0 || removed > 0)
                    YouTubeChannel.LogPublic("[YT] Native trailers synchronized for item " + owner.InternalId
                        + " (" + created.Count + " added, " + removed + " removed).");
            }
        }

        private static void Delete(ILibraryManager library, BaseItem trailer, BaseItem? owner)
        {
            library.DeleteItem(trailer, new DeleteOptions
            {
                DeleteFileLocation = false,
                DeleteFromExternalProvider = false
            }, owner, false);
        }

        // Called on startup and after every save. Removing the option or the
        // Emby player must not leave trailers that can no longer play.
        internal static void QueueCleanupIfDisabled(string reason)
        {
            if (Enabled || Interlocked.Exchange(ref _cleanupQueued, 1) == 1) return;
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    RemoveAll(reason);
                }
                catch (Exception error) when (error is not OutOfMemoryException)
                {
                    YouTubeChannel.LogPublic("[YT] Native trailer cleanup failed (" + error.GetType().Name + ").");
                }
                finally { Interlocked.Exchange(ref _cleanupQueued, 0); }
            });
        }

        private static void RemoveAll(string reason)
        {
            var library = Plugin.ResolveService<ILibraryManager>();
            if (library == null) return;
            lock (Gate)
            {
                if (Enabled) return;
                var rows = library.GetItemList(new InternalItemsQuery
                {
                    Recursive = true,
                    IncludeItemTypes = new[] { nameof(Trailer) },
                    ExtraTypes = TrailerTypes,
                    HasAnyProviderId = new[] { ProviderKey }
                }).Where(item => GetPluginVideoId(item) != null).ToArray();
                foreach (var trailer in rows) Delete(library, trailer, null);
                if (rows.Length > 0)
                    YouTubeChannel.LogPublic("[YT] Native trailers removed after " + reason + " (" + rows.Length + ").");
            }
        }

        internal static int Install(object harmony, Type harmonyMethodType)
        {
            int count = 0;
            try
            {
                var patch = harmony.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(method => method.Name == "Patch"
                        && method.GetParameters().Length >= 3
                        && typeof(MethodBase).IsAssignableFrom(method.GetParameters()[0].ParameterType)
                        && method.GetParameters().Any(parameter => parameter.Name == "prefix")
                        && method.GetParameters().Any(parameter => parameter.Name == "postfix"));
                if (patch == null) throw new MissingMethodException("Harmony", "Patch");

                void Add(MethodInfo original, string callback, string kind)
                {
                    var method = typeof(YouTubeTrailers).GetMethod(callback, BindingFlags.NonPublic | BindingFlags.Static)!;
                    var harmonyMethod = Activator.CreateInstance(harmonyMethodType, method);
                    var parameters = patch.GetParameters();
                    var arguments = new object?[parameters.Length];
                    arguments[0] = original;
                    for (int i = 1; i < parameters.Length; i++)
                        if (parameters[i].Name == kind) arguments[i] = harmonyMethod;
                    patch.Invoke(harmony, arguments);
                    count++;
                }

                // Native LocalTrailers reads the extras straight from the
                // database; materialize the owner's rows just before that.
                var serviceType = FindType("Emby.Api.UserLibrary.UserLibraryService");
                var requestType = serviceType?.Assembly.GetType("Emby.Api.UserLibrary.GetLocalTrailers", false);
                var localTrailers = requestType == null ? null : serviceType!.GetMethod("Get",
                    BindingFlags.Public | BindingFlags.Instance, null, new[] { requestType }, null);
                if (localTrailers == null)
                    YouTubeChannel.LogPublic("[YT] Native trailers unavailable (LocalTrailers endpoint not found).");
                else
                {
                    Add(localTrailers, nameof(LocalTrailersPrefix), "prefix");
                    var dtoService = Plugin.ResolveService<IDtoService>();
                    if (dtoService != null)
                        foreach (var target in YouTubeChapterDtoInterceptor.FindTargets(dtoService.GetType()))
                            Add(target, target.ReturnType == typeof(BaseItemDto[])
                                ? nameof(CountManyPostfix) : nameof(CountOnePostfix), "postfix");
                }
                YouTubeChannel.LogPublic("[YT] Native trailer hooks installed (" + count + " patches).");
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] Native trailer hooks unavailable (" + error.GetType().Name + ").");
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

        private static void LocalTrailersPrefix(object __instance, object __0)
        {
            try
            {
                if (!Enabled || __instance == null || __0 == null) return;
                var type = __0.GetType();
                var itemId = type.GetProperty("Id")?.GetValue(__0) as string;
                var userId = type.GetProperty("UserId")?.GetValue(__0) is Guid guid ? guid : Guid.Empty;
                var library = Plugin.ResolveService<ILibraryManager>();
                if (string.IsNullOrEmpty(itemId) || library == null) return;
                var owner = library.GetItemById(library.GetInternalId(itemId));
                if (owner == null || !IsOwner(owner)) return;
                // Write only for a user who may see the owner. The URL user id
                // alone is not trusted: resolve the same effective user as the
                // native method (signed-in user, or the requested one for admins).
                var resolveUser = __instance.GetType().GetMethod("GetUserForRequest",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                    new[] { typeof(Guid), typeof(bool) }, null);
                if (resolveUser?.Invoke(__instance, new object[] { userId, false }) is not User user
                    || !owner.IsVisible(user)) return;
                Ensure(owner);
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                // Never break the native endpoint; it then returns what exists.
                YouTubeChannel.LogPublic("[YT] Native trailer preparation failed (" + error.GetType().Name + ").");
            }
        }

        // Before the first LocalTrailers request no rows exist yet. Clients
        // choose local trailers by this count, so report the metadata count.
        // Only item DTOs whose native count was requested and is zero change;
        // RemoteTrailers stay untouched for editors and other clients.
        internal static void ApplyCount(BaseItem item, BaseItemDto dto)
        {
            if (dto.LocalTrailerCount != 0 || !IsOwner(item) || item.RemoteTrailers == null
                || item.RemoteTrailers.Length == 0) return;
            var count = GetVideoIds(item.RemoteTrailers).Length;
            if (count > 0) dto.LocalTrailerCount = count;
        }

        private static void CountOnePostfix(BaseItem __0, BaseItemDto? __result)
        {
            if (__0 != null && __result != null && Enabled) ApplyCount(__0, __result);
        }

        private static void CountManyPostfix(BaseItem[]? __0, BaseItemDto[]? __result)
        {
            if (!Enabled || __0 == null || __result == null || __0.Length > 4096 || __result.Length > 4096) return;
            var items = __0.Where(item => item != null && item.InternalId > 0 && IsOwner(item))
                .GroupBy(item => item.InternalId).ToDictionary(group => group.Key, group => group.First());
            if (items.Count == 0) return;
            foreach (var dto in __result)
                if (dto != null && long.TryParse(dto.Id, NumberStyles.None, CultureInfo.InvariantCulture, out long id)
                    && items.TryGetValue(id, out var item)) ApplyCount(item, dto);
        }
    }

    // Emby collects IMediaSourceProvider exports and asks each of them for
    // dynamic sources. Only plugin trailer rows get one; all else gets none.
    public class YouTubeTrailerMediaSourceProvider : IMediaSourceProvider
    {
        // Emby asks again when the stream starts. Its channel path keeps the
        // callback result for five minutes; do the same instead of measuring
        // and selecting a second, possibly different source.
        private static readonly TimeSpan Reuse = TimeSpan.FromMinutes(5);
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (DateTime At, List<MediaSourceInfo> Sources)> Recent
            = new(StringComparer.Ordinal);

        public async Task<List<MediaSourceInfo>> GetMediaSources(BaseItem item, CancellationToken cancellationToken)
        {
            var videoId = YouTubeTrailers.GetPluginVideoId(item);
            if (videoId == null || !YouTubeTrailers.Enabled) return new List<MediaSourceInfo>();
            var now = DateTime.UtcNow;
            if (Recent.TryGetValue(videoId, out var recent) && now - recent.At < Reuse) return Copy(recent.Sources);
            foreach (var stale in Recent.Where(pair => now - pair.Value.At >= Reuse).Select(pair => pair.Key).ToArray())
                Recent.TryRemove(stale, out _);
            var sources = (await YouTubeDirectPlayback.GetMediaSourcesAsync(videoId, cancellationToken)
                .ConfigureAwait(false)).ToList();
            foreach (var source in sources)
            {
                // Caption delivery is bound to YouTube channel items. A trailer
                // offers video and audio only, which also keeps TV burn-in off.
                source.MediaStreams?.RemoveAll(stream => stream.Type == MediaStreamType.Subtitle);
                source.DefaultSubtitleStreamIndex = -1;
            }
            Recent[videoId] = (now, sources);
            return Copy(sources);
        }

        // Emby adjusts returned sources per user and request (default audio
        // track, headers, streams). Cached templates are never handed out.
        private static readonly MethodInfo ShallowCopy =
            typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private static List<MediaSourceInfo> Copy(List<MediaSourceInfo> templates) =>
            templates.Select(template =>
            {
                var source = (MediaSourceInfo)ShallowCopy.Invoke(template, null)!;
                source.MediaStreams = template.MediaStreams?.Select(stream => (MediaStream)ShallowCopy.Invoke(stream, null)!).ToList();
                source.Chapters = template.Chapters?.Select(chapter => (ChapterInfo)ShallowCopy.Invoke(chapter, null)!).ToArray();
                source.Formats = template.Formats?.ToArray();
                source.TranscodeReasons = template.TranscodeReasons?.ToArray();
                source.RequiredHttpHeaders = template.RequiredHttpHeaders == null ? null
                    : new Dictionary<string, string>(template.RequiredHttpHeaders, StringComparer.OrdinalIgnoreCase);
                return source;
            }).ToList();

        // Sources never require opening. Emby 4.9 and 4.10 differ only in this
        // signature; both shapes exist so the type loads on either runtime.
        public Task<ILiveStream> OpenMediaSource(string openToken, List<ILiveStream> currentLiveStreams,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public virtual Task<ILiveStream> OpenMediaSource(string openToken, string consumerId,
            List<ILiveStream> currentLiveStreams, CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
