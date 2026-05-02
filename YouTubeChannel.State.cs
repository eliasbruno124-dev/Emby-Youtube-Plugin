using System;
using System.Collections.Concurrent;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel
    {
        private const string ChannelIdPrefix = "UC";
        private const int MinChannelIdLength = 20;
        private const string PlaylistPrefix = "PL";
        private const string HandlePrefix = "@";
        private const string FolderSeparator = "_x_";
        private const int MaxMetaCacheEntries = 2000;

        private record VideoMeta(
            string? Overview, DateTime? Premiere, int? Year,
            long? RuntimeTicks, string? ThumbUrl, DateTime CachedAt,
            string? OriginalLang = null);

        private static readonly ConcurrentDictionary<string, VideoMeta> MetaCache = new();
        private static readonly ConcurrentDictionary<string, ShortsPageCacheEntry> ShortsPageCache = new(StringComparer.Ordinal);
        private static readonly TimeSpan MetaCacheTtl = TimeSpan.FromDays(365);
        private static readonly TimeSpan ShortsPageCacheTtl = TimeSpan.FromHours(6);
        private static readonly TimeSpan ShortsPageEmptyCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan NegativeCacheTtl = TimeSpan.FromHours(1);

        private sealed record ShortsPageCacheEntry(System.Collections.Generic.HashSet<string> VideoIds, DateTime CachedAt);

        // Backwards-compatible no-op kept for callers that still invoke it.
        // Cross-folder deduplication used to live here, but the seen set was
        // never consulted while building aggregator views, so it just spent
        // quota and held memory. Keeping the public method avoids breaking
        // calling code while the rest of the plugin is rewired.
        public static void ResetCrossFolderSeen() { }

        private static string StripPrefix(string id)
        {
            if (id.StartsWith(LivePrefix, StringComparison.Ordinal)) return id.Substring(LivePrefix.Length);
            if (id.StartsWith(ReelPrefix, StringComparison.Ordinal)) return id.Substring(ReelPrefix.Length);
            return id;
        }

        private const string LivePrefix = "LIVE_";
        private const string ReelPrefix = "REEL_";
    }
}
