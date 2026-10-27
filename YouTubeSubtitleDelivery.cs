using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Session;
using System.Collections;
using System.Reflection;

namespace Emby.YouTubePlugin
{
    internal static class YouTubeSubtitleDelivery
    {
        private const string SourcePrefix = "youtube-direct-";
        private static readonly MethodInfo Clone = typeof(object).GetMethod(
            "MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;
        private static volatile bool _installed;
        private static InputBinding? _input;
        private static RenderBinding? _render;
        private sealed record InputBinding(PropertyInfo Inputs, PropertyInfo Input0, PropertyInfo Url,
            PropertyInfo Subtitles0, PropertyInfo IsExternal, PropertyInfo SubtitlePath);
        private sealed record RenderBinding(MethodInfo EnsureSoftware, MethodInfo Legacy,
            PropertyInfo Steps, PropertyInfo Demand, PropertyInfo InHardware);

        // The caller must already have established an owned direct YouTube
        // request. This changes delivery, never the user's selected index/Off.
        // Native rendering is a compatibility workaround for clients that fetch
        // external VTT successfully but do not display it, not a renderer diagnosis.
        internal static void PreferNativeRendering(object requestDto)
        {
            if (!_installed || requestDto == null) return;
            var property = requestDto.GetType().GetProperty("DeviceProfile", BindingFlags.Public | BindingFlags.Instance);
            if (property?.CanRead != true || !property.CanWrite || property.GetIndexParameters().Length != 0
                || property.GetValue(requestDto) is not DeviceProfile profile)
                return;

            // Copy the profile rather than modifying shared client capabilities.
            // Remove alternate External/Hls conversion routes as well: merely
            // changing a vtt entry leaves e.g. External/ass eligible for conversion.
            // Direct YouTube caption inventory contains only verified text VTT.
            var local = (DeviceProfile)Clone.Invoke(profile, null)!;
            local.SubtitleProfiles = NativeProfiles();
            property.SetValue(requestDto, local);
        }

        // GET/null-profile requests resolve their device profile inside Emby.
        // Guard the exact native negotiation boundary as well, without changing
        // the shared profile or any caller's array in place.
        internal static int Install(object harmony, Type harmonyMethodType)
        {
            try
            {
                _installed = false;
                if (typeof(StreamBuilder).Assembly.GetName().Version != new Version(4, 10, 2, 0)) return 0;
                var target = typeof(StreamBuilder).GetMethod("GetSubtitleStreams", BindingFlags.NonPublic | BindingFlags.Instance,
                    null, new[] { typeof(MediaSourceInfo), typeof(List<MediaStream>), typeof(int?), typeof(SubtitleProfile[]),
                        typeof(PlayMethod), typeof(EncodingContext), typeof(string), typeof(string), typeof(string), typeof(int) }, null);
                if (target?.ReturnType != typeof(RequestedStreamInfo[])) return 0;
                var patch = harmony.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .SingleOrDefault(method => method.Name == "Patch" && method.GetParameters() is var parameters
                        && parameters.Length >= 3 && parameters[0].ParameterType == typeof(MethodBase)
                        && parameters.Any(parameter => parameter.Name == "prefix"));
                if (patch == null) return 0;
                Type Find(string name) => AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(name, false))
                    .FirstOrDefault(type => type != null) ?? throw new InvalidOperationException();
                var pipeline = Find("Emby.Server.MediaEncoding.Encoder.Pipelines.Video.PipelineBuilderBase");
                if (pipeline.Assembly.GetName().Version != new Version(4, 10, 2, 0)) return 0;
                var nativeSource = pipeline.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single(method => method.Name == "CreateTextSubtitleStreamSource"
                        && method.ReturnType.FullName == "Emby.Ffmpeg.Model.Interfaces.ISubtitleStreamSource"
                        && method.GetParameters().Select(parameter => parameter.ParameterType.FullName).SequenceEqual(new[]
                        {
                            "Emby.Server.MediaEncoding.Encoder.Pipelines.FilterBuilder.StateTrackingFilterBuilder",
                            "Emby.Ffmpeg.Model.FfmpegCommand",
                            "Emby.Server.MediaEncoding.Encoder.Demands.VideoProcessing.SubtitleBurnInDemand",
                            "Emby.Server.MediaEncoding.Encoder.Demands.DemandsList"
                        }));
                var command = nativeSource.GetParameters()[1].ParameterType;
                var demand = nativeSource.GetParameters()[2].ParameterType;
                PropertyInfo Property(Type type, string name, Type? expected = null)
                {
                    var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                    if (property?.CanRead != true || expected != null && property.PropertyType != expected) throw new InvalidOperationException();
                    return property;
                }
                var first = Property(command, "Input0");
                var inputs = Property(command, "Inputs");
                var subtitles = Property(first.PropertyType, "Subtitles0");
                if (!typeof(IEnumerable).IsAssignableFrom(inputs.PropertyType)
                    || !nativeSource.ReturnType.IsAssignableFrom(subtitles.PropertyType)) throw new InvalidOperationException();
                _input = new InputBinding(inputs, first, Property(first.PropertyType, "Url", typeof(string)), subtitles,
                    Property(demand, "IsExternal", typeof(bool)), Property(demand, "SubtitlePath", typeof(string)));
                var tracking = nativeSource.GetParameters()[0].ParameterType;
                var demands = nativeSource.GetParameters()[3].ParameterType;
                MethodInfo Method(Type type, string name, params Type[] types)
                {
                    var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
                    if (method?.ReturnType != typeof(void)) throw new InvalidOperationException();
                    return method;
                }
                var software = Method(pipeline, "AddSoftwareSubtitleBurnInFilter", command, tracking, demand, demands);
                var plan = Find("Emby.Server.MediaEncoding.Encoder.Pipelines.ProcessingSteps.ProcessingPlan");
                var step = Find("Emby.Server.MediaEncoding.Encoder.Pipelines.ProcessingSteps.ProcessingStep");
                var hardware = Property(step, "WillDoInHardware", typeof(bool));
                var steps = Property(plan, "ProcessingSteps");
                if (!hardware.CanWrite || !typeof(IEnumerable).IsAssignableFrom(steps.PropertyType)) throw new InvalidOperationException();
                _render = new RenderBinding(Method(pipeline, "EnsureSoftwareContext", tracking),
                    Method(pipeline, "AddLegacySoftwareSubtitleBurnInFilter", tracking, demand),
                    steps, Property(step, "Demand"), hardware);
                var pipelineArguments = new[] { plan, demands, command,
                    Find("Emby.Server.MediaEncoding.Codecs.Common.Types.ConfiguredVideoDecoder"),
                    Find("Emby.Server.MediaEncoding.Codecs.Common.Types.ConfiguredVideoEncoder"), tracking };
                var pipelines = new[] { "Software", "Vaapi", "Nvidia", "QuickSync", "MediaCodec" }
                    .Select(name => Method(Find("Emby.Server.MediaEncoding.Encoder.Pipelines.Video.PipelineBuilder" + name),
                        "SetupVideoProcessing", pipelineArguments)).ToArray();
                if (pipelines.Any(method => method.IsAbstract)) throw new InvalidOperationException();
                void Add(MethodInfo original, string? prefix, string? postfix)
                {
                    var parameters = patch.GetParameters();
                    var arguments = new object?[parameters.Length];
                    arguments[0] = original;
                    for (var i = 1; i < arguments.Length; i++)
                    {
                        var callbackName = parameters[i].Name == "prefix" ? prefix : parameters[i].Name == "postfix" ? postfix : null;
                        if (callbackName != null) arguments[i] = Activator.CreateInstance(harmonyMethodType,
                            typeof(YouTubeSubtitleDelivery).GetMethod(callbackName, BindingFlags.NonPublic | BindingFlags.Static)!);
                    }
                    patch.Invoke(harmony, arguments);
                }
                Add(nativeSource, nameof(NativeSourcePrefix), nameof(NativeSourcePostfix));
                Add(software, nameof(NativeRenderPrefix), null);
                foreach (var method in pipelines) Add(method, nameof(NativePlanPrefix), null);
                Add(target, nameof(NativeSubtitlePrefix), null);
                _installed = true;
                return 3 + pipelines.Length;
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                YouTubeChannel.LogPublic("[YT] Native subtitle delivery interceptor unavailable (" + error.GetType().Name + ").");
                return 0;
            }
        }

