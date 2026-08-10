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
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> LiveStreamsPageLocks =
            new(StringComparer.Ordinal);
        private static readonly TimeSpan LiveStreamsPageCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan LiveStreamsPageEmptyCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan LiveStreamsPageStaleFallbackTtl = TimeSpan.FromMinutes(15);

        // Live state is volatile. Positive results are deliberately short-lived
        // so an ended stream cannot remain classified as live for hours.
        private static async Task<ChannelPageProbeResult> GetChannelLiveAndUpcomingIdsAsync(
            string channelId, CancellationToken cancellationToken)
        {
            if (!IsChannelId(channelId))
                return new ChannelPageProbeResult(new HashSet<string>(StringComparer.Ordinal), true);

            if (LiveStreamsPageCache.TryGetValue(channelId, out var cached)
                && (DateTime.UtcNow - cached.CachedAt)
                    < (cached.VideoIds.Count == 0 ? LiveStreamsPageEmptyCacheTtl : LiveStreamsPageCacheTtl))
                return new ChannelPageProbeResult(
                    new HashSet<string>(cached.VideoIds, StringComparer.Ordinal),
                    true);

            var probeLock = LiveStreamsPageLocks.GetOrAdd(channelId, _ => new SemaphoreSlim(1, 1));
            await probeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (LiveStreamsPageCache.TryGetValue(channelId, out cached)
                    && (DateTime.UtcNow - cached.CachedAt)
                        < (cached.VideoIds.Count == 0 ? LiveStreamsPageEmptyCacheTtl : LiveStreamsPageCacheTtl))
                    return new ChannelPageProbeResult(
                        new HashSet<string>(cached.VideoIds, StringComparer.Ordinal),
                        true);

                var ids = new HashSet<string>(StringComparer.Ordinal);
                try
                {
                    var url = $"https://www.youtube.com/channel/{channelId}/streams";
                    var html = await GetShortsPageHtmlAsync(url, cancellationToken).ConfigureAwait(false);
                    if (!IsUsableStreamsPage(html))
                    {
                        var fallback = await TryGetShortsPageHtmlWithExternalToolAsync(url, cancellationToken)
                            .ConfigureAwait(false);
                        if (IsUsableStreamsPage(fallback))
                            html = fallback;
                    }

                    if (!IsUsableStreamsPage(html))
                        throw new InvalidOperationException("YouTube returned a consent, block, or incomplete streams page.");

                    ExtractIds(html!, LiveBadgeForwardRegex, idGroup: 1, ids);
                    ExtractIds(html!, LiveBadgeReverseRegex, idGroup: 2, ids);

                    LiveStreamsPageCache[channelId] = new ShortsPageCacheEntry(ids, DateTime.UtcNow);
                    Log($"[YT] Live streams page probe for {channelId}: {ids.Count} live/upcoming ids");
                    return new ChannelPageProbeResult(ids, true);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Log($"[YT] Live streams page probe failed for {channelId}: {ex.Message}");
                    if (LiveStreamsPageCache.TryGetValue(channelId, out var stale)
                        && (DateTime.UtcNow - stale.CachedAt) < LiveStreamsPageStaleFallbackTtl)
                    {
                        return new ChannelPageProbeResult(
                            new HashSet<string>(stale.VideoIds, StringComparer.Ordinal),
                            false);
                    }
                    return new ChannelPageProbeResult(ids, false);
                }
            }
            finally
            {
                probeLock.Release();
            }
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

        private static bool IsUsableStreamsPage(string? html)
        {
            if (string.IsNullOrWhiteSpace(html) || html.Length < 512)
                return false;

            return html.IndexOf("ytInitialData", StringComparison.Ordinal) >= 0
                && (html.IndexOf("channelMetadataRenderer", StringComparison.Ordinal) >= 0
                    || html.IndexOf("tabRenderer", StringComparison.Ordinal) >= 0
                    || html.IndexOf("videoId", StringComparison.Ordinal) >= 0);
        }
    }
}
