using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel
    {
        // Pulls video IDs with a LIVE or UPCOMING badge off the channel's
        // /streams page. No API calls — same idea as the Shorts page scraper.
        //
        // Past streams also live on this page but use a DEFAULT/UPCOMING style,
        // so we only keep LIVE+UPCOMING. The videoId and the style marker can
        // appear in either order in the JSON, so we run two regexes within a
        // ~3000 char window to handle YouTube shuffling things around.
        private static readonly Regex LiveBadgeForwardRegex = new(
            "\\\\?[\"']videoId\\\\?[\"']\\s*:\\s*\\\\?[\"']([A-Za-z0-9_-]{11})\\\\?[\"'][\\s\\S]{0,3000}?\\\\?[\"']style\\\\?[\"']\\s*:\\s*\\\\?[\"'](LIVE|UPCOMING)\\\\?[\"']",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex LiveBadgeReverseRegex = new(
            "\\\\?[\"']style\\\\?[\"']\\s*:\\s*\\\\?[\"'](LIVE|UPCOMING)\\\\?[\"'][\\s\\S]{0,3000}?\\\\?[\"']videoId\\\\?[\"']\\s*:\\s*\\\\?[\"']([A-Za-z0-9_-]{11})\\\\?[\"']",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly ConcurrentDictionary<string, ShortsPageCacheEntry> LiveStreamsPageCache =
            new(StringComparer.Ordinal);

        // Same TTLs as the Shorts page cache — channels going live happens at
        // human pace, so 6h hot / 5min empty is plenty.
        internal static async Task<HashSet<string>> GetChannelLiveAndUpcomingIdsAsync(
            string channelId, CancellationToken cancellationToken)
        {
            if (!IsChannelId(channelId))
                return new HashSet<string>(StringComparer.Ordinal);

            if (LiveStreamsPageCache.TryGetValue(channelId, out var cached)
                && (DateTime.UtcNow - cached.CachedAt)
                    < (cached.VideoIds.Count == 0 ? ShortsPageEmptyCacheTtl : ShortsPageCacheTtl))
                return new HashSet<string>(cached.VideoIds, StringComparer.Ordinal);

            var ids = new HashSet<string>(StringComparer.Ordinal);
            try
            {
                var url = $"https://www.youtube.com/channel/{channelId}/streams";
                var html = await GetShortsPageHtmlAsync(url, cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(html))
                {
                    var fallback = await TryGetShortsPageHtmlWithExternalToolAsync(url, cancellationToken)
                        .ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(fallback)) html = fallback;
                }

                ExtractIds(html, LiveBadgeForwardRegex, idGroup: 1, ids);
                ExtractIds(html, LiveBadgeReverseRegex, idGroup: 2, ids);

                LiveStreamsPageCache[channelId] = new ShortsPageCacheEntry(ids, DateTime.UtcNow);
                Log($"[YT] Live streams page probe for {channelId}: {ids.Count} live/upcoming ids");
            }
            catch (Exception ex)
            {
                Log($"[YT] Live streams page probe failed for {channelId}: {ex.Message}");
                if (LiveStreamsPageCache.TryGetValue(channelId, out var stale))
                    return new HashSet<string>(stale.VideoIds, StringComparer.Ordinal);
            }

            return ids;
        }

        private static void ExtractIds(string html, Regex regex, int idGroup, HashSet<string> ids)
        {
            if (string.IsNullOrEmpty(html)) return;
            foreach (Match match in regex.Matches(html))
            {
                var value = match.Groups[idGroup].Value;
                if (!string.IsNullOrEmpty(value)) ids.Add(value);
            }
        }
    }
}
