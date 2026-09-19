using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Session;
using MediaBrowser.Common;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Events;
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
        private readonly ConcurrentDictionary<string, ResumeSeekTerminal> _resumeSeekTerminals = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, ResumeCheckpoint> _resumeCheckpoints = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, ResumeCheckpoint> _lastResumeProgressBySession = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, long> _resumeTrackingFloorsBySession = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, DateTime> _lastResumeCheckpointFlushByKey = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _lastPlaylistFingerprints = new(StringComparer.Ordinal);
        private readonly object _playlistFingerprintLock = new();
        private static readonly object PlaylistFingerprintFileLock = new();
        private readonly object _resumeCheckpointFileLock = new();
        private readonly YouTubeHomeSectionManager _homeSectionManager;
        private readonly YouTubeCaptionOffGuard _captionOffGuard;
        private readonly IUserManager _userManager;
        private readonly CancellationTokenSource _lifetimeCts = new();
        private readonly CancellationToken _lifetimeToken;
        private readonly object _resumeDispatchLifetimeGate = new();
        private readonly ManualResetEventSlim _pollIdle = new(initialState: true);
        private ILibraryManager? _libraryManager;
        private ISessionManager? _sessionManager;
        private IUserDataManager? _userDataManager;
        private Timer? _pollTimer;
        private Timer? _resumeCheckpointSaveTimer;
        private int _pollRunning;
        private int _currentPollMinutes;
        private int _resumeCheckpointsDirty;
        private int _disposed;
        private int _bootstrapHashChecked;
        private long _nextResumeSeekGeneration;
        private bool _playlistFingerprintsDirty;
        private static PluginEntryPoint? _current;
        private const long ResumeSeekMinimumTicks = TimeSpan.TicksPerSecond * 5;
        // This is a readiness signal, not a resume position. Fire TV can
        // expose a genuine content timeline around one second before it reaches
        // the five-second threshold used for durable resume/checkpoint data.
        private const long FireTvFallbackReadyTicks = TimeSpan.TicksPerSecond;
        private const long ResumeSeekEndGuardTicks = TimeSpan.TicksPerSecond * 10;
        private static readonly TimeSpan ResumeSeekDelay = TimeSpan.FromMilliseconds(1800);
        private static readonly TimeSpan ResumeSeekRetryDelay = TimeSpan.FromMilliseconds(1400);
        private static readonly TimeSpan FireTvFallbackWindow = TimeSpan.FromSeconds(90);
        private static readonly TimeSpan ResumeSeekRetiredTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan ResumeSeekRawProgressFreshness = TimeSpan.FromSeconds(12);
        // Grace window after a sent seek during which we wait for the client to
        // actually process it. Without this, a Progress event that was emitted
        // BEFORE the seek landed would falsely trigger another jump.
        private static readonly TimeSpan ResumeSeekPostSendGrace = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan FireTvManualSeekTolerance = TimeSpan.FromSeconds(8);
        private static readonly TimeSpan ResumeCheckpointFlushDelay = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan ResumeCheckpointFlushInterval = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan ResumeCheckpointTtl = TimeSpan.FromDays(180);
        private static readonly TimeSpan RecentSessionRestartWindow = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan RecentSessionCleanupInterval = TimeSpan.FromSeconds(30);
        private const int ResumeSeekMaxAttempts = 5;
        // A native Fire TV seek is disruptive when it lands late. Its primary
        // URL start remains first; this guarded fallback is intentionally a
        // single recovery attempt for one exact playback identity.
        private const int FireTvResumeSeekMaxAttempts = 1;
        private DateTime _lastRecentSessionCleanupUtc = DateTime.MinValue;

        private sealed record PendingResumeSeek(
            string Key,
            string SessionId,
            string? UserId,
            string VideoId,
            long ItemId,
            long PositionTicks,
            long RuntimeTicks,
            bool SingleSeekAttempt,
            bool IsFireTvFallback,
            bool RequiresObservedProgress,
            long Generation,
            string PlaySessionId,
            ResumeSeekDispatchGate DispatchGate,
            DateTime EarliestSeekUtc,
            DateTime ExpiresUtc,
            bool HasObservedProgress,
            long ObservedProgressTicks,
            DateTime? FirstObservedProgressUtc,
            long LastRawProgressTicks,
            DateTime? LastRawProgressUtc,
            long ProgressSequence,
            int AttemptsSent,
            DateTime? LastSeekUtc,
            bool DispatchInFlight);

        private sealed class ResumeSeekDispatchGate
        {
        }

        private sealed record ResumeSeekTerminal(string? UserId, DateTime UpdatedUtc, bool Active, string Reason);

        private sealed record ResumeCheckpoint(
            string UserId,
            string VideoId,
            long PositionTicks,
            long RuntimeTicks,
            DateTime UpdatedUtc);

        // Static so Plugin.SaveConfiguration can update the hash directly when
        // settings are saved through the UI. Otherwise the next poll would
        // re-detect the same change and trigger a duplicate refresh.
        internal static string LastConfigHash = "";

        private static string ConfigHashPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-config-hash.txt");

        private static string ResumeCheckpointPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-resume-checkpoints.json");

        private static string PlaylistFingerprintPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-playlist-fingerprints.json");

        public PluginEntryPoint(
            IApplicationHost applicationHost,
            ISessionManager sessionManager,
            ILibraryManager libraryManager,
            IUserDataManager userDataManager,
            IUserManager userManager,
            IUserViewManager userViewManager,
            IChannelManager channelManager)
        {
            Plugin.InitializeApplicationHost(applicationHost);
            _sessionManager = sessionManager;
            _libraryManager = libraryManager;
            _userDataManager = userDataManager;
            _userManager = userManager;
            _lifetimeToken = _lifetimeCts.Token;
            _homeSectionManager = new YouTubeHomeSectionManager(userManager, userViewManager);
            _captionOffGuard = new YouTubeCaptionOffGuard(sessionManager);
            ChannelRefreshInvoker.Initialize(channelManager);
        }

        public void Run()
        {
            _current = this;
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
            LoadPlaylistFingerprints();
            _sortNameRepairer.Start();
            AttachImageRepairHook();
            QueueExistingSortNameRepair("startup");
            AttachResumeSeekHook();
            _captionOffGuard.Start();

            try
            {
                if (File.Exists(ConfigHashPath))
                    LastConfigHash = File.ReadAllText(ConfigHashPath).Trim();
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to read config hash: {ex.Message}");
            }

            _currentPollMinutes = Math.Clamp(
                Plugin.Instance?.Options.WatchLaterPollMinutes ?? 3,
                1,
                60);
            _pollTimer = new Timer(
                PollTick,
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMinutes(_currentPollMinutes));

            _userManager.UserCreated += OnHomeSectionUserChanged;
            _userManager.UserPolicyUpdated += OnHomeSectionUserChanged;
            QueueHomeSectionSync("plugin startup", TimeSpan.FromSeconds(15));
        }

        private void OnHomeSectionUserChanged(
            object? sender,
            GenericEventArgs<User> eventArgs)
        {
            // UserPolicyUpdated is raised before Emby finishes persisting its
            // item-share rows. A short delay makes GetUserViews observe the new
            // channel access instead of immediately reading the old shares.
            QueueHomeSectionSync("user access changed", TimeSpan.FromSeconds(2));
        }

        internal static void RequestHomeSectionSync(
            string reason,
            TimeSpan? delay = null)
        {
            _current?.QueueHomeSectionSync(reason, delay ?? TimeSpan.Zero);
        }

        private void QueueHomeSectionSync(string reason, TimeSpan delay)
        {
            var cancellationToken = _lifetimeCts.Token;
            _ = Task.Run(async () =>
            {
                try
                {
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

                    await _homeSectionManager.SyncAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Normal during plugin unload.
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic(
                        $"[YT] Home row sync ({reason}) failed: {ex.Message}");
                }
            });
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
            lock (_resumeDispatchLifetimeGate)
            {
                if (!IsDisposingOrDisposed())
                    OnPlaybackStartCore(sender, e);
            }
        }

        private void OnPlaybackStartCore(object? sender, PlaybackProgressEventArgs e)
        {
            try
            {
                if (_sessionManager == null)
                    return;

                var item = e.Item;
                var session = e.Session;
                if (item == null || session == null || string.IsNullOrEmpty(session.Id))
                    return;

                // Reserve every exact playback identity, including local and
                // live items. That active replacement is what makes delayed
                // Progress/Stop callbacks from the prior YouTube playback
                // harmless; only eligible YouTube VOD continues to scheduling.
                var key = GetResumeSeekKey(e, item);
                if (!TryBeginResumePlayback(key, session))
                    return;

                var videoId = YouTubeImageProvider.TryGetVideoId(item);
                if (string.IsNullOrEmpty(videoId))
                    return;

                // A live/upcoming YouTube item has no stable timeline to resume.
                // Seeking it can jump behind the live edge and trigger reloads on
                // resource-constrained TV clients, so consume any captured intent
                // and keep both plugin and native resume handling out of this path.
                if (IsYouTubeLiveItem(item))
                {
                    PlaybackIntentInterceptor.TryConsume(
                        session.UserId,
                        item.InternalId,
                        session.DeviceId,
                        out _);
                    RemoveResumeCheckpoint(session.UserId, videoId);
                    YouTubeChannel.LogPublic($"[YT] Resume disabled for live YouTube video {videoId}.");
                    return;
                }

                if (string.IsNullOrEmpty(e.PlaySessionId) || string.IsNullOrEmpty(session.UserId))
                {
                    YouTubeChannel.LogPublic("[YT] Resume seek skipped; playback session or user identity is missing.");
                    return;
                }

                var runtimeTicks = item.RunTimeTicks.GetValueOrDefault();
                long positionTicks;
                var isFireTvOrAndroidTv = IsLikelyFireTvOrAndroidTvSession(session);

                if (PlaybackIntentInterceptor.TryConsumeForResume(
                        session.UserId,
                        item.InternalId,
                        session.DeviceId,
                        out var intent,
                        out var ambiguousIntent))
                {
                    positionTicks = intent.StartTimeTicks;

                    if (positionTicks < ResumeSeekMinimumTicks)
                    {
                        // A captured StartTimeTicks=0 is the client's explicit
                        // play-from-beginning request for every client type.
                        // A mere recent-session timestamp cannot prove a
                        // reconnect strongly enough to override that choice.
                        RemoveResumeCheckpoint(session.UserId, videoId);
                        YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {videoId}; play from beginning was requested.");
                        return;
                    }
                }
                else if (ambiguousIntent)
                {
                    YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {videoId}; multiple device intents were ambiguous.");
                    return;
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

                // Protect the requested resume point even on TV clients where
                // server-side seek commands are intentionally suppressed. A
                // Fire TV that briefly reports zero/the beginning while its
                // YouTube player consumes URL start= must not overwrite a good
                // checkpoint and break the next resume attempt.
                _resumeTrackingFloorsBySession[session.Id] = Math.Max(
                    ResumeSeekMinimumTicks,
                    positionTicks - ResumeSeekEndGuardTicks);

                // LG/webOS players remain URL/player-start only. Fire TV and
                // Android TV get one guarded fallback below, but only after
                // fresh playback progress proves that the native URL start
                // did not take effect.
                if (ShouldSuppressServerSideResumeSeek(session))
                {
                    YouTubeChannel.LogPublic($"[YT] Resume seek skipped for {videoId}; TV player will use the URL/player start only.");
                    return;
                }

                var generation = Interlocked.Increment(ref _nextResumeSeekGeneration);
                var now = DateTime.UtcNow;
                var pending = new PendingResumeSeek(
                    key,
                    session.Id,
                    session.UserId,
                    videoId,
                    item.InternalId,
                    positionTicks,
                    runtimeTicks,
                    // Every client gets one raw-progress-confirmed fallback.
                    !isFireTvOrAndroidTv,
                    isFireTvOrAndroidTv,
                    true,
                    generation,
                    e.PlaySessionId ?? string.Empty,
                    new ResumeSeekDispatchGate(),
                    now + ResumeSeekDelay,
                    now + FireTvFallbackWindow,
                    false,
                    0,
                    null,
                    0,
                    null,
                    0,
                    0,
                    null,
                    false);

                lock (_resumeDispatchLifetimeGate)
                {
                    // PlaybackStart can race a matching Stop or a replacement
                    // Start after TryBegin reserved this identity. Do not let
                    // that stale Start insert a pending seek afterwards.
                    if (!IsResumePlaybackReservationCurrent(key, session.UserId)
                        || _pendingResumeSeeks.ContainsKey(key))
                    {
                        return;
                    }

                    _pendingResumeSeeks[key] = pending;
                }

                SchedulePendingResumeSeek(key, generation, ResumeSeekDelay, "delayed start");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackStart resume hook failed: {ex.Message}");
            }
        }

        private void OnPlaybackStopped(object? sender, PlaybackStopEventArgs e)
        {
            lock (_resumeDispatchLifetimeGate)
            {
                if (!IsDisposingOrDisposed())
                    OnPlaybackStoppedCore(sender, e);
            }
        }

        private void OnPlaybackStoppedCore(object? sender, PlaybackStopEventArgs e)
        {
            var item = e.Item;
            var session = e.Session;
            var sessionId = session?.Id;
            var videoId = item == null ? null : YouTubeImageProvider.TryGetVideoId(item);
            var stopKey = item == null || session == null
                ? null
                : GetResumeSeekKey(session.Id, session.UserId, e.PlaySessionId, item.InternalId);
            var isStaleStop = !string.IsNullOrEmpty(sessionId)
                              && !string.IsNullOrEmpty(stopKey)
                              && HasOtherActiveResumePlayback(sessionId, stopKey);

            try
            {
                if (isStaleStop)
                    return;

                if (item != null && !string.IsNullOrEmpty(videoId))
                {
                    if (IsYouTubeLiveItem(item))
                    {
                        // Never recreate a live-stream checkpoint on Stop after
                        // Start/Progress deliberately removed it.
                        RemoveResumeCheckpoint(session?.UserId, videoId);
                    }
                    else if (e.PlayedToCompletion)
                    {
                        RemoveResumeCheckpoint(session?.UserId, videoId);
                    }
                    else if (!HasPendingObservedResumeSeek(sessionId, item.InternalId))
                    {
                        var stopPositionTicks = GetPlaybackPositionTicks(e.PlaybackPositionTicks, session);
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
                    else
                    {
                        // Emby itself can persist a beginning/ad report while
                        // the fallback is pending. Reassert the original
                        // checkpoint, but never track that unconfirmed value.
                        RestoreProtectedNativeResumePosition(item, session, videoId);
                    }

                    SaveResumeCheckpoints();
                }
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] PlaybackStopped resume tracking failed: {ex.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    if (!string.IsNullOrEmpty(stopKey))
                    {
                        if (_pendingResumeSeeks.TryGetValue(stopKey, out var pending))
                            TerminalizePendingResumeSeek(stopKey, pending, "stopped");
                        RetireResumeSeekTerminal(stopKey, session?.UserId, "stopped");
                    }
                    // A clean Stop ends the "is this a network restart?" window.
                    // Without this, an explicit replay within RecentSessionRestartWindow
                    // would be misinterpreted as a reconnect and skip back.
                    var preserveReplacementState = isStaleStop
                        || !string.IsNullOrEmpty(stopKey)
                           && HasOtherActiveResumePlayback(sessionId, stopKey);
                    if (!preserveReplacementState)
                    {
                        _lastResumeProgressBySession.TryRemove(sessionId, out _);
                        if (!_pendingResumeSeeks.Values.Any(p =>
                            string.Equals(p.SessionId, sessionId, StringComparison.Ordinal)))
                        {
                            _resumeTrackingFloorsBySession.TryRemove(sessionId, out _);
                        }
                    }
                    CleanupRecentSessionCheckpoints();
                }
            }
        }

        private void CancelPendingResumeSeeksForSession(string sessionId, string? exceptKey = null)
        {
            foreach (var kvp in _pendingResumeSeeks)
            {
                if (string.Equals(kvp.Value.SessionId, sessionId, StringComparison.Ordinal)
                    && !string.Equals(kvp.Key, exceptKey, StringComparison.Ordinal))
                {
                    TerminalizePendingResumeSeek(kvp.Key, kvp.Value, "replaced playback");
                }
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

        private bool TryBeginResumePlayback(string key, SessionInfo session)
        {
            lock (_resumeDispatchLifetimeGate)
            {
                CleanupResumeSeekTerminals();
                if (HasResumeSeekTerminal(key, session.UserId))
                {
                    YouTubeChannel.LogPublic("[YT] Resume seek skipped; this playback identity already reached a terminal state.");
                    return false;
                }

                if (_pendingResumeSeeks.ContainsKey(key))
                {
                    YouTubeChannel.LogPublic("[YT] Resume seek skipped; duplicate PlaybackStart retained the existing pending playback.");
                    return false;
                }

                CancelPendingResumeSeeksForSession(session.Id, key);
                RetireOtherResumeSeekTerminals(session.Id, key, "new playback");
                _resumeTrackingFloorsBySession.TryRemove(session.Id, out _);
                _resumeSeekTerminals[key] = new ResumeSeekTerminal(
                    session.UserId,
                    DateTime.UtcNow,
                    Active: true,
                    Reason: "playback started");
                return true;
            }
        }

        private bool HasResumeSeekTerminal(string key, string? userId) =>
            _resumeSeekTerminals.TryGetValue(key, out var terminal)
            && string.Equals(terminal.UserId, userId, StringComparison.Ordinal);

        private bool IsResumePlaybackReservationCurrent(string key, string? userId) =>
            _resumeSeekTerminals.TryGetValue(key, out var terminal)
            && terminal.Active
            && string.Equals(terminal.UserId, userId, StringComparison.Ordinal);

        private bool HasOtherActiveResumePlayback(string sessionId, string key)
        {
            var prefix = sessionId + "|";
            return _resumeSeekTerminals.Any(terminal =>
                terminal.Value.Active
                && terminal.Key.StartsWith(prefix, StringComparison.Ordinal)
                && !string.Equals(terminal.Key, key, StringComparison.Ordinal));
        }

        private void TerminalizePendingResumeSeek(
            string key,
            PendingResumeSeek pending,
            string reason)
        {
            lock (_resumeDispatchLifetimeGate)
            {
                lock (pending.DispatchGate)
                {
                    if (!_pendingResumeSeeks.TryGetValue(key, out var current)
                        || current.Generation != pending.Generation
                        || !ReferenceEquals(current.DispatchGate, pending.DispatchGate))
                    {
                        return;
                    }

                    _resumeSeekTerminals[key] = new ResumeSeekTerminal(
                        pending.UserId,
                        DateTime.UtcNow,
                        Active: true,
                        reason);
                    RemovePendingResumeSeek(key, current);
                }
            }
        }

        private void RetireResumeSeekTerminal(string key, string? userId, string reason)
        {
            if (_resumeSeekTerminals.TryGetValue(key, out var current)
                && string.Equals(current.UserId, userId, StringComparison.Ordinal))
            {
                _resumeSeekTerminals.TryUpdate(
                    key,
                    current with { UpdatedUtc = DateTime.UtcNow, Active = false, Reason = reason },
                    current);
            }
        }

        private void RetireOtherResumeSeekTerminals(string sessionId, string exceptKey, string reason)
        {
            var prefix = sessionId + "|";
            foreach (var terminal in _resumeSeekTerminals)
            {
                if (terminal.Key.StartsWith(prefix, StringComparison.Ordinal)
                    && !string.Equals(terminal.Key, exceptKey, StringComparison.Ordinal))
                {
                    RetireResumeSeekTerminal(terminal.Key, terminal.Value.UserId, reason);
                }
            }
        }

        private void CleanupResumeSeekTerminals()
        {
            var now = DateTime.UtcNow;
            HashSet<string>? liveSessionIds = null;
            try
            {
                if (_sessionManager != null)
                {
                    liveSessionIds = _sessionManager.Sessions
                        .Where(session => !string.IsNullOrEmpty(session.Id))
                        .Select(session => session.Id)
                        .ToHashSet(StringComparer.Ordinal);
                }
            }
            catch
            {
                // Do not retire active playback identities when the session
                // snapshot itself is unavailable.
            }

            // A fallback that never receives another Progress callback still
            // needs bounded housekeeping. Preserve its terminal latch for a
            // live playback, but let the terminal pass below retire it when
            // the owning session has disappeared.
            foreach (var pendingEntry in _pendingResumeSeeks)
            {
                var pending = pendingEntry.Value;
                var sessionDisappeared = liveSessionIds != null
                    && !liveSessionIds.Contains(pending.SessionId);
                if (now >= pending.ExpiresUtc || sessionDisappeared)
                {
                    TerminalizePendingResumeSeek(
                        pendingEntry.Key,
                        pending,
                        sessionDisappeared ? "session disappeared" : "fallback window expired");
                }
            }

            foreach (var terminal in _resumeSeekTerminals)
            {
                var sessionIdEnd = terminal.Key.IndexOf('|');
                var terminalSessionId = sessionIdEnd >= 0
                    ? terminal.Key.Substring(0, sessionIdEnd)
                    : string.Empty;
                if (terminal.Value.Active
                    && liveSessionIds != null
                    && !liveSessionIds.Contains(terminalSessionId))
                {
                    _resumeSeekTerminals.TryUpdate(
                        terminal.Key,
                        terminal.Value with
                        {
                            UpdatedUtc = now,
                            Active = false,
                            Reason = "session disappeared"
                        },
                        terminal.Value);
                    continue;
                }

                if (!terminal.Value.Active
                    && now - terminal.Value.UpdatedUtc >= ResumeSeekRetiredTtl)
                    _resumeSeekTerminals.TryRemove(terminal.Key, out _);
            }
        }

        private void OnPlaybackProgress(object? sender, PlaybackProgressEventArgs e)
        {
            lock (_resumeDispatchLifetimeGate)
            {
                if (!IsDisposingOrDisposed())
                    OnPlaybackProgressCore(sender, e);
            }
        }

        private void OnPlaybackProgressCore(object? sender, PlaybackProgressEventArgs e)
        {
            try
            {
                var item = e.Item;
                var session = e.Session;
                if (item == null)
                    return;

                var videoId = YouTubeImageProvider.TryGetVideoId(item);
                var key = GetResumeSeekKey(e, item);
                if (session != null
                    && !string.IsNullOrEmpty(session.Id)
                    && HasOtherActiveResumePlayback(session.Id, key))
                {
                    // An out-of-order progress event from the old play session
                    // must not write its position over the current playback.
                    return;
                }
                var hasPendingSeek = _pendingResumeSeeks.TryGetValue(key, out var pending);
                var currentTicks = GetPlaybackPositionTicks(e.PlaybackPositionTicks, session);
                var now = DateTime.UtcNow;

                if (hasPendingSeek && pending != null && IsPlaybackPaused(e, session))
                {
                    // A deliberate pause is not a failed native resume. Do not
                    // let a delayed task seek after the user has paused.
                    TerminalizePendingResumeSeek(key, pending, "paused");
                    _resumeTrackingFloorsBySession.TryRemove(pending.SessionId, out _);
                    hasPendingSeek = false;
                    pending = null;
                }

                if (!string.IsNullOrEmpty(videoId))
                {
                    if (IsYouTubeLiveItem(item))
                    {
                        RemoveResumeCheckpoint(session?.UserId, videoId);
                        if (hasPendingSeek && pending != null)
                        {
                            RemovePendingResumeSeek(key, pending);
                        }
                        return;
                    }
                }

                if (hasPendingSeek && pending != null)
                {
                    var rawProgressTicks = e.PlaybackPositionTicks;
                    var confirmed = pending.RequiresObservedProgress
                        ? IsFireTvRawPositionConfirmed(pending, rawProgressTicks)
                        : currentTicks >= ResumeSeekMinimumTicks
                          && currentTicks >= pending.PositionTicks - ResumeSeekEndGuardTicks
                          && (!pending.RequiresObservedProgress
                              || rawProgressTicks.HasValue
                                 && rawProgressTicks.Value >= ResumeSeekMinimumTicks
                                 && rawProgressTicks.Value >= pending.PositionTicks - ResumeSeekEndGuardTicks);
                    if (confirmed)
                    {
                        // Confirmation is deliberately based on the raw event,
                        // never on a stale session-level position. Only this
                        // proof releases the protected checkpoint normally.
                        TerminalizePendingResumeSeek(key, pending, "confirmed");
                        _resumeTrackingFloorsBySession.TryRemove(pending.SessionId, out _);
                        YouTubeChannel.LogPublic($"[YT] Resume seek confirmed for {pending.VideoId} at {(pending.IsFireTvFallback ? "raw" : "reported")} position {(pending.IsFireTvFallback ? rawProgressTicks!.Value : currentTicks)} ticks.");
                        hasPendingSeek = false;
                        pending = null;
                    }
                    else if (pending.RequiresObservedProgress)
                    {
                        if (IsReportedManualSeek(e, out var seeksToBeginning))
                        {
                            if (seeksToBeginning)
                                currentTicks = 0;
                            CancelPendingResumeSeekForManualPosition(
                                key,
                                pending,
                                "reported seek event",
                                clearResumeCheckpoint: seeksToBeginning);
                            hasPendingSeek = false;
                            pending = null;
                        }
                        else
                        {
                            var observed = ObserveFireTvFallbackProgress(
                                pending,
                                rawProgressTicks,
                                now,
                                out var manualDiscontinuity);
                            if (manualDiscontinuity)
                            {
                                if (rawProgressTicks == 0)
                                    currentTicks = 0;
                                CancelPendingResumeSeekForManualPosition(key, pending, "progress discontinuity");
                                hasPendingSeek = false;
                                pending = null;
                            }
                            else if (TryUpdatePendingResumeSeek(key, pending, observed))
                            {
                                pending = observed;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }

                // While a raw-progress fallback is pending, retain the prior checkpoint
                // rather than saving a beginning/ad position as a fabricated
                // successful resume. Emby also updates user data independently,
                // so reassert the protected point without tracking raw progress.
                if (!hasPendingSeek || pending == null || !pending.RequiresObservedProgress)
                {
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
                }
                else if (!string.IsNullOrEmpty(videoId))
                {
                    RestoreProtectedNativeResumePosition(item, session, videoId);
                }

                if (!hasPendingSeek || pending == null)
                    return;

                // This merely gives the guarded dispatcher an opportunity. Its
                // observation/attempt checks reject stale data and timer-only
                // retries, so no command is sent without fresh raw progress.
                SchedulePendingResumeSeek(key, pending.Generation, TimeSpan.Zero, "progress");
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
            var userId = e.Session?.UserId ?? string.Empty;
            var playSessionId = e.PlaySessionId ?? string.Empty;
            return GetResumeSeekKey(sessionId, userId, playSessionId, item.InternalId);
        }

        private static string GetResumeSeekKey(
            string? sessionId,
            string? userId,
            string? playSessionId,
            long itemId) =>
            $"{sessionId ?? string.Empty}|{userId ?? string.Empty}|{playSessionId ?? string.Empty}|{itemId}";

        private long GetPlaybackPositionTicks(long? eventPositionTicks, object? session)
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

            return 0;
        }

        private static bool HasUsableFireTvFallbackProgress(
            PendingResumeSeek pending,
            long rawProgressTicks)
        {
            // A real VOD timeline keeps a fallback out of live/unknown-length
            // sources. A positive raw progress report avoids treating a stale
            // session-level value as evidence that the native player is ready.
            return pending.RuntimeTicks > 0
                   && pending.RuntimeTicks - pending.PositionTicks >= ResumeSeekEndGuardTicks
                   && rawProgressTicks >= MinimumRawProgressForFallback(pending)
                   && rawProgressTicks < pending.PositionTicks - ResumeSeekEndGuardTicks
                   && rawProgressTicks < pending.RuntimeTicks - ResumeSeekEndGuardTicks;
        }

        private static long MinimumRawProgressForFallback(PendingResumeSeek pending) =>
            pending.IsFireTvFallback
                ? FireTvFallbackReadyTicks
                : ResumeSeekMinimumTicks;

        // Pure progress bookkeeping: callers supply the clock so the eligibility
        // rules can be checked without a timer or session-position inference.
        private static PendingResumeSeek ObserveFireTvFallbackProgress(
            PendingResumeSeek pending,
            long? rawProgressTicks,
            DateTime now,
            out bool manualDiscontinuity)
        {
            manualDiscontinuity = false;
            if (!rawProgressTicks.HasValue)
            {
                return pending with
                {
                    HasObservedProgress = false,
                    ObservedProgressTicks = 0,
                    FirstObservedProgressUtc = null
                };
            }

            var raw = rawProgressTicks.Value;
            if (raw <= 0)
            {
                // A zero report after a dispatched fallback is ambiguous:
                // it can be a user rewind or a native-player/ad reset. Stop
                // automatic seeks either way, but let the caller preserve
                // the known-good checkpoint unless the event explicitly
                // identifies a seek to the beginning.
                manualDiscontinuity = raw == 0
                                      && pending.AttemptsSent > 0
                                      && pending.LastRawProgressTicks > 0;
                return pending with
                {
                    HasObservedProgress = false,
                    ObservedProgressTicks = 0,
                    FirstObservedProgressUtc = null
                };
            }

            if (!HasUsableFireTvFallbackProgress(pending, raw))
            {
                return pending with
                {
                    HasObservedProgress = false,
                    ObservedProgressTicks = 0,
                    FirstObservedProgressUtc = null
                };
            }

            if (pending.LastRawProgressTicks > 0 && pending.LastRawProgressUtc is { } previousUtc)
            {
                if (raw < pending.LastRawProgressTicks)
                {
                    // Before dispatch, a backwards report only revokes
                    // readiness. Afterwards it is safer to preserve a user
                    // chosen position than to seek over it.
                    manualDiscontinuity = pending.AttemptsSent > 0;
                    return pending with
                    {
                        HasObservedProgress = false,
                        ObservedProgressTicks = 0,
                        FirstObservedProgressUtc = null,
                        LastRawProgressTicks = raw,
                        LastRawProgressUtc = now
                    };
                }

                var elapsedTicks = Math.Max(0, (now - previousUtc).Ticks);
                var progressDelta = raw - pending.LastRawProgressTicks;
                var isNearIntendedPosition = raw >= pending.PositionTicks - ResumeSeekEndGuardTicks;
                if (!isNearIntendedPosition
                    && progressDelta > elapsedTicks + FireTvManualSeekTolerance.Ticks)
                {
                    manualDiscontinuity = true;
                    return pending;
                }
            }

            if (pending.LastRawProgressTicks <= 0)
            {
                return pending with
                {
                    HasObservedProgress = true,
                    ObservedProgressTicks = raw,
                    FirstObservedProgressUtc = now,
                    LastRawProgressTicks = raw,
                    LastRawProgressUtc = now,
                    ProgressSequence = pending.ProgressSequence + 1
                };
            }

            if (raw <= pending.LastRawProgressTicks)
            {
                return pending with
                {
                    ObservedProgressTicks = raw,
                    LastRawProgressUtc = now
                };
            }

            var firstObservedUtc = pending.FirstObservedProgressUtc ?? now;
            var progressSequence = pending.ProgressSequence + 1;
            return pending with
            {
                HasObservedProgress = true,
                ObservedProgressTicks = raw,
                FirstObservedProgressUtc = firstObservedUtc,
                LastRawProgressTicks = raw,
                LastRawProgressUtc = now,
                ProgressSequence = progressSequence
            };
        }

        private static bool CanDispatchFireTvFallback(
            PendingResumeSeek pending,
            DateTime now)
        {
            if (!pending.IsFireTvFallback
                || pending.DispatchInFlight
                || now >= pending.ExpiresUtc
                || pending.AttemptsSent >= FireTvResumeSeekMaxAttempts
                || !pending.HasObservedProgress
                || pending.ObservedProgressTicks < FireTvFallbackReadyTicks
                || pending.LastRawProgressUtc is not { } lastRawProgressUtc
                || now - lastRawProgressUtc > ResumeSeekRawProgressFreshness
                || now < pending.EarliestSeekUtc)
            {
                return false;
            }

            return pending.AttemptsSent == 0;
        }

        private static bool IsFireTvRawPositionConfirmed(
            PendingResumeSeek pending,
            long? rawProgressTicks) =>
            rawProgressTicks.HasValue
            && rawProgressTicks.Value >= ResumeSeekMinimumTicks
            && rawProgressTicks.Value >= pending.PositionTicks - ResumeSeekEndGuardTicks;

        private static bool IsReportedManualSeek(
            PlaybackProgressEventArgs e,
            out bool seeksToBeginning)
        {
            var seekPosition = GetLongProperty(e, "SeekPositionTicks");
            seeksToBeginning = seekPosition.HasValue
                               && seekPosition.Value < ResumeSeekMinimumTicks;
            return seekPosition.HasValue;
        }

        private static long? GetLongProperty(object? source, string name)
        {
            try
            {
                var value = source?.GetType()
                    .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                    ?.GetValue(source);
                return value switch
                {
                    long longValue => longValue,
                    int intValue => intValue,
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        private void CancelPendingResumeSeekForManualPosition(
            string key,
            PendingResumeSeek pending,
            string reason,
            bool clearResumeCheckpoint = false)
        {
            TerminalizePendingResumeSeek(key, pending, reason);
            _resumeTrackingFloorsBySession.TryRemove(pending.SessionId, out _);
            if (clearResumeCheckpoint)
                RemoveResumeCheckpoint(pending.UserId, pending.VideoId);
            YouTubeChannel.LogPublic($"[YT] Resume seek cancelled for {pending.VideoId}; {reason} keeps the client position authoritative.");
        }

        private bool HasPendingObservedResumeSeek(string? sessionId, long itemId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return false;

            return _pendingResumeSeeks.Values.Any(pending =>
                pending.RequiresObservedProgress
                && pending.ItemId == itemId
                && string.Equals(pending.SessionId, sessionId, StringComparison.Ordinal));
        }

        private static bool IsPlaybackPaused(PlaybackProgressEventArgs e, SessionInfo? session)
        {
            if (GetBooleanProperty(e, "IsPaused"))
                return true;

            var playState = session?.GetType()
                .GetProperty("PlayState", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(session);
            return GetBooleanProperty(playState, "IsPaused");
        }

        private static bool GetBooleanProperty(object? source, string name)
        {
            try
            {
                return source?.GetType()
                    .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                    ?.GetValue(source) is bool value
                    && value;
            }
            catch
            {
                return false;
            }
        }

        private bool IsDisposingOrDisposed() =>
            Volatile.Read(ref _disposed) != 0 || _lifetimeToken.IsCancellationRequested;

        private void SchedulePendingResumeSeek(string key, long generation, TimeSpan delay, string reason)
        {
            var cancellationToken = _lifetimeToken;
            _ = Task.Run(async () =>
            {
                try
                {
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

                    if (IsDisposingOrDisposed())
                        return;

                    await TrySendPendingResumeSeekAsync(key, generation, reason).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Plugin unload cancels delayed fallback work.
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Resume seek retry task failed: {ex.Message}");
                }
            });
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

        private async Task TrySendPendingResumeSeekAsync(string key, long generation, string reason)
        {
            if (_sessionManager == null || IsDisposingOrDisposed())
                return;

            if (!_pendingResumeSeeks.TryGetValue(key, out var pending)
                || pending.Generation != generation)
                return;

            var dispatchTask = TryReserveAndBeginResumeSeek(key, generation, pending, out var reserved);
            if (dispatchTask == null || reserved == null)
                return;

            try
            {
                await dispatchTask.ConfigureAwait(false);

                if (IsDisposingOrDisposed()
                    || !_pendingResumeSeeks.TryGetValue(key, out var current)
                    || current.Generation != reserved.Generation
                    || !ReferenceEquals(current.DispatchGate, reserved.DispatchGate))
                {
                    return;
                }

                YouTubeChannel.LogPublic($"[YT] Resume seek dispatched ({reason}, attempt {reserved.AttemptsSent}) for {reserved.VideoId} to {reserved.PositionTicks} ticks.");
            }
            catch (OperationCanceledException) when (IsDisposingOrDisposed())
            {
                // The plugin is unloading; do not report a cancelled command
                // as a failed resume attempt.
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Resume seek failed for {reserved.VideoId}: {ex.Message}");
            }
            finally
            {
                MarkResumeSeekDispatchComplete(key, reserved);
            }
        }

        private Task? TryReserveAndBeginResumeSeek(
            string key,
            long generation,
            PendingResumeSeek expected,
            out PendingResumeSeek? reserved)
        {
            reserved = null;
            lock (_resumeDispatchLifetimeGate)
            {
                lock (expected.DispatchGate)
                {
                    if (IsDisposingOrDisposed()
                        || !_pendingResumeSeeks.TryGetValue(key, out var pending)
                        || pending.Generation != generation
                        || !ReferenceEquals(pending.DispatchGate, expected.DispatchGate))
                        return null;

                    var now = DateTime.UtcNow;
                    if (!IsPendingSeekStillCurrent(pending))
                    {
                        TerminalizePendingResumeSeek(key, pending, "playback identity changed");
                        return null;
                    }

                    if (now >= pending.ExpiresUtc)
                    {
                        TerminalizePendingResumeSeek(key, pending, "fallback window expired");
                        YouTubeChannel.LogPublic($"[YT] Resume seek fallback window expired for {pending.VideoId} after {pending.AttemptsSent} dispatched attempts.");
                        return null;
                    }

                    // A scheduled continuation is only a chance to examine
                    // recent, raw client progress. It must never turn an old
                    // observation (or a still-running send) into a new seek.
                    if (pending.DispatchInFlight
                        || pending.RequiresObservedProgress
                           && (!pending.HasObservedProgress
                               || pending.ObservedProgressTicks < MinimumRawProgressForFallback(pending)
                               || pending.LastRawProgressUtc is not { } lastRawProgressUtc
                               || now - lastRawProgressUtc > ResumeSeekRawProgressFreshness
                               || now < pending.EarliestSeekUtc))
                    {
                        return null;
                    }

                    if (pending.IsFireTvFallback && !CanDispatchFireTvFallback(pending, now))
                        return null;

                    if (!pending.IsFireTvFallback
                        && pending.LastSeekUtc is { } lastSeekUtc
                        && now - lastSeekUtc < ResumeSeekPostSendGrace)
                        return null;

                    // SendMessageToSession can complete even when the playback
                    // client has no controller. Resolve an exact active route
                    // before spending a bounded fallback attempt.
                    if (!YouTubeSessionCommandRouter.TryResolve(
                            _sessionManager!,
                            pending.SessionId,
                            pending.UserId,
                            pending.ItemId,
                            "Playstate",
                            out var commandSessionId))
                    {
                        return null;
                    }

                    var inFlightKey = GetResumeSeekInFlightKey(pending);
                    if (!TryClaimResumeSeekAttempt(inFlightKey, now))
                        return null;

                    if (!pending.RequiresObservedProgress
                        && TryGetSessionPositionTicks(pending.SessionId, out var currentTicks)
                        && currentTicks >= ResumeSeekMinimumTicks
                        && currentTicks >= pending.PositionTicks - ResumeSeekEndGuardTicks)
                    {
                        RemovePendingResumeSeek(key, pending);
                        _resumeTrackingFloorsBySession.TryRemove(pending.SessionId, out _);
                        return null;
                    }

                    var maxAttempts = pending.IsFireTvFallback
                        ? FireTvResumeSeekMaxAttempts
                        : pending.SingleSeekAttempt ? 1 : ResumeSeekMaxAttempts;
                    if (pending.AttemptsSent >= maxAttempts)
                    {
                        TerminalizePendingResumeSeek(key, pending, "attempt budget exhausted");
                        YouTubeChannel.LogPublic($"[YT] Resume seek gave up for {pending.VideoId}; player stayed before the resume point after {pending.AttemptsSent} attempts.");
                        return null;
                    }

                    // Count immediately before the synchronous command
                    // invocation. Progress updates share this gate, so they
                    // cannot consume the sole attempt between validation and
                    // dispatch; they may still update freely before this point.
                    reserved = pending with
                    {
                        AttemptsSent = pending.AttemptsSent + 1,
                        LastSeekUtc = now,
                        DispatchInFlight = true
                    };
                    if (!_pendingResumeSeeks.TryUpdate(key, reserved, pending))
                    {
                        return null;
                    }

                    if (IsDisposingOrDisposed() || !IsPendingSeekStillCurrent(reserved))
                    {
                        RemovePendingResumeSeek(key, reserved);
                        return null;
                    }

                    YouTubeChannel.LogPublic($"[YT] Resume seek transport routedToCompanion={!string.Equals(commandSessionId, pending.SessionId, StringComparison.Ordinal)}.");
                    return SendSeekCommandAsync(reserved, commandSessionId);
                }
            }
        }

        private static string GetResumeSeekInFlightKey(PendingResumeSeek pending) =>
            $"{pending.Key}|{pending.Generation}|{pending.PositionTicks}";

        private void RemovePendingResumeSeek(string key, PendingResumeSeek pending)
        {
            lock (pending.DispatchGate)
            {
                if (_pendingResumeSeeks.TryGetValue(key, out var current)
                    && current.Generation == pending.Generation
                    && ReferenceEquals(current.DispatchGate, pending.DispatchGate)
                    && _pendingResumeSeeks.TryRemove(
                        new KeyValuePair<string, PendingResumeSeek>(key, current)))
                {
                    _resumeSeeksInFlight.TryRemove(GetResumeSeekInFlightKey(pending), out _);
                }
            }
        }

        private void MarkResumeSeekDispatchComplete(string key, PendingResumeSeek reserved)
        {
            lock (reserved.DispatchGate)
            {
                if (_pendingResumeSeeks.TryGetValue(key, out var current)
                    && current.Generation == reserved.Generation
                    && ReferenceEquals(current.DispatchGate, reserved.DispatchGate)
                    && current.DispatchInFlight)
                {
                    _pendingResumeSeeks.TryUpdate(
                        key,
                        current with { DispatchInFlight = false },
                        current);
                }
            }
        }

        private bool TryUpdatePendingResumeSeek(
            string key,
            PendingResumeSeek expected,
            PendingResumeSeek updated)
        {
            lock (expected.DispatchGate)
            {
                if (!_pendingResumeSeeks.TryGetValue(key, out var current)
                    || current.Generation != expected.Generation
                    || !ReferenceEquals(current.DispatchGate, expected.DispatchGate)
                    || !Equals(current, expected))
                {
                    return false;
                }

                return _pendingResumeSeeks.TryUpdate(key, updated, current);
            }
        }

        private bool TryClaimResumeSeekAttempt(string inFlightKey, DateTime now)
        {
            while (true)
            {
                if (_resumeSeeksInFlight.TryGetValue(inFlightKey, out var lastAttempt))
                {
                    if (now - lastAttempt < ResumeSeekRetryDelay)
                        return false;

                    if (_resumeSeeksInFlight.TryUpdate(inFlightKey, now, lastAttempt))
                        return true;

                    continue;
                }

                if (_resumeSeeksInFlight.TryAdd(inFlightKey, now))
                    return true;
            }
        }

        private async Task SendSeekCommandAsync(PendingResumeSeek pending, string commandSessionId)
        {
            if (_sessionManager == null || IsDisposingOrDisposed())
                return;

            var request = new PlaystateRequest
            {
                Command = PlaystateCommand.Seek,
                SeekPositionTicks = pending.PositionTicks,
                ControllingUserId = pending.UserId
            };

            await _sessionManager.SendPlaystateCommand(
                    pending.SessionId,
                    commandSessionId,
                    request,
                    _lifetimeToken)
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

        private static string? GetSessionId(object? session) =>
            GetStringProperty(session, "Id");

        private static string GetStringProperty(object? source, string name) =>
            source?.GetType()
                .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(source) as string ?? string.Empty;

        private static bool ShouldSuppressServerSideResumeSeek(SessionInfo session)
            => IsLikelyLgOrWebOsSession(session);

        private static bool IsLikelyNativeAndroidTabletSession(SessionInfo session)
        {
            if (IsLikelyFireTvOrAndroidTvSession(session))
                return false;

            var client = GetStringProperty(session, "Client");
            if (!ContainsIgnoreCase(client, "Emby for Android"))
                return false;

            var deviceName = GetStringProperty(session, "DeviceName");
            var userAgent = GetStringProperty(session, "UserAgent");
            if (ContainsIgnoreCase(deviceName, "Tab")
                || ContainsIgnoreCase(deviceName, "Tablet")
                || ContainsIgnoreCase(userAgent, "SM-X"))
            {
                return true;
            }

            return ContainsIgnoreCase(userAgent, "Android")
                   && ContainsIgnoreCase(userAgent, "Safari/")
                   && !ContainsIgnoreCase(userAgent, "Mobile")
                   && !ContainsIgnoreCase(userAgent, "TV")
                   && !ContainsIgnoreCase(deviceName, "TV");
        }

        private static bool IsLikelyFireTvOrAndroidTvSession(SessionInfo session)
        {
            var client = GetStringProperty(session, "Client");
            var deviceName = GetStringProperty(session, "DeviceName");
            var userAgent = GetStringProperty(session, "UserAgent");

            return ContainsIgnoreCase(deviceName, "Fire TV")
                   || ContainsIgnoreCase(deviceName, "FireTV")
                   || ContainsIgnoreCase(deviceName, "AFT")
                   || ContainsIgnoreCase(userAgent, "Fire TV")
                   || ContainsIgnoreCase(userAgent, "AFT")
                   || ContainsIgnoreCase(userAgent, "Android TV")
                   || ContainsIgnoreCase(deviceName, "Android TV")
                   || ContainsIgnoreCase(client, "AndroidTv")
                   || ContainsIgnoreCase(client, "Android TV")
                   || ContainsIgnoreCase(client, "Emby for Android TV");
        }

        private static bool IsLikelyLgOrWebOsSession(SessionInfo session)
        {
            var client = GetStringProperty(session, "Client");
            var deviceName = GetStringProperty(session, "DeviceName");
            var userAgent = GetStringProperty(session, "UserAgent");

            return ContainsIgnoreCase(client, "Emby for LG")
                   || ContainsIgnoreCase(client, "WebOS")
                   || ContainsIgnoreCase(deviceName, "LG TV")
                   || ContainsIgnoreCase(deviceName, "WebOS")
                   || ContainsIgnoreCase(userAgent, "Web0S")
                   || ContainsIgnoreCase(userAgent, "WebOS");
        }

        private static bool IsYouTubeLiveItem(BaseItem item) =>
            !string.IsNullOrEmpty(item.ExternalId)
            && item.ExternalId.StartsWith("LIVE_", StringComparison.Ordinal);

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
            => IsSessionStillOnItem(
                pending.SessionId,
                pending.UserId,
                pending.PlaySessionId,
                pending.ItemId,
                "resume seek");

        private bool IsSessionStillOnItem(
            string sessionId,
            string? userId,
            string playSessionId,
            long itemId,
            string logContext)
        {
            try
            {
                var session = _sessionManager?.Sessions
                    .FirstOrDefault(s => string.Equals(s.Id, sessionId, StringComparison.Ordinal));

                if (session == null)
                    return false;

                if (string.IsNullOrEmpty(playSessionId)
                    || !string.Equals(session.UserId, userId, StringComparison.Ordinal)
                    || IsSessionPaused(session))
                {
                    return false;
                }

                // SessionInfo does not expose PlaySessionId on every supported
                // Emby build. When it does, require an exact match; otherwise
                // the immutable pending generation and the Start/Stop cleanup
                // remain the authority for the event's explicit PlaySessionId.
                var currentPlaySessionId = GetCurrentPlaySessionId(session);
                if (!string.IsNullOrEmpty(currentPlaySessionId)
                    && !string.Equals(currentPlaySessionId, playSessionId, StringComparison.Ordinal))
                {
                    return false;
                }

                var nowPlaying = session.FullNowPlayingItem;
                if (nowPlaying != null)
                    return itemId <= 0 || nowPlaying.InternalId == itemId;

                // Some session DTOs only expose the public item id.
                var dtoId = session.NowPlayingItem?.Id;
                if (itemId <= 0)
                    return !string.IsNullOrEmpty(dtoId);

                return long.TryParse(dtoId, out var parsedId) && parsedId == itemId;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] YouTube {logContext} current-item check failed: {ex.Message}");
                return false;
            }
        }

        private static string GetCurrentPlaySessionId(SessionInfo session)
        {
            var playSessionId = GetStringProperty(session, "PlaySessionId");
            if (!string.IsNullOrEmpty(playSessionId))
                return playSessionId;

            var playState = session.GetType()
                .GetProperty("PlayState", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(session);
            return GetStringProperty(playState, "PlaySessionId");
        }

        private static bool IsSessionPaused(SessionInfo session)
        {
            var playState = session.GetType()
                .GetProperty("PlayState", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(session);
            return GetBooleanProperty(playState, "IsPaused");
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

        private const string PluginBuildRevision = "2.0.8.11-resume-player-lifecycle-20260908";

        // Wipes transient caches when the installed plugin version differs
        // from the one we recorded last time. Saves the user from having to
        // clear caches by hand after every upgrade. Library items are NOT
        // touched.
        private static string ChannelSurfaceStampPath =>
            Path.Combine(Plugin.DataPath ?? Path.GetTempPath(), "youtube-channel-surface-v4.txt");

        private const string ChannelSurfaceStamp = "youtube-channel-video-shorts-live-folders";

        private bool WipeCachesIfPluginUpgraded()
        {
            try
            {
                var version = typeof(PluginEntryPoint).Assembly.GetName().Version?.ToString() ?? "0";
                var current = version + "|" + PluginBuildRevision;
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

                        // Hiding Shorts changes the persisted child-item set, so
                        // refresh those children once after an upgrade. Keep the
                        // normal bootstrap refresh shallow for all other installs.
                        var refreshDepth = Plugin.Instance?.Options.ShortsEnabled == false
                            ? ChannelRefreshInvoker.ContentRefreshDepth
                            : ChannelRefreshInvoker.RootRefreshDepth;

                        YouTubeChannel.LogPublic(
                            $"[YT] Triggering post-upgrade channel refresh (depth {refreshDepth})");
                        // Yield to any channel scan Emby is already running near
                        // startup; an active scan already runs the new filtering code.
                        await ChannelRefreshInvoker.TriggerRefreshAsync(
                            refreshDepth, skipIfScanActive: true).ConfigureAwait(false);
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
            _current?.AdjustPollIntervalToConfig(config);
            return hash;
        }

        private void PollTick(object? state)
        {
            if (Volatile.Read(ref _disposed) != 0)
                return;

            if (Interlocked.Exchange(ref _pollRunning, 1) == 1)
                return;

            _pollIdle.Reset();
            if (Volatile.Read(ref _disposed) != 0)
            {
                _pollIdle.Set();
                Interlocked.Exchange(ref _pollRunning, 0);
                return;
            }

            _ = PollTickAsync(_lifetimeCts.Token);
        }

        private async Task PollTickAsync(CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
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
                    await RefreshOnConfigChange(apiKey, config, ct).ConfigureAwait(false);

                await PollWatchLaterPlaylists(apiKey, watchLaterRaw, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Normal during plugin unload or reload.
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Poll tick failed: {ex.Message}");
            }
            finally
            {
                _pollIdle.Set();
                Interlocked.Exchange(ref _pollRunning, 0);
            }
        }

        private void AdjustPollIntervalToConfig(PluginConfiguration config)
        {
            if (Volatile.Read(ref _disposed) != 0)
                return;

            var configured = Math.Clamp(config.WatchLaterPollMinutes, 1, 60);
            if (configured == _currentPollMinutes) return;

            try
            {
                _pollTimer?.Change(TimeSpan.FromMinutes(configured), TimeSpan.FromMinutes(configured));
                _currentPollMinutes = configured;
                YouTubeChannel.LogPublic($"[YT] Auto-refreshed playlist interval updated to {configured} min");
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to adjust poll interval: {ex.Message}");
            }
        }

        private async Task RefreshOnConfigChange(
            string apiKey,
            PluginConfiguration config,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var currentHash = ComputeConfigHash(config);
            var configChanged = !string.Equals(currentHash, LastConfigHash, StringComparison.Ordinal);

            if (!configChanged)
                return;

            LastConfigHash = currentHash;
            TrySaveConfigHash(currentHash);

            if (string.IsNullOrEmpty(apiKey))
                return;

            ct.ThrowIfCancellationRequested();
            try { YouTubeApi.InvalidateAllCache(); }
            catch (Exception ex) { YouTubeChannel.LogPublic($"[YT] Cache invalidation failed: {ex.Message}"); }

            ct.ThrowIfCancellationRequested();
            await ChannelRefreshInvoker.TriggerRefreshAsync(ChannelRefreshInvoker.ContentRefreshDepth).ConfigureAwait(false);
        }

        private async Task PollWatchLaterPlaylists(
            string apiKey,
            string watchLaterRaw,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var playlists = watchLaterRaw
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(YouTubeChannel.IsSupportedPublicPlaylistId)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            // Clean state for entries that were removed, including when the
            // configuration is now empty or temporarily has no API key.
            var configured = new HashSet<string>(playlists, StringComparer.Ordinal);
            lock (_playlistFingerprintLock)
            {
                if (Volatile.Read(ref _disposed) != 0)
                {
                    ct.ThrowIfCancellationRequested();
                    return;
                }

                foreach (var stalePlaylist in _lastPlaylistFingerprints.Keys
                             .Where(id => !configured.Contains(id))
                             .ToList())
                {
                    _lastPlaylistFingerprints.Remove(stalePlaylist);
                    _playlistFingerprintsDirty = true;
                }
            }
            SavePlaylistFingerprints();

            if (playlists.Count == 0 || string.IsNullOrEmpty(apiKey))
                return;

            var anyChanged = false;
            foreach (var playlist in playlists)
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
                    // Hash five pages plus YouTube's total result count. This
                    // catches appends/removals beyond item 250 without making
                    // very large playlists exhaust the daily API allowance.
                    var snapshot = await YouTubeApi.GetPlaylistSnapshotFreshAsync(
                            apiKey, playlist, 250, ct)
                        .ConfigureAwait(false);
                    ct.ThrowIfCancellationRequested();
                    var current = ComputePlaylistFingerprint(snapshot.VideoIds, snapshot.TotalResults);

                    bool hasPrevious;
                    string? previous;
                    bool changed;
                    lock (_playlistFingerprintLock)
                    {
                        if (Volatile.Read(ref _disposed) != 0)
                        {
                            ct.ThrowIfCancellationRequested();
                            return;
                        }

                        hasPrevious = _lastPlaylistFingerprints.TryGetValue(playlist, out previous);
                        changed = !hasPrevious || !string.Equals(current, previous, StringComparison.Ordinal);
                        if (changed)
                        {
                            _lastPlaylistFingerprints[playlist] = current;
                            _playlistFingerprintsDirty = true;
                        }
                    }

                    if (hasPrevious && changed)
                    {
                        YouTubeApi.InvalidateCacheContaining(playlist);
                        anyChanged = true;
                    }
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Auto-refreshed playlist poll failed for {playlist}: {ex.Message}");
                }
            }

            // Keeping the compact fingerprints across process restarts lets
            // the first poll detect changes that happened while Emby was
            // offline. On a first-ever run we establish a baseline without
            // forcing an otherwise unnecessary deep channel refresh.
            ct.ThrowIfCancellationRequested();
            SavePlaylistFingerprints();

            if (anyChanged)
            {
                ct.ThrowIfCancellationRequested();
                await ChannelRefreshInvoker.TriggerRefreshAsync(ChannelRefreshInvoker.ContentRefreshDepth).ConfigureAwait(false);
            }
        }

        private static string ComputePlaylistFingerprint(IEnumerable<string> videoIds, int totalResults)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var value = totalResults.ToString(CultureInfo.InvariantCulture)
                        + "\n"
                        + string.Join("\n", videoIds);
            return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
        }

        private void LoadPlaylistFingerprints()
        {
            try
            {
                lock (PlaylistFingerprintFileLock)
                {
                    if (!File.Exists(PlaylistFingerprintPath))
                        return;

                    var json = File.ReadAllText(PlaylistFingerprintPath);
                    var loaded = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (loaded == null)
                        return;

                    lock (_playlistFingerprintLock)
                    {
                        foreach (var pair in loaded)
                        {
                            if (YouTubeChannel.IsSupportedPublicPlaylistId(pair.Key)
                                && pair.Value is { Length: 64 }
                                && pair.Value.All(Uri.IsHexDigit))
                            {
                                _lastPlaylistFingerprints[pair.Key] = pair.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Failed to load playlist fingerprints: {ex.Message}");
            }
        }

        private void SavePlaylistFingerprints()
        {
            lock (PlaylistFingerprintFileLock)
            {
                lock (_playlistFingerprintLock)
                {
                    if (!_playlistFingerprintsDirty)
                        return;

                    var path = PlaylistFingerprintPath;
                    var tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                    try
                    {
                        var directory = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(directory))
                            Directory.CreateDirectory(directory);

                        File.WriteAllText(tempPath, JsonSerializer.Serialize(_lastPlaylistFingerprints));
                        if (File.Exists(path))
                            File.Replace(tempPath, path, null);
                        else
                            File.Move(tempPath, path);
                        _playlistFingerprintsDirty = false;
                    }
                    catch (Exception ex)
                    {
                        try { if (File.Exists(tempPath)) File.Delete(tempPath); }
                        catch { }
                        YouTubeChannel.LogPublic($"[YT] Failed to save playlist fingerprints: {ex.Message}");
                    }
                }
            }
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
            lock (_resumeDispatchLifetimeGate)
            {
                if (Interlocked.Exchange(ref _disposed, 1) != 0)
                    return;

                _lifetimeCts.Cancel();
                _pendingResumeSeeks.Clear();
                _resumeSeekTerminals.Clear();
                _resumeSeeksInFlight.Clear();
            }
            _captionOffGuard.Dispose();
            if (ReferenceEquals(_current, this))
                _current = null;
            if (_libraryManager != null)
                _libraryManager.ItemUpdated -= OnItemUpdated;
            _userManager.UserCreated -= OnHomeSectionUserChanged;
            _userManager.UserPolicyUpdated -= OnHomeSectionUserChanged;

            if (_sessionManager != null)
            {
                _sessionManager.PlaybackStart -= OnPlaybackStart;
                _sessionManager.PlaybackProgress -= OnPlaybackProgress;
                _sessionManager.PlaybackStopped -= OnPlaybackStopped;
            }

            _pollTimer?.Dispose();
            var pollStopped = _pollIdle.Wait(TimeSpan.FromSeconds(15));
            if (!pollStopped)
                YouTubeChannel.LogPublic("[YT] Playlist poll did not stop within 15 seconds during plugin unload.");
            _resumeCheckpointSaveTimer?.Dispose();
            _sortNameRepairer.Dispose();
            SavePlaylistFingerprints();
            SaveResumeCheckpoints();
            if (pollStopped)
                _lifetimeCts.Dispose();
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
        private static int _nonSkippableRefreshRequested;
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
            if (!skipIfScanActive)
                Interlocked.Exchange(ref _nonSkippableRefreshRequested, 1);

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
                    var maySkipIfScanActive = Interlocked.Exchange(
                        ref _nonSkippableRefreshRequested,
                        0) == 0;
                    await TriggerRefreshCoreAsync(refreshDepth, maySkipIfScanActive).ConfigureAwait(false);

                    if (Interlocked.Exchange(ref _refreshAgainRequested, 0) != 1)
                        break;

                    YouTubeChannel.LogPublic("[YT] TriggerRefresh: running queued follow-up");
                }
            }
            finally
            {
                RefreshGate.Release();

                if (Interlocked.Exchange(ref _refreshAgainRequested, 0) == 1)
                {
                    var maySkipIfScanActive = Volatile.Read(ref _nonSkippableRefreshRequested) == 0;
                    _ = Task.Run(() => TriggerRefreshAsync(RootRefreshDepth, maySkipIfScanActive));
                }
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
                        PluginEntryPoint.RequestHomeSectionSync("channel refresh completed");
                        return;
                    }

                    // From here the continuation owns dropping the gate, when
                    // Emby actually finishes (success, fault or cancel).
                    _ = task.ContinueWith(
                        completedTask =>
                        {
                            Interlocked.Exchange(ref _refreshActive, 0);
                            if (completedTask.Status == TaskStatus.RanToCompletion)
                            {
                                PluginEntryPoint.RequestHomeSectionSync(
                                    "channel refresh completed");
                            }
                        },
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
