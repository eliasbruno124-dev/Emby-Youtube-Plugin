using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // What the client asked Emby to do immediately before playback starts.
    // StartTimeTicks == 0 means "play from beginning"; a larger value means resume.
    internal sealed record PlaybackIntent(
        string ItemId,
        string UserId,
        string DeviceId,
        long StartTimeTicks,
        DateTime CapturedUtc);

    internal sealed record PlaybackInfoPatchContext(
        string ItemId,
        string Client,
        string DeviceName,
        string UserAgent,
        string IpAddress,
        string MaxStreamingBitrate,
        long StartTimeTicks,
        bool HasStartTimeTicks,
        bool IsPlayback,
        bool IsLikelyIos,
        bool IsLikelyNativeAndroid,
        bool IsLikelyNativeTheater);

    internal sealed record PlaybackInfoNormalizationResult(
        int Changed,
        int NativeDirectPlayDisabled);

    // Captures PlaybackInfo requests server-side. Every normal Emby client goes
    // through this endpoint before playback, so this avoids client JS and log parsing.
    internal static class PlaybackIntentInterceptor
    {
        private const string HarmonyId = "emby.youtubeplugin.playback-intent";
        private static readonly TimeSpan IntentTtl = TimeSpan.FromSeconds(30);
        private static readonly Regex YouTubeVideoIdRegex = new(
            @"(?:youtube\.com/watch\?v=|youtu\.be/|/vi/|[?&]v=)([A-Za-z0-9_-]{6,})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly ConcurrentDictionary<string, PlaybackIntent> Intents = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object Sync = new();
        private static object? _harmony;
        private static Type? _harmonyType;
        private static DateTime _lastCleanupUtc = DateTime.MinValue;
        private static int _playbackInfoLogsRemaining = 24;
        private static int _mediaSourceNormalizeLogsRemaining = 24;
        private static int _startStampSuppressLogsRemaining = 12;
        private static int _legacyTabletSourceLogsRemaining = 12;

        internal static void Install()
        {
            lock (Sync)
            {
                if (_harmony != null)
                    return;

                try
                {
                    // MediaInfoService is internal to Emby. Looking it up at
                    // runtime lets unsupported Emby versions fall back cleanly.
                    var mediaInfoServiceType = FindLoadedType("Emby.Server.MediaEncoding.Api.MediaInfoService");
                    if (mediaInfoServiceType == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Playback intent interceptor disabled; MediaInfoService not found.");
                        return;
                    }

                    // Harmony is embedded and loaded by reflection so the plugin
                    // still ships as a single DLL and Emby's type scan stays safe.
                    var harmonyAssembly = EmbeddedDependencyLoader.LoadHarmonyAssembly();
                    var harmonyType = harmonyAssembly?.GetType("HarmonyLib.Harmony", throwOnError: false);
                    var harmonyMethodType = harmonyAssembly?.GetType("HarmonyLib.HarmonyMethod", throwOnError: false);
                    if (harmonyType == null || harmonyMethodType == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Playback intent interceptor disabled; Harmony types not available.");
                        return;
                    }

                    var postfixMethod = typeof(PlaybackIntentInterceptor)
                        .GetMethod(nameof(CapturePlaybackInfoPostfix), BindingFlags.NonPublic | BindingFlags.Static);
                    if (postfixMethod == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Playback intent interceptor disabled; postfix method missing.");
                        return;
                    }

                    var postfix = Activator.CreateInstance(harmonyMethodType, postfixMethod);
                    var harmony = Activator.CreateInstance(harmonyType, HarmonyId);
                    if (harmony == null || postfix == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Playback intent interceptor disabled; Harmony instance could not be created.");
                        return;
                    }

                    var patched = 0;

                    // Clients may use GET or POST PlaybackInfo depending on app
                    // and Emby version. Patch both shapes when they exist.
                    if (PatchPublicMethod(harmony, postfix, mediaInfoServiceType, "Get",
                            "Emby.Server.MediaEncoding.Api.GetPlaybackInfo"))
                    {
                        patched++;
                    }

                    if (PatchPublicMethod(harmony, postfix, mediaInfoServiceType, "Post",
                            "Emby.Server.MediaEncoding.Api.GetPostedPlaybackInfo"))
                    {
                        patched++;
                    }

                    if (patched == 0)
                    {
                        YouTubeChannel.LogPublic("[YT] Playback intent interceptor disabled; no PlaybackInfo methods matched.");
                        return;
                    }

                    _harmony = harmony;
                    _harmonyType = harmonyType;
                    YouTubeChannel.LogPublic($"[YT] Playback intent interceptor installed ({patched} patches).");
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Playback intent interceptor install failed: {ex.Message}");
                }
            }
        }

        internal static void Uninstall()
        {
            lock (Sync)
            {
                try
                {
                    // Remove only our patch id; other Harmony users in the
                    // process must be left alone.
                    var unpatchAll = _harmonyType?.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(m => m.Name == "UnpatchAll"
                                             && m.GetParameters().Length == 1
                                             && m.GetParameters()[0].ParameterType == typeof(string));
                    unpatchAll?.Invoke(_harmony, new object?[] { HarmonyId });
                    _harmony = null;
                    _harmonyType = null;
                    Intents.Clear();
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Playback intent interceptor uninstall failed: {ex.Message}");
                }
            }
        }

        internal static bool TryConsume(string? userId, long itemId, string? deviceId, out PlaybackIntent intent)
        {
            // PlaybackStart consumes the intent once. If nothing was captured,
            // the caller should avoid guessing and simply leave playback alone.
            CleanupExpired();

            var normalizedUserId = Normalize(userId);
            var normalizedItemId = itemId.ToString(CultureInfo.InvariantCulture);
            var normalizedDeviceId = Normalize(deviceId);

            if (TryConsumeKey(MakeKey(normalizedUserId, normalizedItemId, normalizedDeviceId), out intent))
                return true;

            if (TryConsumeKey(MakeKey(normalizedUserId, normalizedItemId, string.Empty), out intent))
                return true;

            // Some clients omit or rename device ids between PlaybackInfo and
            // PlaybackStart. A fresh same-user/same-item intent is still safe.
            foreach (var kvp in Intents.ToArray())
            {
                var candidate = kvp.Value;
                if (!string.Equals(candidate.UserId, normalizedUserId, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(candidate.ItemId, normalizedItemId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(normalizedDeviceId)
                    && !string.IsNullOrEmpty(candidate.DeviceId)
                    && !string.Equals(candidate.DeviceId, normalizedDeviceId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (TryConsumeKey(kvp.Key, out intent))
                    return true;
            }

            intent = default!;
            return false;
        }

        private static bool TryConsumeKey(string key, out PlaybackIntent intent)
        {
            if (Intents.TryRemove(key, out intent!)
                && DateTime.UtcNow - intent.CapturedUtc <= IntentTtl)
            {
                return true;
            }

            intent = default!;
            return false;
        }

        private static bool PatchPublicMethod(
            object harmony,
            object postfix,
            Type serviceType,
            string methodName,
            string requestTypeName)
        {
            var requestType = serviceType.Assembly.GetType(requestTypeName, throwOnError: false);
            if (requestType == null)
            {
                // Expected on unsupported older/newer Emby builds.
                YouTubeChannel.LogPublic($"[YT] Playback intent patch target missing: {requestTypeName}");
                return false;
            }

            var method = serviceType.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: new[] { requestType },
                modifiers: null);

            if (method == null)
            {
                YouTubeChannel.LogPublic($"[YT] Playback intent patch method missing: {serviceType.FullName}.{methodName}({requestType.Name})");
                return false;
            }

            InvokeHarmonyPatch(harmony, method, postfix);
            return true;
        }

        private static void InvokeHarmonyPatch(object harmony, MethodInfo original, object postfix)
        {
            var patchMethod = harmony.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m =>
                {
                    if (m.Name != "Patch")
                        return false;

                    var parameters = m.GetParameters();
                    return parameters.Length >= 3
                           && typeof(MethodBase).IsAssignableFrom(parameters[0].ParameterType)
                           && parameters.Any(p => string.Equals(p.Name, "postfix", StringComparison.OrdinalIgnoreCase));
                });

            if (patchMethod == null)
                throw new MissingMethodException(harmony.GetType().FullName, "Patch");

            var patchParameters = patchMethod.GetParameters();
            var args = new object?[patchParameters.Length];
            args[0] = original;
            for (var i = 1; i < patchParameters.Length; i++)
            {
                if (string.Equals(patchParameters[i].Name, "postfix", StringComparison.OrdinalIgnoreCase))
                    args[i] = postfix;
                else
                    args[i] = null;
            }

            patchMethod.Invoke(harmony, args);
        }

        private static Type? FindLoadedType(string fullName)
        {
            // MediaInfoService lives in Emby's assemblies, not in this plugin.
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type? type = null;
                try
                {
                    type = assembly.GetType(fullName, throwOnError: false);
                }
                catch
                {
                    // Ignore assemblies that cannot be inspected.
                }

                if (type != null)
                    return type;
            }

            return Type.GetType(fullName, throwOnError: false);
        }

        private static void CapturePlaybackInfoPostfix(object __instance, object __0, ref Task<object> __result)
        {
            try
            {
                var context = CapturePlaybackInfo(__instance, __0);
                if (context != null && __result != null)
                    __result = PatchYouTubePlaybackInfoAsync(__result, context);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Playback intent capture failed: {ex.Message}");
            }
        }

        private static PlaybackInfoPatchContext? CapturePlaybackInfo(object service, object requestDto)
        {
            // In Emby 4.9 GET keeps StartTimeTicks in the query string, while
            // POST can carry it directly on the request DTO. Read both.
            var serviceRequest = GetServiceRequest(service);
            var itemId = Normalize(GetStringProperty(requestDto, "Id"));
            var userId = Normalize(GetStringProperty(requestDto, "UserId"));
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(userId))
                return null;

            var isPlayback = TryReadBoolProperty(requestDto, "IsPlayback")
                ?? TryReadBool(serviceRequest?.QueryString["IsPlayback"])
                ?? true;

            var startTimeTicks = TryReadLongProperty(requestDto, "StartTimeTicks")
                ?? TryReadLong(serviceRequest?.QueryString["StartTimeTicks"]);
            var hasStartTimeTicks = startTimeTicks.HasValue;
            var effectiveStartTimeTicks = startTimeTicks ?? 0;

            if (isPlayback && hasStartTimeTicks)
            {
                LogPlaybackInfoCapture(serviceRequest, itemId, effectiveStartTimeTicks);
                CleanupExpired();

                var deviceId = Normalize(GetDeviceId(serviceRequest));
                var intent = new PlaybackIntent(itemId, userId, deviceId, effectiveStartTimeTicks, DateTime.UtcNow);

                // Store an exact key plus a same-user/item fallback. The short TTL
                // prevents an abandoned click from influencing later playback.
                Intents[MakeKey(userId, itemId, deviceId)] = intent;

                if (!string.IsNullOrEmpty(deviceId))
                    Intents[MakeKey(userId, itemId, string.Empty)] = intent;
            }

            var client = GetRequestValue(serviceRequest, "X-Emby-Client") ?? "unknown";
            var deviceName = GetRequestValue(serviceRequest, "X-Emby-Device-Name") ?? "unknown";
            var userAgent = serviceRequest?.UserAgent ?? serviceRequest?.Headers["User-Agent"] ?? "unknown";
            var ipAddress = serviceRequest?.XRealIp
                            ?? serviceRequest?.XForwardedFor
                            ?? serviceRequest?.RemoteIp?.ToString()
                            ?? "unknown";
            var maxBitrate = GetRequestValue(serviceRequest, "MaxStreamingBitrate") ?? "unknown";

            return new PlaybackInfoPatchContext(
                itemId,
                client,
                deviceName,
                userAgent,
                ipAddress,
                maxBitrate,
                effectiveStartTimeTicks,
                hasStartTimeTicks,
                isPlayback,
                IsLikelyIosClient(serviceRequest),
                IsLikelyNativeAndroidClient(serviceRequest),
                IsLikelyNativeTheaterClient(serviceRequest));
        }

        private static void LogPlaybackInfoCapture(IRequest? request, string itemId, long startTimeTicks)
        {
            if (_playbackInfoLogsRemaining <= 0)
                return;

            var isLikelyIos = IsLikelyIosClient(request);
            var isLikelyNativeAndroid = IsLikelyNativeAndroidClient(request);
            var isLikelyNativeTheater = IsLikelyNativeTheaterClient(request);
            if (!isLikelyIos && !isLikelyNativeAndroid && !isLikelyNativeTheater)
                return;

            _playbackInfoLogsRemaining--;
            var client = GetRequestValue(request, "X-Emby-Client");
            var deviceName = GetRequestValue(request, "X-Emby-Device-Name");
            var maxBitrate = GetRequestValue(request, "MaxStreamingBitrate") ?? "unknown";
            var clientKind = isLikelyNativeTheater
                ? "native Theater/Xbox client"
                : isLikelyNativeAndroid
                    ? "native Android client"
                    : "iOS-like client";
            YouTubeChannel.LogPublic(
                $"[YT] PlaybackInfo captured for {clientKind}. Item={itemId}, StartTimeTicks={startTimeTicks}, MaxStreamingBitrate={maxBitrate}, Client={client ?? "unknown"}, Device={deviceName ?? "unknown"}.");
        }

        private static async Task<object> PatchYouTubePlaybackInfoAsync(Task<object> responseTask, PlaybackInfoPatchContext context)
        {
            var response = await responseTask.ConfigureAwait(false);
            try
            {
                // Resolve the YouTube video id from the library item itself. This
                // is authoritative even when Emby rewrote the media source path
                // for a transcode decision, and it lets us normalize the existing
                // library items whose stored media source isn't a clean watch URL.
                var youTubeVideoId = TryResolveYouTubeVideoId(context.ItemId);
                var normalized = NormalizeYouTubeMediaSources(response, youTubeVideoId, context);
                if (normalized.Changed > 0)
                    LogPlaybackInfoNormalization(context, normalized);

                if (context.HasStartTimeTicks && context.StartTimeTicks >= TimeSpan.TicksPerSecond * 5)
                {
                    if (ShouldSuppressYouTubeStartStamp(context))
                        LogYouTubeStartStampSuppressed(context);
                    else
                        StampYouTubeStartTime(response, context.StartTimeTicks);
                }
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackInfo YouTube patch failed: {ex.Message}");
            }

            return response;
        }

        private static void LogPlaybackInfoNormalization(PlaybackInfoPatchContext context, PlaybackInfoNormalizationResult result)
        {
            if (_mediaSourceNormalizeLogsRemaining <= 0)
                return;

            _mediaSourceNormalizeLogsRemaining--;
            var clientKind = context.IsLikelyNativeTheater
                ? "native Theater/Xbox client"
                : context.IsLikelyNativeAndroid
                    ? "native Android client"
                : context.IsLikelyIos
                    ? "iOS-like client"
                    : "client";
            var nativePart = result.NativeDirectPlayDisabled > 0
                ? $", NativeDirectPlayDisabled={result.NativeDirectPlayDisabled}"
                : string.Empty;
            YouTubeChannel.LogPublic(
                $"[YT] PlaybackInfo normalized {result.Changed} YouTube media source(s) for {clientKind}{nativePart}. Item={context.ItemId}, IsPlayback={context.IsPlayback}, StartTimeTicks={context.StartTimeTicks}, MaxStreamingBitrate={context.MaxStreamingBitrate}, Client={context.Client}, Device={context.DeviceName}, Ip={context.IpAddress}, UA={Shorten(context.UserAgent, 160)}.");
        }

        private static void StampYouTubeStartTime(object? response, long startTimeTicks)
        {
            var mediaSources = response?.GetType()
                .GetProperty("MediaSources", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(response) as IEnumerable;
            if (mediaSources == null)
                return;

            var changed = 0;
            var seconds = Math.Max(0, startTimeTicks / TimeSpan.TicksPerSecond);
            foreach (var mediaSource in mediaSources)
            {
                if (mediaSource == null)
                    continue;

                var pathProp = mediaSource.GetType()
                    .GetProperty("Path", BindingFlags.Instance | BindingFlags.Public);
                if (pathProp?.CanWrite != true)
                    continue;

                var path = pathProp.GetValue(mediaSource) as string;
                var stampedPath = PatchYouTubeUrl(path, seconds);
                if (string.IsNullOrEmpty(stampedPath)
                    || string.Equals(path, stampedPath, StringComparison.Ordinal))
                {
                    continue;
                }

                pathProp.SetValue(mediaSource, stampedPath);
                SetNullableLongProperty(mediaSource, "ContainerStartTimeTicks", startTimeTicks);
                changed++;
            }

            if (changed > 0)
                YouTubeChannel.LogPublic($"[YT] PlaybackInfo media source stamped with YouTube start={seconds}s.");
        }

        private static bool ShouldSuppressYouTubeStartStamp(PlaybackInfoPatchContext context)
            => IsLikelyNativeAndroidTablet(context);

        private static bool IsLikelyNativeAndroidTablet(PlaybackInfoPatchContext context)
        {
            if (!context.IsLikelyNativeAndroid)
                return false;

            if (ContainsIgnoreCase(context.DeviceName, "Tab")
                || ContainsIgnoreCase(context.DeviceName, "Tablet")
                || ContainsIgnoreCase(context.UserAgent, "SM-X"))
            {
                return true;
            }

            return ContainsIgnoreCase(context.UserAgent, "Android")
                   && ContainsIgnoreCase(context.UserAgent, "Safari/")
                   && !ContainsIgnoreCase(context.UserAgent, "Mobile")
                   && !ContainsIgnoreCase(context.UserAgent, "TV")
                   && !ContainsIgnoreCase(context.DeviceName, "TV");
        }

        private static string GetContextualYouTubeWatchUrl(string videoId, PlaybackInfoPatchContext context)
        {
            if (IsLikelyNativeAndroidTablet(context))
                return $"https://www.youtube.com/watch?v={videoId}";

            return YouTubeChannel.GetYouTubeWatchUrl(videoId);
        }

        private static void LogNativeAndroidTabletLegacySource(PlaybackInfoPatchContext context, string videoId)
        {
            if (_legacyTabletSourceLogsRemaining <= 0)
                return;

            _legacyTabletSourceLogsRemaining--;
            YouTubeChannel.LogPublic(
                $"[YT] PlaybackInfo using legacy YouTube watch source for native Android tablet. Video={videoId}, Item={context.ItemId}, Client={context.Client}, Device={context.DeviceName}, UA={Shorten(context.UserAgent, 160)}.");
        }

        private static void LogYouTubeStartStampSuppressed(PlaybackInfoPatchContext context)
        {
            if (_startStampSuppressLogsRemaining <= 0)
                return;

            _startStampSuppressLogsRemaining--;
            var seconds = Math.Max(0, context.StartTimeTicks / TimeSpan.TicksPerSecond);
            YouTubeChannel.LogPublic(
                $"[YT] PlaybackInfo YouTube start={seconds}s suppressed for native Android tablet. Item={context.ItemId}, Client={context.Client}, Device={context.DeviceName}, UA={Shorten(context.UserAgent, 160)}.");
        }

        private static PlaybackInfoNormalizationResult NormalizeYouTubeMediaSources(
            object? response,
            string? youTubeVideoId,
            PlaybackInfoPatchContext context)
        {
            var mediaSources = response?.GetType()
                .GetProperty("MediaSources", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(response) as IEnumerable;
            if (mediaSources == null)
                return new PlaybackInfoNormalizationResult(0, 0);

            var changed = 0;
            var nativeDirectPlayDisabled = 0;
            foreach (var mediaSource in mediaSources)
            {
                if (mediaSource == null)
                    continue;

                var pathProp = mediaSource.GetType()
                    .GetProperty("Path", BindingFlags.Instance | BindingFlags.Public);
                var path = pathProp?.GetValue(mediaSource) as string;

                // The library item is the authoritative source of the video id.
                // Only when that lookup is unavailable do we fall back to a
                // YouTube URL still present on the media source. We never key off
                // a bare id so a non-YouTube item can't be misidentified.
                var videoId = youTubeVideoId;
                if (string.IsNullOrEmpty(videoId))
                {
                    videoId = ExtractYouTubeVideoId(path)
                              ?? ExtractYouTubeVideoId(GetStringProperty(mediaSource, "OriginalPath"))
                              ?? ExtractYouTubeVideoId(GetStringProperty(mediaSource, "DirectStreamUrl"));
                }

                if (string.IsNullOrEmpty(videoId))
                    continue;

                var mediaSourceChanged = false;

                // Rewrite the path to a clean watch URL so the web client's
                // YouTube player canPlayItem() matches and the embed gets the id.
                if (pathProp?.CanWrite == true)
                {
                    var normalizedPath = GetContextualYouTubeWatchUrl(videoId, context);
                    if (!string.IsNullOrEmpty(normalizedPath)
                        && !string.Equals(path, normalizedPath, StringComparison.Ordinal))
                    {
                        pathProp.SetValue(mediaSource, normalizedPath);
                        if (IsLikelyNativeAndroidTablet(context))
                            LogNativeAndroidTabletLegacySource(context, videoId);
                        mediaSourceChanged = true;
                    }
                }

                // Existing library items can carry a stored ~40 Mbps bitrate that
                // trips a client's bitrate cap (e.g. iPad web at 4 Mbps) into a
                // transcode it cannot perform for a YouTube watch page.
                if (IsLikelyNativeAndroidTablet(context))
                {
                    mediaSourceChanged |= SetBoolProperty(mediaSource, "IsRemote", false);
                    mediaSourceChanged |= SetPropertyToNull(mediaSource, "Bitrate");
                    mediaSourceChanged |= SetPropertyToNull(mediaSource, "ContainerStartTimeTicks");
                }
                else
                {
                    mediaSourceChanged |= SetBoolProperty(mediaSource, "IsRemote", true);
                    mediaSourceChanged |= SetNumberProperty(mediaSource, "Bitrate", 1_000_000);
                }
                mediaSourceChanged |= SetBoolProperty(mediaSource, "SupportsProbing", false);
                mediaSourceChanged |= SetStringPropertyToNull(mediaSource, "ProbePath");
                mediaSourceChanged |= SetPropertyToNull(mediaSource, "ProbeProtocol");

                if (context.IsLikelyNativeTheater)
                {
                    // Native Theater clients without a browser engine treat Path
                    // as raw video. A YouTube watch URL is HTML, so advertising
                    // direct play here causes MPV/native playback to spin or fail.
                    mediaSourceChanged |= SetBoolProperty(mediaSource, "SupportsDirectPlay", false);
                    mediaSourceChanged |= SetStringPropertyToNull(mediaSource, "DirectStreamUrl");
                    nativeDirectPlayDisabled++;
                }
                else
                {
                    // Browser/webview clients need the source to remain selectable
                    // so their YouTube player can claim the item and embed it.
                    mediaSourceChanged |= SetBoolProperty(mediaSource, "SupportsDirectPlay", true);
                }

                mediaSourceChanged |= SetBoolProperty(mediaSource, "SupportsDirectStream", false);
                mediaSourceChanged |= SetBoolProperty(mediaSource, "SupportsTranscoding", false);
                mediaSourceChanged |= SetBoolProperty(mediaSource, "RequiresOpening", false);
                mediaSourceChanged |= SetBoolProperty(mediaSource, "RequiresClosing", false);
                mediaSourceChanged |= SetBoolProperty(mediaSource, "RequiresLooping", false);

                // Drop any transcode plan Emby attached before we forced direct play.
                mediaSourceChanged |= SetStringPropertyToNull(mediaSource, "TranscodingUrl");
                mediaSourceChanged |= SetStringPropertyToNull(mediaSource, "TranscodingSubProtocol");
                mediaSourceChanged |= SetStringPropertyToNull(mediaSource, "TranscodingContainer");

                if (mediaSourceChanged)
                    changed++;
            }

            return new PlaybackInfoNormalizationResult(changed, nativeDirectPlayDisabled);
        }

        // Resolves the canonical YouTube video id for a library item id using the
        // same strict check (watch path + matching provider id + plugin external
        // id) the rest of the plugin uses, so normal library items are untouched.
        private static string? TryResolveYouTubeVideoId(string? itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return null;

            try
            {
                var library = Plugin.ResolveService<ILibraryManager>();
                if (library == null)
                    return null;

                var item = ResolveBaseItem(library, itemId);
                return item == null ? null : YouTubeImageProvider.TryGetVideoId(item);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackInfo YouTube item lookup failed: {ex.Message}");
                return null;
            }
        }

        private static BaseItem? ResolveBaseItem(ILibraryManager library, string itemId)
        {
            // GetItemById overloads vary across Emby builds (long/Guid/string), so
            // pick whichever exists at runtime instead of binding to one shape.
            var methods = library.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "GetItemById" && m.GetParameters().Length == 1)
                .ToArray();

            if (long.TryParse(itemId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numericId))
            {
                var longMethod = methods.FirstOrDefault(m => m.GetParameters()[0].ParameterType == typeof(long));
                if (longMethod != null)
                    return longMethod.Invoke(library, new object[] { numericId }) as BaseItem;
            }

            if (Guid.TryParse(itemId, out var guidId))
            {
                var guidMethod = methods.FirstOrDefault(m => m.GetParameters()[0].ParameterType == typeof(Guid));
                if (guidMethod != null)
                    return guidMethod.Invoke(library, new object[] { guidId }) as BaseItem;
            }

            var stringMethod = methods.FirstOrDefault(m => m.GetParameters()[0].ParameterType == typeof(string));
            if (stringMethod != null)
                return stringMethod.Invoke(library, new object[] { itemId }) as BaseItem;

            return null;
        }

        private static string? ExtractYouTubeVideoId(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var match = YouTubeVideoIdRegex.Match(value);
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string? PatchYouTubeUrl(string? path, long? startSeconds)
        {
            if (string.IsNullOrWhiteSpace(path)
                || !Uri.TryCreate(path, UriKind.Absolute, out var uri)
                || !IsYouTubeHost(uri.Host))
            {
                return null;
            }

            var queryParts = uri.Query.TrimStart('?')
                .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p =>
                {
                    var keyEnd = p.IndexOf('=');
                    var key = keyEnd >= 0 ? p.Substring(0, keyEnd) : p;
                    if (startSeconds.HasValue
                        && (string.Equals(key, "t", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(key, "start", StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }

                    if (string.Equals(key, "playsinline", StringComparison.OrdinalIgnoreCase))
                        return false;

                    if (string.Equals(key, "enablejsapi", StringComparison.OrdinalIgnoreCase))
                        return false;

                    return true;
                })
                .ToList();

            queryParts.Add("playsinline=1");
            queryParts.Add("enablejsapi=1");
            if (startSeconds.HasValue)
            {
                var secondsText = startSeconds.Value.ToString(CultureInfo.InvariantCulture);
                queryParts.Add($"t={secondsText}s");
                queryParts.Add($"start={secondsText}");
            }

            var builder = new UriBuilder(uri)
            {
                Query = string.Join("&", queryParts),
                Fragment = string.Empty
            };
            return builder.Uri.ToString();
        }

        private static bool SetStringPropertyToNull(object source, string name)
        {
            var prop = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanWrite != true || prop.PropertyType != typeof(string))
                return false;

            if (prop.GetValue(source) is null)
                return false;

            prop.SetValue(source, null);
            return true;
        }

        private static bool SetPropertyToNull(object source, string name)
        {
            var prop = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanWrite != true)
                return false;

            if (prop.PropertyType.IsValueType
                && Nullable.GetUnderlyingType(prop.PropertyType) == null)
            {
                return false;
            }

            if (prop.GetValue(source) is null)
                return false;

            prop.SetValue(source, null);
            return true;
        }

        private static bool IsYouTubeHost(string host) =>
            host.Equals("youtube.com", StringComparison.OrdinalIgnoreCase)
            || host.EndsWith(".youtube.com", StringComparison.OrdinalIgnoreCase)
            || host.Equals("youtu.be", StringComparison.OrdinalIgnoreCase);

        private static void SetNullableLongProperty(object source, string name, long value)
        {
            var prop = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanWrite == true
                && (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?)))
            {
                prop.SetValue(source, value);
            }
        }

        private static bool SetBoolProperty(object source, string name, bool value)
        {
            var prop = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanWrite != true
                || (prop.PropertyType != typeof(bool) && prop.PropertyType != typeof(bool?)))
            {
                return false;
            }

            var current = prop.GetValue(source);
            if (current is bool currentBool && currentBool == value)
                return false;

            prop.SetValue(source, value);
            return true;
        }

        private static bool SetNumberProperty(object source, string name, int value)
        {
            var prop = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanWrite != true)
                return false;

            var current = prop.GetValue(source);
            if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
            {
                if (current is int currentInt && currentInt == value)
                    return false;

                prop.SetValue(source, value);
                return true;
            }

            if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
            {
                if (current is long currentLong && currentLong == value)
                    return false;

                prop.SetValue(source, (long)value);
                return true;
            }

            return false;
        }

        private static IRequest? GetServiceRequest(object service)
        {
            return service.GetType()
                .GetProperty("Request", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.GetValue(service) as IRequest;
        }

        private static string? GetDeviceId(IRequest? request)
        {
            if (request == null)
                return null;

            // Header/query names vary between Emby clients and older
            // MediaBrowser-compatible callers.
            return request.QueryString["X-Emby-Device-Id"]
                   ?? request.Headers["X-Emby-Device-Id"]
                   ?? request.QueryString["X-MediaBrowser-DeviceId"]
                   ?? request.Headers["X-MediaBrowser-DeviceId"]
                   ?? request.QueryString["DeviceId"]
                   ?? request.Headers["DeviceId"];
        }

        private static string? GetRequestValue(IRequest? request, string name)
        {
            if (request == null)
                return null;

            return request.QueryString[name] ?? request.Headers[name];
        }

        private static bool IsLikelyIosClient(IRequest? request)
        {
            var deviceName = GetRequestValue(request, "X-Emby-Device-Name");
            var userAgent = request?.UserAgent ?? request?.Headers["User-Agent"];
            return ContainsIgnoreCase(deviceName, "iOS")
                   || ContainsIgnoreCase(deviceName, "iPad")
                   || ContainsIgnoreCase(userAgent, "iPad")
                   || ContainsIgnoreCase(userAgent, "iPhone")
                   || ContainsIgnoreCase(userAgent, "Mobile/")
                   || ContainsIgnoreCase(userAgent, "VivaiOS");
        }

        private static bool IsLikelyNativeAndroidClient(IRequest? request)
        {
            var client = GetRequestValue(request, "X-Emby-Client");
            return ContainsIgnoreCase(client, "Emby for Android");
        }

        private static bool IsLikelyNativeTheaterClient(IRequest? request)
        {
            var client = GetRequestValue(request, "X-Emby-Client");
            var deviceName = GetRequestValue(request, "X-Emby-Device-Name");
            var userAgent = request?.UserAgent ?? request?.Headers["User-Agent"];

            if (ContainsIgnoreCase(client, "Emby Web")
                || IsLikelyBrowserEngineClient(userAgent))
            {
                return false;
            }

            return ContainsIgnoreCase(client, "Emby Xbox")
                   || ContainsIgnoreCase(client, "Emby Theater")
                   || ContainsIgnoreCase(client, "Emby Theatre")
                   || ContainsIgnoreCase(client, "Emby Windows")
                   || ContainsIgnoreCase(deviceName, "XBOX")
                   || ContainsIgnoreCase(deviceName, "Xbox")
                   || ContainsIgnoreCase(userAgent, "Xbox");
        }

        private static bool IsLikelyBrowserEngineClient(string? userAgent) =>
            ContainsIgnoreCase(userAgent, "Chrome/")
            || ContainsIgnoreCase(userAgent, "Chromium/")
            || ContainsIgnoreCase(userAgent, "Edg/")
            || ContainsIgnoreCase(userAgent, "AppleWebKit/")
            || ContainsIgnoreCase(userAgent, "Safari/")
            || ContainsIgnoreCase(userAgent, "Firefox/")
            || ContainsIgnoreCase(userAgent, "CriOS/")
            || ContainsIgnoreCase(userAgent, "FxiOS/");

        private static bool ContainsIgnoreCase(string? value, string needle) =>
            !string.IsNullOrEmpty(value)
            && value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

        private static string Shorten(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength) + "...";
        }

        private static string? GetStringProperty(object source, string name)
        {
            return source.GetType()
                .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(source) as string;
        }

        private static long? TryReadLongProperty(object source, string name)
        {
            var value = source.GetType()
                .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(source);
            return TryReadLong(value);
        }

        private static long? TryReadLong(object? value)
        {
            return value switch
            {
                long longValue => longValue,
                int intValue => intValue,
                string text when long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) => parsed,
                _ => null
            };
        }

        private static bool? TryReadBoolProperty(object source, string name)
        {
            var value = source.GetType()
                .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(source);
            return TryReadBool(value);
        }

        private static bool? TryReadBool(object? value)
        {
            return value switch
            {
                bool boolValue => boolValue,
                string text when bool.TryParse(text, out var parsed) => parsed,
                _ => null
            };
        }

        private static void CleanupExpired()
        {
            // Keep the dictionary small and make sure stale intents cannot be
            // reused after a cancelled playback attempt.
            var now = DateTime.UtcNow;
            if (now - _lastCleanupUtc < IntentTtl)
                return;

            _lastCleanupUtc = now;
            foreach (var kvp in Intents)
            {
                if (now - kvp.Value.CapturedUtc > IntentTtl)
                    Intents.TryRemove(kvp.Key, out _);
            }
        }

        private static string MakeKey(string userId, string itemId, string deviceId)
        {
            return $"{Normalize(userId)}|{Normalize(itemId)}|{Normalize(deviceId)}";
        }

        private static string Normalize(string? value)
        {
            return (value ?? string.Empty).Trim();
        }
    }
}
