using MediaBrowser.Controller.Plugins;
using System;
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
        private static readonly HttpClient PollHttp = new() { Timeout = TimeSpan.FromSeconds(10) };

        private readonly Dictionary<string, string> _lastVideoIdsByPlaylist = new();
        private Timer? _pollTimer;
        private Timer? _switchTimer;
        private string _lastConfigHash = "";
        private int _pollRunning;

        private static string ConfigHashPath =>
            Path.Combine(Plugin.CachePath ?? Path.GetTempPath(), "..", "youtube-config-hash.txt");

        public void Run()
        {
            YouTubeChannel.ScheduleSortNameFix();

            try
            {
                if (File.Exists(ConfigHashPath))
                    _lastConfigHash = File.ReadAllText(ConfigHashPath).Trim();
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to read config hash: {ex.Message}");
            }

            var minutes = Math.Clamp(Plugin.Instance?.Options.WatchLaterPollMinutes ?? 3, 1, 60);
            _pollTimer = new Timer(PollTick, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));
            _switchTimer = new Timer(_ =>
            {
                try { _pollTimer?.Change(TimeSpan.FromMinutes(minutes), TimeSpan.FromMinutes(minutes)); }
                catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Failed to switch poll interval: {ex.Message}"); }
                _switchTimer?.Dispose();
            }, null, TimeSpan.FromMinutes(3), Timeout.InfiniteTimeSpan);
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

        private async Task RefreshOnConfigChange(string apiKey, PluginConfiguration config)
        {
            var currentHash = ComputeConfigHash(config);
            var configChanged = !string.Equals(currentHash, _lastConfigHash, StringComparison.Ordinal);

            if (!configChanged)
                return;

            _lastConfigHash = currentHash;
            TrySaveConfigHash(currentHash);

            if (string.IsNullOrEmpty(apiKey))
                return;

            try { YouTubeApi.InvalidateAllCache(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Cache invalidation failed: {ex.Message}"); }

            try { YouTubeChannel.ResetCrossFolderSeen(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Seen reset failed: {ex.Message}"); }

            await ChannelRefreshInvoker.TriggerRefreshAsync().ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(12)).ConfigureAwait(false);

            try { YouTubeChannel.ResetCrossFolderSeen(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Seen reset before second refresh failed: {ex.Message}"); }

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
                    var current = await GetPlaylistVideoIdSnapshot(apiKey, playlist).ConfigureAwait(false);
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

        private static async Task<string> GetPlaylistVideoIdSnapshot(string apiKey, string playlist)
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

            return string.Join(",", ids);
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
            _pollTimer?.Dispose();
            _switchTimer?.Dispose();
        }
    }

    internal static class ChannelRefreshInvoker
    {
        private static object? _channelMgr;
        private static object? _registeredChannel;
        private static MethodInfo? _refreshContentMethod;

        public static async Task TriggerRefreshAsync()
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
