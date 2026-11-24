using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Dto;
using System.Reflection;

namespace Emby.YouTubePlugin
{
    // Channel videos are stored as Episodes (series name = YouTube channel).
    // Emby apps treat unwatched episodes as spoilers when the user enables
    // "hide episode spoilers": the web client then ignores the primary image
    // of every unplayed Episode and shows a blank card, without even asking
    // the server for the picture. For YouTube videos that hides every
    // thumbnail. The item data sent to apps therefore calls these videos
    // "Video"; the library keeps them as Episodes, so ids, watched state,
    // resume points and server-side queries are unchanged.
    internal static class YouTubeVideoPresentation
    {
        internal static int Install(object harmony, Type harmonyMethodType)
        {
            try
            {
                var service = Plugin.ResolveService<IDtoService>();
                var patch = harmony.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(method => method.Name == "Patch"
                        && method.GetParameters().Length >= 3
                        && typeof(MethodBase).IsAssignableFrom(method.GetParameters()[0].ParameterType)
                        && method.GetParameters().Any(parameter => parameter.Name == "postfix"));
                if (service == null || patch == null) return 0;
                int count = 0;
                var targets = YouTubeChapterDtoInterceptor.FindTargets(service.GetType());
                EmbyPatchSafety.RequirePatchable(targets);
                foreach (var target in targets)
                {
                    var method = typeof(YouTubeVideoPresentation).GetMethod(
                        target.ReturnType == typeof(BaseItemDto[]) ? nameof(ManyPostfix) : nameof(OnePostfix),
                        BindingFlags.NonPublic | BindingFlags.Static)!;
                    var postfix = Activator.CreateInstance(harmonyMethodType, method);
                    if (postfix == null) continue;
                    var parameters = patch.GetParameters();
                    var arguments = new object?[parameters.Length];
                    arguments[0] = target;
                    for (int i = 1; i < parameters.Length; i++)
                        if (parameters[i].Name == "postfix") arguments[i] = postfix;
                    patch.Invoke(harmony, arguments);
                    count++;
                }
                return count;
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] Video presentation unavailable (" + error.GetType().Name + ").");
                return 0;
            }
        }

        private static void ManyPostfix(BaseItem[] __0, BaseItemDto[]? __result)
        {
            try
            {
                if (__0 == null || __result == null || __0.Length != __result.Length) return;
                for (int i = 0; i < __0.Length; i++) Apply(__0[i], __result[i]);
            }
            catch (Exception) { /* presentation must never break a response */ }
        }

        private static void OnePostfix(BaseItem __0, BaseItemDto? __result)
        {
            try { Apply(__0, __result); }
            catch (Exception) { }
        }

        internal static void Apply(BaseItem? item, BaseItemDto? dto)
        {
            if (item == null || dto == null || Plugin.Instance?.Options.PresentVideosAsVideos == false || !IsChannelVideo(item)) return;
            if (string.Equals(dto.Type, "Episode", StringComparison.Ordinal))
                dto.Type = "Video";
        }

        // List queries load items without provider ids and often without
        // ExternalId; the watch URL path and the stored type are always there.
        // Only Episodes with a YouTube watch path: library trailers (Trailer
        // type) and other libraries' items are never touched.
        internal static bool IsChannelVideo(BaseItem item)
        {
            if (item is not MediaBrowser.Controller.Entities.TV.Episode || item.IsFolder) return false;
            var path = item.Path;
            return path != null
                && (path.StartsWith("https://www.youtube.com/watch", StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("https://youtube.com/watch", StringComparison.OrdinalIgnoreCase));
        }
    }
}
