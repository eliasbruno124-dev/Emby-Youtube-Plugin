using System;
using System.Collections.Concurrent;

namespace Emby.YouTubePlugin
{
    internal static class ChannelContentFlags
    {
        private static readonly ConcurrentDictionary<string, ChannelContentState> States = new(StringComparer.Ordinal);

        public static ChannelContentState? Get(string channelId)
        {
            return States.TryGetValue(channelId, out var state) ? state : null;
        }

        public static void SetHasShorts(string channelId, bool hasShorts)
        {
            Update(channelId, state => state with
            {
                HasShorts = hasShorts,
                ShortsCheckedAt = DateTime.UtcNow
            });
        }

        public static void SetHasLive(string channelId, bool hasLive)
        {
            Update(channelId, state => state with
            {
                HasLive = hasLive,
                LiveCheckedAt = DateTime.UtcNow
            });
        }

        private static void Update(string channelId, Func<ChannelContentState, ChannelContentState> update)
        {
            if (string.IsNullOrWhiteSpace(channelId))
            {
                return;
            }

            States.AddOrUpdate(
                channelId,
                _ => update(new ChannelContentState(null, null, null, null)),
                (_, existing) => update(existing));
        }
    }

    internal sealed record ChannelContentState(
        bool? HasShorts,
        bool? HasLive,
        DateTime? ShortsCheckedAt,
        DateTime? LiveCheckedAt);
}