        private static void NativeSubtitlePrefix(MediaSourceInfo __0, ref SubtitleProfile[] __3, PlayMethod __4, EncodingContext __5)
        {
            if (!_installed || __4 != PlayMethod.Transcode || __5 != EncodingContext.Streaming
                || __0?.Id == null || !__0.Id.StartsWith(SourcePrefix, StringComparison.Ordinal)
                || __0.Id.Length != SourcePrefix.Length + 11
                || !__0.Id.Substring(SourcePrefix.Length).All(YouTubeDirectPlayback.IsVideoIdCharacter)) return;
            try
            {
                if (__0.Protocol == MediaProtocol.Http)
                    YouTubeDirectPlayback.RequireOwnedMediaUri(__0.Path);
                else if (__0.Protocol == MediaProtocol.File && string.Equals(__0.Container, "hls", StringComparison.OrdinalIgnoreCase))
                    YouTubeDirectPlayback.RequireOwnedManifestPath(__0.Path);
                else return;
            }
            catch (Exception error) when (error is not OutOfMemoryException) { return; }
            __3 = NativeProfiles();
        }

        private static void NativeSourcePrefix(object __1, object __2, out int __state)
        {
            __state = -1;
            var binding = _input;
            if (!_installed || binding == null) return;
            try
            {
                if (!IsOwnedCaptionCommand(__1, __2)) return;
                __state = ((IEnumerable)binding.Inputs.GetValue(__1)!).Cast<object>().Count();
            }
            catch (Exception error) when (error is not OutOfMemoryException) { }
        }

