using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Session;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // This is intentionally player-scoped: it does not write user data or
    // change a user's subtitle preference. Some bundled native players may
    // ignore this documented command, so a successful send is only logged as
    // a command send, not as a visual-caption assertion.
    internal sealed class YouTubeCaptionOffGuard : IDisposable
    {
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(6);
        private const int MaxAttempts = 2;

        private readonly ISessionManager _sessionManager;
        private readonly ConcurrentDictionary<string, CaptionPlaybackState> _playbacks = new(StringComparer.Ordinal);
        private readonly CancellationTokenSource _lifetimeCts = new();
        private int _started;
        private int _disposed;

        private sealed record CaptionPlaybackIdentity(
            string Key,
            string SessionId,
            string PlaySessionId,
            string UserId,
            long ItemId);

        private sealed class CaptionPlaybackState
        {
            public CaptionPlaybackIdentity Identity { get; }
            public object Gate { get; } = new();
            public int AttemptsSent;
            public DateTime RetryNotBeforeUtc;
            public bool Cancelled;
            public bool Paused;

            public CaptionPlaybackState(CaptionPlaybackIdentity identity) => Identity = identity;
        }

        public YouTubeCaptionOffGuard(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
        }

        public void Start()
        {
            if (Interlocked.Exchange(ref _started, 1) != 0 || IsDisposingOrDisposed())
                return;

            _sessionManager.PlaybackStart += OnPlaybackStart;
            _sessionManager.PlaybackProgress += OnPlaybackProgress;
            _sessionManager.PlaybackStopped += OnPlaybackStopped;
        }

        private void OnPlaybackStart(object? sender, PlaybackProgressEventArgs e)
        {
            try
            {
                var session = e.Session;
                if (session == null || string.IsNullOrEmpty(session.Id) || IsDisposingOrDisposed())
                    return;
                CancelForSession(session.Id);
                if (e.Item != null && IsYouTubeItem(e.Item) && IsNativeAndroidOrFireTv(session))
                    _playbacks[session.Id] = new CaptionPlaybackState(CreateIdentity(e, session, e.Item));
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Caption-off start hook failed: {ex.Message}");
            }
        }

        private void OnPlaybackProgress(object? sender, PlaybackProgressEventArgs e)
        {
            try
            {
                if (IsDisposingOrDisposed())
                    return;

                var session = e.Session;
                var item = e.Item;
                var sessionId = session?.Id;
                if (session == null || item == null || string.IsNullOrEmpty(sessionId))
                    return;

                if (!_playbacks.TryGetValue(sessionId, out var state)
                    || !MatchesEvent(state.Identity, e, session, item))
                    return;
                lock (state.Gate)
                    state.Paused = IsPaused(e, session);
                if (!state.Paused)
                    _ = TrySendCaptionOffAsync(state);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] Caption-off progress hook failed: {ex.Message}");
            }
        }

        private void OnPlaybackStopped(object? sender, PlaybackStopEventArgs e)
        {
            var session = e.Session;
            if (session != null && e.Item != null
                && _playbacks.TryGetValue(session.Id, out var state)
                && MatchesEvent(state.Identity, e, session, e.Item))
                CancelState(state);
        }

        private async Task TrySendCaptionOffAsync(CaptionPlaybackState state)
        {
            try
            {
                Task dispatch;
                var identity = state.Identity;
                lock (state.Gate)
                {
                    if (state.Cancelled || state.Paused || IsDisposingOrDisposed()
                        || !_playbacks.TryGetValue(identity.SessionId, out var active)
                        || !ReferenceEquals(active, state)
                        || !IsCurrentIdentity(identity, out var currentSession)
                        || currentSession == null || !SupportsSetSubtitleStreamIndex(currentSession)
                        || state.AttemptsSent >= MaxAttempts
                        || DateTime.UtcNow < state.RetryNotBeforeUtc)
                        return;

                    // Reserve before dispatch, including failed sends. Pause
                    // never clears the budget and stale timers cannot create
                    // replacement state after stop or a same-item replay.
                    state.AttemptsSent++;
                    state.RetryNotBeforeUtc = DateTime.UtcNow + RetryDelay;
                    if (state.AttemptsSent == 1)
                        ScheduleRetry(state);
                    var command = new GeneralCommand
                    {
                        Name = "SetSubtitleStreamIndex",
                        Arguments = new Dictionary<string, string> { ["Index"] = "-1" }
                    };
                    dispatch = _sessionManager.SendGeneralCommand(
                        identity.SessionId,
                        identity.SessionId,
                        command,
                        _lifetimeCts.Token);
                }
                await dispatch.ConfigureAwait(false);

                if (IsDisposingOrDisposed())
                    return;

                YouTubeChannel.LogPublic("[YT] Caption-off command sent for native YouTube playback.");
            }
            catch (OperationCanceledException) when (IsDisposingOrDisposed())
            {
                // Plugin unload cancels delayed or in-flight command work.
            }
            catch (Exception ex)
            {
                // The attempt remains counted so a client/server failure cannot
                // turn recurring progress callbacks into a command storm.
                YouTubeChannel.LogPublic($"[YT] Caption-off command failed: {ex.Message}");
            }
        }

        private void ScheduleRetry(CaptionPlaybackState state)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(RetryDelay, _lifetimeCts.Token).ConfigureAwait(false);
                    if (!IsDisposingOrDisposed())
                        await TrySendCaptionOffAsync(state).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
                {
                    // Expected during plugin unload.
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] Caption-off retry task failed: {ex.Message}");
                }
            });
        }

        private bool IsCurrentIdentity(CaptionPlaybackIdentity identity, out SessionInfo? session)
        {
            session = _sessionManager.Sessions.FirstOrDefault(s =>
                string.Equals(s.Id, identity.SessionId, StringComparison.Ordinal));
            if (session == null
                || !string.Equals(session.UserId, identity.UserId, StringComparison.Ordinal)
                || !IsNativeAndroidOrFireTv(session)
                || IsSessionPaused(session))
            {
                return false;
            }

            var nowPlaying = session.FullNowPlayingItem;
            if (nowPlaying == null || nowPlaying.InternalId != identity.ItemId || !IsYouTubeItem(nowPlaying))
                return false;

            // Older session DTOs may not expose PlaySessionId. When available,
            // include it in the recheck so a player restart cannot receive an
            // old delayed command.
            var currentPlaySessionId = GetStringProperty(session, "PlaySessionId");
            return string.IsNullOrEmpty(currentPlaySessionId)
                   || string.Equals(currentPlaySessionId, identity.PlaySessionId, StringComparison.Ordinal);
        }

        private static CaptionPlaybackIdentity CreateIdentity(
            PlaybackProgressEventArgs e,
            SessionInfo session,
            BaseItem item)
        {
            var sessionId = session.Id;
            var playSessionId = e.PlaySessionId ?? string.Empty;
            var userId = session.UserId ?? string.Empty;
            return new CaptionPlaybackIdentity(
                $"{sessionId}|{playSessionId}|{item.InternalId}|{userId}",
                sessionId,
                playSessionId,
                userId,
                item.InternalId);
        }

        private void CancelForSession(string sessionId)
        {
            if (_playbacks.TryGetValue(sessionId, out var state))
                CancelState(state);
        }

        private void CancelState(CaptionPlaybackState state)
        {
            lock (state.Gate)
            {
                state.Cancelled = true;
                ((ICollection<KeyValuePair<string, CaptionPlaybackState>>)_playbacks)
                    .Remove(new KeyValuePair<string, CaptionPlaybackState>(state.Identity.SessionId, state));
            }
        }

        private static bool MatchesEvent(CaptionPlaybackIdentity identity,
            PlaybackProgressEventArgs e, SessionInfo session, BaseItem item) =>
            string.Equals(identity.UserId, session.UserId ?? string.Empty, StringComparison.Ordinal)
            && string.Equals(identity.PlaySessionId, e.PlaySessionId ?? string.Empty, StringComparison.Ordinal)
            && identity.ItemId == item.InternalId;

        private static bool IsYouTubeItem(BaseItem item) =>
            !string.IsNullOrEmpty(YouTubeImageProvider.TryGetVideoId(item));

        private static bool IsNativeAndroidOrFireTv(SessionInfo session)
        {
            var client = GetStringProperty(session, "Client");
            var deviceName = GetStringProperty(session, "DeviceName");
            var userAgent = GetStringProperty(session, "UserAgent");
            return ContainsIgnoreCase(client, "Emby for Android")
                   || ContainsIgnoreCase(client, "EmbyAndroid")
                   || (ContainsIgnoreCase(client, "Emby")
                       && (ContainsIgnoreCase(deviceName, "Fire TV")
                           || ContainsIgnoreCase(deviceName, "FireTV")
                           || ContainsIgnoreCase(deviceName, "AFT")
                           || ContainsIgnoreCase(userAgent, "Fire TV")
                           || ContainsIgnoreCase(userAgent, "AFT")));
        }

        private static bool SupportsSetSubtitleStreamIndex(SessionInfo session)
        {
            var supportedCommands = session.GetType()
                .GetProperty("SupportedCommands", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(session) as IEnumerable;
            if (supportedCommands == null)
                return false;

            foreach (var command in supportedCommands)
            {
                if (string.Equals(command?.ToString(), "SetSubtitleStreamIndex", StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static bool IsPaused(PlaybackProgressEventArgs e, SessionInfo session) =>
            GetBooleanProperty(e, "IsPaused") || IsSessionPaused(session);

        private static bool IsSessionPaused(SessionInfo session)
        {
            var playState = session.GetType()
                .GetProperty("PlayState", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(session);
            return GetBooleanProperty(playState, "IsPaused");
        }

        private static string GetStringProperty(object? source, string name) =>
            source?.GetType()
                .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(source) as string ?? string.Empty;

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

        private static bool ContainsIgnoreCase(string? value, string needle) =>
            !string.IsNullOrEmpty(value)
            && value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

        private bool IsDisposingOrDisposed() =>
            Volatile.Read(ref _disposed) != 0 || _lifetimeCts.IsCancellationRequested;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
                return;

            _lifetimeCts.Cancel();
            if (Interlocked.Exchange(ref _started, 0) != 0)
            {
                _sessionManager.PlaybackStart -= OnPlaybackStart;
                _sessionManager.PlaybackProgress -= OnPlaybackProgress;
                _sessionManager.PlaybackStopped -= OnPlaybackStopped;
            }

            foreach (var state in _playbacks.Values)
                CancelState(state);
        }
    }
}
