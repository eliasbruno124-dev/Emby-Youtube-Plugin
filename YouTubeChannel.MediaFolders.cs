using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel
    {
        private const int ChannelContentProbePageLimit = 3;

        private static async Task<ChannelItemResult> LoadMediaFolderAsync(
            string apiKey,
            PluginConfiguration config,
            string type,
            string term,
            CancellationToken ct)
        {
            var items = new List<ChannelItemInfo>();
            var limit = type == "search"
                ? ClampSearchVideos(config.MaxSearchVideos)
                : ClampVideos(config.MaxChannelVideos);
            var seenIds = new HashSet<string>();
            string? pageToken = null;
            var hasMore = true;
            var reachedEnd = false;
            var observedShorts = false;
            var observedLive = false;

            while (items.Count < limit && hasMore)
            {
                ct.ThrowIfCancellationRequested();

                using var doc = await GetMediaPageAsync(apiKey, config, type, term, pageToken, ct)
                    .ConfigureAwait(false);
                if (doc == null) break;

                var batch = ExtractVideos(doc, IsPlaylistDocument(type));
                pageToken = GetNextPageToken(doc);
                hasMore = type != "search" && !string.IsNullOrEmpty(pageToken);

                if (batch.Count == 0)
                {
                    reachedEnd = true;
                    break;
                }

                var videoIds = AddUniqueItemsForPage(items, batch, seenIds, limit);

                // Pull the channel's /shorts list before enrichment so we can
                // hand it to EnrichBatch and skip per-video URL probes for
                // anything we already know is a Short. The list is cached so
                // the second batch reuses it.
                HashSet<string>? knownShortsIds = null;
                HashSet<string>? knownLiveIds = null;
                if ((type == "channelvideos" || type == "channelshorts" || type == "channellive")
                    && IsChannelId(term))
                {
                    if (type != "channellive")
                        knownShortsIds = await GetChannelShortVideoIdsAsync(term, ct).ConfigureAwait(false);
                    knownLiveIds = await GetChannelLiveAndUpcomingIdsAsync(term, ct).ConfigureAwait(false);
                }

                if (videoIds.Count > 0)
                    await EnrichBatch(apiKey, batch, videoIds, ct, knownShortsIds, knownLiveIds).ConfigureAwait(false);

                CacheInitialThumbnails(batch);
                ApplyCachedMeta(batch);

                if (knownShortsIds != null)
                    ApplyShortsPageMatches(batch, knownShortsIds);

                observedShorts |= batch.Any(item => item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));
                observedLive |= batch.Any(item => item.Id.StartsWith(LivePrefix, StringComparison.Ordinal));

                ApplyFolderFilters(batch, type, config);
                items.AddRange(batch);

                if (!hasMore)
                    reachedEnd = true;
            }

            UpdateChannelContentFlags(type, term, observedShorts, observedLive, reachedEnd);

            if (items.Count == 0)
                return Msg(items, "No results found.");

            return ToResult(items);
        }

        private static async Task<JsonDocument?> GetMediaPageAsync(
            string apiKey,
            PluginConfiguration config,
            string type,
            string term,
            string? pageToken,
            CancellationToken ct)
        {
            return type switch
            {
                "search" => await YouTubeApi.SearchVideosAsync(apiKey, term, pageToken, ct).ConfigureAwait(false),
                "channelvideos" => await YouTubeApi.GetChannelVideosAsync(apiKey, term, pageToken, ct, config.ChannelSortBy).ConfigureAwait(false),
                "channelshorts" => await YouTubeApi.GetChannelVideosAsync(apiKey, term, pageToken, ct, config.ChannelSortBy).ConfigureAwait(false),
                "channellive" => await YouTubeApi.GetChannelVideosAsync(apiKey, term, pageToken, ct, config.ChannelSortBy).ConfigureAwait(false),
                "playlist" => await YouTubeApi.GetPlaylistVideosAsync(apiKey, term, pageToken, ct).ConfigureAwait(false),
                _ => null
            };
        }

        private static bool IsPlaylistDocument(string type) =>
            type is "playlist" or "channelvideos" or "channelshorts" or "channellive";

        private static string? GetNextPageToken(JsonDocument doc)
        {
            return doc.RootElement.TryGetProperty("nextPageToken", out var token)
                ? token.GetString()
                : null;
        }

        private static List<string> AddUniqueItemsForPage(
            List<ChannelItemInfo> currentItems,
            List<ChannelItemInfo> pageItems,
            HashSet<string> seenIds,
            int limit)
        {
            var videoIds = new List<string>();
            var selected = new List<ChannelItemInfo>();

            foreach (var item in pageItems)
            {
                var rawId = StripPrefix(item.Id);
                if (seenIds.Add(rawId))
                {
                    selected.Add(item);
                    videoIds.Add(rawId);
                }

                if (currentItems.Count + selected.Count >= limit)
                    break;
            }

            pageItems.Clear();
            pageItems.AddRange(selected);
            return videoIds;
        }

        private static void CacheInitialThumbnails(List<ChannelItemInfo> batch)
        {
            foreach (var item in batch)
            {
                if (string.IsNullOrEmpty(item.ImageUrl))
                    continue;

                if (MetaCache.TryGetValue(item.Id, out var existing))
                {
                    if (string.IsNullOrEmpty(existing.ThumbUrl))
                        MetaCache[item.Id] = existing with { ThumbUrl = item.ImageUrl };
                }
                else
                {
                    MetaCache[item.Id] = new VideoMeta(null, null, null, null, item.ImageUrl, DateTime.UtcNow);
                }
            }
        }

        private static void ApplyFolderFilters(
            List<ChannelItemInfo> batch, string type, PluginConfiguration config)
        {
            if (type == "channelvideos")
            {
                batch.RemoveAll(item => item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)
                                     || item.Id.StartsWith(LivePrefix, StringComparison.Ordinal));
            }
            else if (type == "channelshorts")
            {
                batch.RemoveAll(item => !item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));
            }
            else if (type == "channellive")
            {
                batch.RemoveAll(item => !item.Id.StartsWith(LivePrefix, StringComparison.Ordinal));
            }

            if (!config.ShortsEnabled)
                batch.RemoveAll(item => item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));
        }

        private static void UpdateChannelContentFlags(
            string type,
            string term,
            bool observedShorts,
            bool observedLive,
            bool reachedEnd)
        {
            if (!IsChannelId(term))
                return;

            if (observedShorts)
                ChannelContentFlags.SetHasShorts(term, true);
            else if (reachedEnd && (type == "channelvideos" || type == "channelshorts"))
                ChannelContentFlags.SetHasShorts(term, false);

            if (observedLive)
                ChannelContentFlags.SetHasLive(term, true);
            else if (reachedEnd && (type == "channelvideos" || type == "channellive"))
                ChannelContentFlags.SetHasLive(term, false);
        }

        private static bool IsChannelId(string channelId) =>
            channelId.StartsWith(ChannelIdPrefix, StringComparison.Ordinal)
            && channelId.Length > MinChannelIdLength;

        private static async Task<bool> ChannelHasShortsAsync(
            string apiKey, string channelId, CancellationToken ct)
        {
            if (!IsChannelId(channelId))
                return false;

            var cached = ChannelContentFlags.Get(channelId);

            try
            {
                // The channel's /shorts page is by far the cheapest reliable
                // signal: zero quota and it matches what YouTube itself calls
                // a Short. We used to do heavy enrichment here, which burned
                // quota on every root refresh.
                var shortsVideoIds = await GetChannelShortVideoIdsAsync(channelId, ct).ConfigureAwait(false);
                var hasShorts = shortsVideoIds.Count > 0;
                ChannelContentFlags.SetHasShorts(channelId, hasShorts);
                return hasShorts;
            }
            catch (Exception ex)
            {
                Log($"[YT] Shorts folder probe failed for {channelId}: {ex.Message}");
                return cached?.HasShorts ?? false;
            }
        }

        private static async Task<bool> ChannelHasLiveAsync(
            string apiKey, string channelId, CancellationToken ct)
        {
            if (!IsChannelId(channelId))
                return false;

            var cached = ChannelContentFlags.Get(channelId);

            try
            {
                // Local detection: scrape the channel's /streams page and
                // look for LIVE/UPCOMING badges. No API quota at all.
                var liveIds = await GetChannelLiveAndUpcomingIdsAsync(channelId, ct)
                    .ConfigureAwait(false);
                var hasLive = liveIds.Count > 0;
                ChannelContentFlags.SetHasLive(channelId, hasLive);
                return hasLive;
            }
            catch (Exception ex)
            {
                Log($"[YT] Live folder probe failed for {channelId}: {ex.Message}");
                return cached?.HasLive ?? false;
            }
        }
    }
}
