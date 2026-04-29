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
        private static void EvictExpiredMetaCache()
        {
            if (MetaCache.Count <= MaxMetaCacheEntries) return;
            var now = DateTime.UtcNow;
            foreach (var kvp in MetaCache)
            {
                if ((now - kvp.Value.CachedAt) > MetaCacheTtl)
                    MetaCache.TryRemove(kvp.Key, out _);
            }
            if (MetaCache.Count <= MaxMetaCacheEntries) return;
            var oldest = MetaCache
                .OrderBy(kvp => kvp.Value.CachedAt)
                .Take(MetaCache.Count - MaxMetaCacheEntries)
                .Select(kvp => kvp.Key)
                .ToList();
            foreach (var key in oldest)
                MetaCache.TryRemove(key, out _);
        }

        private static async Task EnrichBatch(
            string apiKey, List<ChannelItemInfo> batch, List<string> videoIds,
            CancellationToken ct)
        {
            // Remember which IDs YouTube actually returned. If a successful
            // request does not return an ID, that video is no longer available
            // to this API key or region and should not stay in the listing.
            var foundIds = new HashSet<string>(StringComparer.Ordinal);
            // Videos can still be returned by the API while being unplayable in Emby:
            //   - status.privacyStatus = "private"
            //   - status.uploadStatus not in ["processed", "uploaded"]
            //     (i.e. "rejected", "deleted", "failed")
            //   - status.embeddable = false (the YouTube iframe player refuses to load)
            //   - duration = 0s and not a live stream (stuck transcode / unavailable)
            // Remove those items too, so Emby does not keep dead entries with
            // broken thumbnails.
            var unplayableIds = new HashSet<string>(StringComparer.Ordinal);
            // Only a successful chunk can prove that an ID is missing. If a
            // chunk fails because of the network or rate limiting, keep those
            // items; dropping them would make transient API trouble look like
            // deleted videos.
            var queriedIds = new HashSet<string>(StringComparer.Ordinal);
            try
            {
                // YouTube allows up to 50 IDs per videos.list request.
                for (int i = 0; i < videoIds.Count; i += 50)
                {
                    var chunkList = videoIds.Skip(i).Take(50).ToList();
                    using var doc = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, chunkList, ct)
                        .ConfigureAwait(false);
                    if (doc == null) continue; // Transient failure: keep this chunk untouched.
                    foreach (var qid in chunkList) queriedIds.Add(qid);

                    if (doc.RootElement.TryGetProperty("items", out var items)
                        && items.ValueKind == JsonValueKind.Array)
                    {
                        var detailsMap = new Dictionary<string, JsonElement>();
                        foreach (var item in items.EnumerateArray())
                        {
                            var id = YouTubeApi.GetString(item, "id");
                            if (!string.IsNullOrEmpty(id))
                            {
                                detailsMap[id] = item.Clone();
                                foundIds.Add(id);
                            }
                        }

                        foreach (var batchItem in batch)
                        {
                            var rawId = batchItem.Id;
                            if (rawId.StartsWith(LivePrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(LivePrefix.Length);
                            else if (rawId.StartsWith(ReelPrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(ReelPrefix.Length);

                            if (!detailsMap.TryGetValue(rawId, out var detail)) continue;

                            // Drop videos that YouTube returned but that the
                            // embedded player cannot actually play.
                            bool isLiveStream = detail.TryGetProperty("liveStreamingDetails", out _);
                            if (detail.TryGetProperty("status", out var statusEl)
                                && statusEl.ValueKind == JsonValueKind.Object)
                            {
                                var privacy = YouTubeApi.GetString(statusEl, "privacyStatus");
                                var upload = YouTubeApi.GetString(statusEl, "uploadStatus");
                                bool embeddable = true;
                                if (statusEl.TryGetProperty("embeddable", out var embEl)
                                    && embEl.ValueKind == JsonValueKind.False)
                                    embeddable = false;

                                if (string.Equals(privacy, "private", StringComparison.OrdinalIgnoreCase)
                                    || (!string.IsNullOrEmpty(upload)
                                        && !string.Equals(upload, "processed", StringComparison.OrdinalIgnoreCase)
                                        && !string.Equals(upload, "uploaded", StringComparison.OrdinalIgnoreCase))
                                    || !embeddable)
                                {
                                    unplayableIds.Add(rawId);
                                    Log($"[YT] Dropping unplayable video {rawId} (privacy={privacy}, upload={upload}, embeddable={embeddable})");
                                    continue;
                                }
                            }

                            // Age-restricted videos need a logged-in YouTube
                            // account, which the embedded Emby playback path
                            // does not have.
                            if (detail.TryGetProperty("contentDetails", out var cdEl)
                                && cdEl.ValueKind == JsonValueKind.Object
                                && cdEl.TryGetProperty("contentRating", out var crEl)
                                && crEl.ValueKind == JsonValueKind.Object
                                && crEl.TryGetProperty("ytRating", out var ytrEl)
                                && string.Equals(ytrEl.GetString(), "ytAgeRestricted", StringComparison.OrdinalIgnoreCase))
                            {
                                unplayableIds.Add(rawId);
                                Log($"[YT] Dropping age-restricted video {rawId}");
                                continue;
                            }

                            // Region checks are only safe when the user picked
                            // a region. Without that, guessing would hide videos
                            // that may be playable on the actual server.
                            var serverRegion = (Plugin.Instance?.Options.TrendingRegion ?? "").Trim();
                            if (!string.IsNullOrEmpty(serverRegion)
                                && cdEl.ValueKind == JsonValueKind.Object
                                && cdEl.TryGetProperty("regionRestriction", out var rrEl)
                                && rrEl.ValueKind == JsonValueKind.Object)
                            {
                                bool blocked = false;
                                if (rrEl.TryGetProperty("blocked", out var blockedEl)
                                    && blockedEl.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var b in blockedEl.EnumerateArray())
                                    {
                                        if (string.Equals(b.GetString(), serverRegion, StringComparison.OrdinalIgnoreCase))
                                        { blocked = true; break; }
                                    }
                                }
                                if (!blocked
                                    && rrEl.TryGetProperty("allowed", out var allowedEl)
                                    && allowedEl.ValueKind == JsonValueKind.Array)
                                {
                                    bool inAllowed = false;
                                    foreach (var a in allowedEl.EnumerateArray())
                                    {
                                        if (string.Equals(a.GetString(), serverRegion, StringComparison.OrdinalIgnoreCase))
                                        { inAllowed = true; break; }
                                    }
                                    if (!inAllowed) blocked = true;
                                }
                                if (blocked)
                                {
                                    unplayableIds.Add(rawId);
                                    Log($"[YT] Dropping region-blocked video {rawId} (region={serverRegion})");
                                    continue;
                                }
                            }

                            // Runtime.
                            var duration = YouTubeApi.GetNestedString(detail, "contentDetails", "duration");
                            var ts = YouTubeApi.ParseDuration(duration);
                            if (ts.HasValue && ts.Value.TotalSeconds > 0)
                            {
                                batchItem.RunTimeTicks = ts.Value.Ticks;
                            }
                            else if (!isLiveStream
                                  && (duration == "P0D" || duration == "PT0S"
                                      || (ts.HasValue && ts.Value.TotalSeconds == 0)))
                            {
                                // Non-live videos with a zero runtime are either
                                // still processing or unavailable.
                                unplayableIds.Add(rawId);
                                Log($"[YT] Dropping zero-duration non-live video {rawId}");
                                continue;
                            }

                            // Detect Shorts with stable signals.
                            bool isShort = false;
                            JsonElement snipEl = default;
                            bool hasSnippet = detail.TryGetProperty("snippet", out snipEl);

                            // 1. The explicit "shorts" tag is the strongest signal.
                            if (hasSnippet
                                && snipEl.TryGetProperty("tags", out var tagsEl)
                                && tagsEl.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var tag in tagsEl.EnumerateArray())
                                {
                                    var t = tag.GetString();
                                    if (t != null && string.Equals(t.Trim(), "shorts", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isShort = true;
                                        break;
                                    }
                                }
                            }

                            // 2. Creators often mark Shorts in the title or description.
                            if (!isShort && hasSnippet)
                            {
                                var sTitle = YouTubeApi.GetString(snipEl, "title") ?? "";
                                var sDesc = YouTubeApi.GetString(snipEl, "description") ?? "";
                                if (sTitle.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0
                                 || sDesc.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0)
                                    isShort = true;
                            }

                            // 3. Shorts can be up to three minutes, so the
                            // duration threshold must match that or Emby may
                            // treat the same video as two different items.
                            if (!isShort && ts.HasValue && ts.Value.TotalSeconds > 0 && ts.Value.TotalSeconds <= ReelMaxSeconds)
                                isShort = true;

                            if (isShort
                                && !batchItem.Id.StartsWith(ReelPrefix)
                                && !batchItem.Id.StartsWith(LivePrefix))
                            {
                                batchItem.Id = ReelPrefix + rawId;
                                batchItem.Name = $"▶ Short: {batchItem.Name}";
                            }

                            // Description and optional engagement stats.
                            var desc = YouTubeApi.GetNestedString(detail, "snippet", "description");
                            long? viewCount = null;
                            long? likeCount = null;
                            long? commentCount = null;
                            if (detail.TryGetProperty("statistics", out var stats))
                            {
                                if (long.TryParse(YouTubeApi.GetString(stats, "viewCount"), out var v)) viewCount = v;
                                if (long.TryParse(YouTubeApi.GetString(stats, "likeCount"), out var l)) likeCount = l;
                                if (long.TryParse(YouTubeApi.GetString(stats, "commentCount"), out var c)) commentCount = c;
                            }

                            var statsParts = new List<string>();
                            if (viewCount.HasValue) statsParts.Add($"👁 {viewCount:N0}");
                            if (likeCount.HasValue && Plugin.Instance?.Options.ShowLikeCount == true) statsParts.Add($"👍 {likeCount:N0}");
                            if (commentCount.HasValue && Plugin.Instance?.Options.ShowCommentCount == true) statsParts.Add($"💬 {commentCount:N0}");
                            string statsLine = statsParts.Count > 0 ? string.Join("  ·  ", statsParts) : "";

                            string? overview = null;
                            if (!string.IsNullOrWhiteSpace(desc))
                                overview = (statsLine.Length > 0 ? statsLine + "\n\n" : "") + desc;
                            else if (statsLine.Length > 0)
                                overview = statsLine;

                            if (!string.IsNullOrEmpty(overview))
                                batchItem.Overview = overview;

                            // Premiere date.
                            var pubStr = YouTubeApi.GetNestedString(detail, "snippet", "publishedAt");
                            var premiere = YouTubeApi.ParsePublishedAt(pubStr);
                            if (premiere.HasValue)
                            {
                                batchItem.PremiereDate = premiere;
                                batchItem.DateCreated = premiere;
                                batchItem.ProductionYear = premiere.Value.Year;
                            }

                            // Live status.
                            if (detail.TryGetProperty("liveStreamingDetails", out var lsd))
                            {
                                var concurrentViewers = YouTubeApi.GetString(lsd, "concurrentViewers");
                                if (!string.IsNullOrEmpty(concurrentViewers)
                                    && !batchItem.Id.StartsWith(LivePrefix))
                                {
                                    // This item is live right now.
                                    batchItem.Name = $"🔴 LIVE: {batchItem.Name}";
                                    batchItem.Id = LivePrefix + rawId;
                                }
                            }

                            // Original audio language for the watch URL hint.
                            var origLang = YouTubeApi.GetNestedString(detail, "snippet", "defaultAudioLanguage")
                                        ?? YouTubeApi.GetNestedString(detail, "snippet", "defaultLanguage");

                            // Keep the enriched metadata for later folder loads.
                            MetaCache[batchItem.Id] = new VideoMeta(
                                overview, premiere, premiere?.Year,
                                ts?.Ticks, batchItem.ImageUrl, DateTime.UtcNow, origLang);
                        }
                    }
                }

                EvictExpiredMetaCache();

                // A previous version probed every YouTube watch page here. That
                // made refreshes painfully slow and could false-drop videos when
                // YouTube rate-limited the probe. The API metadata above is the
                // better tradeoff for normal channel browsing.

                // Remove videos only when we know they were checked in a
                // successful chunk and YouTube either omitted them or marked them
                // as unplayable.
                if ((queriedIds.Count > 0 && foundIds.Count < queriedIds.Count)
                    || unplayableIds.Count > 0)
                {
                    batch.RemoveAll(item =>
                    {
                        var raw = item.Id;
                        if (raw.StartsWith(LivePrefix, StringComparison.Ordinal))
                            raw = raw.Substring(LivePrefix.Length);
                        else if (raw.StartsWith(ReelPrefix, StringComparison.Ordinal))
                            raw = raw.Substring(ReelPrefix.Length);
                        if (unplayableIds.Contains(raw)) return true;
                        // A missing ID only counts after a successful chunk.
                        return queriedIds.Contains(raw) && !foundIds.Contains(raw);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[YouTubeChannel] EnrichBatch error: {ex.Message}");
            }
        }

        private static void ApplyCachedMeta(List<ChannelItemInfo> batch)
        {
            foreach (var item in batch)
            {
                if (!MetaCache.TryGetValue(item.Id, out var cached)) continue;

                if (!string.IsNullOrEmpty(cached.ThumbUrl) && string.IsNullOrEmpty(item.ImageUrl))
                    item.ImageUrl = cached.ThumbUrl;

                if (!string.IsNullOrEmpty(cached.Overview))
                    item.Overview = cached.Overview;

                if (cached.Premiere.HasValue && !item.PremiereDate.HasValue)
                {
                    item.PremiereDate = cached.Premiere;
                    item.DateCreated = cached.Premiere;
                }
                if (cached.Year.HasValue && !item.ProductionYear.HasValue)
                    item.ProductionYear = cached.Year;
                if (cached.RuntimeTicks.HasValue && !item.RunTimeTicks.HasValue)
                    item.RunTimeTicks = cached.RuntimeTicks;

                // Rebuild the watch URL if enrichment discovered a language hint.
                if (!string.IsNullOrEmpty(cached.OriginalLang))
                {
                    string raw = item.Id;
                    if (raw.StartsWith(LivePrefix)) raw = raw.Substring(LivePrefix.Length);
                    else if (raw.StartsWith(ReelPrefix)) raw = raw.Substring(ReelPrefix.Length);
                    bool isLive = item.Id.StartsWith(LivePrefix);
                    item.MediaSources = MakeMediaSources(raw, isLive,
                        isLive ? null : (cached.RuntimeTicks ?? item.RunTimeTicks),
                        cached.OriginalLang);
                }
            }
        }
    }
}
