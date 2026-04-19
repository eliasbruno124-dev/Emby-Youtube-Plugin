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
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public class YouTubeChannel : IChannel, IRequiresMediaInfoCallback
    {
        public string Name => "YouTube";
        public string Description => "YouTube integration via official YouTube Data API v3.";
        public string Id => "youtube_channel_10";

        public string DataVersion => "1.0.0";
        public ChannelParentalRating ParentalRating => ChannelParentalRating.GeneralAudience;
        public bool IsEnabledByDefault => true;

        private const string ChannelIdPrefix = "UC";
        private const int MinChannelIdLength = 20;
        private const string PlaylistPrefix = "PL";
        private const string HandlePrefix = "@";
        private const string FolderSeparator = "_x_";
        private const int MaxMetaCacheEntries = 2000;

        private record VideoMeta(
            string? Overview, DateTime? Premiere, int? Year,
            long? RuntimeTicks, string? ThumbUrl, DateTime CachedAt);

        private static readonly ConcurrentDictionary<string, VideoMeta> MetaCache = new();
        private static readonly TimeSpan MetaCacheTtl = TimeSpan.FromDays(365);
        private static readonly TimeSpan NegativeCacheTtl = TimeSpan.FromHours(1);

        // ── Rate limiter for enrichment ──
        private static readonly SemaphoreSlim EnrichSemaphore = new(4, 4);
        private const int EnrichDelayMs = 200;
        private const int MaxForegroundEnrich = 0;

        private const string LivePrefix = "LIVE_";
        private const string ReelPrefix = "REEL_";
        private const int ReelMaxSeconds = 180;

        private static bool NeedsEnrichment(ChannelItemInfo item)
        {
            if (MetaCache.TryGetValue(item.Id, out var cached))
            {
                if (cached.Overview == null && (DateTime.UtcNow - cached.CachedAt) > NegativeCacheTtl)
                    return true;
                return false;
            }
            if (string.IsNullOrEmpty(item.Overview)) return true;
            if (!item.PremiereDate.HasValue || !item.RunTimeTicks.HasValue)
                return true;
            return false;
        }

        public ChannelFeatures GetChannelFeatures() => new ChannelFeatures();

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

        public async Task<ChannelItemResult> GetChannelItems(
            InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            var items = new List<ChannelItemInfo>();
            var plugin = Plugin.Instance;
            if (plugin == null)
                return Msg(items, "ERROR: Plugin not initialized.");

            var config = plugin.Options;
            var apiKey = (config.ApiKey ?? "").Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
                return Msg(items, "ERROR: Please configure a YouTube API Key in the plugin settings.");

            try
            {
                // ── Root level: build folders ──
                if (string.IsNullOrEmpty(query.FolderId))
                {
                    // Watch Later
                    var watchLater = (config.WatchLaterPlaylist ?? "").Trim();
                    if (watchLater.Length > 2)
                    {
                        var d = await YouTubeApi.GetPlaylistDetailsAsync(apiKey, watchLater, cancellationToken)
                            .ConfigureAwait(false);
                        var thumbUrl = !string.IsNullOrEmpty(d.thumb) ? d.thumb : null;
                        items.Add(new ChannelItemInfo
                        {
                            Name = "\u2B50 " + (d.name ?? "Watch Later"),
                            Id = $"playlist{FolderSeparator}{watchLater}",
                            Type = ChannelItemType.Folder,
                            ImageUrl = thumbUrl
                        });
                    }

                    // Trending
                    if (config.ShowTrending)
                        items.Add(new ChannelItemInfo
                        {
                            Name = "Trending",
                            Id = "trending_x_all",
                            Type = ChannelItemType.Folder
                        });

                    // User content sources
                    var savedItems = (config.SavedItems ?? "")
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in savedItems)
                    {
                        var term = s.Trim();
                        if (string.IsNullOrEmpty(term)) continue;
                        cancellationToken.ThrowIfCancellationRequested();

                        if (term.StartsWith(HandlePrefix))
                        {
                            var d = await YouTubeApi.GetChannelDetailsAsync(apiKey, term, true, cancellationToken)
                                .ConfigureAwait(false);
                            items.Add(new ChannelItemInfo
                            {
                                Name = d.name ?? term,
                                Id = $"channel{FolderSeparator}{d.id ?? term}",
                                Type = ChannelItemType.Folder,
                                ImageUrl = d.thumb
                            });
                        }
                        else if (term.StartsWith(ChannelIdPrefix) && term.Length > MinChannelIdLength)
                        {
                            var d = await YouTubeApi.GetChannelDetailsAsync(apiKey, term, false, cancellationToken)
                                .ConfigureAwait(false);
                            items.Add(new ChannelItemInfo
                            {
                                Name = d.name ?? "Channel",
                                Id = $"channel{FolderSeparator}{term}",
                                Type = ChannelItemType.Folder,
                                ImageUrl = d.thumb
                            });
                        }
                        else if (term.StartsWith(PlaylistPrefix))
                        {
                            var d = await YouTubeApi.GetPlaylistDetailsAsync(apiKey, term, cancellationToken)
                                .ConfigureAwait(false);
                            items.Add(new ChannelItemInfo
                            {
                                Name = d.name ?? "Playlist",
                                Id = $"playlist{FolderSeparator}{term}",
                                Type = ChannelItemType.Folder,
                                ImageUrl = d.thumb
                            });
                        }
                        else
                        {
                            items.Add(new ChannelItemInfo
                            {
                                Name = $"Search: {term}",
                                Id = $"search{FolderSeparator}{term}",
                                Type = ChannelItemType.Folder
                            });
                        }
                    }

                    return new ChannelItemResult
                    {
                        Items = items,
                        TotalRecordCount = items.Count
                    };
                }

                // ── Subfolder: load videos ──
                if (query.FolderId.Contains(FolderSeparator))
                {
                    var sepIdx = query.FolderId.IndexOf(FolderSeparator, StringComparison.Ordinal);
                    if (sepIdx < 0) return new ChannelItemResult { Items = items };
                    string type = query.FolderId.Substring(0, sepIdx);
                    string term = query.FolderId.Substring(sepIdx + FolderSeparator.Length);

                    // ── Channel → show subcategories ──
                    if (type == "channel")
                    {
                        items.Add(new ChannelItemInfo
                        {
                            Name = "📺 Videos",
                            Id = $"channelvideos{FolderSeparator}{term}",
                            Type = ChannelItemType.Folder
                        });
                        items.Add(new ChannelItemInfo
                        {
                            Name = "⚡ Shorts",
                            Id = $"channelshorts{FolderSeparator}{term}",
                            Type = ChannelItemType.Folder
                        });
                        items.Add(new ChannelItemInfo
                        {
                            Name = "🔴 Live",
                            Id = $"channellive{FolderSeparator}{term}",
                            Type = ChannelItemType.Folder
                        });
                        return new ChannelItemResult
                        {
                            Items = items,
                            TotalRecordCount = items.Count
                        };
                    }

                    if (type == "trending")
                    {
                        var trendingResult = await LoadTrending(apiKey, cancellationToken,
                            (config.TrendingRegion ?? "").Trim(),
                            (config.TrendingCategory ?? "").Trim())
                            .ConfigureAwait(false);
                        ScheduleSortNameFix();
                        return trendingResult;
                    }

                    int limit = type == "search"
                        ? ClampVideos(config.MaxSearchVideos)
                        : ClampVideos(config.MaxChannelVideos);

                    var seenIds = new HashSet<string>();
                    string? pageToken = null;
                    bool hasMore = true;

                    while (items.Count < limit && hasMore)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        JsonDocument? doc = null;
                        if (type == "search")
                            doc = await YouTubeApi.SearchVideosAsync(apiKey, term, pageToken, cancellationToken)
                                .ConfigureAwait(false);
                        else if (type == "channelvideos")
                            doc = await YouTubeApi.GetChannelVideosAsync(apiKey, term, pageToken, cancellationToken, config.ChannelSortBy)
                                .ConfigureAwait(false);
                        else if (type == "channelshorts")
                            doc = await YouTubeApi.GetChannelVideosAsync(apiKey, term, pageToken, cancellationToken, config.ChannelSortBy)
                                .ConfigureAwait(false);
                        else if (type == "channellive")
                            doc = await YouTubeApi.GetChannelLiveAsync(apiKey, term, pageToken, cancellationToken)
                                .ConfigureAwait(false);
                        else if (type == "playlist")
                            doc = await YouTubeApi.GetPlaylistVideosAsync(apiKey, term, pageToken, cancellationToken)
                                .ConfigureAwait(false);

                        if (doc == null) break;

                        // Extract videos
                        var tempItems = ExtractVideos(doc, type == "playlist");

                        // Get next page token
                        pageToken = null;
                        if (doc.RootElement.TryGetProperty("nextPageToken", out var npt))
                            pageToken = npt.GetString();
                        hasMore = !string.IsNullOrEmpty(pageToken);

                        doc.Dispose();

                        if (tempItems.Count == 0) break;

                        // Collect video IDs for batch enrichment
                        var videoIds = new List<string>();
                        var batch = new List<ChannelItemInfo>();

                        foreach (var item in tempItems)
                        {
                            var rawId = item.Id;
                            if (rawId.StartsWith(LivePrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(LivePrefix.Length);
                            else if (rawId.StartsWith(ReelPrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(ReelPrefix.Length);
                            if (seenIds.Add(rawId))
                            {
                                batch.Add(item);
                                videoIds.Add(rawId);
                            }
                            if (items.Count + batch.Count >= limit) break;
                        }

                        // Batch enrich with video details (duration, description, view count)
                        if (videoIds.Count > 0)
                        {
                            await EnrichBatch(apiKey, batch, videoIds, cancellationToken)
                                .ConfigureAwait(false);
                        }

                        foreach (var item in batch)
                        {
                            if (!string.IsNullOrEmpty(item.ImageUrl))
                            {
                                var cacheId = item.Id;
                                if (MetaCache.TryGetValue(cacheId, out var existing))
                                {
                                    if (string.IsNullOrEmpty(existing.ThumbUrl))
                                        MetaCache[cacheId] = existing with { ThumbUrl = item.ImageUrl };
                                }
                                else
                                {
                                    MetaCache[cacheId] = new VideoMeta(null, null, null, null, item.ImageUrl, DateTime.UtcNow);
                                }
                            }
                        }

                        ApplyCachedMeta(batch);

                        // Filter: channelvideos = only regular videos, channelshorts = only shorts
                        if (type == "channelvideos")
                            batch.RemoveAll(item => item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)
                                                 || item.Id.StartsWith(LivePrefix, StringComparison.Ordinal));
                        else if (type == "channelshorts")
                            batch.RemoveAll(item => !item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));
                        else if (type == "channellive")
                            batch.RemoveAll(item => !item.Id.StartsWith(LivePrefix, StringComparison.Ordinal));

                        foreach (var item in batch)
                            items.Add(item);

                        if (items.Count >= limit) break;
                    }

                    if (items.Count == 0)
                        return Msg(items, "No results found.");

                    ScheduleSortNameFix();
                    return new ChannelItemResult
                    {
                        Items = items,
                        TotalRecordCount = items.Count
                    };
                }

                return new ChannelItemResult { Items = items };
            }
            catch (Exception ex)
            {
                return Msg(items, $"ERROR: {ex.Message}");
            }
        }

        // ── Batch enrichment using videos.list ──
        private static async Task EnrichBatch(
            string apiKey, List<ChannelItemInfo> batch, List<string> videoIds,
            CancellationToken ct)
        {
            try
            {
                // YouTube API allows up to 50 IDs per request
                for (int i = 0; i < videoIds.Count; i += 50)
                {
                    var chunk = videoIds.Skip(i).Take(50);
                    using var doc = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, chunk, ct)
                        .ConfigureAwait(false);
                    if (doc == null) continue;

                    if (doc.RootElement.TryGetProperty("items", out var items)
                        && items.ValueKind == JsonValueKind.Array)
                    {
                        var detailsMap = new Dictionary<string, JsonElement>();
                        foreach (var item in items.EnumerateArray())
                        {
                            var id = YouTubeApi.GetString(item, "id");
                            if (!string.IsNullOrEmpty(id))
                                detailsMap[id] = item.Clone();
                        }

                        foreach (var batchItem in batch)
                        {
                            var rawId = batchItem.Id;
                            if (rawId.StartsWith(LivePrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(LivePrefix.Length);
                            else if (rawId.StartsWith(ReelPrefix, StringComparison.Ordinal))
                                rawId = rawId.Substring(ReelPrefix.Length);

                            if (!detailsMap.TryGetValue(rawId, out var detail)) continue;

                            // Duration
                            var duration = YouTubeApi.GetNestedString(detail, "contentDetails", "duration");
                            var ts = YouTubeApi.ParseDuration(duration);
                            if (ts.HasValue && ts.Value.TotalSeconds > 0)
                            {
                                batchItem.RunTimeTicks = ts.Value.Ticks;

                                // Detect Shorts by duration
                                bool isCurrentlyReel = batchItem.Id.StartsWith(ReelPrefix);
                                if (!isCurrentlyReel && ts.Value.TotalSeconds <= ReelMaxSeconds
                                    && !batchItem.Id.StartsWith(LivePrefix))
                                {
                                    // Check dimension hints from contentDetails
                                    var def = YouTubeApi.GetNestedString(detail, "contentDetails", "definition");
                                    // We'll do dimension check at playback time
                                }
                            }

                            // Description + view count
                            var desc = YouTubeApi.GetNestedString(detail, "snippet", "description");
                            long? viewCount = null;
                            if (detail.TryGetProperty("statistics", out var stats))
                            {
                                var vc = YouTubeApi.GetString(stats, "viewCount");
                                if (long.TryParse(vc, out var v)) viewCount = v;
                            }

                            string? overview = null;
                            if (!string.IsNullOrWhiteSpace(desc))
                                overview = (viewCount > 0 ? $"{viewCount:N0} views\n\n" : "") + desc;
                            else if (viewCount > 0)
                                overview = $"{viewCount:N0} views";

                            if (!string.IsNullOrEmpty(overview))
                                batchItem.Overview = overview;

                            // Premiere date
                            var pubStr = YouTubeApi.GetNestedString(detail, "snippet", "publishedAt");
                            var premiere = YouTubeApi.ParsePublishedAt(pubStr);
                            if (premiere.HasValue)
                            {
                                batchItem.PremiereDate = premiere;
                                batchItem.DateCreated = premiere;
                                batchItem.ProductionYear = premiere.Value.Year;
                            }

                            // Live status
                            if (detail.TryGetProperty("liveStreamingDetails", out var lsd))
                            {
                                var concurrentViewers = YouTubeApi.GetString(lsd, "concurrentViewers");
                                if (!string.IsNullOrEmpty(concurrentViewers)
                                    && !batchItem.Id.StartsWith(LivePrefix))
                                {
                                    // This is currently live
                                    batchItem.Name = $"🔴 LIVE: {batchItem.Name}";
                                    batchItem.Id = LivePrefix + rawId;
                                }
                            }

                            // Cache
                            MetaCache[batchItem.Id] = new VideoMeta(
                                overview, premiere, premiere?.Year,
                                ts?.Ticks, batchItem.ImageUrl, DateTime.UtcNow);
                        }
                    }
                }

                EvictExpiredMetaCache();
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
            }
        }

        // ── Trending ──
        private static async Task<ChannelItemResult> LoadTrending(
            string apiKey, CancellationToken ct, string region = "", string category = "")
        {
            var allVideos = new List<ChannelItemInfo>();
            var seenIds = new HashSet<string>();

            try
            {
                // Get trending for multiple categories
                string?[] categories = string.IsNullOrEmpty(category)
                    ? new string?[] { null, "10", "20", "1" } // All, Music, Gaming, Film
                    : new string?[] { category };

                foreach (var cat in categories)
                {
                    ct.ThrowIfCancellationRequested();
                    string? reg = string.IsNullOrEmpty(region) ? null : region;

                    using var doc = await YouTubeApi.GetTrendingAsync(apiKey, reg, cat, ct)
                        .ConfigureAwait(false);
                    if (doc == null) continue;

                    var videos = ExtractTrendingVideos(doc);
                    foreach (var v in videos)
                    {
                        var rawId = v.Id;
                        if (rawId.StartsWith(LivePrefix)) rawId = rawId.Substring(LivePrefix.Length);
                        if (seenIds.Add(rawId)) allVideos.Add(v);
                    }
                }
            }
            catch (Exception ex)
            {
                return Msg(new List<ChannelItemInfo>(), $"ERROR: {ex.Message}");
            }

            if (allVideos.Count == 0)
                return Msg(new List<ChannelItemInfo>(), "No results.");

            return new ChannelItemResult
            {
                Items = allVideos,
                TotalRecordCount = allVideos.Count
            };
        }

        // ── Extract trending videos (from videos.list which has full details) ──
        private static List<ChannelItemInfo> ExtractTrendingVideos(JsonDocument doc)
        {
            var list = new List<ChannelItemInfo>();
            if (!doc.RootElement.TryGetProperty("items", out var items)
                || items.ValueKind != JsonValueKind.Array)
                return list;

            foreach (var el in items.EnumerateArray())
            {
                var videoId = YouTubeApi.GetString(el, "id");
                if (string.IsNullOrWhiteSpace(videoId)) continue;

                var title = YouTubeApi.GetNestedString(el, "snippet", "title") ?? "Untitled";
                var author = YouTubeApi.GetNestedString(el, "snippet", "channelTitle") ?? "Unknown";
                var desc = YouTubeApi.GetNestedString(el, "snippet", "description");
                var pubStr = YouTubeApi.GetNestedString(el, "snippet", "publishedAt");
                var premiere = YouTubeApi.ParsePublishedAt(pubStr);
                var thumb = YouTubeApi.GetBestThumbnail(el)
                            ?? $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";

                // Duration
                var duration = YouTubeApi.GetNestedString(el, "contentDetails", "duration");
                var ts = YouTubeApi.ParseDuration(duration);

                // View count
                long? viewCount = null;
                if (el.TryGetProperty("statistics", out var stats))
                {
                    var vc = YouTubeApi.GetString(stats, "viewCount");
                    if (long.TryParse(vc, out var v)) viewCount = v;
                }

                string? overview = null;
                if (!string.IsNullOrWhiteSpace(desc))
                    overview = (viewCount > 0 ? $"{viewCount:N0} views\n\n" : "") + desc;
                else if (viewCount > 0)
                    overview = $"{viewCount:N0} views";

                // Live detection
                bool isLive = false;
                if (el.TryGetProperty("liveStreamingDetails", out var lsd))
                {
                    var concurrentViewers = YouTubeApi.GetString(lsd, "concurrentViewers");
                    if (!string.IsNullOrEmpty(concurrentViewers))
                        isLive = true;
                }

                // Shorts detection
                bool isReel = !isLive && ts.HasValue && ts.Value.TotalSeconds > 0 && ts.Value.TotalSeconds <= ReelMaxSeconds;

                string itemId = isLive ? LivePrefix + videoId
                    : isReel ? ReelPrefix + videoId
                    : videoId;
                string displayTitle = isLive ? $"🔴 LIVE: {title}"
                    : isReel ? $"▶ Short: {title}"
                    : title;

                var info = new ChannelItemInfo
                {
                    Name = displayTitle,
                    SeriesName = author,
                    Studios = new List<string> { author },
                    Overview = overview,
                    ProductionYear = premiere?.Year,
                    DateCreated = premiere,
                    PremiereDate = premiere,
                    RunTimeTicks = isLive ? null : ts?.Ticks,
                    ContentType = MediaBrowser.Model.Channels.ChannelMediaContentType.Episode,
                    Id = itemId,
                    Type = ChannelItemType.Media,
                    MediaType = MediaBrowser.Model.Channels.ChannelMediaType.Video,
                    ImageUrl = thumb
                };

                MetaCache[itemId] = new VideoMeta(overview, premiere, premiere?.Year, ts?.Ticks, thumb, DateTime.UtcNow);
                list.Add(info);
            }
            return list;
        }

        // ── Extract videos from search/channel/playlist results ──
        private static List<ChannelItemInfo> ExtractVideos(JsonDocument doc, bool isPlaylist = false)
        {
            var list = new List<ChannelItemInfo>();
            if (!doc.RootElement.TryGetProperty("items", out var items)
                || items.ValueKind != JsonValueKind.Array)
                return list;

            foreach (var el in items.EnumerateArray())
            {
                string? videoId = null;

                if (isPlaylist)
                {
                    videoId = YouTubeApi.GetNestedString(el, "contentDetails", "videoId")
                              ?? YouTubeApi.GetNestedString(el, "snippet", "resourceId.videoId");

                    // Nested resourceId
                    if (string.IsNullOrEmpty(videoId)
                        && el.TryGetProperty("snippet", out var snip)
                        && snip.TryGetProperty("resourceId", out var rid))
                    {
                        videoId = YouTubeApi.GetString(rid, "videoId");
                    }
                }
                else
                {
                    // Search results have id.videoId
                    if (el.TryGetProperty("id", out var idProp))
                    {
                        if (idProp.ValueKind == JsonValueKind.Object)
                            videoId = YouTubeApi.GetString(idProp, "videoId");
                        else if (idProp.ValueKind == JsonValueKind.String)
                            videoId = idProp.GetString();
                    }
                }

                if (string.IsNullOrWhiteSpace(videoId)) continue;

                var title = YouTubeApi.GetNestedString(el, "snippet", "title") ?? "Untitled";
                var author = YouTubeApi.GetNestedString(el, "snippet", "channelTitle") ?? "Unknown";
                var desc = YouTubeApi.GetNestedString(el, "snippet", "description");
                var pubStr = YouTubeApi.GetNestedString(el, "snippet", "publishedAt");
                var premiere = YouTubeApi.ParsePublishedAt(pubStr);
                var thumb = YouTubeApi.GetBestThumbnail(el)
                            ?? $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";

                // Live badge from snippet
                var liveBroadcastContent = YouTubeApi.GetNestedString(el, "snippet", "liveBroadcastContent");
                bool isLive = liveBroadcastContent == "live" || liveBroadcastContent == "upcoming";

                string? overview = !string.IsNullOrWhiteSpace(desc) ? desc : null;

                string itemId = isLive ? LivePrefix + videoId : videoId;
                string displayTitle = isLive ? $"🔴 LIVE: {title}" : title;

                var info = new ChannelItemInfo
                {
                    Name = displayTitle,
                    SeriesName = author,
                    Studios = new List<string> { author },
                    Overview = overview,
                    ProductionYear = premiere?.Year,
                    DateCreated = premiere,
                    PremiereDate = premiere,
                    ContentType = MediaBrowser.Model.Channels.ChannelMediaContentType.Episode,
                    Id = itemId,
                    Type = ChannelItemType.Media,
                    MediaType = MediaBrowser.Model.Channels.ChannelMediaType.Video,
                    ImageUrl = thumb
                };

                list.Add(info);
            }
            return list;
        }

        // ── Media Playback ──
        // Returns YouTube watch URL for direct play by Emby client (same approach as Trailers plugin).
        public Task<IEnumerable<MediaSourceInfo>> GetChannelItemMediaInfo(
            string id, CancellationToken cancellationToken)
        {
            bool isLive = id.StartsWith(LivePrefix, StringComparison.Ordinal);
            bool isReel = !isLive && id.StartsWith(ReelPrefix, StringComparison.Ordinal);
            string videoId = isLive ? id.Substring(LivePrefix.Length)
                : isReel ? id.Substring(ReelPrefix.Length)
                : id;

            var sources = new List<MediaSourceInfo>
            {
                new MediaSourceInfo
                {
                    Id = videoId,
                    Path = $"https://www.youtube.com/watch?v={videoId}",
                    Protocol = MediaProtocol.Http,
                    IsRemote = false,
                    SupportsTranscoding = false,
                    SupportsDirectStream = false,
                    SupportsDirectPlay = true,
                    IsInfiniteStream = isLive,
                    RequiresOpening = false,
                    RequiresClosing = false,
                    RequiresLooping = false,
                    SupportsProbing = false,
                }
            };

            return Task.FromResult<IEnumerable<MediaSourceInfo>>(sources);
        }

        private static int ClampVideos(int val) => Math.Clamp(val, 1, 150);

        public IEnumerable<ImageType> GetSupportedChannelImages() =>
            new List<ImageType> { ImageType.Thumb, ImageType.Primary };

        public Task<DynamicImageResponse> GetChannelImage(
            ImageType type, CancellationToken cancellationToken)
        {
            var response = new DynamicImageResponse();
            var t = GetType();
            var stream = t.Assembly.GetManifestResourceStream(t.Namespace + ".thumb.png");
            if (stream != null)
            {
                response.Format = ImageFormat.Png;
                response.Stream = stream;
                return Task.FromResult(response);
            }
            var assemblyDir = Path.GetDirectoryName(t.Assembly.Location) ?? "";
            var filePath = Path.Combine(assemblyDir, "thumb.png");
            if (File.Exists(filePath))
            {
                response.Format = ImageFormat.Png;
                response.Path = filePath;
            }
            return Task.FromResult(response);
        }

        private static ChannelItemResult Msg(List<ChannelItemInfo> items, string msg)
        {
            items.Add(new ChannelItemInfo
            {
                Name = msg,
                Id = "msg",
                Type = ChannelItemType.Folder
            });
            return new ChannelItemResult
            {
                Items = items,
                TotalRecordCount = items.Count
            };
        }

        // ── SortName fix: default sort = newest first ──
        [DllImport("sqlite3")] private static extern int sqlite3_open(string filename, out IntPtr db);
        [DllImport("sqlite3")] private static extern int sqlite3_exec(IntPtr db, string sql, IntPtr cb, IntPtr arg, out IntPtr errmsg);
        [DllImport("sqlite3")] private static extern int sqlite3_close(IntPtr db);
        [DllImport("sqlite3")] private static extern void sqlite3_free(IntPtr ptr);

        private static int _sortFixScheduled;

        internal static void ScheduleSortNameFix()
        {
            if (Interlocked.CompareExchange(ref _sortFixScheduled, 1, 0) != 0) return;
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(15_000).ConfigureAwait(false);
                    FixSortNames();
                }
                catch { }
                finally { Interlocked.Exchange(ref _sortFixScheduled, 0); }
            });
        }

        private static void FixSortNames()
        {
            var dbPath = Plugin.LibraryDbPath;
            if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath)) return;

            if (sqlite3_open(dbPath, out var db) != 0) return;
            try
            {
                sqlite3_exec(db, "PRAGMA busy_timeout = 5000;", IntPtr.Zero, IntPtr.Zero, out _);

                const string sql = @"
                    UPDATE MediaItems
                    SET SortName = printf('%010d', 9999999999 - COALESCE(PremiereDate, DateCreated, 0))
                                   || ' ' || SortName
                    WHERE type = 8
                      AND PremiereDate IS NOT NULL
                      AND ExternalId IS NOT NULL
                      AND (length(ExternalId) = 11 OR ExternalId LIKE 'LIVE_%' OR ExternalId LIKE 'REEL_%')
                      AND SortName NOT GLOB '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9] *'";

                sqlite3_exec(db, sql, IntPtr.Zero, IntPtr.Zero, out var errmsg);
                if (errmsg != IntPtr.Zero) sqlite3_free(errmsg);
            }
            finally { sqlite3_close(db); }
        }
    }
}
