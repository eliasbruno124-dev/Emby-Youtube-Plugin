using MediaBrowser.Common;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Emby.YouTubePlugin
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages, IHasThumbImage
    {
        private static readonly string PluginVersion =
            typeof(Plugin).Assembly.GetName().Version?.ToString(4) ?? "0.0.0.0";

        public override string Name => "YouTube";
        public override string Description => $"YouTube channel integration for Emby using the YouTube Data API v3. (v{PluginVersion})";
        public override Guid Id => Guid.Parse("B2C3D4E5-F6A7-4B5C-9D0E-1F2A3B4C5D6E");

        public static Plugin? Instance { get; private set; }
        public static string? LibraryDbPath { get; private set; }
        public static string? CachePath { get; private set; }
        public static string? DataPath { get; private set; }
        public static string? SystemConfigurationFilePath { get; private set; }
        public static IApplicationHost? AppHost { get; private set; }
        internal static ILogger? PluginLogger { get; private set; }

        public Plugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            InitializePaths(applicationPaths);
        }

        // Pulls a service out of Emby's IoC container via reflection. Emby
        // doesn't give us a typed Resolve<T> on IApplicationHost directly, so
        // we call it through the concrete type at runtime.
        internal static T? ResolveService<T>(IApplicationHost? host = null) where T : class
        {
            host ??= AppHost;
            if (host == null) return null;
            try
            {
                var resolve = host.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == "Resolve" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
                return resolve?.MakeGenericMethod(typeof(T)).Invoke(host, null) as T;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[YT] ResolveService<{typeof(T).Name}> failed: {ex.Message}");
                return null;
            }
        }

        internal static void InitializeApplicationHost(IApplicationHost applicationHost)
        {
            AppHost = applicationHost;
            TryInitLogger(applicationHost);
        }

        private static void TryInitLogger(IApplicationHost applicationHost)
        {
            if (PluginLogger != null)
                return;

            var factory = ResolveService<ILoggerFactory>(applicationHost);
            PluginLogger = factory?.CreateLogger("YouTubePlugin");
            if (PluginLogger == null)
                System.Diagnostics.Debug.WriteLine("[YT] ILoggerFactory not available; falling back to Debug.WriteLine");
        }

        public PluginConfiguration Options => Configuration;

        public ImageFormat ThumbImageFormat => ImageFormat.Png;

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "YouTubeConfiguration",
                    DisplayName = "YouTube",
                    EmbeddedResourcePath = "Emby.YouTubePlugin.Configuration.YouTubeConfiguration.html",
                    EnableInMainMenu = true,
                    MenuIcon = "ondemand_video"
                },
                new PluginPageInfo
                {
                    Name = "YouTubeConfigurationjs",
                    EmbeddedResourcePath = "Emby.YouTubePlugin.Configuration.YouTubeConfiguration.js"
                }
            };
        }

        public override void SaveConfiguration()
        {
            NormalizeConfiguration(Configuration);

            // Only a change to a content-affecting field (the exact set
            // ComputeConfigHash covers) warrants cache invalidation and a
            // channel refresh. The configuration page auto-saves on every edit,
            // so a re-save with no content change — or a Donate/poll-interval
            // only change — must not invalidate caches and burn the tracked
            // YouTube API quota on a needless refresh. Same gate the poll loop's
            // RefreshOnConfigChange already uses.
            var contentChanged = !string.Equals(
                PluginEntryPoint.ComputeConfigHash(Configuration),
                PluginEntryPoint.LastConfigHash,
                StringComparison.Ordinal);

            base.SaveConfiguration();

            // Update the shared config hash so the polling loop doesn't
            // re-detect this same change and fire a duplicate refresh.
            try { PluginEntryPoint.MarkConfigSaved(Configuration); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] MarkConfigSaved failed: {ex.Message}"); }

            if (!contentChanged)
                return;

            try { YouTubeApi.InvalidateAllCache(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Cache invalidation after settings save failed: {ex.Message}"); }

            try { YouTubeChannel.ResetCrossFolderSeen(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Seen reset after settings save failed: {ex.Message}"); }

            // Kick off a channel refresh right away so users see their changes
            // immediately. Serialized via ChannelRefreshInvoker so a flurry of
            // saves doesn't fan out into parallel scans.
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                try { await ChannelRefreshInvoker.TriggerRefreshAsync(ChannelRefreshInvoker.ContentRefreshDepth).ConfigureAwait(false); }
                catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Immediate refresh after save failed: {ex.Message}"); }
            });
        }

        public override PluginInfo GetPluginInfo()
        {
            var info = base.GetPluginInfo();
            var assemblyDir = Path.GetDirectoryName(AssemblyFilePath) ?? string.Empty;
            var thumbPath = Path.Combine(assemblyDir, "thumb.png");

            if (File.Exists(thumbPath))
            {
                var imageTag = File.GetLastWriteTimeUtc(thumbPath)
                    .Ticks
                    .ToString(CultureInfo.InvariantCulture);

                SetStringPropertyIfExists(info, "ImageTag", imageTag);

                var pngBytes = File.ReadAllBytes(thumbPath);
                var imageDataUrl = "data:image/png;base64," + Convert.ToBase64String(pngBytes);
                SetStringPropertyIfExists(info, "ImageUrl", imageDataUrl);
            }

            return info;
        }

        public Stream GetThumbImage()
        {
            var type = GetType();
            var stream = type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.png");
            return stream ?? new MemoryStream();
        }

        private static void InitializePaths(IApplicationPaths applicationPaths)
        {
            try
            {
                DataPath = applicationPaths.DataPath;
                SystemConfigurationFilePath = applicationPaths.SystemConfigurationFilePath;
                var candidate = Path.Combine(applicationPaths.DataPath, "library.db");
                if (File.Exists(candidate))
                    LibraryDbPath = candidate;

                var cacheDir = Path.Combine(applicationPaths.DataPath, "youtube-cache");
                Directory.CreateDirectory(cacheDir);
                CachePath = cacheDir;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to initialize plugin paths: {ex.Message}");
            }
        }

        private static void NormalizeConfiguration(PluginConfiguration config)
        {
            config.ApiKey = (config.ApiKey ?? string.Empty).Trim();
            config.SavedItems = (config.SavedItems ?? string.Empty).Trim();
            config.WatchLaterPlaylist = (config.WatchLaterPlaylist ?? string.Empty).Trim();
            config.TrendingRegion = (config.TrendingRegion ?? string.Empty).Trim().ToUpperInvariant();
            if (config.TrendingRegion.Length > 0 && !IsRegionCodeShape(config.TrendingRegion))
                config.TrendingRegion = string.Empty;
            config.TrendingCategory = (config.TrendingCategory ?? string.Empty).Trim();
            if (config.TrendingCategory == "0")
                config.TrendingCategory = string.Empty;
            config.ChannelSortBy = NormalizeSort(config.ChannelSortBy);
            config.MaxChannelVideos = Math.Clamp(config.MaxChannelVideos, 1, 150);
            config.MaxSearchVideos = Math.Clamp(config.MaxSearchVideos, 1, 150);
            config.WatchLaterPollMinutes = Math.Clamp(config.WatchLaterPollMinutes, 1, 60);
            if (config.HideShorts)
            {
                config.ShowShorts = false;
                config.HideShorts = false;
            }
            config.Donate = string.IsNullOrWhiteSpace(config.Donate)
                ? "https://paypal.me/eliasbruno123"
                : config.Donate.Trim();
        }

        private static string NormalizeSort(string? sortBy)
        {
            return (sortBy ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "date" => "date",
                "newest" => "date",
                "viewcount" => "viewCount",
                "popular" => "viewCount",
                "rating" => "rating",
                "relevance" => "relevance",
                _ => "date"
            };
        }

        private static bool IsRegionCodeShape(string value) =>
            value.Length == 2
            && value[0] is >= 'A' and <= 'Z'
            && value[1] is >= 'A' and <= 'Z';

        private static void SetStringPropertyIfExists(object target, string propertyName, string value)
        {
            var property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            if (property?.CanWrite == true && property.PropertyType == typeof(string))
            {
                property.SetValue(target, value);
            }
        }
    }
}
