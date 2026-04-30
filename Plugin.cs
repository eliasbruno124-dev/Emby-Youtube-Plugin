using MediaBrowser.Common;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
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
            typeof(Plugin).Assembly.GetName().Version?.ToString() ?? "0.0.0";

        public override string Name => "YouTube";
        public override string Description => $"Official YouTube integration for Emby via YouTube Data API v3. (v{PluginVersion})";
        public override Guid Id => Guid.Parse("B2C3D4E5-F6A7-4B5C-9D0E-1F2A3B4C5D6E");

        public static Plugin? Instance { get; private set; }
        public static string? LibraryDbPath { get; private set; }
        public static string? CachePath { get; private set; }
        public static IApplicationHost? AppHost { get; private set; }

        public Plugin(
            IApplicationHost applicationHost,
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            AppHost = applicationHost;
            InitializePaths(applicationPaths);
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
            base.SaveConfiguration();

            try { YouTubeApi.InvalidateAllCache(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Cache invalidation after settings save failed: {ex.Message}"); }

            try { YouTubeChannel.ResetCrossFolderSeen(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Seen reset after settings save failed: {ex.Message}"); }

            // Update the shared config hash so the polling loop won't re-detect
            // this same change and trigger a duplicate refresh.
            try { PluginEntryPoint.MarkConfigSaved(Configuration); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] MarkConfigSaved failed: {ex.Message}"); }

            // Kick off a channel refresh right away so users see their changes
            // immediately instead of waiting up to 15s for the next config-hash poll.
            // Fire-and-forget — SaveConfiguration must stay synchronous for Emby.
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                try { await ChannelRefreshInvoker.TriggerRefreshAsync().ConfigureAwait(false); }
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
            config.TrendingCategory = (config.TrendingCategory ?? string.Empty).Trim();
            if (config.TrendingCategory == "0")
                config.TrendingCategory = string.Empty;
            config.ChannelSortBy = NormalizeSort(config.ChannelSortBy);
            config.MaxChannelVideos = Math.Clamp(config.MaxChannelVideos, 1, 150);
            config.MaxSearchVideos = Math.Clamp(config.MaxSearchVideos, 1, 150);
            config.RecentlyAddedPerChannel = Math.Clamp(config.RecentlyAddedPerChannel, 1, 25);
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
