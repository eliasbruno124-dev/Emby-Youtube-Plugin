using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Session;
using MediaBrowser.Common;
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
        private readonly YouTubeSortNameRepairer _sortNameRepairer = new();
        private readonly ConcurrentDictionary<long, byte> _imageRepairsInFlight = new();
        private readonly ConcurrentDictionary<string, DateTime> _resumeSeeksInFlight = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, PendingResumeSeek> _pendingResumeSeeks = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, ResumeCheckpoint> _resumeCheckpoints = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, ResumeCheckpoint> _lastResumeProgressBySession = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, ResumePositionEstimate> _resumePositionEstimatesBySession = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, long> _resumeTrackingFloorsBySession = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, DateTime> _lastResumeCheckpointFlushByKey = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, DateTime> _youtubeAutoplayUnlocksInFlight = new(StringComparer.Ordinal);
        private readonly Dictionary<string, string> _lastVideoIdsByPlaylist = new();
        private readonly object _resumeCheckpointFileLock = new();
        private ILibraryManager? _libraryManager;
        private ISessionManager? _sessionManager;
        private IUserDataManager? _userDataManager;
        private Timer? _pollTimer;
        private Timer? _switchTimer;
        private Timer? _resumeCheckpointSaveTimer;
        private int _pollRunning;
        private int _currentPollMinutes;
        private int _resumeCheckpointsDirty;
        private const long ResumeSeekMinimumTicks = TimeSpan.TicksPerSecond * 5;
        private const long ResumeSeekEndGuardTicks = TimeSpan.TicksPerSecond * 10;
        private static readonly TimeSpan ResumeSeekDelay = TimeSpan.FromMilliseconds(1800);
        private static readonly TimeSpan ResumeSeekRetryDelay = TimeSpan.FromMilliseconds(1400);
        // Grace window after a sent seek during which we wait for the client to
        // actually process it. Without this, a Progress event that was emitted
        // BEFORE the seek landed would falsely trigger another jump.
        private static readonly TimeSpan ResumeSeekPostSendGrace = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan ResumeCheckpointFlushDelay = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan ResumeCheckpointFlushInterval = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan ResumeCheckpointTtl = TimeSpan.FromDays(180);
        private static readonly TimeSpan RecentSessionRestartWindow = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan RecentSessionCleanupInterval = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan ResumePositionEstimateTtl = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan[] YouTubeAutoplayUnlockDelays =
        {
            TimeSpan.FromMilliseconds(120),
            TimeSpan.FromMilliseconds(450),
            TimeSpan.FromMilliseconds(900),
            TimeSpan.FromMilliseconds(1600),
            TimeSpan.FromMilliseconds(2800)
        };
        private static readonly TimeSpan YouTubeAutoplayUnmuteDelay = TimeSpan.FromSeconds(8);
        private const int ResumeSeekMaxAttempts = 5;
        private DateTime _lastRecentSessionCleanupUtc = DateTime.MinValue;

        private sealed record PendingResumeSeek(
            string Key,
            string SessionId,
            string? UserId,
            string VideoId,
            long ItemId,
            long PositionTicks,
            long RuntimeTicks,
            int AttemptsSent,
            DateTime? LastSeekUtc);

        private sealed record ResumeCheckpoint(
            string UserId,
            string VideoId,
            long PositionTicks,
            long RuntimeTicks,
            DateTime UpdatedUtc);

        private sealed record ResumePositionEstimate(
            string UserId,
            string VideoId,
            long ItemId,
            long BasePositionTicks,
            long RuntimeTicks,
            DateTime BaseUtc);

        // Static so Plugin.SaveConfiguration can update the hash directly when
        // settings are saved through the UI. Otherwise the next poll would
        // re-detect the same change and trigger a duplicate refresh.
        internal static string LastConfigHash = "";

        private static string ConfigHashPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-config-hash.txt");

        private static string ResumeCheckpointPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-resume-checkpoints.json");

        public PluginEntryPoint(
            IApplicationHost applicationHost,
            ISessionManager sessionManager,
            ILibraryManager libraryManager,
            IUserDataManager userDataManager,
            IChannelManager channelManager)
        {
            Plugin.InitializeApplicationHost(applicationHost);
            _sessionManager = sessionManager;
            _libraryManager = libraryManager;
            _userDataManager = userDataManager;
            ChannelRefreshInvoker.Initialize(channelManager);
        }

        public void Run()
        {
            // If the plugin DLL was updated, wipe transient caches automatically
            // so users don't have to clear them by hand. Persistent plugin
            // state stays intact: config hash, quota, logs and resume
            // checkpoints are not cache files. Runs BEFORE LoadShortsProbeCache
            // so stale probe cache entries can't sneak back into memory.
            var upgradeRefreshQueued = WipeCachesIfPluginUpgraded();
            EnsureChannelSurfaceMigration(upgradeRefreshQueued);

            // Capture PlaybackInfo before PlaybackStart so resume and
            // "play from beginning" can be told apart reliably.
            EmbeddedDependencyLoader.Register();
            PlaybackIntentInterceptor.Install();
            DashboardYouTubePlayerInterceptor.Install();
            YouTubeChannel.LoadShortsProbeCache();
            LoadResumeCheckpoints();
            _sortNameRepairer.Start();
            AttachImageRepairHook();
            QueueExistingSortNameRepair("startup");
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
                if (_sessionManager == null)
                {
                    YouTubeChannel.LogPublic("[YT] Resume seek hook disabled; session manager not available.");
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
                _resumePositionEstimatesBySession.TryRemove(session.Id, out _);
                _resumeTrackingFloorsBySession.TryRemove(session.Id, out _);

                var videoId = YouTubeImageProvider.TryGetVideoId(item);
                if (string.IsNullOrEmpty(videoId))
                    return;

                ScheduleYouTubeWebViewAutoplayUnlock(e, item, videoId);

                var runtimeTicks = item.RunTimeTicks.GetValueOrDefault();
                long positionTicks;

                if (PlaybackIntentInterceptor.TryConsume(session.UserId, item.InternalId, session.DeviceId, out var intent))
                {
                    positionTicks = intent.StartTimeTicks;

                    if (positionTicks < ResumeSeekMinimumTicks)
                    {
                        if (TryGetRecentSessionCheckpoint(session.Id, videoId, out var restartCheckpoint))
                        {
                            positionTicks = restartCheckpoint.PositionTicks;
                            runtimeTicks = runtimeTicks > 0 ? runtimeTicks : restartCheckpoint.RuntimeTicks;
                            YouTubeChannel.LogPublic($"[YT] Resume seek using recent session checkpoint for {videoId} after player restart.");
                        }
                        else
                        {
                            // Emby sends StartTimeTicks=0 for an explicit "play from beginning".
                            RemoveResumeCheckpoint(session.UserId, videoId);
                            YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {videoId}; play from beginning was requested.");
                            return;
                        }
                    }
                }
                else if (TryGetResumeCheckpoint(session.UserId, videoId, out var checkpoint))
                {
                    positionTicks = checkpoint.PositionTicks;
                    runtimeTicks = runtimeTicks > 0 ? runtimeTicks : checkpoint.RuntimeTicks;
                    YouTubeChannel.LogPublic($"[YT] Resume seek using plugin checkpoint for {videoId}; playback intent was not captured.");
                }
                else
                {
                    YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {videoId}; no playback intent or checkpoint was available.");
                    return;
                }

                if (runtimeTicks > 0 && runtimeTicks - positionTicks < ResumeSeekEndGuardTicks)
                {
                    RemoveResumeCheckpoint(session.UserId, videoId);
                    return;
                }

                var key = GetResumeSeekKey(e, item);
                var pending = new PendingResumeSeek(
                    key,
                    session.Id,
                    session.UserId,
                    videoId,
                    item.InternalId,
                    positionTicks,
                    runtimeTicks,
                    0,
                    null);

                _pendingResumeSeeks[key] = pending;
                _resumeTrackingFloorsBySession[session.Id] = Math.Max(
                    ResumeSeekMinimumTicks,
                    positionTicks - ResumeSeekEndGuardTicks);

                SchedulePendingResumeSeek(key, ResumeSeekDelay, "delayed start");
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
                var item = e.Item;
                var session = e.Session;
                var sessionId = session?.Id;

                if (item != null)
                {
                    var videoId = YouTubeImageProvider.TryGetVideoId(item);
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        if (e.PlayedToCompletion)
                        {
                            RemoveResumeCheckpoint(session?.UserId, videoId);
                        }
                        else
                        {
                            var stopPositionTicks = GetPlaybackPositionTicks(e.PlaybackPositionTicks, session, videoId, item.InternalId);
                            if (CanTrackPlaybackPosition(sessionId, stopPositionTicks))
                            {
                                TrackResumeCheckpoint(
                                    sessionId,
                                    session?.UserId,
                                    videoId,
                                    stopPositionTicks,
                                    item.RunTimeTicks.GetValueOrDefault(),
                                    forceSave: true);
                                SaveNativeResumePosition(
                                    item,
                                    session,
                                    stopPositionTicks,
                                    UserDataSaveReason.PlaybackProgress);
                            }
                            else
                            {
                                RestoreProtectedNativeResumePosition(item, session, videoId);
                            }
                        }

                        SaveResumeCheckpoints();
                    }
                }

                if (!string.IsNullOrEmpty(sessionId))
                {
                    CancelPendingResumeSeeksForSession(sessionId);
                    // A clean Stop ends the "is this a network restart?" window.
                    // Without this, an explicit replay within RecentSessionRestartWindow
                    // would be misinterpreted as a reconnect and skip back.
                    _lastResumeProgressBySession.TryRemove(sessionId, out _);
                    _resumePositionEstimatesBySession.TryRemove(sessionId, out _);
                    _resumeTrackingFloorsBySession.TryRemove(sessionId, out _);
                    CleanupRecentSessionCheckpoints();
                }
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackStopped resume tracking failed: {ex.Message}");
            }
        }

        private void CancelPendingResumeSeeksForSession(string sessionId)
        {
            foreach (var kvp in _pendingResumeSeeks)
            {
                if (string.Equals(kvp.Value.SessionId, sessionId, StringComparison.Ordinal))
                    _pendingResumeSeeks.TryRemove(kvp.Key, out _);
            }

            // The in-flight stamps share the "<sessionId>|..." key prefix. Drop
            // them with the pending seeks; otherwise every playback leaves one
            // entry behind for the lifetime of the process.
            var prefix = sessionId + "|";
            foreach (var key in _resumeSeeksInFlight.Keys)
            {
                if (key.StartsWith(prefix, StringComparison.Ordinal))
                    _resumeSeeksInFlight.TryRemove(key, out _);
            }
        }

        private void OnPlaybackProgress(object? sender, PlaybackProgressEventArgs e)
        {
            try
            {
                var item = e.Item;
                var session = e.Session;
                if (item == null)
                    return;

                var videoId = YouTubeImageProvider.TryGetVideoId(item);
                var key = GetResumeSeekKey(e, item);
                var hasPendingSeek = _pendingResumeSeeks.TryGetValue(key, out var pending);
                var currentTicks = GetPlaybackPositionTicks(e.PlaybackPositionTicks, session, videoId, item.InternalId);

                if (!string.IsNullOrEmpty(videoId))
                {
                    if (CanTrackPlaybackPosition(session?.Id, currentTicks))
                    {
                        var runtimeTicksForCheckpoint = item.RunTimeTicks.GetValueOrDefault();
                        TrackResumeCheckpoint(
                            session?.Id,
                            session?.UserId,
                            videoId,
                            currentTicks,
                            runtimeTicksForCheckpoint);
                        SaveNativeResumePosition(
                            item,
                            session,
                            currentTicks,
                            UserDataSaveReason.PlaybackProgress);
                    }
                    else
                    {
                        RestoreProtectedNativeResumePosition(item, session, videoId);
                    }
                }

                if (!hasPendingSeek || pending == null)
                    return;

                if (currentTicks >= ResumeSeekMinimumTicks
                    && currentTicks >= pending.PositionTicks - ResumeSeekEndGuardTicks)
                {
                    // The client is already in the requested area, so the
                    // pending retry is no longer needed.
                    _pendingResumeSeeks.TryRemove(key, out _);
                    _resumeSeeksInFlight.TryRemove($"{key}|{pending.PositionTicks}", out _);
                    _resumeTrackingFloorsBySession.TryRemove(pending.SessionId, out _);
                    return;
                }

                // Don't re-issue a seek while the client may still be processing
                // the previous one — that Progress was likely emitted before our
                // seek landed and would cause a visible double jump.
                if (pending.LastSeekUtc is { } lastSeek
                    && DateTime.UtcNow - lastSeek < ResumeSeekPostSendGrace)
                {
                    return;
                }

                // Progress can arrive before the delayed task fires. Treat it
                // as an early retry point while the player is definitely alive.
                SchedulePendingResumeSeek(key, TimeSpan.Zero, "progress");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackProgress resume hook failed: {ex.Message}");
            }
        }

        private bool CanTrackPlaybackPosition(string? sessionId, long positionTicks)
        {
            if (positionTicks < ResumeSeekMinimumTicks)
                return false;

            if (string.IsNullOrEmpty(sessionId)
                || !_resumeTrackingFloorsBySession.TryGetValue(sessionId, out var floorTicks))
            {
                return true;
            }

            if (positionTicks >= floorTicks)
            {
                _resumeTrackingFloorsBySession.TryRemove(sessionId, out _);
                return true;
            }

            return false;
        }

        private static string GetResumeSeekKey(PlaybackProgressEventArgs e, BaseItem item)
        {
            var sessionId = e.Session?.Id ?? string.Empty;
            var playSessionId = e.PlaySessionId ?? string.Empty;
            return $"{sessionId}|{playSessionId}|{item.InternalId}";
        }

        private long GetPlaybackPositionTicks(long? eventPositionTicks, object? session, string? videoId, long itemId)
        {
            var positionTicks = eventPositionTicks.GetValueOrDefault();
            if (positionTicks > 0)
                return positionTicks;

            var sessionId = GetSessionId(session);
            if (!string.IsNullOrEmpty(sessionId)
                && TryGetSessionPositionTicks(sessionId, out var sessionPositionTicks)
                && sessionPositionTicks > 0)
            {
                return sessionPositionTicks;
            }

            return TryGetEstimatedResumePositionTicks(sessionId, videoId, itemId, out var estimatedTicks)
                ? estimatedTicks
                : 0;
        }

        private void SchedulePendingResumeSeek(string key, TimeSpan delay, string reason)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay).ConfigureAwait(false);

                    await TrySendPendingResumeSeekAsync(key, reason).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Resume seek retry task failed: {ex.Message}");
                }
            });
        }

        private void ScheduleYouTubeWebViewAutoplayUnlock(
            PlaybackProgressEventArgs e,
            BaseItem item,
            string videoId)
        {
            if (_sessionManager == null)
                return;

            var session = e.Session;
            if (session == null
                || string.IsNullOrEmpty(session.Id)
                || !IsLikelyYouTubeWebViewAutoplayClient(session))
            {
                return;
            }

            var key = $"{session.Id}|{e.PlaySessionId ?? string.Empty}|{item.InternalId}";
            if (!_youtubeAutoplayUnlocksInFlight.TryAdd(key, DateTime.UtcNow))
                return;

            var sessionId = session.Id;
            var userId = session.UserId;
            var itemId = item.InternalId;
            _ = Task.Run(async () =>
            {
                try
                {
                    YouTubeChannel.LogPublic($"[YT] YouTube WebView autoplay unlock scheduled for {videoId} on {DescribeSession(session)}.");

                    for (var i = 0; i < YouTubeAutoplayUnlockDelays.Length; i++)
                    {
                        await Task.Delay(YouTubeAutoplayUnlockDelays[i]).ConfigureAwait(false);
                        if (!IsSessionStillOnItem(sessionId, itemId, "autoplay unlock"))
                            return;

                        await SendGeneralCommandAsync(sessionId, userId, GeneralCommandType.Mute).ConfigureAwait(false);
                        await Task.Delay(TimeSpan.FromMilliseconds(80)).ConfigureAwait(false);
                        if (!IsSessionStillOnItem(sessionId, itemId, "autoplay unlock"))
                            return;

                        await SendPlaystateCommandAsync(sessionId, userId, PlaystateCommand.Unpause).ConfigureAwait(false);
                        YouTubeChannel.LogPublic($"[YT] YouTube WebView autoplay unlock sent for {videoId} (attempt {i + 1}).");
                    }

                    await Task.Delay(YouTubeAutoplayUnmuteDelay).ConfigureAwait(false);
                    if (IsSessionStillOnItem(sessionId, itemId, "autoplay unmute"))
                    {
                        await SendGeneralCommandAsync(sessionId, userId, GeneralCommandType.Unmute).ConfigureAwait(false);
                        YouTubeChannel.LogPublic($"[YT] YouTube WebView autoplay unmute sent for {videoId}.");
                    }
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] YouTube WebView autoplay unlock failed for {videoId}: {ex.Message}");
                }
                finally
                {
                    _youtubeAutoplayUnlocksInFlight.TryRemove(key, out _);
                }
            });
        }

        private async Task SendGeneralCommandAsync(string sessionId, string? userId, GeneralCommandType commandType)
        {
            if (_sessionManager == null)
                return;

            var command = new GeneralCommand
            {
                Name = commandType.ToString(),
                ControllingUserId = userId
            };

            await _sessionManager.SendGeneralCommand(
                    sessionId,
                    sessionId,
                    command,
                    CancellationToken.None)
                .ConfigureAwait(false);
        }

        private async Task SendPlaystateCommandAsync(string sessionId, string? userId, PlaystateCommand commandType)
        {
            if (_sessionManager == null)
                return;

            var request = new PlaystateRequest
            {
                Command = commandType,
                ControllingUserId = userId
            };

            await _sessionManager.SendPlaystateCommand(
                    sessionId,
                    sessionId,
                    request,
                    CancellationToken.None)
                .ConfigureAwait(false);
        }

        private async Task TrySendPendingResumeSeekAsync(string key, string reason)
        {
            if (_sessionManager == null)
                return;

            if (!_pendingResumeSeeks.TryGetValue(key, out var pending))
                return;

            // The user may have switched videos during the delay. Never seek a
            // session that has already moved on to another item.
            if (!IsPendingSeekStillCurrent(pending))
            {
                YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {pending.VideoId}; session moved to another item.");
                _pendingResumeSeeks.TryRemove(key, out _);
                _resumePositionEstimatesBySession.TryRemove(pending.SessionId, out _);
                _resumeTrackingFloorsBySession.TryRemove(pending.SessionId, out _);
                return;
            }

            var inFlightKey = $"{key}|{pending.PositionTicks}";
            var now = DateTime.UtcNow;
            if (_resumeSeeksInFlight.TryGetValue(inFlightKey, out var lastAttempt)
                && now - lastAttempt < ResumeSeekRetryDelay)
            {
                return;
            }

            if (TryGetSessionPositionTicks(pending.SessionId, out var currentTicks)
                && currentTicks >= ResumeSeekMinimumTicks
                && currentTicks >= pending.PositionTicks - ResumeSeekEndGuardTicks)
            {
                _pendingResumeSeeks.TryRemove(key, out _);
                _resumeSeeksInFlight.TryRemove(inFlightKey, out _);
                _resumeTrackingFloorsBySession.TryRemove(pending.SessionId, out _);
                return;
            }

            if (pending.AttemptsSent >= ResumeSeekMaxAttempts)
            {
                _pendingResumeSeeks.TryRemove(key, out _);
                _resumeSeeksInFlight.TryRemove(inFlightKey, out _);
                _resumePositionEstimatesBySession.TryRemove(pending.SessionId, out _);
                YouTubeChannel.LogPublic($"[YT] Resume seek gave up for {pending.VideoId}; player stayed before the resume point after {pending.AttemptsSent} attempts.");
                return;
            }

            _resumeSeeksInFlight[inFlightKey] = now;

            try
            {
                await SendSeekCommandAsync(pending).ConfigureAwait(false);

                ArmZeroPositionEstimateIfNeeded(pending);

                // TryUpdate avoids resurrecting a pending entry that was just
                // removed (e.g. by PlaybackStopped) — if the slot moved on, the
                // increment is simply skipped.
                var updated = pending with
                {
                    AttemptsSent = pending.AttemptsSent + 1,
                    LastSeekUtc = DateTime.UtcNow
                };
                _pendingResumeSeeks.TryUpdate(key, updated, pending);
                YouTubeChannel.LogPublic($"[YT] Resume seek sent ({reason}, attempt {updated.AttemptsSent}) for {pending.VideoId} to {pending.PositionTicks} ticks.");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Resume seek failed for {pending.VideoId}: {ex.Message}");
            }
        }

        private async Task SendSeekCommandAsync(PendingResumeSeek pending)
        {
            if (_sessionManager == null)
                return;

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
        }

        // Cache the PropertyInfo per session type so we don't pay reflection
        // cost on every retry. Some Emby builds expose the property on a
        // derived type, so keyed by runtime Type is safest.
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> _sessionPositionPropertyCache = new();

        private bool TryGetSessionPositionTicks(string sessionId, out long positionTicks)
        {
            positionTicks = 0;

            try
            {
                var session = _sessionManager?.Sessions
                    .FirstOrDefault(s => string.Equals(s.Id, sessionId, StringComparison.Ordinal));
                if (session == null)
                    return false;

                var prop = _sessionPositionPropertyCache.GetOrAdd(
                    session.GetType(),
                    t => t.GetProperty("PlaybackPositionTicks", BindingFlags.Instance | BindingFlags.Public));
                if (prop?.GetValue(session) is long longValue)
                {
                    positionTicks = longValue;
                    return true;
                }
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Resume seek session-position check failed: {ex.Message}");
            }

            return false;
        }

        private void SaveNativeResumePosition(
            BaseItem item,
            SessionInfo? session,
            long positionTicks,
            UserDataSaveReason reason)
        {
            if (_userDataManager == null
                || session == null
                || session.UserInternalId <= 0
                || positionTicks < ResumeSeekMinimumTicks)
            {
                return;
            }

            try
            {
                var data = _userDataManager.GetUserData(session.UserInternalId, item);
                if (data == null)
                    return;

                var playedToCompletion = _userDataManager.UpdatePlayState(item, data, positionTicks);
                if (!playedToCompletion && data.PlaybackPositionTicks <= 0)
                    data.PlaybackPositionTicks = positionTicks;

                _userDataManager.SaveUserData(
                    session.UserInternalId,
                    item,
                    data,
                    playedToCompletion ? UserDataSaveReason.PlaybackFinished : reason,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Native resume position save failed: {ex.Message}");
            }
        }

        private void RestoreProtectedNativeResumePosition(BaseItem item, SessionInfo? session, string videoId)
        {
            if (session == null || session.UserInternalId <= 0)
                return;

            var positionTicks = 0L;
            if (TryGetResumeCheckpoint(session.UserId, videoId, out var checkpoint))
            {
                positionTicks = checkpoint.PositionTicks;
            }
            else if (_resumeTrackingFloorsBySession.TryGetValue(session.Id, out var floorTicks))
            {
                positionTicks = floorTicks + ResumeSeekEndGuardTicks;
            }

            if (positionTicks >= ResumeSeekMinimumTicks)
            {
                SaveNativeResumePosition(
                    item,
                    session,
                    positionTicks,
                    UserDataSaveReason.PlaybackProgress);
            }
        }

        private void ArmZeroPositionEstimateIfNeeded(PendingResumeSeek pending)
        {
            var session = _sessionManager?.Sessions
                .FirstOrDefault(s => string.Equals(s.Id, pending.SessionId, StringComparison.Ordinal));
            if (!ShouldUseZeroPositionEstimate(session))
                return;

            _resumePositionEstimatesBySession[pending.SessionId] = new ResumePositionEstimate(
                NormalizeResumeComponent(pending.UserId),
                NormalizeResumeComponent(pending.VideoId),
                pending.ItemId,
                pending.PositionTicks,
                pending.RuntimeTicks,
                DateTime.UtcNow);
        }

        private static bool ShouldUseZeroPositionEstimate(object? session)
        {
            var client = GetStringProperty(session, "Client");
            return client.IndexOf("Android", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool TryGetEstimatedResumePositionTicks(
            string? sessionId,
            string? videoId,
            long itemId,
            out long positionTicks)
        {
            positionTicks = 0;
            if (string.IsNullOrEmpty(sessionId)
                || !_resumePositionEstimatesBySession.TryGetValue(sessionId, out var estimate))
            {
                return false;
            }

            if (DateTime.UtcNow - estimate.BaseUtc > ResumePositionEstimateTtl
                || estimate.ItemId != itemId
                || !string.Equals(estimate.VideoId, NormalizeResumeComponent(videoId), StringComparison.OrdinalIgnoreCase))
            {
                _resumePositionEstimatesBySession.TryRemove(sessionId, out _);
                return false;
            }

            var elapsedTicks = Math.Max(0, (DateTime.UtcNow - estimate.BaseUtc).Ticks);
            var estimatedTicks = estimate.BasePositionTicks + elapsedTicks;
            if (estimate.RuntimeTicks > 0)
                estimatedTicks = Math.Min(estimatedTicks, Math.Max(0, estimate.RuntimeTicks - TimeSpan.TicksPerSecond));

            if (estimatedTicks < ResumeSeekMinimumTicks)
                return false;

            positionTicks = estimatedTicks;
            return true;
        }

        private static string? GetSessionId(object? session) =>
            GetStringProperty(session, "Id");

        private static string GetStringProperty(object? source, string name) =>
            source?.GetType()
                .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(source) as string ?? string.Empty;

        private static bool IsLikelyYouTubeWebViewAutoplayClient(SessionInfo session)
        {
            var client = GetStringProperty(session, "Client");
            var deviceName = GetStringProperty(session, "DeviceName");
            var userAgent = GetStringProperty(session, "UserAgent");

            if (ContainsIgnoreCase(client, "Emby Web"))
                return false;

            var isTheaterFamily = ContainsIgnoreCase(client, "Emby Xbox")
                                  || ContainsIgnoreCase(client, "Emby Windows")
                                  || ContainsIgnoreCase(client, "Emby Theater")
                                  || ContainsIgnoreCase(client, "Emby Theatre")
                                  || ContainsIgnoreCase(deviceName, "XBOX")
                                  || ContainsIgnoreCase(deviceName, "Xbox");

            if (!isTheaterFamily)
                return false;

            return string.IsNullOrEmpty(userAgent)
                   || ContainsIgnoreCase(userAgent, "Chrome/")
                   || ContainsIgnoreCase(userAgent, "Chromium/")
                   || ContainsIgnoreCase(userAgent, "Edg/")
                   || ContainsIgnoreCase(userAgent, "AppleWebKit/")
                   || ContainsIgnoreCase(userAgent, "Safari/");
        }

        private static string DescribeSession(SessionInfo session)
        {
            var client = GetStringProperty(session, "Client");
            var deviceName = GetStringProperty(session, "DeviceName");
            var userAgent = GetStringProperty(session, "UserAgent");
            return $"client={NonEmptyOrUnknown(client)}, device={NonEmptyOrUnknown(deviceName)}, ua={Shorten(NonEmptyOrUnknown(userAgent), 140)}";
        }

        private static string NonEmptyOrUnknown(string value) =>
            string.IsNullOrWhiteSpace(value) ? "unknown" : value;

        private static bool ContainsIgnoreCase(string? value, string needle) =>
            !string.IsNullOrEmpty(value)
            && value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

        private static string Shorten(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength) + "...";
        }

        private bool IsPendingSeekStillCurrent(PendingResumeSeek pending)
            => IsSessionStillOnItem(pending.SessionId, pending.ItemId, "resume seek");

        private bool IsSessionStillOnItem(string sessionId, long itemId, string logContext)
        {
            try
            {
                var session = _sessionManager?.Sessions
                    .FirstOrDefault(s => string.Equals(s.Id, sessionId, StringComparison.Ordinal));

                if (session == null)
                    return false;

                var nowPlaying = session.FullNowPlayingItem;
                if (nowPlaying != null)
                    return nowPlaying.InternalId == itemId;

                // Some session DTOs only expose the public item id.
                var dtoId = session.NowPlayingItem?.Id;
                return long.TryParse(dtoId, out var parsedId) && parsedId == itemId;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] YouTube {logContext} current-item check failed: {ex.Message}");
                return false;
            }
        }

        private void TrackResumeCheckpoint(
            string? sessionId,
            string? userId,
            string videoId,
            long positionTicks,
            long runtimeTicks,
            bool forceSave = false)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                return;

            CleanupRecentSessionCheckpoints();

            var normalizedUserId = NormalizeResumeComponent(userId);
            var normalizedVideoId = NormalizeResumeComponent(videoId);
            if (string.IsNullOrEmpty(normalizedVideoId))
                return;

            if (positionTicks < ResumeSeekMinimumTicks)
                return;

            if (runtimeTicks > 0 && runtimeTicks - positionTicks < ResumeSeekEndGuardTicks)
            {
                RemoveResumeCheckpoint(normalizedUserId, normalizedVideoId);
                if (!string.IsNullOrEmpty(sessionId))
                    _lastResumeProgressBySession.TryRemove(sessionId, out _);
                return;
            }

            var checkpoint = new ResumeCheckpoint(
                normalizedUserId,
                normalizedVideoId,
                positionTicks,
                runtimeTicks,
                DateTime.UtcNow);

            if (!string.IsNullOrEmpty(sessionId))
                _lastResumeProgressBySession[sessionId] = checkpoint;

            if (string.IsNullOrEmpty(normalizedUserId))
                return;

            var key = MakeResumeCheckpointKey(normalizedUserId, normalizedVideoId);
            if (!forceSave
                && _resumeCheckpoints.TryGetValue(key, out var existing)
                && Math.Abs(existing.PositionTicks - positionTicks) < TimeSpan.TicksPerSecond * 3
                && DateTime.UtcNow - existing.UpdatedUtc < ResumeCheckpointFlushInterval)
            {
                return;
            }

            _resumeCheckpoints[key] = checkpoint;

            if (forceSave
                || !_lastResumeCheckpointFlushByKey.TryGetValue(key, out var lastFlush)
                || DateTime.UtcNow - lastFlush >= ResumeCheckpointFlushInterval)
            {
                _lastResumeCheckpointFlushByKey[key] = DateTime.UtcNow;
                MarkResumeCheckpointsDirty();
            }
        }

        private bool TryGetResumeCheckpoint(string? userId, string videoId, out ResumeCheckpoint checkpoint)
        {
            checkpoint = default!;
            var normalizedUserId = NormalizeResumeComponent(userId);
            var normalizedVideoId = NormalizeResumeComponent(videoId);
            if (string.IsNullOrEmpty(normalizedUserId) || string.IsNullOrEmpty(normalizedVideoId))
                return false;

            var key = MakeResumeCheckpointKey(normalizedUserId, normalizedVideoId);
            if (!_resumeCheckpoints.TryGetValue(key, out var candidate))
                return false;

            if (DateTime.UtcNow - candidate.UpdatedUtc > ResumeCheckpointTtl
                || candidate.PositionTicks < ResumeSeekMinimumTicks
                || candidate.RuntimeTicks > 0 && candidate.RuntimeTicks - candidate.PositionTicks < ResumeSeekEndGuardTicks)
            {
                RemoveResumeCheckpoint(normalizedUserId, normalizedVideoId);
                return false;
            }

            checkpoint = candidate;
            return true;
        }

        private bool TryGetRecentSessionCheckpoint(string? sessionId, string videoId, out ResumeCheckpoint checkpoint)
        {
            checkpoint = default!;
            if (string.IsNullOrEmpty(sessionId))
                return false;

            if (!_lastResumeProgressBySession.TryGetValue(sessionId, out var candidate))
                return false;

            if (!string.Equals(candidate.VideoId, NormalizeResumeComponent(videoId), StringComparison.OrdinalIgnoreCase)
                || DateTime.UtcNow - candidate.UpdatedUtc > RecentSessionRestartWindow
                || candidate.PositionTicks < ResumeSeekMinimumTicks
                || candidate.RuntimeTicks > 0 && candidate.RuntimeTicks - candidate.PositionTicks < ResumeSeekEndGuardTicks)
            {
                return false;
            }

            checkpoint = candidate;
            return true;
        }

        private void CleanupRecentSessionCheckpoints()
        {
            // Called from every PlaybackProgress, so cheap-out unless enough
            // time has passed since the last scan. Lock-free single-writer is
            // fine; an occasional concurrent scan just does duplicate work.
            var now = DateTime.UtcNow;
            if (now - _lastRecentSessionCleanupUtc < RecentSessionCleanupInterval)
                return;
            _lastRecentSessionCleanupUtc = now;

            var cutoff = now - RecentSessionRestartWindow;
            foreach (var kvp in _lastResumeProgressBySession)
            {
                if (kvp.Value.UpdatedUtc < cutoff)
                    _lastResumeProgressBySession.TryRemove(kvp.Key, out _);
            }

            foreach (var kvp in _resumePositionEstimatesBySession)
            {
                if (now - kvp.Value.BaseUtc > ResumePositionEstimateTtl)
                    _resumePositionEstimatesBySession.TryRemove(kvp.Key, out _);
            }
        }

        private void RemoveResumeCheckpoint(string? userId, string? videoId)
        {
            var normalizedUserId = NormalizeResumeComponent(userId);
            var normalizedVideoId = NormalizeResumeComponent(videoId);
            if (string.IsNullOrEmpty(normalizedUserId) || string.IsNullOrEmpty(normalizedVideoId))
                return;

            var key = MakeResumeCheckpointKey(normalizedUserId, normalizedVideoId);
            if (_resumeCheckpoints.TryRemove(key, out _))
            {
                _lastResumeCheckpointFlushByKey.TryRemove(key, out _);
                MarkResumeCheckpointsDirty();
            }
        }

        private void LoadResumeCheckpoints()
        {
            try
            {
                var path = ResumeCheckpointPath;
                if (!File.Exists(path))
                    return;

                var json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<Dictionary<string, ResumeCheckpoint>>(json);
                if (loaded == null)
                    return;

                var cutoff = DateTime.UtcNow - ResumeCheckpointTtl;
                foreach (var kvp in loaded)
                {
                    var checkpoint = kvp.Value;
                    if (checkpoint.UpdatedUtc < cutoff
                        || checkpoint.PositionTicks < ResumeSeekMinimumTicks
                        || string.IsNullOrEmpty(checkpoint.UserId)
                        || string.IsNullOrEmpty(checkpoint.VideoId))
                    {
                        continue;
                    }

                    // Rebuild the key from the value so the in-memory dictionary's
                    // OrdinalIgnoreCase contract holds regardless of how the JSON
                    // was serialized.
                    var key = MakeResumeCheckpointKey(checkpoint.UserId, checkpoint.VideoId);
                    _resumeCheckpoints[key] = checkpoint;
                }

                YouTubeChannel.LogPublic($"[YT] Loaded {_resumeCheckpoints.Count} YouTube resume checkpoints.");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to load YouTube resume checkpoints: {ex.Message}");
            }
        }

        private void MarkResumeCheckpointsDirty()
        {
            Interlocked.Exchange(ref _resumeCheckpointsDirty, 1);

            lock (_resumeCheckpointFileLock)
            {
                _resumeCheckpointSaveTimer ??= new Timer(_ => SaveResumeCheckpoints(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _resumeCheckpointSaveTimer.Change(ResumeCheckpointFlushDelay, Timeout.InfiniteTimeSpan);
            }
        }

        private void SaveResumeCheckpoints()
        {
            if (Interlocked.Exchange(ref _resumeCheckpointsDirty, 0) == 0)
                return;

            try
            {
                var cutoff = DateTime.UtcNow - ResumeCheckpointTtl;
                foreach (var kvp in _resumeCheckpoints)
                {
                    if (kvp.Value.UpdatedUtc < cutoff
                        && _resumeCheckpoints.TryRemove(new KeyValuePair<string, ResumeCheckpoint>(kvp.Key, kvp.Value)))
                    {
                        _lastResumeCheckpointFlushByKey.TryRemove(kvp.Key, out _);
                    }
                }

                // Trim the flush-tracker for keys that no longer exist so this
                // dictionary does not grow unbounded over the plugin lifetime.
                foreach (var flushKey in _lastResumeCheckpointFlushByKey.Keys)
                {
                    if (!_resumeCheckpoints.ContainsKey(flushKey))
                        _lastResumeCheckpointFlushByKey.TryRemove(flushKey, out _);
                }

                var snapshot = _resumeCheckpoints.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
                var path = ResumeCheckpointPath;
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(snapshot);
                lock (_resumeCheckpointFileLock)
                {
                    // Write to a sibling temp file and replace atomically so a
                    // crash mid-write never leaves a truncated/corrupt JSON.
                    var tempPath = path + ".tmp";
                    File.WriteAllText(tempPath, json);
                    if (File.Exists(path))
                        File.Replace(tempPath, path, null);
                    else
                        File.Move(tempPath, path);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref _resumeCheckpointsDirty, 1);
                // Re-arm the timer so we keep retrying even if no further
                // playback progress arrives to trigger another mark-dirty.
                try
                {
                    lock (_resumeCheckpointFileLock)
                    {
                        _resumeCheckpointSaveTimer?.Change(ResumeCheckpointFlushDelay, Timeout.InfiniteTimeSpan);
                    }
                }
                catch
                {
                }

                YouTubeChannel.LogPublic($"[YT] Failed to save YouTube resume checkpoints: {ex.Message}");
            }
        }

        private static string MakeResumeCheckpointKey(string userId, string videoId) =>
            $"{NormalizeResumeComponent(userId)}|{NormalizeResumeComponent(videoId)}";

        private static string NormalizeResumeComponent(string? value) =>
            (value ?? string.Empty).Trim();

        private static string PluginVersionStampPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-plugin-version.txt");

        // Wipes transient caches when the installed plugin version differs
        // from the one we recorded last time. Saves the user from having to
        // clear caches by hand after every upgrade. Library items are NOT
        // touched.
        private static string ChannelSurfaceStampPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-channel-surface-v3.txt");

        private const string ChannelSurfaceStamp = "youtube-channel-container-video-folders";

        private bool WipeCachesIfPluginUpgraded()
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
                    return false;

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
                    var legacyShortsProbe = Path.Combine(dataDir, "shorts-probe-cache.json");
                    try { if (File.Exists(legacyShortsProbe)) File.Delete(legacyShortsProbe); } catch { }
                }

                try { File.WriteAllText(stampPath, current); }
                catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Failed to write plugin version stamp: {ex.Message}"); }

                // Trigger a shallow channel refresh in the background so the
                // channel root updates after an upgrade. Wait long enough that
                // Emby's startup metadata queue has had a chance to settle.
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(2)).ConfigureAwait(false);
                        if (string.IsNullOrWhiteSpace(Plugin.Instance?.Options.ApiKey))
                        {
                            YouTubeChannel.LogPublic("[YT] Skipping post-upgrade channel refresh; API key is not configured.");
                            return;
                        }

                        YouTubeChannel.LogPublic("[YT] Triggering post-upgrade channel refresh");
                        // Yield to any channel scan Emby is already running near
                        // startup: this refresh only needs to repopulate the
                        // wiped caches, which Emby's own scan does anyway.
                        await ChannelRefreshInvoker.TriggerRefreshAsync(skipIfScanActive: true).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        YouTubeChannel.LogPublic($"[YT] Post-upgrade refresh failed: {ex.Message}");
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] WipeCachesIfPluginUpgraded failed: {ex.Message}");
                return false;
            }
        }

        private void EnsureChannelSurfaceMigration(bool upgradeRefreshAlreadyQueued)
        {
            try
            {
                var stampPath = ChannelSurfaceStampPath;
                try
                {
                    if (File.Exists(stampPath)
                        && string.Equals(File.ReadAllText(stampPath).Trim(), ChannelSurfaceStamp, StringComparison.Ordinal))
                        return;
                }
                catch { }

                if (upgradeRefreshAlreadyQueued)
                {
                    TryWriteChannelSurfaceStamp(stampPath);
                    YouTubeChannel.LogPublic("[YT] Channel surface migration covered by post-upgrade refresh.");
                    return;
                }

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(75)).ConfigureAwait(false);
                        if (string.IsNullOrWhiteSpace(Plugin.Instance?.Options.ApiKey))
                        {
                            YouTubeChannel.LogPublic("[YT] Skipping channel surface refresh; API key is not configured.");
                            return;
                        }

                        YouTubeChannel.LogPublic("[YT] Triggering channel surface refresh");
                        await ChannelRefreshInvoker.TriggerRefreshAsync(skipIfScanActive: true).ConfigureAwait(false);
                        TryWriteChannelSurfaceStamp(stampPath);
                    }
                    catch (Exception ex)
                    {
                        YouTubeChannel.LogPublic($"[YT] Channel surface refresh failed: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] EnsureChannelSurfaceMigration failed: {ex.Message}");
            }
        }

        private static void TryWriteChannelSurfaceStamp(string stampPath)
        {
            try { File.WriteAllText(stampPath, ChannelSurfaceStamp); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Failed to write channel surface stamp: {ex.Message}"); }
        }

        private void AttachImageRepairHook()
        {
            try
            {
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

        private void OnItemUpdated(object? sender, ItemChangeEventArgs e)
        {
            var item = e.Item;
            if (item == null)
                return;

            if (string.IsNullOrEmpty(YouTubeImageProvider.TryGetVideoId(item)))
                return;

            _sortNameRepairer.Enqueue(item);

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

                    // Don't write to library.db while a channel refresh is
                    // persisting items; concurrent writers make SQLite fail with
                    // "Busy: database is locked" on macOS Emby.
                    while (ChannelRefreshInvoker.IsRefreshInProgress)
                        await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);

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

        private void QueueExistingSortNameRepair(string reason)
        {
            var libraryManager = _libraryManager;
            if (libraryManager == null)
            {
                YouTubeChannel.LogPublic($"[YT] SortName repair library scan skipped ({reason}); LibraryManager not available.");
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

                    var items = libraryManager.GetItemList(new InternalItemsQuery());
                    var queued = _sortNameRepairer.Enqueue(items);
                    YouTubeChannel.LogPublic($"[YT] SortName repair queued {queued} existing YouTube items ({reason}).");
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] SortName repair library scan failed ({reason}): {ex.Message}");
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

            await ChannelRefreshInvoker.TriggerRefreshAsync(ChannelRefreshInvoker.ContentRefreshDepth).ConfigureAwait(false);
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
                await ChannelRefreshInvoker.TriggerRefreshAsync(ChannelRefreshInvoker.ContentRefreshDepth).ConfigureAwait(false);
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
                c.ShowRootFoldersAtTopLevel ? "1" : "0",
                c.ShowTrending ? "1" : "0",
                c.ShowCategories ? "1" : "0",
                c.ShortsEnabled ? "1" : "0",
                (c.TrendingRegion ?? "").Trim(),
                (c.TrendingCategory ?? "").Trim(),
                c.ShowLikeCount ? "1" : "0",
                c.ShowCommentCount ? "1" : "0",
                (c.ChannelSortBy ?? "").Trim(),
                c.MaxChannelVideos.ToString(),
                c.MaxSearchVideos.ToString(),
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
            _resumeCheckpointSaveTimer?.Dispose();
            _sortNameRepairer.Dispose();
            SaveResumeCheckpoints();
            DashboardYouTubePlayerInterceptor.Uninstall();
            PlaybackIntentInterceptor.Uninstall();
        }
    }

    internal static class ChannelRefreshInvoker
    {
        public const int RootRefreshDepth = 1;
        public const int ContentRefreshDepth = 3;
        private static IChannelManager? _channelMgr;
        private static IChannel? _registeredChannel;
        private static int _refreshAgainRequested;
        private static int _nextRefreshDepth = RootRefreshDepth;

        // Serializes channel refreshes. Save-triggered refreshes, watch-later
        // changes and bootstrap config-hash mismatches all funnel through the
        // same lock so we never run two YouTube scans at the same time.
        private static readonly SemaphoreSlim RefreshGate = new(1, 1);

        // True while Emby's RefreshChannelContent is actively persisting channel
        // items. The sort-name and image repair queues check this so they never
        // write to library.db at the same time as the refresh; concurrent writers
        // make SQLite fail with "Busy: database is locked" on macOS Emby.
        private static int _refreshActive;

        // Monotonic stamp of the last GetChannelItems call. RefreshChannelContent
        // drives that method whether the refresh was started by the plugin or by
        // Emby's own "Refresh Internet Channels" scheduled task, so it lets the
        // repair queues also back off during Emby-initiated scans (which never
        // set _refreshActive). Treated as "scan active" for a short quiet window.
        private static long _lastChannelScanTicks;
        // Emby's deep channel scan calls GetChannelItems in bursts with long
        // quiet stretches between folders — a 78 s gap was observed mid-scan in
        // the 2026-06-01 19:46 log. The window must out-last those gaps, or the
        // gate would briefly report "quiet" mid-scan and let the repair queue /
        // a redundant plugin refresh slip in. 120 s covers the observed gaps
        // with margin; the only cost is cosmetic sort/image repairs pausing a
        // bit longer after the last scan or a UI browse.
        private const long ScanQuietWindowMs = 120000;

        // A change-driven refresh (config save / watch-later) must still be
        // applied, so instead of skipping it waits for an in-progress scan to go
        // quiet, then refreshes alone. Bounded so it can never wait forever:
        // covers Emby's observed ~3.5 min scan plus the quiet-window settle time
        // with margin, then refreshes anyway as a last resort.
        private static readonly TimeSpan MaxScanWait = TimeSpan.FromMinutes(7);
        private static readonly TimeSpan ScanWaitPollDelay = TimeSpan.FromSeconds(3);

        public static void NoteChannelScanActivity()
            => Volatile.Write(ref _lastChannelScanTicks, Environment.TickCount64);

        public static bool IsRefreshInProgress
        {
            get
            {
                if (Volatile.Read(ref _refreshActive) != 0)
                    return true;

                var last = Volatile.Read(ref _lastChannelScanTicks);
                return last != 0 && Environment.TickCount64 - last < ScanQuietWindowMs;
            }
        }

        public static void Initialize(IChannelManager channelManager)
        {
            _channelMgr = channelManager;
            _registeredChannel = null;
        }

        public static async Task TriggerRefreshAsync(int requestedDepth = RootRefreshDepth, bool skipIfScanActive = false)
        {
            RaiseNextRefreshDepth(requestedDepth);

            if (!await RefreshGate.WaitAsync(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false))
            {
                // A refresh is already in flight. Queue one follow-up pass so
                // settings saved mid-refresh are still picked up afterwards.
                Interlocked.Exchange(ref _refreshAgainRequested, 1);
                YouTubeChannel.LogPublic($"[YT] TriggerRefresh: queued follow-up depth {NormalizeRefreshDepth(requestedDepth)} (refresh already in progress)");
                return;
            }

            try
            {
                while (true)
                {
                    var refreshDepth = ConsumeNextRefreshDepth();
                    await TriggerRefreshCoreAsync(refreshDepth, skipIfScanActive).ConfigureAwait(false);

                    if (Interlocked.Exchange(ref _refreshAgainRequested, 0) != 1)
                        break;

                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: running queued follow-up");
                }
            }
            finally
            {
                RefreshGate.Release();

                if (Volatile.Read(ref _refreshAgainRequested) == 1)
                    _ = Task.Run(() => TriggerRefreshAsync());
            }
        }

        private static async Task TriggerRefreshCoreAsync(int refreshDepth, bool skipIfScanActive = false)
        {
            try
            {
                if (!EnsureChannelManager())
                {
                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: ChannelManager not ready (will retry on next poll)");
                    return;
                }

                // Emby serializes refreshes of the same channel, so if its own
                // "Refresh Internet Channels" task (or another scan) is already
                // walking the YouTube channel, launching our RefreshChannelContent
                // now makes it block behind that scan and trip the 120 s timeout
                // below — and re-run the whole YouTube API scan a second time.
                // This is exactly what happened on 2026-06-01 19:46: the
                // post-upgrade refresh fired 33 s into Emby's scheduled scan and
                // timed out at 120 s while Emby's scan finished fine on its own.
                if (IsRefreshInProgress)
                {
                    if (skipIfScanActive)
                    {
                        // Pure repopulation (post-upgrade): Emby's scan already
                        // does exactly this, so there is nothing to add — yield
                        // entirely rather than duplicate the scan.
                        YouTubeChannel.LogPublic("[YT] TriggerRefresh: skipped; a channel scan is already in progress (it will populate the channel)");
                        return;
                    }

                    // Change-driven refresh (config save / watch-later): the
                    // user's change must be applied, so don't skip. Wait for the
                    // in-progress scan to go quiet, then refresh alone — never
                    // concurrently — so the change still lands without the
                    // collision/timeout.
                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: a channel scan is in progress; waiting for it to finish before refreshing");
                    var waited = TimeSpan.Zero;
                    while (IsRefreshInProgress && waited < MaxScanWait)
                    {
                        await Task.Delay(ScanWaitPollDelay).ConfigureAwait(false);
                        waited += ScanWaitPollDelay;
                    }

                    if (IsRefreshInProgress)
                        YouTubeChannel.LogPublic($"[YT] TriggerRefresh: scan still active after {MaxScanWait.TotalSeconds:0}s; refreshing anyway");
                    else
                        YouTubeChannel.LogPublic($"[YT] TriggerRefresh: scan finished after ~{waited.TotalSeconds:0}s; refreshing now");
                }

                YouTubeChannel.LogPublic($"[YT] TriggerRefresh: invoking RefreshChannelContent on registered YouTube channel (depth {refreshDepth})");

                // Hold the gate up for the whole refresh so the repair queues
                // never write to library.db while Emby is persisting channel
                // items. The gate is cleared by a continuation on the REAL task,
                // so it survives even when the refresh runs past our 120 s
                // tracking window below. Clearing it early on the timeout would
                // let the repair queue resume while Emby is still persisting and
                // re-trigger Emby's own SaveItems "database is locked" (the empty
                // home-screen symptom) — which retry/backoff cannot rescue
                // because it is Emby's write, not ours, that fails.
                Interlocked.Exchange(ref _refreshActive, 1);
                var gateHandedOff = false;
                try
                {
                    var task = _channelMgr!.RefreshChannelContent(
                        _registeredChannel!,
                        refreshDepth,
                        null,
                        CancellationToken.None);

                    if (task == null)
                    {
                        YouTubeChannel.LogPublic("[YT] TriggerRefresh: completed (no refresh task)");
                        return;
                    }

                    // From here the continuation owns dropping the gate, when
                    // Emby actually finishes (success, fault or cancel).
                    _ = task.ContinueWith(
                        _ => Interlocked.Exchange(ref _refreshActive, 0),
                        CancellationToken.None,
                        TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                    gateHandedOff = true;

                    var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(120))).ConfigureAwait(false);
                    if (completed != task)
                    {
                        // Stop blocking the plugin's refresh loop, but leave the
                        // gate up; the continuation drops it once Emby finishes,
                        // keeping the repair queue paused until then.
                        YouTubeChannel.LogPublic("[YT] TriggerRefresh still running after 120s; gate held until Emby finishes");
                        return;
                    }

                    await task.ConfigureAwait(false);
                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: completed");
                }
                finally
                {
                    // If the gate was never handed to the continuation (null task
                    // or a synchronous throw before hand-off), drop it here so it
                    // can never stick.
                    if (!gateHandedOff)
                        Interlocked.Exchange(ref _refreshActive, 0);
                }
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] TriggerRefresh failed: {ex.Message}");
            }
        }

        private static void RaiseNextRefreshDepth(int requestedDepth)
        {
            var normalized = NormalizeRefreshDepth(requestedDepth);

            while (true)
            {
                var current = Volatile.Read(ref _nextRefreshDepth);
                if (current >= normalized)
                    return;

                if (Interlocked.CompareExchange(ref _nextRefreshDepth, normalized, current) == current)
                    return;
            }
        }

        private static int ConsumeNextRefreshDepth()
        {
            return NormalizeRefreshDepth(
                Interlocked.Exchange(ref _nextRefreshDepth, RootRefreshDepth));
        }

        private static int NormalizeRefreshDepth(int requestedDepth)
        {
            if (requestedDepth < RootRefreshDepth)
                return RootRefreshDepth;

            return Math.Min(requestedDepth, ContentRefreshDepth);
        }

        private static bool EnsureChannelManager()
        {
            if (_channelMgr != null && _registeredChannel != null)
                return true;

            if (_channelMgr == null) return false;

            _registeredChannel = FindRegisteredYouTubeChannel();
            return _registeredChannel != null;
        }

        private static IChannel? FindRegisteredYouTubeChannel()
        {
            if (_channelMgr == null)
                return null;

            try
            {
                return _channelMgr.GetChannel<YouTubeChannel>();
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] TriggerRefresh: GetChannel<YouTubeChannel> failed: {ex.Message}");
                return null;
            }
        }
    }
}
