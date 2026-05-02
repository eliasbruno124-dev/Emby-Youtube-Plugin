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
        private static async Task<ChannelItemResult> LoadRecentlyAdded(
            string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            var perChannel = Math.Clamp(config.RecentlyAddedPerChannel, 1, 25);
            var savedItems = (config.SavedItems ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Resolve handles once, then work with channel IDs from here on.
            var channelIds = new List<string>();
            foreach (var raw in savedItems)
            {
                var term = raw.Trim();
                if (string.IsNullOrEmpty(term)) continue;
                if (term.StartsWith(HandlePrefix))
                {
                    var d = await YouTubeApi.GetChannelDetailsAsync(apiKey, term, true, ct).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(d.id)) channelIds.Add(d.id!);
                }
                else if (term.StartsWith(ChannelIdPrefix) && term.Length > MinChannelIdLength)
                {
                    channelIds.Add(term);
                }
            }

            var allItems = new List<ChannelItemInfo>();
            var seen = new HashSet<string>();
            // Union of all channels' /shorts and /streams page lists, used to
            // short-circuit the per-video URL probe and the API liveStreamingDetails
            // signal inside EnrichBatch.
            var allShortsIds = new HashSet<string>(StringComparer.Ordinal);
            var allLiveIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var channelId in channelIds)
            {
                ct.ThrowIfCancellationRequested();
                using var doc = await YouTubeApi.GetChannelVideosAsync(apiKey, channelId, null, ct, "date")
                    .ConfigureAwait(false);
                if (doc == null) continue;

                var items = ExtractVideos(doc, isPlaylist: true);
                var shortsVideoIds = await GetChannelShortVideoIdsAsync(channelId, ct).ConfigureAwait(false);
                ApplyShortsPageMatches(items, shortsVideoIds);
                foreach (var sid in shortsVideoIds) allShortsIds.Add(sid);

                var liveVideoIds = await GetChannelLiveAndUpcomingIdsAsync(channelId, ct).ConfigureAwait(false);
                foreach (var lid in liveVideoIds) allLiveIds.Add(lid);

                int taken = 0;
                foreach (var v in items)
                {
                    if (taken >= perChannel) break;
                    var rawId = v.Id;
                    if (rawId.StartsWith(LivePrefix)) rawId = rawId.Substring(LivePrefix.Length);
                    else if (rawId.StartsWith(ReelPrefix)) rawId = rawId.Substring(ReelPrefix.Length);
                    if (seen.Add(rawId))
                    {
                        allItems.Add(v);
                        taken++;
                    }
                }
            }

            // Enrich in batches. One videos.list call can cover up to 50 IDs.
            if (allItems.Count > 0)
            {
                var ids = allItems.Select(i =>
                {
                    var r = i.Id;
                    if (r.StartsWith(LivePrefix)) return r.Substring(LivePrefix.Length);
                    if (r.StartsWith(ReelPrefix)) return r.Substring(ReelPrefix.Length);
                    return r;
                }).ToList();
                await EnrichBatch(apiKey, allItems, ids, ct, allShortsIds, allLiveIds).ConfigureAwait(false);
                ApplyCachedMeta(allItems);
            }

            if (!config.ShortsEnabled)
                allItems.RemoveAll(i => i.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));

            // Show the newest item first.
            var sorted = allItems
                .OrderByDescending(i => i.PremiereDate ?? i.DateCreated ?? DateTimeOffset.MinValue)
                .ToList();

            if (sorted.Count == 0)
                return Msg(new List<ChannelItemInfo>(), "No videos yet.");

            return new ChannelItemResult
            {
                Items = sorted,
                TotalRecordCount = sorted.Count
            };
        }

        // Popular videos, optionally narrowed down by region or category.
        private static async Task<ChannelItemResult> LoadTrending(
            string apiKey, CancellationToken ct, string region = "", string category = "")
        {
            var allVideos = new List<ChannelItemInfo>();
            var seenIds = new HashSet<string>(StringComparer.Ordinal);

            var cfg = Plugin.Instance?.Options;
            const int target = 50;
            bool showShorts = cfg?.ShortsEnabled != false;
            bool isCategoryChild = !string.IsNullOrEmpty(category);

            void TryAdd(ChannelItemInfo v)
            {
                var rawId = StripPrefix(v.Id);
                if (!seenIds.Add(rawId)) return;
                if (!showShorts && v.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)) return;
                allVideos.Add(v);
            }

            try
            {
                if (!isCategoryChild)
                {
                    // Each bucket is one cheap videos.list call (not search.list).
                    string?[] buckets = new string?[] { null, "10", "20", "24", "17", "22", "23", "28", "1" };
                    foreach (var cat in buckets)
                    {
                        if (allVideos.Count >= target) break;
                        ct.ThrowIfCancellationRequested();
                        string? reg = string.IsNullOrEmpty(region) ? null : region;
                        using var doc = await YouTubeApi.GetTrendingAsync(apiKey, reg, cat, ct)
                            .ConfigureAwait(false);
                        if (doc == null) continue;
                        foreach (var v in ExtractTrendingVideos(doc))
                        {
                            TryAdd(v);
                            if (allVideos.Count >= target) break;
                        }
                    }
                }
                else
                {
                    ct.ThrowIfCancellationRequested();
                    string? reg = string.IsNullOrEmpty(region) ? null : region;
                    using var doc = await YouTubeApi.GetTrendingAsync(apiKey, reg, category, ct)
                        .ConfigureAwait(false);
                    if (doc != null)
                    {
                        foreach (var v in ExtractTrendingVideos(doc))
                        {
                            TryAdd(v);
                            if (allVideos.Count >= target) break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Msg(new List<ChannelItemInfo>(), $"ERROR: {ex.Message}");
            }

            if (allVideos.Count == 0)
                return Msg(new List<ChannelItemInfo>(), "No results.");

            // Local Shorts upgrade via URL probe (no extra API quota).
            // ExtractTrendingVideos only catches Shorts via tag/hashtag, so this
            // flips the rest so the Shorts folder filter (and the user's
            // ShortsEnabled toggle) actually work in here too.
            try
            {
                await ApplyShortsProbeUpgradeAsync(allVideos, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log($"[YT] Trending Shorts upgrade failed: {ex.Message}");
            }

            // Re-apply the ShortsEnabled filter in case the probe just promoted
            // something to a Short while the user has Shorts off.
            if (!showShorts)
                allVideos.RemoveAll(v => v.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));

            if (allVideos.Count == 0)
                return Msg(new List<ChannelItemInfo>(), "No results.");

            return new ChannelItemResult
            {
                Items = allVideos,
                TotalRecordCount = allVideos.Count
            };
        }

    }
}
