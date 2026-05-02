using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public class PluginEntryPoint : IServerEntryPoint
    {
        private readonly ConcurrentDictionary<long, byte> _imageRepairsInFlight = new();
        private readonly Dictionary<string, string> _lastVideoIdsByPlaylist = new();
        private ILibraryManager? _libraryManager;
        private Timer? _pollTimer;
        private Timer? _switchTimer;
        private int _pollRunning;
        private int _currentPollMinutes;

        // Static so Plugin.SaveConfiguration can update the hash directly when
        // settings are saved through the UI. Otherwise the next poll would
        // re-detect the same change and trigger a duplicate refresh.
        internal static string LastConfigHash = "";

        private static string ConfigHashPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-config-hash.txt");

        public PluginEntryPoint()
        {
        }

        public void Run()
        {
            // If the plugin DLL was updated, wipe transient caches automatically
            // so users don't have to clear them by hand. Library items are
            // left alone — only HTTP/JSON/probe caches under the plugin's
            // cache dir get nuked. Runs BEFORE LoadShortsProbeCache so a stale
            // cache from a previous version can't sneak back into memory.
            WipeCachesIfPluginUpgraded();

            YouTubeChannel.ScheduleSortNameFix();
            YouTubeChannel.LoadShortsProbeCache();
            AttachImageRepairHook();

            try
            {
                if (File.Exists(ConfigHashPath))
                    LastConfigHash = File.ReadAllText(ConfigHashPath).Trim();
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to read config hash: {ex.Message}");
            }

            _pollTimer = new Timer(PollTick, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));
            _switchTimer = new Timer(_ =>
            {
                try
                {
                    var minutes = Math.Clamp(Plugin.Instance?.Options.WatchLaterPollMinutes ?? 3, 1, 60);
                    _pollTimer?.Change(TimeSpan.FromMinutes(minutes), TimeSpan.FromMinutes(minutes));
                    _currentPollMinutes = minutes;
                }
                catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Failed to switch poll interval: {ex.Message}"); }
                _switchTimer?.Dispose();
            }, null, TimeSpan.FromMinutes(3), Timeout.InfiniteTimeSpan);
        }

        private static string PluginVersionStampPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-plugin-version.txt");

        // Wipes transient caches when the installed plugin version differs
        // from the one we recorded last time. Saves the user from having to
        // clear caches by hand after every upgrade. Library items are NOT
        // touched.
        private void WipeCachesIfPluginUpgraded()
        {
            try
            {
                var current = typeof(PluginEntryPoint).Assembly.GetName().Version?.ToString() ?? "0";
                string? previous = null;
                var stampPath = PluginVersionStampPath;
                try
                {
                    if (File.Exists(stampPath))
                        previous = File.ReadAllText(stampPath).Trim();
                }
                catch { }

                if (string.Equals(previous, current, StringComparison.Ordinal))
                    return;

                YouTubeChannel.LogPublic($"[YT] Plugin version changed ({previous ?? "<none>"} -> {current}); wiping caches.");

                var cacheDir = Plugin.CachePath;
                if (!string.IsNullOrEmpty(cacheDir) && Directory.Exists(cacheDir))
                {
                    try { Directory.Delete(cacheDir, recursive: true); }
                    catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Failed to delete cache dir: {ex.Message}"); }
                    try { Directory.CreateDirectory(cacheDir); } catch { }
                }

                var dataDir = Plugin.DataPath;
                if (!string.IsNullOrEmpty(dataDir) && Directory.Exists(dataDir))
                {
                    // Wipe every file the plugin has ever written into the
                    // Emby data dir. The naming pattern is consistent
                    // ("youtube-*" and "shorts-probe-cache.json"), so this
                    // also catches legacy names from older plugin versions
                    // without having to list each one by hand.
                    foreach (var file in Directory.EnumerateFiles(dataDir, "youtube-*"))
                    {
                        var name = Path.GetFileName(file);
                        // Keep the version stamp itself — we rewrite it below.
                        if (name == "youtube-plugin-version.txt") continue;
                        // Keep the API quota counter so daily-limit tracking
                        // stays accurate across plugin upgrades.
                        if (name == "youtube-quota.json") continue;
                        try { File.Delete(file); } catch { }
                    }
                    var legacyShortsProbe = Path.Combine(dataDir, "shorts-probe-cache.json");
                    try { if (File.Exists(legacyShortsProbe)) File.Delete(legacyShortsProbe); } catch { }
                }

                try { File.WriteAllText(stampPath, current); }
                catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Failed to write plugin version stamp: {ex.Message}"); }

                // Trigger a channel refresh in the background so items get
                // re-classified (Shorts/Live tags) without the user having to
                // poke it manually after every upgrade. Wait a bit first so
                // Emby is done registering the channel.
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(15)).ConfigureAwait(false);
                        YouTubeChannel.LogPublic("[YT] Triggering post-upgrade channel refresh");
                        await ChannelRefreshInvoker.TriggerRefreshAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        YouTubeChannel.LogPublic($"[YT] Post-upgrade refresh failed: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] WipeCachesIfPluginUpgraded failed: {ex.Message}");
            }
        }

        private void AttachImageRepairHook()
        {
            try
            {
                _libraryManager ??= ResolveLibraryManager();
                if (_libraryManager == null)
                {
                    YouTubeChannel.LogPublic("[YTIMG] LibraryManager not available; post-refresh image repair disabled.");
                    return;
                }

                _libraryManager.ItemUpdated += OnItemUpdated;
                YouTubeChannel.LogPublic("[YTIMG] Post-refresh image repair hook attached.");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YTIMG] Failed to attach image repair hook: {ex.Message}");
            }
        }

        private static ILibraryManager? ResolveLibraryManager() =>
            Plugin.ResolveService<ILibraryManager>();

        private void OnItemUpdated(object? sender, ItemChangeEventArgs e)
        {
            var item = e.Item;
            if (item == null)
                return;

            if (string.IsNullOrEmpty(YouTubeImageProvider.TryGetVideoId(item)))
                return;

            var itemId = item.InternalId;
            if (!_imageRepairsInFlight.TryAdd(itemId, 0))
                return;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250)).ConfigureAwait(false);

                    if (item.HasImage(ImageType.Primary))
                        return;

                    if (!YouTubeImageProvider.EnsurePrimaryImage(item, "item updated"))
                        return;

                    item.UpdateToRepository(ItemUpdateType.ImageUpdate);
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YTIMG] Post-refresh image repair failed for item {itemId}: {ex.Message}");
                }
                finally
                {
                    _imageRepairsInFlight.TryRemove(itemId, out _);
                }
            });
        }

        // Called from Plugin.SaveConfiguration so a settings save updates the
        // hash and the running poll interval in one shot. Returns the new hash.
        internal static string MarkConfigSaved(PluginConfiguration config)
        {
            var hash = ComputeConfigHash(config);
            LastConfigHash = hash;
            TrySaveConfigHash(hash);
            return hash;
        }

        private void PollTick(object? state)
        {
            if (Interlocked.Exchange(ref _pollRunning, 1) == 1)
                return;

            _ = PollTickAsync();
        }

        private async Task PollTickAsync()
        {
            try
            {
                var config = Plugin.Instance?.Options;
                if (config == null) return;

                var apiKey = (config.ApiKey ?? "").Trim();
                var watchLaterRaw = (config.WatchLaterPlaylist ?? "").Trim();

                AdjustPollIntervalToConfig(config);

                // Catches the case where the plugin was redeployed without
                // going through SaveConfiguration (e.g. someone edited the XML
                // on disk). We only do this once per process so the poll loop
                // stays focused on playlist change detection.
                if (Interlocked.CompareExchange(ref _bootstrapHashChecked, 1, 0) == 0)
                    await RefreshOnConfigChange(apiKey, config).ConfigureAwait(false);

                await PollWatchLaterPlaylists(apiKey, watchLaterRaw).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Poll tick failed: {ex.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref _pollRunning, 0);
            }
        }

        private static int _bootstrapHashChecked;

        private void AdjustPollIntervalToConfig(PluginConfiguration config)
        {
            // Only matters once we're past the bootstrap fast-poll phase.
            if (_currentPollMinutes <= 0) return;

            var configured = Math.Clamp(config.WatchLaterPollMinutes, 1, 60);
            if (configured == _currentPollMinutes) return;

            try
            {
                _pollTimer?.Change(TimeSpan.FromMinutes(configured), TimeSpan.FromMinutes(configured));
                _currentPollMinutes = configured;
                YouTubeChannel.LogPublic($"[YT] Watch Later poll interval updated to {configured} min");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to adjust poll interval: {ex.Message}");
            }
        }

        private async Task RefreshOnConfigChange(string apiKey, PluginConfiguration config)
        {
            var currentHash = ComputeConfigHash(config);
            var configChanged = !string.Equals(currentHash, LastConfigHash, StringComparison.Ordinal);

            if (!configChanged)
                return;

            LastConfigHash = currentHash;
            TrySaveConfigHash(currentHash);

            if (string.IsNullOrEmpty(apiKey))
                return;

            try { YouTubeApi.InvalidateAllCache(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Cache invalidation failed: {ex.Message}"); }

            await ChannelRefreshInvoker.TriggerRefreshAsync().ConfigureAwait(false);
        }

        private async Task PollWatchLaterPlaylists(string apiKey, string watchLaterRaw)
        {
            if (watchLaterRaw.Length <= 2 || string.IsNullOrEmpty(apiKey))
                return;

            var playlists = watchLaterRaw
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (playlists.Count == 0)
                return;

            var anyChanged = false;
            foreach (var playlist in playlists)
            {
                try
                {
                    // Up to 250 IDs gives us solid change detection without
                    // burning extra quota every poll. Goes through the
                    // cache-bypass helper so we always see live state.
                    var ids = await YouTubeApi.GetPlaylistVideoIdsFreshAsync(
                            apiKey, playlist, 250, CancellationToken.None)
                        .ConfigureAwait(false);
                    var current = string.Join(",", ids);

                    _lastVideoIdsByPlaylist.TryGetValue(playlist, out var previous);
                    if (!string.IsNullOrEmpty(previous) && current != previous)
                    {
                        YouTubeApi.InvalidateCacheContaining(playlist);
                        anyChanged = true;
                    }

                    _lastVideoIdsByPlaylist[playlist] = current;
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Watch Later poll failed for {playlist}: {ex.Message}");
                }
            }

            if (anyChanged)
                await ChannelRefreshInvoker.TriggerRefreshAsync().ConfigureAwait(false);
        }

        private static void TrySaveConfigHash(string currentHash)
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigHashPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(ConfigHashPath, currentHash);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to save config hash: {ex.Message}");
            }
        }

        internal static string ComputeConfigHash(PluginConfiguration c)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var blob = string.Join("|", new[]
            {
                (c.ApiKey ?? "").Trim(),
                (c.SavedItems ?? "").Trim(),
                (c.WatchLaterPlaylist ?? "").Trim(),
                c.ShowTrending ? "1" : "0",
                c.ShowCategories ? "1" : "0",
                c.ShowRecentlyAdded ? "1" : "0",
                c.ShowLiveFolders ? "1" : "0",
                c.ShortsEnabled ? "1" : "0",
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

        public void Dispose()
        {
            if (_libraryManager != null)
                _libraryManager.ItemUpdated -= OnItemUpdated;

            _pollTimer?.Dispose();
            _switchTimer?.Dispose();
        }
    }

    internal static class ChannelRefreshInvoker
    {
        private static object? _channelMgr;
        private static object? _registeredChannel;
        private static MethodInfo? _refreshContentMethod;

        // Serializes channel refreshes. Save-triggered refreshes, watch-later
        // changes and bootstrap config-hash mismatches all funnel through the
        // same lock so we never run two YouTube scans at the same time.
        private static readonly SemaphoreSlim RefreshGate = new(1, 1);

        public static async Task TriggerRefreshAsync()
        {
            if (!await RefreshGate.WaitAsync(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false))
            {
                // A refresh is already in flight. Just bail out — the running
                // refresh will pick up whatever the latest config says, so this
                // caller doesn't need to wait its turn.
                YouTubeChannel.LogPublic("[YT] TriggerRefresh: skipped (refresh already in progress)");
                return;
            }

            try
            {
                await TriggerRefreshCoreAsync().ConfigureAwait(false);
            }
            finally
            {
                RefreshGate.Release();
            }
        }

        private static async Task TriggerRefreshCoreAsync()
        {
            try
            {
                if (!EnsureChannelManager())
                {
                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: ChannelManager not ready (will retry on next poll)");
                    return;
                }

                YouTubeChannel.LogPublic($"[YT] TriggerRefresh: invoking {_refreshContentMethod!.Name} on registered YouTube channel");

                var result = _refreshContentMethod.Invoke(_channelMgr, BuildRefreshArgs());
                if (result is Task task)
                {
                    var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(120))).ConfigureAwait(false);
                    if (completed != task)
                    {
                        YouTubeChannel.LogPublic("[YT] TriggerRefresh timed out after 120 seconds");
                        return;
                    }

                    await task.ConfigureAwait(false);
                }

                YouTubeChannel.LogPublic("[YT] TriggerRefresh: completed");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] TriggerRefresh failed: {ex.Message}");
            }
        }

        private static object?[] BuildRefreshArgs()
        {
            var pars = _refreshContentMethod!.GetParameters();
            var args = new object?[pars.Length];
            for (var i = 0; i < pars.Length; i++)
            {
                var pt = pars[i].ParameterType;
                var name = pars[i].Name ?? "";
                if (pt.Name == "IChannel" || name == "channel")
                    args[i] = _registeredChannel;
                else if (pt == typeof(CancellationToken))
                    args[i] = CancellationToken.None;
                else if (name.Contains("maxRefresh", StringComparison.OrdinalIgnoreCase))
                    args[i] = 5;
                else if (pt == typeof(string))
                    args[i] = null;
                else if (pars[i].HasDefaultValue)
                    args[i] = pars[i].DefaultValue;
                else
                    args[i] = pt.IsValueType ? Activator.CreateInstance(pt) : null;
            }

            return args;
        }

        private static bool EnsureChannelManager()
        {
            if (_channelMgr != null && _registeredChannel != null && _refreshContentMethod != null)
                return true;

            var appHost = Plugin.AppHost;
            if (appHost == null) return false;

            var iface = FindChannelManagerInterface();
            if (iface == null) return false;

            var resolve = appHost.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Resolve" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
            if (resolve == null) return false;

            _channelMgr = resolve.MakeGenericMethod(iface).Invoke(appHost, null);
            if (_channelMgr == null) return false;

            _refreshContentMethod = _channelMgr.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "RefreshChannelContent");
            if (_refreshContentMethod == null) return false;

            _registeredChannel = FindRegisteredYouTubeChannel(_channelMgr);
            return _registeredChannel != null;
        }

        private static Type? FindChannelManagerInterface()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var iface = asm.GetTypes().FirstOrDefault(t => t.IsInterface && t.Name == "IChannelManager");
                    if (iface != null) return iface;
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Failed scanning assembly for IChannelManager: {ex.Message}");
                }
            }

            return null;
        }

        private static object? FindRegisteredYouTubeChannel(object channelManager)
        {
            var channelsProp = channelManager.GetType().GetProperty("Channels",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (channelsProp == null)
                return null;

            var channels = channelsProp.GetValue(channelManager);
            if (channels is not System.Collections.IEnumerable enumerable)
                return null;

            foreach (var ch in enumerable)
            {
                if (ch?.GetType().Name == "YouTubeChannel")
                    return ch;
            }

            return null;
        }
    }
}
