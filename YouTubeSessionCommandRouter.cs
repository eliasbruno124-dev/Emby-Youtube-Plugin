using MediaBrowser.Controller.Session;
using System;
using System.Linq;

namespace Emby.YouTubePlugin
{
    internal static class YouTubeSessionCommandRouter
    {
        // Android can report playback through the web layer while its native
        // bridge owns the socket under a second client name. Never broadcast a
        // command: use the playback session, or one unambiguous bridge belonging
        // to the same signed-in device and app version.
        internal static bool TryResolve(
            ISessionManager manager,
            string playbackSessionId,
            string? userId,
            long itemId,
            string messageName,
            out string commandSessionId)
        {
            commandSessionId = string.Empty;
            if (string.IsNullOrEmpty(playbackSessionId) || string.IsNullOrEmpty(userId)
                || itemId <= 0 || string.IsNullOrEmpty(messageName))
                return false;

            var sessions = manager.Sessions.ToArray();
            var playback = sessions.FirstOrDefault(s =>
                string.Equals(s.Id, playbackSessionId, StringComparison.Ordinal));
            if (playback == null
                || !string.Equals(playback.UserId, userId, StringComparison.Ordinal)
                || playback.FullNowPlayingItem?.InternalId != itemId)
                return false;

            if (HasActiveTransport(playback, messageName))
            {
                commandSessionId = playback.Id;
                return true;
            }

            if (string.IsNullOrEmpty(playback.DeviceId)
                || string.IsNullOrEmpty(playback.ApplicationVersion))
                return false;

            SessionInfo? bridge = null;
            foreach (var candidate in sessions)
            {
                if (string.IsNullOrEmpty(candidate.Id)
                    || string.Equals(candidate.Id, playback.Id, StringComparison.Ordinal)
                    || !IsNativeBridgePair(playback.Client, candidate.Client)
                    || !string.Equals(candidate.UserId, userId, StringComparison.Ordinal)
                    || !string.Equals(candidate.DeviceId, playback.DeviceId, StringComparison.Ordinal)
                    || !string.Equals(candidate.ApplicationVersion, playback.ApplicationVersion, StringComparison.Ordinal)
                    // An independently playing companion is not an idle bridge,
                    // even when it happens to show the same video.
                    || candidate.FullNowPlayingItem != null
                    || candidate.NowPlayingItem != null
                    || !HasActiveTransport(candidate, messageName))
                    continue;

                if (bridge != null)
                    return false;
                bridge = candidate;
            }

            if (bridge == null)
                return false;
            commandSessionId = bridge.Id;
            return true;
        }

        private static bool IsNativeBridgePair(string? first, string? second) =>
            IsAndroidWebLayer(first) && IsAndroidBridge(second)
            || IsAndroidBridge(first) && IsAndroidWebLayer(second);

        private static bool IsAndroidWebLayer(string? client) =>
            string.Equals(client, "Emby for Android", StringComparison.OrdinalIgnoreCase)
            || string.Equals(client, "EmbyAndroid", StringComparison.OrdinalIgnoreCase);

        private static bool IsAndroidBridge(string? client) =>
            string.Equals(client, "AndroidTv", StringComparison.OrdinalIgnoreCase);

        private static bool HasActiveTransport(SessionInfo session, string messageName)
        {
            // Emby's send task can complete without an error when no controller
            // accepts the message. Check the transport before consuming a retry.
            foreach (var controller in session.SessionControllers ?? Array.Empty<ISessionController>())
            {
                try
                {
                    if (controller != null && controller.IsSessionActive && controller.SupportsMessage(messageName))
                        return true;
                }
                catch (ObjectDisposedException)
                {
                    // The socket closed while the session snapshot was read.
                }
            }
            return false;
        }
    }
}
