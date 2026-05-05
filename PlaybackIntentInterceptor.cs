using MediaBrowser.Model.Services;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

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

    // Captures PlaybackInfo requests server-side. Every normal Emby client goes
    // through this endpoint before playback, so this avoids client JS and log parsing.
    internal static class PlaybackIntentInterceptor
    {
        private const string HarmonyId = "emby.youtubeplugin.playback-intent";
        private static readonly TimeSpan IntentTtl = TimeSpan.FromSeconds(30);
        private static readonly ConcurrentDictionary<string, PlaybackIntent> Intents = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object Sync = new();
        private static object? _harmony;
        private static Type? _harmonyType;
        private static DateTime _lastCleanupUtc = DateTime.MinValue;

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

        private static void CapturePlaybackInfoPostfix(object __instance, object __0)
        {
            try
            {
                CapturePlaybackInfo(__instance, __0);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Playback intent capture failed: {ex.Message}");
            }
        }

        private static void CapturePlaybackInfo(object service, object requestDto)
        {
            // In Emby 4.9 GET keeps StartTimeTicks in the query string, while
            // POST can carry it directly on the request DTO. Read both.
            var serviceRequest = GetServiceRequest(service);
            var itemId = Normalize(GetStringProperty(requestDto, "Id"));
            var userId = Normalize(GetStringProperty(requestDto, "UserId"));
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(userId))
                return;

            var isPlayback = TryReadBoolProperty(requestDto, "IsPlayback")
                ?? TryReadBool(serviceRequest?.QueryString["IsPlayback"])
                ?? true;
            if (!isPlayback)
                return;

            var startTimeTicks = TryReadLongProperty(requestDto, "StartTimeTicks")
                ?? TryReadLong(serviceRequest?.QueryString["StartTimeTicks"]);
            if (!startTimeTicks.HasValue)
                return;

            CleanupExpired();

            var deviceId = Normalize(GetDeviceId(serviceRequest));
            var intent = new PlaybackIntent(itemId, userId, deviceId, startTimeTicks.Value, DateTime.UtcNow);

            // Store an exact key plus a same-user/item fallback. The short TTL
            // prevents an abandoned click from influencing later playback.
            Intents[MakeKey(userId, itemId, deviceId)] = intent;

            if (!string.IsNullOrEmpty(deviceId))
                Intents[MakeKey(userId, itemId, string.Empty)] = intent;
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