        private static void NativeSourcePostfix(object __1, object __2, int __state, ref object? __result)
        {
            var binding = _input;
            if (__state < 0 || binding == null) return;
            // Emby 4.10.2 adds the correct external subtitle input, then returns
            // Inputs[1].Subtitles0. A separate audio input already occupies that
            // slot. Select only this call's newly added matching caption input;
            // never assume the last input or alter any audio/input options.
            var path = binding.SubtitlePath.GetValue(__2) as string;
            var matches = ((IEnumerable)binding.Inputs.GetValue(__1)!).Cast<object>().Skip(__state)
                .Where(input => string.Equals(binding.Url.GetValue(input) as string, path, StringComparison.Ordinal))
                .Select(input => binding.Subtitles0.GetValue(input)).Where(stream => stream != null).Take(2).ToArray();
            if (matches.Length == 1) __result = matches[0];
        }

        private static void NativePlanPrefix(object __0, object __2)
        {
            var binding = _render;
            if (!_installed || binding == null) return;
            foreach (var step in (IEnumerable)binding.Steps.GetValue(__0)!)
            {
                var demand = binding.Demand.GetValue(step);
                if (demand != null && IsOwnedCaptionCommand(__2, demand))
                    binding.InHardware.SetValue(step, false);
            }
        }

        private static bool NativeRenderPrefix(object __instance, object __0, object __1, object __2)
        {
            var binding = _render;
            if (!_installed || binding == null || !IsOwnedCaptionCommand(__0, __2)) return true;
            // Keep native hardware download/state tracking and native timestamp
            // handling. Only the text renderer changes to Emby's libass path;
            // the native pipeline still uploads again for hardware encoding.
            binding.EnsureSoftware.Invoke(__instance, new[] { __1 });
            binding.Legacy.Invoke(__instance, new[] { __1, __2 });
            return false;
        }

        private static bool IsOwnedCaptionCommand(object command, object demand)
        {
            var binding = _input;
            if (binding == null || !binding.IsExternal.DeclaringType!.IsInstanceOfType(demand)) return false;
            try
            {
                if (binding.IsExternal.GetValue(demand) is not true
                    || binding.Input0.GetValue(command) is not object first
                    || binding.Url.GetValue(first) is not string primary
                    || binding.SubtitlePath.GetValue(demand) is not string caption
                    || !IsOwnedCaptionPath(caption)) return false;
                if (Uri.TryCreate(primary, UriKind.Absolute, out var uri) && uri.Scheme == "http")
                    YouTubeDirectPlayback.RequireOwnedMediaUri(primary);
                else YouTubeDirectPlayback.RequireOwnedManifestPath(primary);
                return true;
            }
            catch (Exception error) when (error is not OutOfMemoryException) { return false; }
        }

        private static bool IsOwnedCaptionPath(string path)
        {
            if (string.IsNullOrWhiteSpace(Plugin.CachePath) || !Path.IsPathRooted(path)) return false;
            var root = Path.GetFullPath(Path.Combine(Plugin.CachePath, "direct-captions-v1"));
            var full = Path.GetFullPath(path);
            var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!string.Equals(Path.GetDirectoryName(full), root, comparison)) return false;
            var name = Path.GetFileName(full);
            if (name.Length != 151 || !name.StartsWith("ytcap-", StringComparison.Ordinal)
                || !name.EndsWith(".vtt", StringComparison.Ordinal) || name[17] != '-' || name[82] != '-'
                || !name.Substring(6, 11).All(YouTubeDirectPlayback.IsVideoIdCharacter)
                || !name.Substring(18, 64).All(IsHex) || !name.Substring(83, 64).All(IsHex)) return false;
            var directory = new DirectoryInfo(root);
            var file = new FileInfo(full);
            return directory.Exists && directory.LinkTarget == null && file.Exists && file.LinkTarget == null
                && file.Length is > 0 and <= 2 * 1024 * 1024;
        }

        private static bool IsHex(char value) => value is >= '0' and <= '9' or >= 'a' and <= 'f';

        private static SubtitleProfile[] NativeProfiles() => new[]
        {
            new SubtitleProfile { Format = "vtt", Method = SubtitleDeliveryMethod.Encode },
            new SubtitleProfile { Format = "srt", Method = SubtitleDeliveryMethod.Encode }
        };
    }
}
