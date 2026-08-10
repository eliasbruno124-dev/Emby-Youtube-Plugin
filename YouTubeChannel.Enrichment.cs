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
        // True if the video has a 'shorts' tag or #shorts in title/description.
        // Shared by enrichment and trending so we don't repeat the same checks.
        private static bool HasShortsTagOrHashtag(JsonElement detail)
        {
            if (!detail.TryGetProperty("snippet", out var snip)) return false;
            if (snip.TryGetProperty("tags", out var tags) && tags.ValueKind == JsonValueKind.Array)
                foreach (var tag in tags.EnumerateArray())
                    if (string.Equals(tag.GetString()?.Trim(), "shorts", StringComparison.OrdinalIgnoreCase))
                        return true;
            var title = YouTubeApi.GetString(snip, "title") ?? "";
            var desc = YouTubeApi.GetString(snip, "description") ?? "";
            return title.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0
                || desc.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // For batches that already have full metadata (Trending / Categories),
        // run the URL probe to catch Shorts the API didn't tag for us. No extra
        // quota — just hits the cached probe results.
        internal static async Task ApplyShortsProbeUpgradeAsync(
            List<ChannelItemInfo> batch, CancellationToken ct)
        {
            if (batch == null || batch.Count == 0) return;

            var candidates = new List<(ChannelItemInfo item, string id)>();
            foreach (var item in batch)
            {
                if (item == null || string.IsNullOrEmpty(item.Id)) continue;
                if (item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)) continue;
                if (item.Id.StartsWith(LivePrefix, StringComparison.Ordinal)) continue;

                var rawId = item.Id;
                // Strip prefix if it slipped in.
                if (rawId.StartsWith(LivePrefix, StringComparison.Ordinal))
                    rawId = rawId.Substring(LivePrefix.Length);
                else if (rawId.StartsWith(ReelPrefix, StringComparison.Ordinal))
                    rawId = rawId.Substring(ReelPrefix.Length);

                // Don't bother probing things longer than 4 minutes.
                double? secs = null;
                if (item.RunTimeTicks.HasValue && item.RunTimeTicks.Value > 0)
                    secs = TimeSpan.FromTicks(item.RunTimeTicks.Value).TotalSeconds;
                else if (MetaCache.TryGetValue(item.Id, out var meta)
                         && meta.RuntimeTicks.HasValue && meta.RuntimeTicks.Value > 0)
                    secs = TimeSpan.FromTicks(meta.RuntimeTicks.Value).TotalSeconds;

                if (!secs.HasValue || secs.Value <= 0 || secs.Value > 240.0) continue;

                candidates.Add((item, rawId));
            }

            if (candidates.Count == 0) return;

            using var sem = new SemaphoreSlim(8);
            await Task.WhenAll(candidates.Select(async c =>
            {
                await sem.WaitAsync(ct).ConfigureAwait(false);
                try
                {
                    bool isShort = await IsShortByUrlProbeAsync(c.id, ct).ConfigureAwait(false);
                    if (isShort
                        && !c.item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)
                        && !c.item.Id.StartsWith(LivePrefix, StringComparison.Ordinal))
                    {
                        c.item.Id = ReelPrefix + c.id;
                        if (!c.item.Name.StartsWith("▶ Short:", StringComparison.Ordinal))
                            c.item.Name = $"▶ Short: {c.item.Name}";
                    }
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
                catch { /* probe can fail, no big deal */ }
                finally { sem.Release(); }
            })).ConfigureAwait(false);
        }

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
            CancellationToken ct,
            HashSet<string>? knownShortsIds = null)
        {
            // Remember which IDs YouTube actually returned. If a successful
            // request does not return an ID, the video is gone for this key/region
            // and we drop it.
            var foundIds = new HashSet<string>(StringComparer.Ordinal);
            // Some videos come back from the API but won't actually play in Emby:
            //   - private
            //   - upload status not processed/uploaded
            //   - embeddable=false (iframe player refuses)
            //   - duration 0s and not live (stuck transcode)
            // We toss those out so we don't end up with broken thumbnails.
            var unplayableIds = new HashSet<string>(StringComparer.Ordinal);
            // Only count an ID as missing when the chunk actually succeeded.
            // Otherwise a flaky network would look like a bunch of deleted videos.
            var queriedIds = new HashSet<string>(StringComparer.Ordinal);
            try
            {
                var serverRegion = await YouTubeApi.ResolveContentRegionAsync(
                    apiKey,
                    Plugin.Instance?.Options.TrendingRegion,
                    null,
                    ct).ConfigureAwait(false);

                // YouTube caps videos.list at 50 IDs per request.
                for (int i = 0; i < videoIds.Count; i += 50)
                {
                    var chunkList = videoIds.Skip(i).Take(50).ToList();
                    using var doc = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, chunkList, ct)
                        .ConfigureAwait(false);
                    if (doc == null) continue; // network blip, leave this chunk alone
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

                        // Warm up the URL probe cache in parallel for everything that
                        // doesn't already have a known classification. Saves us from
                        // doing the HTTP round-trips one by one in the loop below.
                        //
                        // If the caller already grabbed the channel's /shorts page
                        // (knownShortsIds), those IDs are skipped — we know they're
                        // Shorts. Everything else still gets probed because YouTube
                        // sometimes hides Shorts from the /shorts tab while still
                        // serving them as Shorts (only the redirect tells us).
                        // Probe results are persisted, so each video is hit once.
                        var probeCandidates = new List<string>();
                        foreach (var (vid, det) in detailsMap)
                        {
                            if (knownShortsIds != null && knownShortsIds.Contains(vid)) continue;
                            if (ShortsUrlProbeCache.ContainsKey(vid) || HasShortsTagOrHashtag(det)) continue;
                            var durStr = YouTubeApi.GetNestedString(det, "contentDetails", "duration");
                            var tsDet = YouTubeApi.ParseDuration(durStr);
                            if (tsDet.HasValue && tsDet.Value.TotalSeconds > 0 && tsDet.Value.TotalSeconds <= 360)
                                probeCandidates.Add(vid);
                        }
                        if (probeCandidates.Count > 0)
                        {
                            using var sem = new SemaphoreSlim(8);
                            await Task.WhenAll(probeCandidates.Select(async vid =>
                            {
                                await sem.WaitAsync(ct).ConfigureAwait(false);
                                try { await IsShortByUrlProbeAsync(vid, ct).ConfigureAwait(false); }
                                finally { sem.Release(); }
                            })).ConfigureAwait(false);
                        }

                        foreach (var batchItem in batch)
                        {
                            var rawId = batchItem.Id;
                            if (rawId.StartsWith(LivePrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(LivePrefix.Length);
                            else if (rawId.StartsWith(ReelPrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(ReelPrefix.Length);

                            if (!detailsMap.TryGetValue(rawId, out var detail)) continue;

                            // Throw out anything YouTube returned that the embedded
                            // player can't actually play.
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

                            // Age-restricted needs a logged-in YouTube account,
                            // which the embedded player doesn't have.
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

                            // Region checks only kick in if the user actually picked
                            // a region — otherwise we'd be guessing.
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
                                // Non-live with zero runtime = still processing
                                // or just unavailable.
                                unplayableIds.Add(rawId);
                                Log($"[YT] Dropping zero-duration non-live video {rawId}");
                                continue;
                            }

                            // Three ways to know it's a Short, in order of how sure we are:
                            //   1. 'shorts' tag or #shorts in metadata
                            //   2. videoId showed up on the channel's /shorts page
                            //   3. URL probe says so (only ≤4 min videos)
                            // Hard cap: anything over 240s is never a Short, even if
                            // a probe somehow lights up. Keeps long uploads out of the
                            // Shorts folder.
                            bool durationAllowsShort = ts.HasValue
                                                       && ts.Value.TotalSeconds > 0
                                                       && ts.Value.TotalSeconds <= 240.0;
                            bool isShort = durationAllowsShort && HasShortsTagOrHashtag(detail);
                            if (!isShort && durationAllowsShort && knownShortsIds != null)
                                isShort = knownShortsIds.Contains(rawId);
                            if (!isShort && durationAllowsShort)
                                isShort = await IsShortByUrlProbeAsync(rawId, ct).ConfigureAwait(false);

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

                            // Reconcile the lightweight listing with the current
                            // video details. This also removes a stale LIVE_ prefix
                            // after a broadcast has ended.
                            var isLiveOrUpcoming = IsLiveOrUpcomingVideo(detail);
                            if (isLiveOrUpcoming)
                            {
                                if (!batchItem.Id.StartsWith(LivePrefix, StringComparison.Ordinal))
                                {
                                    batchItem.Name = $"🔴 LIVE: {RemoveLivePrefix(batchItem.Name)}";
                                    batchItem.Id = LivePrefix + rawId;
                                }
                                batchItem.RunTimeTicks = null;
                                batchItem.MediaSources = MakeMediaSources(rawId, true);
                            }
                            else if (batchItem.Id.StartsWith(LivePrefix, StringComparison.Ordinal))
                            {
                                batchItem.Id = rawId;
                                batchItem.Name = RemoveLivePrefix(batchItem.Name);
                                batchItem.MediaSources = MakeMediaSources(rawId, false);
                            }

                            // Audio language hint for the watch URL.
                            var origLang = YouTubeApi.GetNestedString(detail, "snippet", "defaultAudioLanguage")
                                        ?? YouTubeApi.GetNestedString(detail, "snippet", "defaultLanguage");

                            // Cache the enriched metadata so later folder loads are cheap.
                            MetaCache[batchItem.Id] = new VideoMeta(
                                overview, premiere, premiere?.Year,
                                isLiveOrUpcoming ? null : ts?.Ticks,
                                batchItem.ImageUrl,
                                DateTime.UtcNow,
                                origLang);
                        }
                    }
                }

                EvictExpiredMetaCache();
                PersistShortsProbeCache();
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[YouTubeChannel] EnrichBatch error: {ex.Message}");
            }
            finally
            {
                // Always strip unplayable items even if a later chunk blew up.
                // Items in chunks that never finished are guarded by queriedIds,
                // so a transient failure won't accidentally remove them.
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
                        return queriedIds.Contains(raw) && !foundIds.Contains(raw);
                    });
                }
            }
        }

        private static bool IsLiveOrUpcomingVideo(JsonElement detail)
        {
            var broadcastState = YouTubeApi.GetNestedString(detail, "snippet", "liveBroadcastContent");
            if (string.Equals(broadcastState, "live", StringComparison.OrdinalIgnoreCase)
                || string.Equals(broadcastState, "upcoming", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(broadcastState, "none", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!detail.TryGetProperty("liveStreamingDetails", out var liveDetails)
                || liveDetails.ValueKind != JsonValueKind.Object)
                return false;

            if (!string.IsNullOrEmpty(YouTubeApi.GetString(liveDetails, "actualEndTime")))
                return false;

            return !string.IsNullOrEmpty(YouTubeApi.GetString(liveDetails, "concurrentViewers"))
                || !string.IsNullOrEmpty(YouTubeApi.GetString(liveDetails, "scheduledStartTime"))
                || !string.IsNullOrEmpty(YouTubeApi.GetString(liveDetails, "actualStartTime"));
        }

        private static string RemoveLivePrefix(string value)
        {
            const string prefix = "🔴 LIVE: ";
            return value.StartsWith(prefix, StringComparison.Ordinal)
                ? value.Substring(prefix.Length)
                : value;
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
            }
        }
    }
}
