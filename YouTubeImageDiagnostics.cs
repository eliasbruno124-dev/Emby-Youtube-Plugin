using MediaBrowser.Controller.Drawing;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using System.Reflection;

namespace Emby.YouTubePlugin
{
    // Support aid for "YouTube cards stay blank": Emby's web client only asks
    // for an image when the item data it receives carries an image tag. This
    // read-only postfix logs, throttled, how many YouTube videos left the
    // server with and without a primary image tag, and once which image
    // enhancers (poster overlay plugins) claim YouTube videos. No database
    // access, no changes to the returned data.
    internal static class YouTubeImageDiagnostics
    {
        private static int _events;
        private static int _missingReports;
        private static int _enhancersLogged;

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
                foreach (var target in YouTubeChapterDtoInterceptor.FindTargets(service.GetType()))
                {
                    var method = typeof(YouTubeImageDiagnostics).GetMethod(
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
                YouTubeChannel.LogPublic("[YTIMG] Image diagnostics unavailable (" + error.GetType().Name + ").");
                return 0;
            }
        }

        private static void ManyPostfix(BaseItem[] __0, DtoOptions __1, BaseItemDto[]? __result)
        {
            try
            {
                if (__0 == null || __result == null || __0.Length != __result.Length) return;
                Inspect(__0, __1, __result);
            }
            catch (Exception) { /* diagnostics must never affect a response */ }
        }

        private static void OnePostfix(BaseItem __0, DtoOptions __1, BaseItemDto? __result)
        {
            try
            {
                if (__0 == null || __result == null) return;
                Inspect(new[] { __0 }, __1, new[] { __result });
            }
            catch (Exception) { }
        }

        private static void Inspect(BaseItem[] items, DtoOptions options, BaseItemDto[] dtos)
        {
            int videos = 0, withTag = 0, withoutTag = 0;
            BaseItem? firstMissing = null;
            BaseItem? firstVideo = null;
            for (int i = 0; i < items.Length; i++)
            {
                if (!IsYouTubeVideo(items[i])) continue;
                videos++;
                firstVideo ??= items[i];
                if (dtos[i].ImageTags != null && dtos[i].ImageTags.ContainsKey(ImageType.Primary)) withTag++;
                else { withoutTag++; firstMissing ??= items[i]; }
            }
            if (videos == 0) return;
            if (firstVideo != null && Interlocked.Exchange(ref _enhancersLogged, 1) == 0)
                LogEnhancers(firstVideo);

            var n = Interlocked.Increment(ref _events);
            var imagesRequested = options == null || options.EnableImages;
            if (withoutTag > 0 && imagesRequested && Interlocked.Increment(ref _missingReports) <= 20)
            {
                var image = firstMissing!.GetImageInfo(ImageType.Primary, 0);
                var state = image == null ? "no primary image stored"
                    : YouTubeLazyImage.IsLazyPath(image.Path) ? "image waiting for first display"
                    : image.IsLocalFile ? "image stored locally" : "plain image URL";
                YouTubeChannel.LogPublic($"[YTIMG] Item data sent WITHOUT image tag for {withoutTag} of {videos} YouTube videos "
                    + $"(first: item {firstMissing.InternalId}, {state}). Clients show these as blank cards.");
            }
            else if (n <= 5 || n % 200 == 0)
            {
                YouTubeChannel.LogPublic($"[YTIMG] Item data sent: {videos} YouTube videos, {withTag} with image tag"
                    + (imagesRequested ? "" : " (client asked without images)") + $". Events since start: {n}.");
            }
        }

        // List queries load items without provider ids and often without
        // ExternalId; the watch URL path is always there.
        private static bool IsYouTubeVideo(BaseItem item)
        {
            try
            {
                var path = item.Path;
                return !item.IsFolder && path != null
                    && (path.StartsWith("https://www.youtube.com/watch", StringComparison.OrdinalIgnoreCase)
                        || path.StartsWith("https://youtube.com/watch", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception) { return false; }
        }

        private static void LogEnhancers(BaseItem video)
        {
            try
            {
                var processor = Plugin.ResolveService<IImageProcessor>();
                var enhancers = processor?.ImageEnhancers ?? Array.Empty<MediaBrowser.Controller.Providers.IImageEnhancer>();
                var claiming = new List<string>();
                foreach (var enhancer in enhancers)
                {
                    try { if (enhancer.Supports(video, ImageType.Primary)) claiming.Add(enhancer.GetType().FullName ?? enhancer.GetType().Name); }
                    catch (Exception ex) { claiming.Add((enhancer.GetType().FullName ?? "?") + " (throws " + ex.GetType().Name + ")"); }
                }
                YouTubeChannel.LogPublic($"[YTIMG] Image enhancers installed: {enhancers.Length}; applying to YouTube videos: "
                    + (claiming.Count == 0 ? "none" : string.Join(", ", claiming)) + ".");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic("[YTIMG] Image enhancer check failed (" + ex.GetType().Name + ").");
            }
        }
    }
}
