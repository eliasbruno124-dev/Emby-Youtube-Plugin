using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

                if (!TryReadAndPatchResource(__instance, resourceName!, out var patchedBytes))
                    return true;

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

        private static string PatchIndexHtml(string source)
        {
            var token = DashboardCacheToken;
            if (source.Contains("ytplugin=", StringComparison.Ordinal)
                || source.Contains($"data-appversion=\"{token}\"", StringComparison.Ordinal))
            {
                return source;
            }

            var culture = GetSystemConfigValue("UICulture", CultureInfo.CurrentUICulture.Name);
            var lang = culture.Split(new[] { '-' }, 2)[0];
            var serverName = GetSystemConfigValue("ServerName", "Emby Server");
            var encodedToken = WebUtility.HtmlEncode(token);
            var encodedCulture = WebUtility.HtmlEncode(culture);
            var encodedLang = WebUtility.HtmlEncode(lang);
            var encodedServerName = WebUtility.HtmlEncode(serverName);
            var queryToken = Uri.EscapeDataString(token);

            var patched = source;
            if (patched.Contains("<html>", StringComparison.Ordinal))
            {
                patched = patched.Replace(
                    "<html>",
                    $"<html data-appversion=\"{encodedToken}\" data-culture=\"{encodedCulture}\" lang=\"{encodedLang}\">",
                    StringComparison.Ordinal);
            }

            patched = patched.Replace(
                "<meta name=\"description\" content=\"Emby Server\">",
                $"<meta name=\"description\" content=\"{encodedServerName}\">",
                StringComparison.Ordinal);

            patched = patched.Replace(
                "<script src=\"apploader.js\" defer></script>",
                $"<script src=\"apploader.js?v={queryToken}\" defer></script>",
                StringComparison.Ordinal);

            return patched;
        }

        private static string PatchAppLoader(string source)
        {
            var token = PluginCacheQueryPart;
            if (source.Contains(token, StringComparison.Ordinal))
                return source;

            var patched = source;
            return ReplaceRequired(ref patched,
                "docElem?globalThis.urlCacheParam=\"v=\"+docElem:appMode||(globalThis.urlCacheParam=\"v=\"+Date.now()),",
                $"docElem?globalThis.urlCacheParam=\"v=\"+docElem+\"&{token}\":appMode||(globalThis.urlCacheParam=\"v=\"+Date.now()+\"&{token}\"),",
                "apploader cache token")
                ? patched
                : source;
        }

        private static string PatchAppJs(string source)
        {
            var token = $"?{PluginCacheQueryPart}";
            var patched = source;

            if (!patched.Contains(AppJsPatchMarker, StringComparison.Ordinal)
                && !ReplaceRequired(ref patched,
                    "appHost.supports(\"youtube\"))switch(appMode){case\"android\":case\"tizen\":case\"webos\":",
                    "(globalThis." + AppJsPatchMarker + "=1,true))switch(appMode){case\"android\":case\"tizen\":case\"webos\":case\"vegaos\":case\"xbox\":case\"uwp\":",
                    "app.js YouTube player registration"))
            {
                return source;
            }

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
            if (!ReplaceRequired(ref patched,
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}",
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}" + PlayerPatchHelpers,
                    "iframe helper"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched, "var params,tag,firstScriptTag;", "var params,startSeconds,tag,firstScriptTag;", "iframe start var"))
                return source;

            if (!ReplaceRequired(ref patched,
                    "params=new URLSearchParams(options.url.split(\"?\")[1]),window.onYouTubeIframeAPIReady=function(){",
                    "params=new URLSearchParams(options.url.split(\"?\")[1]),startSeconds=ytPluginStartSeconds20260606(params),window.onYouTubeIframeAPIReady=function(){",
                    "iframe parse start"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "):event.target.playVideo()},onStateChange:function(event){",
                    "):(ytPluginDiag20260606(\"if-ready\"),startSeconds>0&&event.target.seekTo(startSeconds,!0),event.target.playVideo())},onStateChange:function(event){",
                    "iframe seek on ready"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "playerVars:Object.assign({},playerVars)}",
                    "playerVars:Object.assign({},playerVars,{enablejsapi:1,playsinline:1,mute:1,origin:window.location.origin},startSeconds>0?{start:startSeconds}:null)}",
                    "iframe playerVars start/inline/jsapi"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\"]",
                "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\",\"Seek\",\"SeekRelative\"]",
                "iframe seek support"))
            {
                return source;
            }

            // The iframe player now starts muted (mute:1 above) so autoplay is never blocked by
            // a webview/browser without media-engagement history; restore sound once it actually
            // reaches PLAYING. Best-effort: a pattern miss must not discard the whole iframe patch.
            ReplaceOptional(ref patched,
                "if(event.data===YT.PlayerState.PLAYING){var rejectFn=reject;",
                "if(event.data===YT.PlayerState.PLAYING){ytPluginDiag20260606(\"if-playing\"),event.target&&!instance.ytUnmuted&&(instance.ytUnmuted=!0,setTimeout(function(){event.target.unMute&&event.target.unMute()},600));var rejectFn=reject;");

            // Diagnostic beacons at the iframe player's own console.log points.
            ReplaceOptional(ref patched,
                "console.log(\"youtube playing: \"+options.url)",
                "(ytPluginDiag20260606(\"if-play\"),console.log(\"youtube playing: \"+options.url))");
            ReplaceOptional(ref patched,
                "console.log(\"youtubeplayer, received error code during playback : \"+event.data)",
                "(ytPluginDiag20260606(\"if-err-\"+event.data),console.log(\"youtubeplayer, received error code during playback : \"+event.data))");

            PatchLocalPlayerFlag(ref patched, "iframe");
            PatchCanPlayItem(ref patched, "iframe");
            return patched;
        }

        private static string PatchWebViewPlayer(string source)
        {
            if (source.Contains(PatchMarker, StringComparison.Ordinal))
                return source;

            var patched = source;
            if (!ReplaceRequired(ref patched,
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}",
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}" + PlayerPatchHelpers,
                    "webview helper"))
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
                    "case\"youtubePlayerReady\":if(signal.aborted)return stopInternal(this,!0,!1),void reject(getSignalRejectReason(signal));var _instance$videoDialog=null==(_instance$videoDialog=this.videoDialog)?void 0:_instance$videoDialog.querySelector(\"iframe\");_instance$videoDialog&&sendMessage(_instance$videoDialog,\"playVideo\");break;",
                    "case\"youtubePlayerReady\":if(signal.aborted)return stopInternal(this,!0,!1),void reject(getSignalRejectReason(signal));var startSeconds=null==lastPlayerData?void 0:lastPlayerData.startTime,_instance$videoDialog=null==(_instance$videoDialog=this.videoDialog)?void 0:_instance$videoDialog.querySelector(\"iframe\");ytPluginDiag20260606(\"wv-ready\"),_instance$videoDialog&&(ytPluginDiag20260606(\"wv-muteplay\"),sendMessage(_instance$videoDialog,\"mute\"),startSeconds>0&&sendMessage(_instance$videoDialog,\"seekTo\",[startSeconds,!0]),sendMessage(_instance$videoDialog,\"playVideo\"));break;",
                    "webview seek on ready"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "YoutubePlayer.prototype.play=function(options,signal){var instance;return",
                    "YoutubePlayer.prototype.play=function(options,signal){var instance,params;return",
                    "webview params var"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "signal.aborted?reject(getSignalRejectReason(signal)):(instance.playerData={resolve:resolve,reject:reject,signal:signal},function(instance,options){var dlg=document.querySelector(\".youtubePlayerContainer\"),instance=(dlg||((dlg=document.createElement(\"div\")).classList.add(\"youtubePlayerContainer\"),document.body.insertBefore(dlg,document.body.firstChild),instance.videoDialog=dlg),window.removeEventListener(\"message\",instance.boundOnWindowMessage),window.addEventListener(\"message\",instance.boundOnWindowMessage),new URLSearchParams(options.url.split(\"?\")[1]).get(\"v\"));",
                    "signal.aborted?reject(getSignalRejectReason(signal)):(params=new URLSearchParams(options.url.split(\"?\")[1]),instance.playerData={resolve:resolve,reject:reject,signal:signal,startTime:ytPluginStartSeconds20260606(params)},function(instance,options,params){var dlg=document.querySelector(\".youtubePlayerContainer\"),instance=(dlg||((dlg=document.createElement(\"div\")).classList.add(\"youtubePlayerContainer\"),document.body.insertBefore(dlg,document.body.firstChild),instance.videoDialog=dlg),window.removeEventListener(\"message\",instance.boundOnWindowMessage),window.addEventListener(\"message\",instance.boundOnWindowMessage),params.get(\"v\"));",
                    "webview parse start"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "}(instance,options),options.fullscreen&&",
                    "}(instance,options,params),options.fullscreen&&",
                    "webview pass params"))
            {
                return source;
            }

            // Best-effort safety net: the mediabrowser.github.io embed ignores a
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
                        + "(lastPlayerData.ytStartSeeked=!0,sendMessage(_ytSeekIf,\"seekTo\",[lastPlayerData.startTime,!0]));"
                        // Muted autoplay is the only autoplay a fresh webview (Xbox/UWP WebView2)
                        // allows, so playback is started muted; once it actually reaches PLAYING we
                        // restore sound. Delayed slightly so playback is settled before unmuting.
                        + "_ytSeekIf&&!lastPlayerData.ytUnmuted&&(lastPlayerData.ytUnmuted=!0,setTimeout(function(){sendMessage(_ytSeekIf,\"unMute\")},600));",
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
            PatchCanPlayItem(ref patched, "webview");
            return patched;
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

        private static void PatchCanPlayItem(ref string source, string moduleName)
        {
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
                   || normalized.EndsWith("modules/youtubeplayer/plugin_webview.js", StringComparison.OrdinalIgnoreCase);
        }

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

        private static string PluginVersion =>
            typeof(DashboardYouTubePlayerInterceptor).Assembly.GetName().Version?.ToString(3) ?? "0.0.0";

        private static string DashboardCacheToken =>
            $"{Plugin.AppHost?.ApplicationVersion?.ToString() ?? "emby"}-yt{PluginVersion}";

        private static string PluginCacheQueryPart =>
            $"ytplugin={PluginVersion}";

        private static string GetSystemConfigValue(string elementName, string fallback)
        {
            var path = Plugin.SystemConfigurationFilePath;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return fallback;

            try
            {
                return XDocument.Load(path).Root?.Element(elementName)?.Value ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }

        private const string PlayerPatchHelpers =
            "function ytPluginPatch20260606(){return 1}"
            + "function ytPluginStartSeconds20260606(params){var raw=params.get(\"start\")||params.get(\"t\");if(!raw)return 0;if(/^\\d+$/.test(raw))return parseInt(raw,10);var match=/^(?:(\\d+)h)?(?:(\\d+)m)?(?:(\\d+)s?)?$/.exec(raw);return match?3600*parseInt(match[1]||\"0\",10)+60*parseInt(match[2]||\"0\",10)+parseInt(match[3]||\"0\",10):0}"
            + "function ytPluginCanPlayItem20260606(item){try{var yt=/^(https?:\\/\\/)?([^\\/]+\\.)?(youtube\\.com|youtu\\.be)\\//i,sources=item&&(item.MediaSources||item.mediaSources)||[];for(var i=0;i<sources.length;i++){var source=sources[i]||{},path=source.Path||source.path||source.DirectStreamUrl||source.directStreamUrl;if(path&&yt.test(path))return!0}var path=item&&(item.Path||item.path);return!!(path&&yt.test(path))}catch(e){return!1}}"
            // Diagnostic beacon: pings the server so the in-webview playback flow is
            // visible in the plugin log as [YT][DIAG] lines (client console is unreachable).
            + "function ytPluginDiag20260606(m){try{new Image().src=\"modules/youtubeplayer/ytdiag.js?m=\"+encodeURIComponent(m)+\"&t=\"+Date.now()}catch(e){}}";

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
