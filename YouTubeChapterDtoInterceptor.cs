using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Dto;
using System.Globalization;
using System.Reflection;

namespace Emby.YouTubePlugin
{
    // Item detail and NowPlaying DTOs have their own Chapters collection. The
    // playback media source alone cannot update a previously fetched item DTO.
    internal static class YouTubeChapterDtoInterceptor
    {
        internal static int Install(object harmony, Type harmonyMethodType)
        {
            try
            {
                var service = Plugin.ResolveService<IDtoService>();
                if (service == null)
                {
                    YouTubeChannel.LogPublic("[YT] Native chapter item DTO interceptor unavailable (service not found).");
                    return 0;
                }
                var patch = harmony.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(method => method.Name == "Patch"
                        && method.GetParameters().Length >= 3
                        && typeof(MethodBase).IsAssignableFrom(method.GetParameters()[0].ParameterType)
                        && method.GetParameters().Any(parameter => parameter.Name == "postfix"));
                if (patch == null)
                {
                    YouTubeChannel.LogPublic("[YT] Native chapter item DTO interceptor unavailable (patch method not found).");
                    return 0;
                }
                int count = 0;
                foreach (var target in FindTargets(service.GetType()))
                {
                    var method = typeof(YouTubeChapterDtoInterceptor).GetMethod(
                        target.ReturnType == typeof(BaseItemDto[]) ? nameof(ApplyManyPostfix) : nameof(ApplyOnePostfix),
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
                YouTubeChannel.LogPublic("[YT] Native chapter item DTO interceptor installed (" + count + " patches).");
                return count;
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] Native chapter item DTO interceptor unavailable (" + error.GetType().Name + ").");
                return 0;
            }
        }

        internal static MethodInfo[] FindTargets(Type implementation)
        {
            if (!typeof(IDtoService).IsAssignableFrom(implementation)) return Array.Empty<MethodInfo>();
            var map = implementation.GetInterfaceMap(typeof(IDtoService));
            var result = new List<MethodInfo>();
            for (int i = 0; i < map.InterfaceMethods.Length; i++)
            {
                var method = map.InterfaceMethods[i];
                var parameters = method.GetParameters();
                bool many = method.Name == "GetBaseItemDtos" && method.ReturnType == typeof(BaseItemDto[]);
                bool one = method.Name == "GetBaseItemDto" && method.ReturnType == typeof(BaseItemDto);
                if (!(many || one) || parameters.Length is not (3 or 4)
                    || parameters[0].ParameterType != (many ? typeof(BaseItem[]) : typeof(BaseItem))
                    || parameters[1].ParameterType != typeof(DtoOptions) || parameters[2].ParameterType != typeof(User)
                    || parameters.Length == 4 && parameters[3].ParameterType != typeof(CancellationToken)
                    || map.TargetMethods[i].IsAbstract) continue;
                result.Add(map.TargetMethods[i]);
            }
            return result.Distinct().ToArray();
        }

        private static void ApplyOnePostfix(BaseItem __0, BaseItemDto? __result)
        {
            if (__0 != null && __result != null) YouTubeChapterSync.ApplyItemDto(__0, __result);
        }

        private static void ApplyManyPostfix(BaseItem[]? __0, BaseItemDto[]? __result)
        {
            if (!YouTubeDirectPlayback.Enabled || __0 == null || __result == null
                || __0.Length > 4096 || __result.Length > 4096) return;
            // Match by native identity, not output order: native implementations
            // may filter or reorder arrays while assembling results.
            var items = __0.Where(item => item != null && item.InternalId > 0)
                .GroupBy(item => item.InternalId).ToDictionary(group => group.Key, group => group.First());
            foreach (var dto in __result)
                if (dto != null && long.TryParse(dto.Id, NumberStyles.None, CultureInfo.InvariantCulture, out long id)
                    && items.TryGetValue(id, out var item)) YouTubeChapterSync.ApplyItemDto(item, dto);
        }
    }
}
