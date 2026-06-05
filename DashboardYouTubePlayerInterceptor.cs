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
        private static readonly object Sync = new();
        private static object? _harmony;
        private static Type? _harmonyType;
        private static int _patchedResponseLogsRemaining = 24;

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
                if (!IsTargetResource(resourceName))
                    return true;

                if (!TryReadAndPatchResource(__instance, resourceName!, out var patchedBytes))
                    return true;

                var request = GetProperty(__instance, "Request") as IRequest;
                var resultFactory = GetProperty(__instance, "ResultFactory") as IHttpResultFactory
                                    ?? GetField(__instance, "_resultFactory") as IHttpResultFactory;
                if (request == null || resultFactory == null)
                    return true;

                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cache-Control"] = "no-cache, no-store, must-revalidate",
                    ["Pragma"] = "no-cache",
                    ["Expires"] = "0"
                };

                var result = resultFactory.GetResult(
                    request,
                    new ReadOnlyMemory<byte>(patchedBytes),
                    GetContentType(resourceName),
                    headers);

                __result = Task.FromResult(result);

                if (_patchedResponseLogsRemaining > 0)
                {
                    _patchedResponseLogsRemaining--;
                    YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player response patched for {NormalizeResourceName(resourceName)}.");
                }

                return false;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Dashboard YouTube player response patch failed: {ex.Message}");
                return true;
            }
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
            // The app shell (index.html / apploader.js / app.js) is intentionally
            // NOT rewritten anymore: bypassing Emby's own dashboard-resource path
            // for the boot files could break the web dashboard. We only touch the
            // YouTube player modules, which Emby serves as plain static files.
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
            if (source.Contains($"plugin_webview.js{token}", StringComparison.Ordinal)
                || source.Contains($"plugin.js{token}", StringComparison.Ordinal))
            {
                return source;
            }

            var patched = source;
            patched = patched.Replace(
                "\"./modules/youtubeplayer/plugin_webview.js\"",
                $"\"./modules/youtubeplayer/plugin_webview.js{token}\"",
                StringComparison.Ordinal);
            patched = patched.Replace(
                "\"./modules/youtubeplayer/plugin.js\"",
                $"\"./modules/youtubeplayer/plugin.js{token}\"",
                StringComparison.Ordinal);

            return patched;
        }

        private static string PatchIframePlayer(string source)
        {
            if (source.Contains("getYoutubeStartSeconds", StringComparison.Ordinal))
                return source;

            var patched = source;
            if (!ReplaceRequired(ref patched,
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}",
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}" + StartSecondsHelper,
                    "iframe helper"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched, "var params,tag,firstScriptTag;", "var params,startSeconds,tag,firstScriptTag;", "iframe start var"))
                return source;

            if (!ReplaceRequired(ref patched,
                    "params=new URLSearchParams(options.url.split(\"?\")[1]),window.onYouTubeIframeAPIReady=function(){",
                    "params=new URLSearchParams(options.url.split(\"?\")[1]),startSeconds=getYoutubeStartSeconds(params),window.onYouTubeIframeAPIReady=function(){",
                    "iframe parse start"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "):event.target.playVideo()},onStateChange:function(event){",
                    "):(startSeconds>0&&event.target.seekTo(startSeconds,!0),event.target.playVideo())},onStateChange:function(event){",
                    "iframe seek on ready"))
            {
                return source;
            }

            if (!ReplaceRequired(ref patched,
                    "playerVars:Object.assign({},playerVars)}",
                    "playerVars:Object.assign({},playerVars,{playsinline:1},startSeconds>0?{start:startSeconds}:null)}",
                    "iframe playerVars start/inline"))
            {
                return source;
            }

            return ReplaceRequired(ref patched,
                "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\"]",
                "[\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\",\"Seek\",\"SeekRelative\"]",
                "iframe seek support")
                ? patched
                : source;
        }

        private static string PatchWebViewPlayer(string source)
        {
            if (source.Contains("getYoutubeStartSeconds", StringComparison.Ordinal))
                return source;

            var patched = source;
            if (!ReplaceRequired(ref patched,
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}",
                    "function getSignalRejectReason(signal){signal=signal.reason;return signal||((signal=new Error(\"Aborted\")).name=\"AbortError\"),signal}" + StartSecondsHelper,
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
                    "case\"youtubePlayerReady\":if(signal.aborted)return stopInternal(this,!0,!1),void reject(getSignalRejectReason(signal));var startSeconds=null==lastPlayerData?void 0:lastPlayerData.startTime,_instance$videoDialog=null==(_instance$videoDialog=this.videoDialog)?void 0:_instance$videoDialog.querySelector(\"iframe\");_instance$videoDialog&&(startSeconds>0&&sendMessage(_instance$videoDialog,\"seekTo\",[startSeconds,!0]),sendMessage(_instance$videoDialog,\"playVideo\"));break;",
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
                    "signal.aborted?reject(getSignalRejectReason(signal)):(params=new URLSearchParams(options.url.split(\"?\")[1]),instance.playerData={resolve:resolve,reject:reject,signal:signal,startTime:getYoutubeStartSeconds(params)},function(instance,options,params){var dlg=document.querySelector(\".youtubePlayerContainer\"),instance=(dlg||((dlg=document.createElement(\"div\")).classList.add(\"youtubePlayerContainer\"),document.body.insertBefore(dlg,document.body.firstChild),instance.videoDialog=dlg),window.removeEventListener(\"message\",instance.boundOnWindowMessage),window.addEventListener(\"message\",instance.boundOnWindowMessage),params.get(\"v\"));",
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
                        + "var _ytSeekIf=this.videoDialog&&this.videoDialog.querySelector(\"iframe\");"
                        + "lastPlayerData.startTime>0&&!lastPlayerData.ytStartSeeked&&_ytSeekIf&&"
                        + "(lastPlayerData.ytStartSeeked=!0,sendMessage(_ytSeekIf,\"seekTo\",[lastPlayerData.startTime,!0]));",
                    StringComparison.Ordinal);
            }

            return patched;
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
            // Only the YouTube player modules are intercepted. Emby's app shell
            // (index.html / apploader.js / app.js) is left untouched so the web
            // dashboard boot path is never altered by the plugin.
            var normalized = NormalizeResourceName(resourceName);
            return normalized.EndsWith("modules/youtubeplayer/plugin.js", StringComparison.OrdinalIgnoreCase)
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

        private const string StartSecondsHelper =
            "function getYoutubeStartSeconds(params){var raw=params.get(\"start\")||params.get(\"t\");if(!raw)return 0;if(/^\\d+$/.test(raw))return parseInt(raw,10);var match=/^(?:(\\d+)h)?(?:(\\d+)m)?(?:(\\d+)s?)?$/.exec(raw);return match?3600*parseInt(match[1]||\"0\",10)+60*parseInt(match[2]||\"0\",10)+parseInt(match[3]||\"0\",10):0}";

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
