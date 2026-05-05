using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Session;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly ConcurrentDictionary<string, DateTime> _resumeSeeksInFlight = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, PendingResumeSeek> _pendingResumeSeeks = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, PlaybackProgressSnapshot> _youtubePlaybackProgress = new(StringComparer.Ordinal);
        private readonly Dictionary<string, string> _lastVideoIdsByPlaylist = new();
        private ILibraryManager? _libraryManager;
        private ISessionManager? _sessionManager;
        private IUserDataManager? _userDataManager;
        private Timer? _pollTimer;
        private Timer? _switchTimer;
        private int _pollRunning;
        private int _currentPollMinutes;
        private const long ResumeSeekMinimumTicks = TimeSpan.TicksPerSecond * 5;
        private const long ResumeSeekEndGuardTicks = TimeSpan.TicksPerSecond * 10;
        private const long UserDataProgressSaveDeltaTicks = TimeSpan.TicksPerSecond * 15;
        private static readonly TimeSpan ResumeSeekDelay = TimeSpan.FromMilliseconds(1800);
        private static readonly TimeSpan UserDataProgressSaveInterval = TimeSpan.FromSeconds(20);

        private sealed record PendingResumeSeek(
            string Key,
            string SessionId,
            string? UserId,
            string VideoId,
            long ItemId,
            long PositionTicks,
            long RuntimeTicks);

        private sealed record PlaybackProgressSnapshot(
            string Key,
            long UserInternalId,
            string VideoId,
            long ItemId,
            long PositionTicks,
            long SavedPositionTicks,
            DateTime LastSeenUtc,
            DateTime LastSavedUtc);

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

            // Capture PlaybackInfo before PlaybackStart so resume and
            // "play from beginning" can be told apart reliably.
            EmbeddedDependencyLoader.Register();
            PlaybackIntentInterceptor.Install();
            YouTubeChannel.ScheduleSortNameFix();
            YouTubeChannel.LoadShortsProbeCache();
            AttachImageRepairHook();
            AttachResumeSeekHook();

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

        private void AttachResumeSeekHook()
        {
            try
            {
                _sessionManager ??= Plugin.ResolveService<ISessionManager>();
                _userDataManager ??= Plugin.ResolveService<IUserDataManager>();

                if (_sessionManager == null || _userDataManager == null)
                {
                    YouTubeChannel.LogPublic("[YT] Resume seek hook disabled; playback services not available.");
                    return;
                }

                _sessionManager.PlaybackStart += OnPlaybackStart;
                _sessionManager.PlaybackProgress += OnPlaybackProgress;
                _sessionManager.PlaybackStopped += OnPlaybackStopped;
                YouTubeChannel.LogPublic("[YT] Resume seek hooks attached.");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to attach resume seek hook: {ex.Message}");
            }
        }

        private void OnPlaybackStart(object? sender, PlaybackProgressEventArgs e)
        {
            try
            {
                if (_sessionManager == null)
                    return;

                var item = e.Item;
                var session = e.Session;
                if (item == null || session == null || string.IsNullOrEmpty(session.Id))
                    return;

                CancelPendingResumeSeeksForSession(session.Id);

                var videoId = YouTubeImageProvider.TryGetVideoId(item);
                if (string.IsNullOrEmpty(videoId))
                    return;

                // Diagnostics so client-specific failures (LG, Theater, etc.)
                // can be identified without a debugger attached.
                YouTubeChannel.LogPublic(
                    $"[YT] PlaybackStart fired: video={videoId} item={item.InternalId} "
                    + $"client={session.Client ?? "?"} device={session.DeviceName ?? "?"} "
                    + $"deviceId={session.DeviceId ?? "?"} userId={session.UserId ?? "?"} "
                    + $"position={e.PlaybackPositionTicks.GetValueOrDefault()}");

                // The captured intent is the source of truth. Without it we do
                // not force a seek, because that would break explicit restarts.
                if (!PlaybackIntentInterceptor.TryConsume(session.UserId, item.InternalId, session.DeviceId, out var intent))
                {
                    YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {videoId}; playback intent was not captured.");
                    return;
                }

                YouTubeChannel.LogPublic(
                    $"[YT] Intent consumed for {videoId}: StartTimeTicks={intent.StartTimeTicks} capturedAt={intent.CapturedUtc:O}");

                var positionTicks = intent.StartTimeTicks;
                if (positionTicks < ResumeSeekMinimumTicks)
                {
                    // Emby sends StartTimeTicks=0 for "play from beginning".
                    YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {videoId}; play from beginning was requested.");
                    return;
                }

                var runtimeTicks = item.RunTimeTicks.GetValueOrDefault();
                if (runtimeTicks > 0 && runtimeTicks - positionTicks < ResumeSeekEndGuardTicks)
                    return;

                var key = GetResumeSeekKey(e, item);
                var pending = new PendingResumeSeek(
                    key,
                    session.Id,
                    session.UserId,
                    videoId,
                    item.InternalId,
                    positionTicks,
                    runtimeTicks);

                _pendingResumeSeeks[key] = pending;

                // YouTube iframe playback needs a moment to exist before all
                // clients accept the seek command reliably.
                _ = Task.Run(async () =>
                {
                    await Task.Delay(ResumeSeekDelay).ConfigureAwait(false);
                    await TrySendPendingResumeSeekAsync(key, "delayed start").ConfigureAwait(false);
                });
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackStart resume hook failed: {ex.Message}");
            }
        }

        private void OnPlaybackStopped(object? sender, PlaybackStopEventArgs e)
        {
            try
            {
                TrackYouTubePlaybackProgress(e, forceSave: true, playedToCompletion: e.PlayedToCompletion);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackStopped user data save failed: {ex.Message}");
            }
            finally
            {
                var sessionId = e.Session?.Id;
                if (!string.IsNullOrEmpty(sessionId))
                    CancelPendingResumeSeeksForSession(sessionId);

                if (e.Item != null)
                    _youtubePlaybackProgress.TryRemove(GetResumeSeekKey(e, e.Item), out _);
            }
        }

        private void CancelPendingResumeSeeksForSession(string sessionId)
        {
            foreach (var kvp in _pendingResumeSeeks)
            {
                if (string.Equals(kvp.Value.SessionId, sessionId, StringComparison.Ordinal))
                    _pendingResumeSeeks.TryRemove(kvp.Key, out _);
            }
        }

        private void OnPlaybackProgress(object? sender, PlaybackProgressEventArgs e)
        {
            try
            {
                var item = e.Item;
                if (item == null)
                    return;

                TrackYouTubePlaybackProgress(e, forceSave: false, playedToCompletion: false);

                var key = GetResumeSeekKey(e, item);
                if (!_pendingResumeSeeks.TryGetValue(key, out var pending))
                    return;

                var currentTicks = e.PlaybackPositionTicks.GetValueOrDefault();
                if (currentTicks >= ResumeSeekMinimumTicks
                    && Math.Abs(currentTicks - pending.PositionTicks) <= ResumeSeekEndGuardTicks)
                {
                    // The client is already in the requested area, so the
                    // pending retry is no longer needed.
                    _pendingResumeSeeks.TryRemove(key, out _);
                    return;
                }

                // Progress can arrive before the delayed task fires. Treat it
                // as an early retry point while the player is definitely alive.
                _ = Task.Run(async () =>
                    await TrySendPendingResumeSeekAsync(key, "progress").ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackProgress resume hook failed: {ex.Message}");
            }
        }

        private static string GetResumeSeekKey(PlaybackProgressEventArgs e, BaseItem item)
        {
            var sessionId = e.Session?.Id ?? string.Empty;
            var playSessionId = e.PlaySessionId ?? string.Empty;
            return $"{sessionId}|{playSessionId}|{item.InternalId}";
        }

        private void TrackYouTubePlaybackProgress(
            PlaybackProgressEventArgs e,
            bool forceSave,
            bool playedToCompletion)
        {
            if (_userDataManager == null)
                return;

            var item = e.Item;
            if (item == null)
                return;

            var videoId = YouTubeImageProvider.TryGetVideoId(item);
            if (string.IsNullOrEmpty(videoId))
                return;

            var userInternalId = GetPlaybackUserInternalId(e);
            if (userInternalId <= 0)
                return;

            var key = GetResumeSeekKey(e, item);
            _youtubePlaybackProgress.TryGetValue(key, out var previous);

            var positionTicks = GetPlaybackPositionTicks(e);
            if (positionTicks <= 0 && previous != null)
                positionTicks = previous.PositionTicks;

            if (!playedToCompletion && positionTicks < ResumeSeekMinimumTicks)
                return;

            var now = DateTime.UtcNow;
            var lastSavedUtc = previous?.LastSavedUtc ?? DateTime.MinValue;
            var savedPositionTicks = previous?.SavedPositionTicks ?? 0;
            var shouldSave = forceSave
                             || previous == null
                             || now - lastSavedUtc >= UserDataProgressSaveInterval
                             || Math.Abs(positionTicks - savedPositionTicks) >= UserDataProgressSaveDeltaTicks;

            _youtubePlaybackProgress[key] = new PlaybackProgressSnapshot(
                key,
                userInternalId,
                videoId,
                item.InternalId,
                positionTicks,
                savedPositionTicks,
                now,
                lastSavedUtc);

            if (!shouldSave)
                return;

            var reason = playedToCompletion
                ? UserDataSaveReason.PlaybackFinished
                : UserDataSaveReason.PlaybackProgress;

            if (!SaveYouTubePlaybackPosition(item, userInternalId, videoId, positionTicks, playedToCompletion, reason))
                return;

            _youtubePlaybackProgress[key] = new PlaybackProgressSnapshot(
                key,
                userInternalId,
                videoId,
                item.InternalId,
                positionTicks,
                positionTicks,
                now,
                now);
        }

        private bool SaveYouTubePlaybackPosition(
            BaseItem item,
            long userInternalId,
            string videoId,
            long positionTicks,
            bool playedToCompletion,
            UserDataSaveReason reason)
        {
            if (_userDataManager == null)
                return false;

            var runtimeTicks = item.RunTimeTicks.GetValueOrDefault();
            var completed = playedToCompletion
                            || (runtimeTicks > 0 && runtimeTicks - positionTicks < ResumeSeekEndGuardTicks);

            if (!completed && positionTicks < ResumeSeekMinimumTicks)
                return false;

            try
            {
                var userData = _userDataManager.GetUserData(userInternalId, item);
                if (userData == null)
                    return false;

                _userDataManager.UpdatePlayState(item, userData, completed && runtimeTicks > 0 ? runtimeTicks : positionTicks);

                if (completed)
                {
                    userData.Played = true;
                    userData.PlaybackPositionTicks = 0;
                    userData.HideFromResume = true;
                    userData.LastPlayedDate ??= DateTimeOffset.UtcNow;
                    if (userData.PlayCount <= 0)
                        userData.PlayCount = 1;
                }
                else
                {
                    userData.PlaybackPositionTicks = positionTicks;
                    userData.HideFromResume = false;
                }

                _userDataManager.SaveUserData(userInternalId, item, userData, reason, CancellationToken.None);
                return true;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to save YouTube progress for {videoId}: {ex.Message}");
                return false;
            }
        }

        private static long GetPlaybackUserInternalId(PlaybackProgressEventArgs e)
        {
            var sessionUserId = e.Session?.UserInternalId ?? 0;
            if (sessionUserId > 0)
                return sessionUserId;

            return e.Users?.FirstOrDefault()?.InternalId ?? 0;
        }

        private static long GetPlaybackPositionTicks(PlaybackProgressEventArgs e)
        {
            return e.PlaybackPositionTicks
                   ?? e.Session?.PlayState?.PositionTicks
                   ?? 0;
        }

        private async Task TrySendPendingResumeSeekAsync(string key, string reason)
        {
            if (_sessionManager == null)
                return;

            if (!_pendingResumeSeeks.TryRemove(key, out var pending))
                return;

            // The user may have switched videos during the delay. Never seek a
            // session that has already moved on to another item.
            if (!IsPendingSeekStillCurrent(pending))
            {
                YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {pending.VideoId}; session moved to another item.");
                return;
            }

            var inFlightKey = $"{key}|{pending.PositionTicks}";
            if (!_resumeSeeksInFlight.TryAdd(inFlightKey, DateTime.UtcNow))
                return;

            try
            {
                var request = new PlaystateRequest
                {
                    Command = PlaystateCommand.Seek,
                    SeekPositionTicks = pending.PositionTicks,
                    ControllingUserId = pending.UserId
                };

                await _sessionManager.SendPlaystateCommand(
                        pending.SessionId,
                        pending.SessionId,
                        request,
                        CancellationToken.None)
                    .ConfigureAwait(false);

                YouTubeChannel.LogPublic($"[YT] Resume seek sent ({reason}) for {pending.VideoId} to {pending.PositionTicks} ticks.");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Resume seek failed for {pending.VideoId}: {ex.Message}");
            }
            finally
            {
                _resumeSeeksInFlight.TryRemove(inFlightKey, out _);
            }
        }

        private bool IsPendingSeekStillCurrent(PendingResumeSeek pending)
        {
            try
            {
                var session = _sessionManager?.Sessions
                    .FirstOrDefault(s => string.Equals(s.Id, pending.SessionId, StringComparison.Ordinal));

                if (session == null)
                    return false;

                var nowPlaying = session.FullNowPlayingItem;
                if (nowPlaying != null)
                    return nowPlaying.InternalId == pending.ItemId;

                // Some session DTOs only expose the public item id.
                var dtoId = session.NowPlayingItem?.Id;
                return long.TryParse(dtoId, out var parsedId) && parsedId == pending.ItemId;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Resume seek current-item check failed: {ex.Message}");
                return false;
            }
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

            if (_sessionManager != null)
            {
                _sessionManager.PlaybackStart -= OnPlaybackStart;
                _sessionManager.PlaybackProgress -= OnPlaybackProgress;
                _sessionManager.PlaybackStopped -= OnPlaybackStopped;
            }

            _pollTimer?.Dispose();
            _switchTimer?.Dispose();
            PlaybackIntentInterceptor.Uninstall();
        }
    }

    internal static class ChannelRefreshInvoker
    {
        private static object? _channelMgr;
        private static object? _registeredChannel;
        private static MethodInfo? _refreshContentMethod;
        private static int _refreshAgainRequested;

        // Serializes channel refreshes. Save-triggered refreshes, watch-later
        // changes and bootstrap config-hash mismatches all funnel through the
        // same lock so we never run two YouTube scans at the same time.
        private static readonly SemaphoreSlim RefreshGate = new(1, 1);

        public static async Task TriggerRefreshAsync()
        {
            if (!await RefreshGate.WaitAsync(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false))
            {
                // A refresh is already in flight. Queue one follow-up pass so
                // settings saved mid-refresh are still picked up afterwards.
                Interlocked.Exchange(ref _refreshAgainRequested, 1);
                YouTubeChannel.LogPublic("[YT] TriggerRefresh: queued follow-up (refresh already in progress)");
                return;
            }

            try
            {
                while (true)
                {
                    await TriggerRefreshCoreAsync().ConfigureAwait(false);

                    if (Interlocked.Exchange(ref _refreshAgainRequested, 0) != 1)
                        break;

                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: running queued follow-up");
                }
            }
            finally
            {
                RefreshGate.Release();

                if (Volatile.Read(ref _refreshAgainRequested) == 1)
                    _ = Task.Run(TriggerRefreshAsync);
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
            var getChannel = channelManager.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "GetChannel"
                                     && m.IsGenericMethodDefinition
                                     && m.GetParameters().Length == 0);
            if (getChannel != null)
            {
                try
                {
                    var channel = getChannel.MakeGenericMethod(typeof(YouTubeChannel)).Invoke(channelManager, null);
                    if (channel != null)
                        return channel;
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] TriggerRefresh: GetChannel<YouTubeChannel> failed: {ex.Message}");
                }
            }

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
