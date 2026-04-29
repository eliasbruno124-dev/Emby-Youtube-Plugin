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

            while (items.Count < limit && hasMore)
            {
                ct.ThrowIfCancellationRequested();

                using var doc = await GetMediaPageAsync(apiKey, config, type, term, pageToken, ct)
                    .ConfigureAwait(false);
                if (doc == null) break;

                var batch = ExtractVideos(doc, IsPlaylistDocument(type));
                pageToken = GetNextPageToken(doc);
                hasMore = type != "search" && !string.IsNullOrEmpty(pageToken);

                if (batch.Count == 0) break;

                var videoIds = AddUniqueItemsForPage(items, batch, seenIds, limit);
                if (videoIds.Count > 0)
                    await EnrichBatch(apiKey, batch, videoIds, ct).ConfigureAwait(false);

                CacheInitialThumbnails(batch);
                ApplyCachedMeta(batch);
                ApplyFolderFilters(batch, type, config);
                items.AddRange(batch);
            }

            if (items.Count == 0)
                return Msg(items, "No results found.");

            MarkAsSeen(items);
            ScheduleSortNameFix();
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


        private static async Task<bool> ChannelHasLiveAsync(
            string apiKey, string channelId, CancellationToken ct)
        {
            if (!channelId.StartsWith(ChannelIdPrefix, StringComparison.Ordinal)
                || channelId.Length <= MinChannelIdLength)
                return false;

            try
            {
                using var doc = await YouTubeApi.GetChannelVideosAsync(apiKey, channelId, null, ct, "date")
                    .ConfigureAwait(false);
                if (doc == null) return false;

                var probeItems = ExtractVideos(doc, isPlaylist: true);
                if (probeItems.Count == 0) return false;

                var videoIds = probeItems
                    .Select(i => StripPrefix(i.Id))
                    .Distinct(StringComparer.Ordinal)
                    .Take(50)
                    .ToList();

                await EnrichBatch(apiKey, probeItems, videoIds, ct).ConfigureAwait(false);
                ApplyCachedMeta(probeItems);

                return probeItems.Any(i => i.Id.StartsWith(LivePrefix, StringComparison.Ordinal));
            }
            catch (Exception ex)
            {
                Log($"[YT] Live folder probe failed for {channelId}: {ex.Message}");
                return false;
            }
        }
    }
}
