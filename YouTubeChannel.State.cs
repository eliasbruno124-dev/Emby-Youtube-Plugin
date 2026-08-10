using System;
using System.Collections.Concurrent;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel
    {
        private const string ChannelIdPrefix = "UC";
        private const int MinChannelIdLength = 20;
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
        private static readonly TimeSpan ShortsPageEmptyCacheTtl = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan NegativeCacheTtl = TimeSpan.FromHours(1);

        private sealed record ShortsPageCacheEntry(System.Collections.Generic.HashSet<string> VideoIds, DateTime CachedAt);
        private sealed record ChannelPageProbeResult(
            System.Collections.Generic.HashSet<string> VideoIds,
            bool LookupSucceeded);

        // Backwards-compatible no-op kept for callers that still invoke it.
        // Cross-folder dedup used to live here, but the seen set was never
        // consulted while building aggregator views — so it just spent quota
        // and held memory. Keeping the public method around so we don't break
        // existing callers while the rest of the plugin gets rewired.
        public static void ResetCrossFolderSeen() { }

        private static string StripPrefix(string id)
        {
            if (id.StartsWith(LivePrefix, StringComparison.Ordinal)) return id.Substring(LivePrefix.Length);
            if (id.StartsWith(ReelPrefix, StringComparison.Ordinal)) return id.Substring(ReelPrefix.Length);
            return id;
        }

        internal static bool IsSupportedPublicPlaylistId(string value) =>
            value.Length > MinChannelIdLength
            && (value.StartsWith("PL", StringComparison.Ordinal)
                || value.StartsWith("UU", StringComparison.Ordinal)
                || value.StartsWith("OL", StringComparison.Ordinal))
            && HasResourceIdCharacters(value);

        private static bool IsSupportedChannelId(string value) =>
            value.Length == 24
            && value.StartsWith(ChannelIdPrefix, StringComparison.Ordinal)
            && HasResourceIdCharacters(value);

        private static bool IsSupportedHandle(string value) =>
            value.Length > 1
            && value.StartsWith(HandlePrefix, StringComparison.Ordinal)
            && value.Skip(1).All(character => !char.IsWhiteSpace(character));

        private static bool HasResourceIdCharacters(string value) =>
            value.All(character =>
                character is >= 'A' and <= 'Z'
                || character is >= 'a' and <= 'z'
                || character is >= '0' and <= '9'
                || character == '_'
                || character == '-');

        private static bool IsPrivateWatchLaterPlaylistId(string value) =>
            value.StartsWith("WL", StringComparison.Ordinal);

        private static bool HasReservedResourcePrefix(string value) =>
            value.StartsWith(ChannelIdPrefix, StringComparison.Ordinal)
            || value.StartsWith("PL", StringComparison.Ordinal)
            || value.StartsWith("UU", StringComparison.Ordinal)
            || value.StartsWith("OL", StringComparison.Ordinal)
            || value.StartsWith("WL", StringComparison.Ordinal)
            || value.StartsWith(HandlePrefix, StringComparison.Ordinal);

        private const string LivePrefix = "LIVE_";
        private const string ReelPrefix = "REEL_";
    }
}
