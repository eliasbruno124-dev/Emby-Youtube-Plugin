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
        private const int MaxMediaPageRequests = 10;

        private static async Task<ChannelItemResult> LoadMediaFolderAsync(
            string apiKey,
            PluginConfiguration config,
            string type,
            string term,
            CancellationToken ct)
        {
            if (type == "channellive" && IsChannelId(term))
                return await LoadLiveFolderAsync(apiKey, config, term, ct).ConfigureAwait(false);

            var items = new List<ChannelItemInfo>();
            var limit = type == "search"
                ? ClampSearchVideos(config.MaxSearchVideos)
                : ClampVideos(config.MaxChannelVideos);
            var seenIds = new HashSet<string>(StringComparer.Ordinal);
            var seenPageTokens = new HashSet<string>(StringComparer.Ordinal);
            string? pageToken = null;
            var hasMore = true;
            var reachedEnd = false;
            var observedShorts = false;
            var observedLive = false;
            var pageRequests = 0;
            var usesSearchEndpoint = type == "search"
                || ((type == "channelvideos" || type == "channelshorts")
                    && !string.Equals(config.ChannelSortBy, "date", StringComparison.OrdinalIgnoreCase));
            var maxPageRequests = usesSearchEndpoint
                ? Math.Min(5, ((limit + 49) / 50) + 2)
                : MaxMediaPageRequests;

            HashSet<string>? knownShortsIds = null;
            if ((type == "channelvideos" || type == "channelshorts") && IsChannelId(term))
            {
                var shortsProbe = await GetChannelShortVideoIdsAsync(term, ct).ConfigureAwait(false);
                knownShortsIds = shortsProbe.VideoIds;
            }

            while (items.Count < limit && hasMore && pageRequests < maxPageRequests)
            {
                ct.ThrowIfCancellationRequested();

                var requestedPageToken = pageToken ?? string.Empty;
                if (!seenPageTokens.Add(requestedPageToken))
                {
                    Log($"[YT] Stopping {type} pagination for {term}: repeated page token.");
                    break;
                }
                pageRequests++;

                using var doc = await GetMediaPageAsync(apiKey, config, type, term, pageToken, ct)
                    .ConfigureAwait(false);
                if (doc == null) break;

                var batch = ExtractVideos(doc, IsPlaylistDocument(type));
                pageToken = GetNextPageToken(doc);
                hasMore = !string.IsNullOrEmpty(pageToken);
                if (!hasMore)
                    reachedEnd = true;

                if (batch.Count == 0)
                    continue;

                // Keep the complete unique page until enrichment and folder
                // filtering finish. Cutting it to the requested limit here used
                // to lose valid videos later on the same page whenever an early
                // item was a Short, live, private, or otherwise unplayable.
                var videoIds = KeepUniquePageItems(batch, seenIds);

                if (videoIds.Count > 0)
                    await EnrichBatch(apiKey, batch, videoIds, ct, knownShortsIds).ConfigureAwait(false);

                CacheInitialThumbnails(batch);
                ApplyCachedMeta(batch);

                if (knownShortsIds != null)
                    ApplyShortsPageMatches(batch, knownShortsIds);

                observedShorts |= batch.Any(item => item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));
                observedLive |= batch.Any(item => item.Id.StartsWith(LivePrefix, StringComparison.Ordinal));

                ApplyFolderFilters(batch, type, config);
                var remaining = limit - items.Count;
                if (remaining > 0)
                    items.AddRange(batch.Take(remaining));
            }

            if (hasMore && pageRequests >= maxPageRequests)
                Log($"[YT] Stopping {type} pagination for {term} after {maxPageRequests} pages.");

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

        private static List<string> KeepUniquePageItems(
            List<ChannelItemInfo> pageItems,
            HashSet<string> seenIds)
        {
            var videoIds = new List<string>();
            var selected = new List<ChannelItemInfo>();

            foreach (var item in pageItems)
            {
                var rawId = StripPrefix(item.Id);
                if (!seenIds.Add(rawId)) continue;
                selected.Add(item);
                videoIds.Add(rawId);
            }

            pageItems.Clear();
            pageItems.AddRange(selected);
            return videoIds;
        }

        private static async Task<ChannelItemResult> LoadLiveFolderAsync(
            string apiKey,
            PluginConfiguration config,
            string channelId,
            CancellationToken ct)
        {
            var liveProbe = await GetChannelLiveAndUpcomingIdsAsync(channelId, ct).ConfigureAwait(false);
            var liveIds = liveProbe.VideoIds;
            if (liveIds.Count == 0)
            {
                if (liveProbe.LookupSucceeded)
                {
                    ChannelContentFlags.SetHasLive(channelId, false);
                    return Msg(new List<ChannelItemInfo>(), "No live or upcoming streams found.");
                }

                return Msg(new List<ChannelItemInfo>(), "Live streams are temporarily unavailable.");
            }

            var limit = ClampVideos(config.MaxChannelVideos);
            var result = new List<ChannelItemInfo>();
            var seenIds = new HashSet<string>(StringComparer.Ordinal);
            var orderedIds = liveIds.OrderBy(id => id, StringComparer.Ordinal).ToList();
            var allDetailLookupsSucceeded = true;
            var effectiveRegion = await YouTubeApi.ResolveContentRegionAsync(
                apiKey,
                config.TrendingRegion,
                null,
                ct).ConfigureAwait(false);

            for (var index = 0; index < orderedIds.Count && result.Count < limit; index += 50)
            {
                ct.ThrowIfCancellationRequested();
                var chunk = orderedIds.Skip(index).Take(50).ToList();
                using var doc = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, chunk, ct)
                    .ConfigureAwait(false);
                if (doc == null)
                {
                    allDetailLookupsSucceeded = false;
                    continue;
                }

                var playableIds = GetPlayableLiveVideoIds(doc, liveIds);
                foreach (var item in ExtractTrendingVideos(doc, effectiveRegion))
                {
                    var rawId = StripPrefix(item.Id);
                    if (!playableIds.Contains(rawId) || !seenIds.Add(rawId))
                        continue;

                    var previousId = item.Id;
                    item.Id = LivePrefix + rawId;
                    item.Name = $"🔴 LIVE: {RemoveLivePrefix(item.Name)}";
                    item.RunTimeTicks = null;
                    item.MediaSources = MakeMediaSources(rawId, true);

                    if (MetaCache.TryGetValue(previousId, out var meta)
                        || MetaCache.TryGetValue(rawId, out meta))
                    {
                        MetaCache[item.Id] = meta with
                        {
                            RuntimeTicks = null,
                            CachedAt = DateTime.UtcNow
                        };
                    }

                    result.Add(item);
                    if (result.Count >= limit) break;
                }
            }

            if (result.Count == 0)
            {
                if (allDetailLookupsSucceeded)
                {
                    ChannelContentFlags.SetHasLive(channelId, false);
                    return Msg(result, "No live or upcoming streams found.");
                }

                return Msg(result, "Live stream details are temporarily unavailable.");
            }

            ChannelContentFlags.SetHasLive(channelId, true);
            return ToResult(result);
        }

        private static HashSet<string> GetPlayableLiveVideoIds(
            JsonDocument doc,
            HashSet<string> candidateIds)
        {
            var playable = new HashSet<string>(StringComparer.Ordinal);
            if (!doc.RootElement.TryGetProperty("items", out var items)
                || items.ValueKind != JsonValueKind.Array)
                return playable;

            foreach (var detail in items.EnumerateArray())
            {
                var id = YouTubeApi.GetString(detail, "id");
                if (string.IsNullOrEmpty(id)
                    || !candidateIds.Contains(id)
                    || !IsLiveOrUpcomingVideo(detail))
                    continue;

                if (detail.TryGetProperty("status", out var status)
                    && status.ValueKind == JsonValueKind.Object)
                {
                    var privacy = YouTubeApi.GetString(status, "privacyStatus");
                    if (string.Equals(privacy, "private", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (status.TryGetProperty("embeddable", out var embeddable)
                        && embeddable.ValueKind == JsonValueKind.False)
                        continue;

                    var upload = YouTubeApi.GetString(status, "uploadStatus");
                    if (!string.IsNullOrEmpty(upload)
                        && !string.Equals(upload, "processed", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(upload, "uploaded", StringComparison.OrdinalIgnoreCase))
                        continue;
                }

                if (detail.TryGetProperty("contentDetails", out var contentDetails)
                    && contentDetails.ValueKind == JsonValueKind.Object
                    && contentDetails.TryGetProperty("contentRating", out var contentRating)
                    && contentRating.ValueKind == JsonValueKind.Object
                    && contentRating.TryGetProperty("ytRating", out var ytRating)
                    && string.Equals(
                        ytRating.GetString(),
                        "ytAgeRestricted",
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                playable.Add(id);
            }

            return playable;
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
        }

        private static bool IsChannelId(string channelId) =>
            IsSupportedChannelId(channelId);

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
                var shortsProbe = await GetChannelShortVideoIdsAsync(channelId, ct).ConfigureAwait(false);
                if (!shortsProbe.LookupSucceeded)
                    return cached?.HasShorts ?? shortsProbe.VideoIds.Count > 0;

                var hasShorts = shortsProbe.VideoIds.Count > 0;
                ChannelContentFlags.SetHasShorts(channelId, hasShorts);
                return hasShorts;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
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
                // First scrape the channel's /streams page without API quota,
                // then validate the small candidate set through videos.list so
                // stale or misleading page badges cannot create an empty folder.
                var liveProbe = await GetChannelLiveAndUpcomingIdsAsync(channelId, ct)
                    .ConfigureAwait(false);
                var liveIds = liveProbe.VideoIds;
                if (!liveProbe.LookupSucceeded && liveIds.Count == 0)
                    return cached?.HasLive ?? false;

                var hasLive = false;
                var allDetailLookupsSucceeded = true;
                foreach (var chunk in liveIds.Chunk(50))
                {
                    using var details = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, chunk, ct)
                        .ConfigureAwait(false);
                    if (details == null)
                    {
                        allDetailLookupsSucceeded = false;
                        continue;
                    }

                    if (GetPlayableLiveVideoIds(details, liveIds).Count > 0)
                    {
                        hasLive = true;
                        break;
                    }
                }

                if (!hasLive && !allDetailLookupsSucceeded)
                {
                    Log($"[YT] Live candidate validation temporarily unavailable for {channelId}; preserving cached folder state.");
                    return cached?.HasLive ?? false;
                }

                ChannelContentFlags.SetHasLive(channelId, hasLive);
                return hasLive;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log($"[YT] Live folder probe failed for {channelId}: {ex.Message}");
                return cached?.HasLive ?? false;
            }
        }
    }
}
