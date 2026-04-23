using MediaBrowser.Common;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public class Plugin : BasePluginSimpleUI<PluginConfiguration>, IHasThumbImage
    {
        private static readonly string PluginVersion =
            typeof(Plugin).Assembly.GetName().Version?.ToString(3) ?? "0.0.0";

        public override string Name => "YouTube";
        public override string Description => $"Official YouTube integration for Emby via YouTube Data API v3. (v{PluginVersion})";
        public override Guid Id => Guid.Parse("B2C3D4E5-F6A7-4B5C-9D0E-1F2A3B4C5D6E");

        public static Plugin? Instance { get; private set; }
        public static string? LibraryDbPath { get; private set; }
        public static string? CachePath { get; private set; }
        public static IApplicationHost? AppHost { get; private set; }

        public Plugin(IApplicationHost applicationHost) : base(applicationHost)
        {
            Instance = this;
            AppHost = applicationHost;
            try
            {
                var paths = applicationHost.Resolve<IServerApplicationPaths>();
                if (paths != null)
                {
                    var candidate = Path.Combine(paths.DataPath, "library.db");
                    if (File.Exists(candidate))
                        LibraryDbPath = candidate;

                    var cacheDir = Path.Combine(paths.DataPath, "youtube-cache");
                    Directory.CreateDirectory(cacheDir);
                    CachePath = cacheDir;
                }
            }
            catch { }
        }

        public PluginConfiguration Options => GetOptions();

        public ImageFormat ThumbImageFormat => ImageFormat.Png;

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

        public Stream GetThumbImage()
        {
            var type = GetType();
            var stream = type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.png");
            return stream ?? new MemoryStream();
        }
    }

    public class PluginEntryPoint : IServerEntryPoint
    {
        private Timer? _pollTimer;
        // Snapshot of the last-seen video IDs per polled playlist (key = playlist ID).
        // Used to detect newly-added Watch Later videos so we can invalidate the
        // matching cache entry and trigger a refresh.
        private readonly Dictionary<string, string> _lastVideoIdsByPlaylist = new();
        // Track config to detect first-time setup / changes and auto-trigger refresh.
        // Many users thought the plugin was broken because the channel was not refreshed
        // automatically after they entered their API key + saved channels for the first time.
        // The hash of the last-seen config is persisted to disk so this also works across
        // server restarts (e.g. user adds API key, then restarts emby).
        private string _lastConfigHash = "";
        private bool _firstTickDone;
        private static string ConfigHashPath =>
            System.IO.Path.Combine(Plugin.CachePath ?? System.IO.Path.GetTempPath(), "..", "youtube-config-hash.txt");
        private static object? _channelMgr;
        private static object? _registeredChannel;
        private static MethodInfo? _refreshContentMethod;
        private static readonly HttpClient PollHttp = new() { Timeout = TimeSpan.FromSeconds(10) };

        public void Run()
        {
            YouTubeChannel.ScheduleSortNameFix();
            // Load last-known config hash from disk so we can detect changes that happened
            // while the server was offline (e.g. API key was just added before a restart).
            try
            {
                if (File.Exists(ConfigHashPath))
                    _lastConfigHash = File.ReadAllText(ConfigHashPath).Trim();
            }
            catch { }

            var minutes = Math.Clamp(Plugin.Instance?.Options.WatchLaterPollMinutes ?? 3, 1, 60);
            // Poll every 15s for the first ~3 minutes so config changes (API key, saved
            // channels) are picked up quickly and the channel can refresh itself.
            // After that, fall back to the user-configured WatchLater poll interval.
            _pollTimer = new Timer(PollTick, null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15));
            // Schedule a one-shot switch to the slower interval after 3 minutes.
            new Timer(_ =>
            {
                try { _pollTimer?.Change(TimeSpan.FromMinutes(minutes), TimeSpan.FromMinutes(minutes)); }
                catch { }
            }, null, TimeSpan.FromMinutes(3), Timeout.InfiniteTimeSpan);
        }

        // Hash covers EVERY content-affecting setting so toggling any of them
        // (e.g. Trending Region, Hide Shorts, Show Live Folders) wipes the
        // cache and triggers an immediate channel refresh — otherwise users
        // think the change "didn't apply" because Emby keeps showing the
        // stale folder contents until the next scheduled refresh.
        private static string ComputeConfigHash(PluginConfiguration c)
        {
            using var sha = System.Security.Cryptography.SHA1.Create();
            var blob = string.Join("|", new[]
            {
                (c.ApiKey ?? "").Trim(),
                (c.SavedItems ?? "").Trim(),
                (c.WatchLaterPlaylist ?? "").Trim(),
                c.ShowTrending ? "1" : "0",
                c.ShowCategories ? "1" : "0",
                c.ShowRecentlyAdded ? "1" : "0",
                c.ShowLiveFolders ? "1" : "0",
                c.HideShorts ? "1" : "0",
                (c.TrendingRegion ?? "").Trim(),
                (c.TrendingCategory ?? "").Trim(),
                c.ShowLikeCount ? "1" : "0",
                c.ShowCommentCount ? "1" : "0",
                (c.ChannelSortBy ?? "").Trim(),
                c.MaxChannelVideos.ToString(),
                c.MaxSearchVideos.ToString(),
                c.RecentlyAddedPerChannel.ToString(),
            });
            return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(blob)));
        }

        private void PollTick(object? state)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var config = Plugin.Instance?.Options;
                    if (config == null) return;

                    var apiKey = (config.ApiKey ?? "").Trim();
                    var savedItems = (config.SavedItems ?? "").Trim();
                    var watchLaterRaw = (config.WatchLaterPlaylist ?? "").Trim();

                    // Compare against the last-seen config hash (persisted to disk so this
                    // also detects changes that happened while the server was offline).
                    var currentHash = ComputeConfigHash(config);
                    bool configChanged = !string.Equals(currentHash, _lastConfigHash, StringComparison.Ordinal);
                    _firstTickDone = true;

                    if (configChanged && !string.IsNullOrEmpty(apiKey))
                    {
                        // Persist new hash so we don't trigger again unnecessarily
                        try
                        {
                            var dir = Path.GetDirectoryName(ConfigHashPath);
                            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                            File.WriteAllText(ConfigHashPath, currentHash);
                        }
                        catch { }
                        _lastConfigHash = currentHash;

                        // Drop everything cached + clear cross-folder dedup so the
                        // next refresh starts with a clean slate.
                        try { YouTubeApi.InvalidateAllCache(); } catch { }
                        try { YouTubeChannel.ResetCrossFolderSeen(); } catch { }

                        // First refresh: register / wake up the channel. The very
                        // first call after server start often returns nothing because
                        // Emby hasn't fully wired up the channel yet, so we kick off
                        // a second refresh ~10s later as insurance.
                        TriggerRefresh();
                        _ = Task.Delay(TimeSpan.FromSeconds(12)).ContinueWith(_ =>
                        {
                            try { YouTubeChannel.ResetCrossFolderSeen(); } catch { }
                            TriggerRefresh();
                        });
                    }
                    else if (configChanged)
                    {
                        // API key still missing - just remember the (empty) hash but don't refresh.
                        _lastConfigHash = currentHash;
                    }

                    // Watch Later polling: poll EVERY configured playlist so new
                    // videos surface quickly regardless of which slot they're in.
                    // Cost = 1 quota unit per playlist per poll interval.
                    // (Tip: increase WatchLaterPollMinutes if you have many
                    // playlists and want to keep daily quota usage low.)
                    if (watchLaterRaw.Length <= 2) return;
                    var playlists = watchLaterRaw
                        .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 2)
                        .Distinct(StringComparer.Ordinal)
                        .ToList();
                    if (playlists.Count == 0) return;
                    if (string.IsNullOrEmpty(apiKey)) return;

                    bool anyChanged = false;
                    foreach (var playlist in playlists)
                    {
                        try
                        {
                            var url = $"https://www.googleapis.com/youtube/v3/playlistItems" +
                                      $"?part=contentDetails&playlistId={Uri.EscapeDataString(playlist)}" +
                                      $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";

                            var json = await PollHttp.GetStringAsync(url).ConfigureAwait(false);
                            QuotaTracker.Record(1);

                            var ids = new List<string>();
                            using var doc = JsonDocument.Parse(json);
                            if (doc.RootElement.TryGetProperty("items", out var items)
                                && items.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var item in items.EnumerateArray())
                                {
                                    if (item.TryGetProperty("contentDetails", out var cd)
                                        && cd.TryGetProperty("videoId", out var vid))
                                    {
                                        var videoId = vid.GetString();
                                        if (!string.IsNullOrEmpty(videoId))
                                            ids.Add(videoId);
                                    }
                                }
                            }

                            var current = string.Join(",", ids);
                            _lastVideoIdsByPlaylist.TryGetValue(playlist, out var previous);
                            if (!string.IsNullOrEmpty(previous) && current != previous)
                            {
                                YouTubeApi.InvalidateCacheContaining(playlist);
                                anyChanged = true;
                            }
                            _lastVideoIdsByPlaylist[playlist] = current;
                        }
                        catch { }
                    }

                    if (anyChanged) TriggerRefresh();
                }
                catch { }
            });
        }

        private static void TriggerRefresh()
        {
            try
            {
                if (!EnsureChannelManager())
                {
                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: ChannelManager not ready (will retry on next poll)");
                    return;
                }

                YouTubeChannel.LogPublic($"[YT] TriggerRefresh: invoking {_refreshContentMethod!.Name} on registered YouTube channel");

                var pars = _refreshContentMethod!.GetParameters();
                var args = new object?[pars.Length];
                for (int i = 0; i < pars.Length; i++)
                {
                    var pt = pars[i].ParameterType;
                    var name = pars[i].Name ?? "";
                    if (pt.Name == "IChannel" || name == "channel")
                        args[i] = _registeredChannel;
                    else if (pt == typeof(CancellationToken))
                        args[i] = CancellationToken.None;
                    else if (name.Contains("maxRefresh", StringComparison.OrdinalIgnoreCase))
                        // Hierarchy depth: root → channel_x_ → channelvideos_x_ → videos.
                        // 2 only refreshed root + channel_x_ leaving channelvideos_x_ empty,
                        // so videos never appeared in Latest until the user navigated in.
                        args[i] = 5;
                    else if (pt == typeof(string))
                        args[i] = null;
                    else if (pars[i].HasDefaultValue)
                        args[i] = pars[i].DefaultValue;
                    else
                        args[i] = pt.IsValueType ? Activator.CreateInstance(pt) : null;
                }

                var result = _refreshContentMethod.Invoke(_channelMgr, args);
                if (result is Task task)
                    task.Wait(TimeSpan.FromSeconds(120));

                YouTubeChannel.LogPublic("[YT] TriggerRefresh: completed");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] TriggerRefresh failed: {ex.Message}");
            }
        }

        private static bool EnsureChannelManager()
        {
            if (_channelMgr != null && _registeredChannel != null && _refreshContentMethod != null)
                return true;

            var appHost = Plugin.AppHost;
            if (appHost == null) return false;

            Type? iface = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try { iface = asm.GetTypes().FirstOrDefault(t => t.IsInterface && t.Name == "IChannelManager"); }
                catch { }
                if (iface != null) break;
            }
            if (iface == null) return false;

            var resolve = appHost.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Resolve" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
            if (resolve == null) return false;

            _channelMgr = resolve.MakeGenericMethod(iface).Invoke(appHost, null);
            if (_channelMgr == null) return false;

            _refreshContentMethod =
                _channelMgr.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == "RefreshChannelContent");
            if (_refreshContentMethod == null) return false;

            var channelsProp = _channelMgr.GetType().GetProperty("Channels",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (channelsProp != null)
            {
                var channels = channelsProp.GetValue(_channelMgr);
                if (channels is System.Collections.IEnumerable enumerable)
                {
                    foreach (var ch in enumerable)
                    {
                        if (ch?.GetType().Name == "YouTubeChannel")
                        {
                            _registeredChannel = ch;
                            break;
                        }
                    }
                }
            }

            return _registeredChannel != null;
        }

        public void Dispose()
        {
            _pollTimer?.Dispose();
        }
    }
}
