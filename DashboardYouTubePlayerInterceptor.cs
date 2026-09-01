using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    internal static class DashboardYouTubePlayerInterceptor
    {
        private const string HarmonyId = "emby.youtubeplugin.dashboard-youtube-player";
        private const string PatchMarker = "ytPluginPatch20260606";
        private const string AppJsPatchMarker = "ytPluginAppJs20260608";
        private static readonly object Sync = new();
        private static object? _harmony;
        private static Type? _harmonyType;
        private static int _patchedResponseLogsRemaining = 24;
        private static int _optionalPatchLogsRemaining = 12;

        internal static void Install()
        {
            lock (Sync)
            {
                if (_harmony != null)
                    return;

                try
                {
                    var webAppServiceType = FindLoadedType("Emby.Web.Api.WebAppService");
                    var dashboardResourceType = FindLoadedType("Emby.Web.Api.GetDashboardResource");
                    if (webAppServiceType == null || dashboardResourceType == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Dashboard YouTube player interceptor disabled; WebAppService target not found.");
                        return;
                    }

                    var harmonyAssembly = EmbeddedDependencyLoader.LoadHarmonyAssembly();
                    var harmonyType = harmonyAssembly?.GetType("HarmonyLib.Harmony", throwOnError: false);
                    var harmonyMethodType = harmonyAssembly?.GetType("HarmonyLib.HarmonyMethod", throwOnError: false);
                    if (harmonyType == null || harmonyMethodType == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Dashboard YouTube player interceptor disabled; Harmony types not available.");
                        return;
                    }

                    var prefixMethod = typeof(DashboardYouTubePlayerInterceptor)
                        .GetMethod(nameof(GetDashboardResourcePrefix), BindingFlags.NonPublic | BindingFlags.Static);
                    if (prefixMethod == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Dashboard YouTube player interceptor disabled; prefix method missing.");
                        return;
                    }

                    var method = webAppServiceType.GetMethod(
                        "Get",
                        BindingFlags.Public | BindingFlags.Instance,
                        binder: null,
                        types: new[] { dashboardResourceType },
                        modifiers: null);
                    if (method == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Dashboard YouTube player interceptor disabled; GetDashboardResource method missing.");
                        return;
                    }

                    var prefix = Activator.CreateInstance(harmonyMethodType, prefixMethod);
                    var harmony = Activator.CreateInstance(harmonyType, HarmonyId);
                    if (harmony == null || prefix == null)
                    {
                        YouTubeChannel.LogPublic("[YT] Dashboard YouTube player interceptor disabled; Harmony instance could not be created.");
                        return;
                    }

                    InvokeHarmonyPatch(harmony, method, prefix);
                    _harmony = harmony;
                    _harmonyType = harmonyType;
                    YouTubeChannel.LogPublic("[YT] Dashboard YouTube player interceptor installed.");
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player interceptor install failed: {ex.Message}");
                }
            }
        }

        internal static void Uninstall()
        {
            lock (Sync)
            {
                try
                {
                    var unpatchAll = _harmonyType?.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(m => m.Name == "UnpatchAll"
                                             && m.GetParameters().Length == 1
                                             && m.GetParameters()[0].ParameterType == typeof(string));
                    unpatchAll?.Invoke(_harmony, new object?[] { HarmonyId });
                    _harmony = null;
                    _harmonyType = null;
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player interceptor uninstall failed: {ex.Message}");
                }
            }
        }

        private static bool GetDashboardResourcePrefix(object __instance, object __0, ref Task<object> __result)
        {
            try
            {
                var resourceName = GetStringProperty(__0, "ResourceName");

                // Client-side telemetry channel: the patched player JS pings
                // modules/youtubeplayer/ytdiag.js?m=<event> so we can see the real
                // in-webview playback flow (ready/state/error) in the plugin log.
                if (IsDiagResource(resourceName))
                {
                    var diagRequest = GetProperty(__instance, "Request") as IRequest;
                    var diagResultFactory = GetProperty(__instance, "ResultFactory") as IHttpResultFactory
                                            ?? GetField(__instance, "_resultFactory") as IHttpResultFactory;
                    LogDiagIfMatch(__instance, resourceName);

                    if (diagRequest == null || diagResultFactory == null)
                        return true;

                    __result = Task.FromResult(diagResultFactory.GetResult(
                        diagRequest,
                        ReadOnlyMemory<byte>.Empty,
                        "application/x-javascript",
                        NoCacheHeaders()));
                    return false;
                }

                if (!IsTargetResource(resourceName))
                    return true;

                byte[] patchedBytes;
                if (IsEmbedResource(resourceName))
                {
                    if (!TryReadEmbeddedPlayer(out patchedBytes))
                        return true;
                }
                else if (!TryReadAndPatchResource(__instance, resourceName!, out patchedBytes))
                {
                    return true;
                }

                var request = GetProperty(__instance, "Request") as IRequest;
                var resultFactory = GetProperty(__instance, "ResultFactory") as IHttpResultFactory
                                    ?? GetField(__instance, "_resultFactory") as IHttpResultFactory;
                if (request == null || resultFactory == null)
                    return true;

                var result = resultFactory.GetResult(
                    request,
                    new ReadOnlyMemory<byte>(patchedBytes),
                    GetContentType(resourceName),
                    NoCacheHeaders());

                __result = Task.FromResult(result);

                if (_patchedResponseLogsRemaining > 0)
                {
                    _patchedResponseLogsRemaining--;
                    YouTubeChannel.LogPublic(
                        $"[YT] Dashboard YouTube player response patched for {NormalizeResourceName(resourceName)} ({DescribeRequest(request)}).");
                }

                return false;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player response patch failed: {ex.Message}");
                return true;
            }
        }

        private static int _diagLogsRemaining = 400;

        private static bool IsDiagResource(string? resourceName)
        {
            var normalized = NormalizeResourceName(resourceName);
            return normalized.EndsWith("modules/youtubeplayer/ytdiag.js", StringComparison.OrdinalIgnoreCase);
        }

        private static void LogDiagIfMatch(object service, string? resourceName)
        {
            if (!IsDiagResource(resourceName))
                return;

            try
            {
                if (_diagLogsRemaining > 0)
                {
                    _diagLogsRemaining--;
                    var request = GetProperty(service, "Request") as IRequest;
                    var msg = request?.QueryString["m"];
                    YouTubeChannel.LogPublic($"[YT][DIAG] {msg ?? "?"} ({DescribeRequest(request)})");
                }
            }
            catch
            {
                // Telemetry must never break resource serving.
            }
        }

        private static Dictionary<string, string> NoCacheHeaders() =>
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Cache-Control"] = "no-cache, no-store, must-revalidate",
                ["Pragma"] = "no-cache",
                ["Expires"] = "0"
            };

        private static string DescribeRequest(IRequest? request)
        {
            if (request == null)
                return "client=?, device=?, ip=?, ua=?";

            var client = RequestValue(request, "X-Emby-Client") ?? "?";
            var device = RequestValue(request, "X-Emby-Device-Name") ?? "?";
            var ip = request.XRealIp
                     ?? request.XForwardedFor
                     ?? request.RemoteIp?.ToString()
                     ?? "?";
            var userAgent = request.UserAgent ?? request.Headers["User-Agent"] ?? "?";
            return $"client={client}, device={device}, ip={ip}, ua={Shorten(userAgent, 140)}";
        }

        private static string? RequestValue(IRequest request, string name)
        {
            return request.QueryString[name] ?? request.Headers[name];
        }

        private static string Shorten(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength) + "...";
        }

        private static bool TryReadAndPatchResource(object service, string resourceName, out byte[] patchedBytes)
        {
            patchedBytes = Array.Empty<byte>();

            var dashboardPath = GetStringProperty(service, "DashboardUIPath");
            if (string.IsNullOrWhiteSpace(dashboardPath))
                return false;

            var normalizedResourceName = NormalizeResourceName(resourceName);
            var dashboardFullPath = Path.GetFullPath(dashboardPath);
            var resourceFullPath = Path.GetFullPath(Path.Combine(
                dashboardFullPath,
                normalizedResourceName.Replace('/', Path.DirectorySeparatorChar)));

            if (!resourceFullPath.StartsWith(dashboardFullPath, StringComparison.OrdinalIgnoreCase)
                || !File.Exists(resourceFullPath))
            {
                return false;
            }

            var original = File.ReadAllText(resourceFullPath, Encoding.UTF8);
            var patched = PatchResource(normalizedResourceName, original);

            if (string.IsNullOrEmpty(patched)
                || string.Equals(original, patched, StringComparison.Ordinal))
            {
                return false;
            }

            patchedBytes = Encoding.UTF8.GetBytes(patched);
            return true;
        }

        private static bool TryReadEmbeddedPlayer(out byte[] content)
        {
            content = Array.Empty<byte>();
            try
            {
                var assembly = typeof(DashboardYouTubePlayerInterceptor).Assembly;
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(name => name.EndsWith(".YouTubeEmbed.html", StringComparison.Ordinal));
                if (resourceName == null)
                    return false;

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    return false;

                using var buffer = new MemoryStream();
                stream.CopyTo(buffer);
                content = buffer.ToArray();
                return content.Length > 0;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Embedded YouTube player read failed: {ex.Message}");
                return false;
            }
        }

        private static string PatchResource(string normalizedResourceName, string source)
        {
            // Keep the shell patch narrow: app.js is only adjusted at the
            // YouTube-player registration site so browser-engine Theater/Xbox
            // clients load Emby's normal YouTube player instead of falling
            // through to a native/raw playback path.
            if (normalizedResourceName.EndsWith("app.js", StringComparison.OrdinalIgnoreCase))
                return PatchAppJs(source);

            if (normalizedResourceName.EndsWith("plugin_webview.js", StringComparison.OrdinalIgnoreCase))
                return PatchWebViewPlayer(source);

            return PatchIframePlayer(source);
        }

        private static string PatchAppJs(string source)
        {
            var patched = source;

            if (!patched.Contains(AppJsPatchMarker, StringComparison.Ordinal))
            {
                const string registrationPrefix =
                    "appHost.supports(\"youtube\"))switch(appMode){case\"android\":case\"tizen\":case\"webos\":";
                const string registrationPrefixWithIos =
                    registrationPrefix + "case\"ios\":";
                var replacement =
                    "(globalThis." + AppJsPatchMarker + "=1,true))switch(appMode){case\"android\":case\"tizen\":case\"webos\":case\"vegaos\":case\"xbox\":case\"uwp\":case\"ios\":";

                // Emby 4.10 builds exist both with and without an existing iOS
                // case directly after webOS. Consume it when present so the
                // transformed switch contains every platform exactly once.
                var registrationAnchor = patched.Contains(registrationPrefixWithIos, StringComparison.Ordinal)
                    ? registrationPrefixWithIos
                    : registrationPrefix;
                if (!ReplaceRequired(
                        ref patched,
                        registrationAnchor,
                        replacement,
                        "app.js YouTube player registration"))
                {
                    return source;
                }
            }

            // The cache token must keep the plugin path ending in ".js": app.js routes
            // "./..." plugin paths through getDynamicImport (which unwraps the module's
            // ES default export into the player constructor) only when
            // url.startsWith("./") && url.endsWith(".js"). Any other shape falls into
            // pluginmanager.loadPluginFromUrl, which calls `new pluginFactory` on the raw
            // AMD exports object ({__esModule, default}) and kills Emby Web at boot with
            // "pluginFactory is not a constructor". If Emby ever changes that branch,
            // skip the token entirely rather than risk the blank-web failure again.
            if (!patched.Contains("url.startsWith(\"./\")&&url.endsWith(\".js\")", StringComparison.Ordinal))
            {
                YouTubeChannel.LogPublic("[YT] Dashboard app.js cache token skipped; plugin-path .js branch not found.");
                return patched;
            }

            var token = $"?{PluginCacheQueryPart}&ext=.js";

            if (!patched.Contains($"plugin_webview.js{token}", StringComparison.Ordinal))
            {
                patched = patched.Replace(
                    "\"./modules/youtubeplayer/plugin_webview.js\"",
                    $"\"./modules/youtubeplayer/plugin_webview.js{token}\"",
                    StringComparison.Ordinal);
            }

            if (!patched.Contains($"plugin.js{token}", StringComparison.Ordinal))
            {
                patched = patched.Replace(
                    "\"./modules/youtubeplayer/plugin.js\"",
                    $"\"./modules/youtubeplayer/plugin.js{token}\"",
                    StringComparison.Ordinal);
            }

            return patched;
        }

        private static string PatchIframePlayer(string source)
        {
            if (source.Contains(PatchMarker, StringComparison.Ordinal))
                return source;

            var patched = source;
            if (!InjectPlayerPatchHelpers(ref patched, "iframe"))
            {
                return source;
            }

            const string currentStart = "var tag,firstScriptTag,params=new URLSearchParams(options.url.split(\"?\")[1]);";
            if (patched.Contains(currentStart, StringComparison.Ordinal))
            {
                if (!ReplaceRequired(ref patched,
                        currentStart,
                        "var tag,firstScriptTag,params=new URLSearchParams(options.url.split(\"?\")[1]),startSeconds=ytPluginStartSeconds20260606(params);",
                        "iframe 4.10 parse start"))
                {
                    return source;
                }
            }
            else
            {
                if (!ReplaceRequired(ref patched,
                        "var params,tag,firstScriptTag;",
                        "var params,startSeconds,tag,firstScriptTag;",
                        "iframe legacy start var"))
                {
                    return source;
                }

                if (!ReplaceRequired(ref patched,
                        "params=new URLSearchParams(options.url.split(\"?\")[1]),window.onYouTubeIframeAPIReady=function(){",
                        "params=new URLSearchParams(options.url.split(\"?\")[1]),startSeconds=ytPluginStartSeconds20260606(params),window.onYouTubeIframeAPIReady=function(){",
                        "iframe legacy parse start"))
                {
                    return source;
                }
            }

            const string currentReady = "}else event.target.playVideo()},onStateChange:function(event){";
            if (patched.Contains(currentReady, StringComparison.Ordinal))
            {
                if (!ReplaceRequired(ref patched,
                        currentReady,
                        "}else{ytPluginDiag20260606(\"if-ready\");ytPluginStartCaptionGuard20260822(instance,event.target);startSeconds>0&&event.target.seekTo(startSeconds,!0);event.target.playVideo()}},onStateChange:function(event){",
                        "iframe 4.10 seek on ready"))
                {
                    return source;
                }
            }
            else if (!ReplaceRequired(ref patched,
                         "):event.target.playVideo()},onStateChange:function(event){",
                         "):(ytPluginDiag20260606(\"if-ready\"),ytPluginStartCaptionGuard20260822(instance,event.target),startSeconds>0&&event.target.seekTo(startSeconds,!0),event.target.playVideo())},onStateChange:function(event){",
                         "iframe legacy seek on ready"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "},onStateChange:function(event){",
                    "},onApiChange:function(event){ytPluginForceCaptionsOff20260822(event.target)},onStateChange:function(event){ytPluginForceCaptionsOff20260822(event.target);",
                    "iframe permanent caption-off events"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "playerVars:Object.assign({},playerVars)}",
                    "playerVars:ytPluginPlayerVars20260822(playerVars,startSeconds)}",
                    "iframe playerVars start/inline/jsapi/controls"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "YoutubePlayer.prototype.setSubtitleStreamIndex=function(index){},",
                    "YoutubePlayer.prototype.setSubtitleStreamIndex=function(index){ytPluginStartCaptionGuard20260822(this,this.currentYoutubePlayer);return Promise.resolve()},",
                    "iframe permanent caption-off selection"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "function onEndedInternal(instance,triggerStopped){",
                    "function onEndedInternal(instance,triggerStopped){ytPluginStopCaptionGuard20260822(instance);",
                    "iframe caption guard cleanup on end"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "function stopInternal(instance,destroyPlayer,triggerStopped){",
                    "function stopInternal(instance,destroyPlayer,triggerStopped){ytPluginStopCaptionGuard20260822(instance);",
                    "iframe caption guard cleanup on stop"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "YoutubePlayer.prototype.destroy=function(){var dlg=this.videoDialog;",
                    "YoutubePlayer.prototype.destroy=function(){ytPluginStopCaptionGuard20260822(this);var dlg=this.videoDialog;",
                    "iframe caption guard cleanup on destroy"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "onError:function(event){console.log(\"youtubeplayer, received error code during playback : \"+event.data);",
                    "onError:function(event){ytPluginStopCaptionGuard20260822(instance);console.log(\"youtubeplayer, received error code during playback : \"+event.data);",
                    "iframe caption guard cleanup on error"))
            {
                return source;
            }

            // Wrap the YT.Player options with host overrides (youtube-nocookie
            // on Android/Samsung). The opening Object.assign and its closing
            // paren must both land or neither, or the parentheses unbalance — so
            // this is applied as an atomic pair, and as OPTIONAL: a missed anchor
            // (e.g. a future Emby minifier change to this niche tablet feature)
            // keeps the rest of the iframe patch (seek/diag/canPlayItem) instead
            // of discarding the whole thing via `return source`.
            ApplyIframeHostOptionsWrap(ref patched);

            if (!ReplaceRequired(ref patched,
                "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\"]",
                "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\",\"Seek\",\"SeekRelative\"]",
                "iframe seek support"))
            {
                return source;
            }

            ReplaceOptional(ref patched,
                "if(event.data===YT.PlayerState.PLAYING){var rejectFn=reject;",
                "if(event.data===YT.PlayerState.PLAYING){ytPluginDiag20260606(\"if-playing\");var rejectFn=reject;");

            // Diagnostic beacons at the iframe player's own console.log points.
            ReplaceOptional(ref patched,
                "console.log(\"youtube playing: \"+options.url)",
                "(ytPluginDiag20260606(\"if-play\"),console.log(\"youtube playing: \"+options.url))");
            ReplaceOptional(ref patched,
                "console.log(\"youtubeplayer, received error code during playback : \"+event.data)",
                "(ytPluginDiag20260606(\"if-err-\"+event.data),console.log(\"youtubeplayer, received error code during playback : \"+event.data))");

            PatchLocalPlayerFlag(ref patched, "iframe");
            if (!PatchCanPlayItem(ref patched, "iframe"))
                return source;

            return patched;
        }

        private static string PatchWebViewPlayer(string source)
        {
            if (source.Contains(PatchMarker, StringComparison.Ordinal))
                return source;

            var patched = source;
            if (!InjectPlayerPatchHelpers(ref patched, "webview"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "iframeBaseUrl=\"https://mediabrowser.github.io\",iframeUrl=iframeBaseUrl+\"/youtube-embed\"",
                    "iframeBaseUrl=window.location.origin,iframeUrl=\"modules/youtubeplayer/youtube-embed.html\"",
                    "webview local player"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "if(event.origin===iframeBaseUrl){var data=event.data,",
                    "if(event.origin===iframeBaseUrl&&this.videoDialog&&this.videoDialog.querySelector(\"iframe\")&&event.source===this.videoDialog.querySelector(\"iframe\").contentWindow){var data=event.data,",
                    "webview message source validation"))
            {
                return source;
            }

            ReplaceOptional(ref patched,
                "switch(event){case\"youtubePlayerReady\":",
                "switch(event){case\"youtubeAutoplayBlocked\":ytPluginDiag20260606(\"wv-autoplay-blocked\");break;"
                    + "case\"youtubeAutoplayRetryMuted\":ytPluginDiag20260606(\"wv-autoplay-retry-muted\");break;"
                    + "case\"youtubeAutoplayRetryFailed\":ytPluginDiag20260606(\"wv-autoplay-retry-failed\");break;"
                    + "case\"youtubeAutoplayRecoveredMuted\":ytPluginDiag20260606(\"wv-autoplay-recovered-muted\");break;"
                    + "case\"youtubePlayerReady\":");

            if (!ReplaceRequired(ref patched,
                    "instance.playerData=null,triggerStopped&&_events.default.trigger(instance,\"stopped\",[{}])}",
                    "instance.playerData=null,instance.destroy(),triggerStopped&&_events.default.trigger(instance,\"stopped\",[{}])}",
                    "webview deterministic teardown on end"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "function stopInternal(instance,destroyPlayer,triggerStopped){var _instance$videoDialog3=null==(_instance$videoDialog3=instance.videoDialog)?void 0:_instance$videoDialog3.querySelector(\"iframe\");_instance$videoDialog3&&(sendMessage(_instance$videoDialog3,\"stopVideo\"),onEndedInternal(instance,triggerStopped)),destroyPlayer&&instance.destroy()}",
                    "function stopInternal(instance,destroyPlayer,triggerStopped){var wasActive=!!(instance.videoDialog||instance.playerData),_instance$videoDialog3=null==(_instance$videoDialog3=instance.videoDialog)?void 0:_instance$videoDialog3.querySelector(\"iframe\");if(!wasActive)return;try{_instance$videoDialog3&&sendMessage(_instance$videoDialog3,\"stopVideo\")}catch(e){}onEndedInternal(instance,triggerStopped)}",
                    "webview deterministic teardown on stop"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\"]",
                    "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\",\"Seek\",\"SeekRelative\"]",
                    "webview seek support"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "YoutubePlayer.prototype.setSubtitleStreamIndex=function(index){},",
                    "YoutubePlayer.prototype.setSubtitleStreamIndex=function(index){var iframe=this.videoDialog&&this.videoDialog.querySelector(\"iframe\");iframe&&sendMessage(iframe,\"forceCaptionsOff\",[]);return Promise.resolve()},",
                    "webview permanent caption-off selection"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "var _instance$videoDialog=null==(_instance$videoDialog=this.videoDialog)?void 0:_instance$videoDialog.querySelector(\"iframe\");_instance$videoDialog&&sendMessage(_instance$videoDialog,\"playVideo\");break;",
                    "var startSeconds=null==lastPlayerData?void 0:lastPlayerData.startTime,_instance$videoDialog=null==(_instance$videoDialog=this.videoDialog)?void 0:_instance$videoDialog.querySelector(\"iframe\");ytPluginDiag20260606(\"wv-ready\"),_instance$videoDialog&&(startSeconds>0&&sendMessage(_instance$videoDialog,\"seekTo\",[startSeconds,!0]),sendMessage(_instance$videoDialog,\"playVideo\"));break;",
                    "webview seek on ready"))
            {
                return source;
            }

            const string currentPlay = "var instance=this;return new Promise";
            if (patched.Contains(currentPlay, StringComparison.Ordinal))
            {
                if (!ReplaceRequired(ref patched,
                        currentPlay,
                        "var instance=this,params;return new Promise",
                        "webview 4.10 params var"))
                {
                    return source;
                }

                if (!ReplaceRequired(ref patched,
                        "else instance.playerData={resolve:resolve,reject:reject,signal:signal},function(instance,options){",
                        "else params=new URLSearchParams(options.url.split(\"?\")[1]),instance.playerData={resolve:resolve,reject:reject,signal:signal,startTime:ytPluginStartSeconds20260606(params)},function(instance,options,params){",
                        "webview 4.10 parse start"))
                {
                    return source;
                }

                if (!ReplaceRequired(ref patched,
                        "new URLSearchParams(options.url.split(\"?\")[1]).get(\"v\")",
                        "params.get(\"v\")",
                        "webview 4.10 video id"))
                {
                    return source;
                }
            }
            else
            {
                if (!ReplaceRequired(ref patched,
                        "YoutubePlayer.prototype.play=function(options,signal){var instance;return",
                        "YoutubePlayer.prototype.play=function(options,signal){var instance,params;return",
                        "webview legacy params var"))
                {
                    return source;
                }

                if (!ReplaceRequired(ref patched,
                        "signal.aborted?reject(getSignalRejectReason(signal)):(instance.playerData={resolve:resolve,reject:reject,signal:signal},function(instance,options){var dlg=document.querySelector(\".youtubePlayerContainer\"),instance=(dlg||((dlg=document.createElement(\"div\")).classList.add(\"youtubePlayerContainer\"),document.body.insertBefore(dlg,document.body.firstChild),instance.videoDialog=dlg),window.removeEventListener(\"message\",instance.boundOnWindowMessage),window.addEventListener(\"message\",instance.boundOnWindowMessage),new URLSearchParams(options.url.split(\"?\")[1]).get(\"v\"));",
                        "signal.aborted?reject(getSignalRejectReason(signal)):(params=new URLSearchParams(options.url.split(\"?\")[1]),instance.playerData={resolve:resolve,reject:reject,signal:signal,startTime:ytPluginStartSeconds20260606(params)},function(instance,options,params){var dlg=document.querySelector(\".youtubePlayerContainer\"),instance=(dlg||((dlg=document.createElement(\"div\")).classList.add(\"youtubePlayerContainer\"),document.body.insertBefore(dlg,document.body.firstChild),instance.videoDialog=dlg),window.removeEventListener(\"message\",instance.boundOnWindowMessage),window.addEventListener(\"message\",instance.boundOnWindowMessage),params.get(\"v\"));",
                        "webview legacy parse start"))
                {
                    return source;
                }
            }

            if (!ReplaceRequired(ref patched,
                    "}(instance,options),options.fullscreen&&",
                    "}(instance,options,params),options.fullscreen&&",
                    "webview pass params"))
            {
                return source;
            }

            // Best-effort safety net: the local compatibility embed ignores a
            // URL start param, so the only lever is postMessage("seekTo"). The
            // on-ready seek can be dropped while the embed is still cueing, so
            // re-assert it exactly once when playback first reaches PLAYING.
            // Non-required: never regress the working seek if Emby changes this line.
            const string playingMarker = "(lastPlayerData.state=youtubeData)===YT.PlayerState.PLAYING){";
            if (patched.Contains(playingMarker, StringComparison.Ordinal))
            {
                patched = patched.Replace(
                    playingMarker,
                    playingMarker
                        + "ytPluginDiag20260606(\"wv-playing\");"
                        + "var _ytSeekIf=this.videoDialog&&this.videoDialog.querySelector(\"iframe\");"
                        + "lastPlayerData.startTime>0&&!lastPlayerData.ytStartSeeked&&_ytSeekIf&&"
                        + "(lastPlayerData.ytStartSeeked=!0,sendMessage(_ytSeekIf,\"seekTo\",[lastPlayerData.startTime,!0]));",
                    StringComparison.Ordinal);
            }

            ReplaceOptional(ref patched,
                "src=\"'+(iframeUrl+\"?videoId=\"+instance)+'\"",
                "src=\"'+(iframeUrl+\"?videoId=\"+instance+\"&playsinline=1&enablejsapi=1\")+'\"");

            // Diagnostic beacons at the webview player's own console.log points.
            ReplaceOptional(ref patched,
                "console.log(\"youtube playing: \"+options.url)",
                "(ytPluginDiag20260606(\"wv-play\"),console.log(\"youtube playing: \"+options.url))");
            ReplaceOptional(ref patched,
                "console.log(\"youtubeData: \"+youtubeData)",
                "(ytPluginDiag20260606(\"wv-state-\"+youtubeData),console.log(\"youtubeData: \"+youtubeData))");
            ReplaceOptional(ref patched,
                "console.log(\"youtubeplayer, received error code during playback : \"+youtubeData)",
                "(ytPluginDiag20260606(\"wv-err-\"+youtubeData),console.log(\"youtubeplayer, received error code during playback : \"+youtubeData))");

            PatchLocalPlayerFlag(ref patched, "webview");
            if (!PatchCanPlayItem(ref patched, "webview"))
                return source;

            return patched;
        }

        private static bool InjectPlayerPatchHelpers(ref string source, string moduleName)
        {
            if (source.Contains(PatchMarker, StringComparison.Ordinal))
                return true;

            const string cssLoader = "require([\"css!modules/youtubeplayer/style.css\"]);";
            if (source.Contains(cssLoader, StringComparison.Ordinal))
            {
                source = source.Replace(
                    cssLoader,
                    cssLoader + PlayerPatchHelpers,
                    StringComparison.Ordinal);
                return true;
            }

            const string legacyAbortHelper =
                "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}";
            if (source.Contains(legacyAbortHelper, StringComparison.Ordinal))
            {
                source = source.Replace(
                    legacyAbortHelper,
                    legacyAbortHelper + PlayerPatchHelpers,
                    StringComparison.Ordinal);
                return true;
            }

            YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player patch pattern missing: {moduleName} helper injection.");
            return false;
        }

        private static void PatchLocalPlayerFlag(ref string source, string moduleName)
        {
            if (source.Contains("this.isLocalPlayer", StringComparison.Ordinal))
                return;

            var patched = false;
            patched |= ReplaceOptional(ref source,
                "this.name=\"Youtube Player\",this.type=\"mediaplayer\",this.id=\"youtubeplayer\",this.priority=1",
                "this.name=\"Youtube Player\",this.type=\"mediaplayer\",this.id=\"youtubeplayer\",this.isLocalPlayer=!0,this.priority=1");
            patched |= ReplaceOptional(ref source,
                "this.name=\"YouTube Player\",this.type=\"mediaplayer\",this.id=\"youtubeplayer\",this.priority=1",
                "this.name=\"YouTube Player\",this.type=\"mediaplayer\",this.id=\"youtubeplayer\",this.isLocalPlayer=!0,this.priority=1");
            patched |= ReplaceOptional(ref source,
                "this.name=\"Youtube Player\";this.type=\"mediaplayer\";this.id=\"youtubeplayer\";this.priority=1",
                "this.name=\"Youtube Player\";this.type=\"mediaplayer\";this.id=\"youtubeplayer\";this.isLocalPlayer=!0;this.priority=1");
            patched |= ReplaceOptional(ref source,
                "this.name=\"YouTube Player\";this.type=\"mediaplayer\";this.id=\"youtubeplayer\";this.priority=1",
                "this.name=\"YouTube Player\";this.type=\"mediaplayer\";this.id=\"youtubeplayer\";this.isLocalPlayer=!0;this.priority=1");

            if (!patched && _optionalPatchLogsRemaining > 0)
            {
                _optionalPatchLogsRemaining--;
                YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player optional isLocalPlayer patch not applied for {moduleName}; pattern not found.");
            }
        }

        private static bool PatchCanPlayItem(ref string source, string moduleName)
        {
            if (source.Contains("return ytPluginCanPlayItem20260606(item)", StringComparison.Ordinal))
                return true;

            var patched = false;
            patched |= ReplaceOptional(ref source,
                "canPlayItem:function(item){return!1}",
                "canPlayItem:function(item){return ytPluginCanPlayItem20260606(item)}");
            patched |= ReplaceOptional(ref source,
                "canPlayItem:function(){return!1}",
                "canPlayItem:function(item){return ytPluginCanPlayItem20260606(item)}");
            patched |= ReplaceOptional(ref source,
                "canPlayItem(item){return!1}",
                "canPlayItem(item){return ytPluginCanPlayItem20260606(item)}");
            patched |= ReplaceOptional(ref source,
                "canPlayItem(){return!1}",
                "canPlayItem(item){return ytPluginCanPlayItem20260606(item)}");
            patched |= ReplaceOptional(ref source,
                "YoutubePlayer.prototype.canPlayItem=function(item){return!1}",
                "YoutubePlayer.prototype.canPlayItem=function(item){return ytPluginCanPlayItem20260606(item)}");
            patched |= ReplaceOptional(ref source,
                "YoutubePlayer.prototype.canPlayItem=function(){return!1}",
                "YoutubePlayer.prototype.canPlayItem=function(item){return ytPluginCanPlayItem20260606(item)}");

            if (!patched && _optionalPatchLogsRemaining > 0)
            {
                _optionalPatchLogsRemaining--;
                YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player optional canPlayItem patch not applied for {moduleName}; pattern not found.");
            }

            return patched;
        }

        private static void ApplyIframeHostOptionsWrap(ref string source)
        {
            const string startOld = "new YT.Player(\"player\",{height:";
            const string startNew = "new YT.Player(\"player\",Object.assign({},ytPluginHostOptions20260617(),{height:";
            const string legacyEndOld = "},playerVars:ytPluginPlayerVars20260822(playerVars,startSeconds)}),(resizeListener=";
            const string legacyEndNew = "},playerVars:ytPluginPlayerVars20260822(playerVars,startSeconds)})),(resizeListener=";
            const string currentEndOld = "},playerVars:ytPluginPlayerVars20260822(playerVars,startSeconds)}),instance.resizeListener);";
            const string currentEndNew = "},playerVars:ytPluginPlayerVars20260822(playerVars,startSeconds)})),instance.resizeListener);";

            // Both edits balance each other (opening wrap + its closing paren),
            // so commit only when BOTH anchors are present. A partial apply can
            // never unbalance the parentheses, and a full miss leaves the prior
            // iframe patches untouched.
            if (!source.Contains(startOld, StringComparison.Ordinal))
            {
                if (_optionalPatchLogsRemaining > 0)
                {
                    _optionalPatchLogsRemaining--;
                    YouTubeChannel.LogPublic("[YT] Dashboard iframe host-options wrap skipped; anchors not found (rest of iframe patch kept).");
                }
                return;
            }

            var endOld = source.Contains(currentEndOld, StringComparison.Ordinal)
                ? currentEndOld
                : source.Contains(legacyEndOld, StringComparison.Ordinal)
                    ? legacyEndOld
                    : null;
            var endNew = string.Equals(endOld, currentEndOld, StringComparison.Ordinal)
                ? currentEndNew
                : legacyEndNew;
            if (endOld == null)
            {
                if (_optionalPatchLogsRemaining > 0)
                {
                    _optionalPatchLogsRemaining--;
                    YouTubeChannel.LogPublic("[YT] Dashboard iframe host-options wrap skipped; closing anchor not found (rest of iframe patch kept).");
                }
                return;
            }

            source = source
                .Replace(startOld, startNew, StringComparison.Ordinal)
                .Replace(endOld, endNew, StringComparison.Ordinal);
        }

        private static bool ReplaceOptional(ref string source, string oldValue, string newValue)
        {
            if (!source.Contains(oldValue, StringComparison.Ordinal))
                return false;

            source = source.Replace(oldValue, newValue, StringComparison.Ordinal);
            return true;
        }

        private static bool ReplaceRequired(ref string source, string oldValue, string newValue, string label)
        {
            if (!source.Contains(oldValue, StringComparison.Ordinal))
            {
                YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player patch pattern missing: {label}.");
                return false;
            }

            source = source.Replace(oldValue, newValue, StringComparison.Ordinal);
            return true;
        }

        private static bool IsTargetResource(string? resourceName)
        {
            // Intercept only the YouTube player modules plus app.js' small
            // player-registration block. index.html and apploader.js stay under
            // Emby's original generation path.
            var normalized = NormalizeResourceName(resourceName);
            return normalized.EndsWith("app.js", StringComparison.OrdinalIgnoreCase)
                   || normalized.EndsWith("modules/youtubeplayer/plugin.js", StringComparison.OrdinalIgnoreCase)
                   || normalized.EndsWith("modules/youtubeplayer/plugin_webview.js", StringComparison.OrdinalIgnoreCase)
                   || IsEmbedResource(normalized);
        }

        private static bool IsEmbedResource(string? resourceName) =>
            NormalizeResourceName(resourceName)
                .EndsWith("modules/youtubeplayer/youtube-embed.html", StringComparison.OrdinalIgnoreCase);

        private static string NormalizeResourceName(string? resourceName)
        {
            var normalized = (resourceName ?? string.Empty)
                .Replace('\\', '/')
                .TrimStart('/')
                .Replace("../", string.Empty, StringComparison.Ordinal);

            var queryStart = normalized.IndexOfAny(new[] { '?', '#' });
            return queryStart >= 0 ? normalized.Substring(0, queryStart) : normalized;
        }

        private static string GetContentType(string? resourceName)
        {
            var normalized = NormalizeResourceName(resourceName);
            return normalized.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                ? "text/html; charset=UTF-8"
                : "application/x-javascript";
        }

        // Full four-part version plus a dashboard patch revision: hotfix deploys can keep
        // the same assembly version, but browsers must still refetch patched modules.
        private static string PluginVersion =>
            typeof(DashboardYouTubePlayerInterceptor).Assembly.GetName().Version?.ToString() ?? "0.0.0.0";

        private const string DashboardPatchRevision = "20260831-emby410-player-v12";

        private static string PluginCacheQueryPart =>
            $"ytplugin={PluginVersion}&ytpatch={DashboardPatchRevision}";

        private const string PlayerPatchHelpers =
            "function ytPluginPatch20260606(){return 1}"
            + "function ytPluginStartSeconds20260606(params){var raw=params.get(\"start\")||params.get(\"t\");if(!raw)return 0;if(/^\\d+$/.test(raw))return parseInt(raw,10);var match=/^(?:(\\d+)h)?(?:(\\d+)m)?(?:(\\d+)s?)?$/.exec(raw);return match?3600*parseInt(match[1]||\"0\",10)+60*parseInt(match[2]||\"0\",10)+parseInt(match[3]||\"0\",10):0}"
            + "function ytPluginCanPlayItem20260606(item){try{var yt=/^(https?:\\/\\/)?([^\\/]+\\.)?(youtube\\.com|youtu\\.be)\\//i,sources=item&&(item.MediaSources||item.mediaSources)||[];for(var i=0;i<sources.length;i++){var source=sources[i]||{},path=source.Path||source.path||source.DirectStreamUrl||source.directStreamUrl;if(path&&yt.test(path))return!0}var path=item&&(item.Path||item.path);return!!(path&&yt.test(path))}catch(e){return!1}}"
            + "function ytPluginPlayerVars20260822(playerVars,startSeconds){var result=Object.assign({},playerVars,{enablejsapi:1,playsinline:1,controls:1,disablekb:0,cc_load_policy:0,origin:window.location.origin,widget_referrer:window.location.href});delete result.cc_lang_pref;startSeconds>0&&(result.start=startSeconds);return result}"
            + "function ytPluginForceCaptionsOff20260822(player){if(!player||player.ytPluginCaptionForceBusy20260822)return!1;var applied=!1;player.ytPluginCaptionForceBusy20260822=!0;try{try{typeof player.setOption===\"function\"&&(player.setOption(\"captions\",\"track\",{}),applied=!0)}catch(e){}try{typeof player.unloadModule===\"function\"&&(player.unloadModule(\"captions\"),applied=!0)}catch(e){}}finally{player.ytPluginCaptionForceBusy20260822=!1}return applied}"
            + "function ytPluginStopCaptionGuard20260822(instance){try{if(!instance)return;var timers=instance.ytCaptionOffTimers||[];for(var i=0;i<timers.length;i++)clearTimeout(timers[i]);instance.ytCaptionOffTimers=[];instance.ytCaptionOffInterval&&(clearInterval(instance.ytCaptionOffInterval),instance.ytCaptionOffInterval=0);instance.ytCaptionOffPlayer=null}catch(e){}}"
            + "function ytPluginStartCaptionGuard20260822(instance,player){try{if(!instance||!instance.videoDialog||!player)return;ytPluginStopCaptionGuard20260822(instance);instance.ytCaptionOffPlayer=player;var keep=function(){instance.videoDialog&&instance.ytCaptionOffPlayer===player&&instance.currentYoutubePlayer===player&&ytPluginForceCaptionsOff20260822(player)};keep();instance.ytCaptionOffTimers=[100,500,1500,4000].map(function(delay){return setTimeout(keep,delay)});instance.ytCaptionOffInterval=setInterval(keep,2000)}catch(e){}}"
            // Diagnostic beacon: pings the server so the in-webview playback flow is
            // visible in the plugin log as [YT][DIAG] lines (client console is unreachable).
            + "function ytPluginDiag20260606(m){try{new Image().src=\"modules/youtubeplayer/ytdiag.js?m=\"+encodeURIComponent(m)+\"&t=\"+Date.now()}catch(e){}}"
            + "function ytPluginHostOptions20260617(){try{var ua=(globalThis.navigator&&navigator.userAgent)||\"\";if(!/AFT|Fire[ \\/-]?TV|Android[ \\/-]?TV/i.test(ua)&&/Android|SamsungBrowser/i.test(ua)){ytPluginDiag20260606(\"if-host-nocookie\");return{host:\"https://www.youtube-nocookie.com\"}}}catch(e){}return{}}";

        private static object? GetProperty(object source, string name)
        {
            return source.GetType()
                .GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.GetValue(source);
        }

        private static string? GetStringProperty(object source, string name)
        {
            return GetProperty(source, name) as string;
        }

        private static object? GetField(object source, string name)
        {
            return source.GetType()
                .GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.GetValue(source);
        }

        private static Type? FindLoadedType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType(fullName, throwOnError: false);
                    if (type != null)
                        return type;
                }
                catch
                {
                    // Ignore assemblies that cannot be inspected.
                }
            }

            return Type.GetType(fullName, throwOnError: false);
        }

        private static void InvokeHarmonyPatch(object harmony, MethodInfo original, object prefix)
        {
            var patchMethod = harmony.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m =>
                {
                    if (m.Name != "Patch")
                        return false;

                    var parameters = m.GetParameters();
                    return parameters.Length >= 2
                           && typeof(MethodBase).IsAssignableFrom(parameters[0].ParameterType)
                           && parameters.Any(p => string.Equals(p.Name, "prefix", StringComparison.OrdinalIgnoreCase));
                });

            if (patchMethod == null)
                throw new MissingMethodException(harmony.GetType().FullName, "Patch");

            var patchParameters = patchMethod.GetParameters();
            var args = new object?[patchParameters.Length];
            args[0] = original;
            for (var i = 1; i < patchParameters.Length; i++)
            {
                if (string.Equals(patchParameters[i].Name, "prefix", StringComparison.OrdinalIgnoreCase))
                    args[i] = prefix;
                else
                    args[i] = null;
            }

            patchMethod.Invoke(harmony, args);
        }
    }
}
