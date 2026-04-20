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
    public class YouTubeChannel : IChannel, IDisableMediaSourceDisplay
    {
        private static void Log(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
            try { File.AppendAllText("/config/data/youtube-debug.log", DateTime.UtcNow.ToString("o") + " " + msg + "\n"); } catch { }
        }

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
            long? RuntimeTicks, string? ThumbUrl, DateTime CachedAt,
            string? OriginalLang = null);

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

        private static List<MediaSourceInfo> MakeMediaSources(string videoId, bool isLive = false, long? runTimeTicks = null, string? originalLang = null)
        {
            string hl = ResolveHl(originalLang);
            string url = $"https://www.youtube.com/watch?v={videoId}";
            if (!string.IsNullOrEmpty(hl)) url += $"&hl={hl}&persist_hl=1";
            return new List<MediaSourceInfo>
            {
                new MediaSourceInfo
                {
                    Id = videoId,
                    Path = url,
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
                    RunTimeTicks = isLive ? null : runTimeTicks,
                }
            };
        }

        // ── Resolve &hl= value based on plugin config + per-video original language ──
        private static string ResolveHl(string? originalLang)
        {
            var hint = (Plugin.Instance?.Options?.PlayerLanguageHint ?? "").Trim();
            if (string.IsNullOrEmpty(hint) || hint.Equals("off", StringComparison.OrdinalIgnoreCase))
                return "";
            if (hint.Equals("original", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(originalLang)) return "";
                // Reduce to primary subtag: "de-DE" -> "de"
                int dash = originalLang.IndexOf('-');
                return dash > 0 ? originalLang.Substring(0, dash).ToLowerInvariant() : originalLang.ToLowerInvariant();
            }
            return hint.ToLowerInvariant();
        }

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

            Log($"[YT] GetChannelItems called. FolderId={query.FolderId ?? "(root)"}, ApiKey={!string.IsNullOrEmpty(apiKey)}, SavedItems={config.SavedItems ?? "(empty)"}");

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
                        var thumbUrl = !string.IsNullOrEmpty(d.thumb) ? d.thumb : FolderIcons.WatchLater;
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
                            Name = "🔥 Trending",
                            Id = "trending_x_all",
                            Type = ChannelItemType.Folder,
                            ImageUrl = FolderIcons.Trending
                        });

                    // Categories Browser
                    if (config.ShowCategories)
                        items.Add(new ChannelItemInfo
                        {
                            Name = "📂 Categories",
                            Id = "categories_x_root",
                            Type = ChannelItemType.Folder,
                            ImageUrl = FolderIcons.Categories
                        });

                    // Recently Added (mix newest from all channels)
                    if (config.ShowRecentlyAdded
                        && !string.IsNullOrWhiteSpace(config.SavedItems))
                        items.Add(new ChannelItemInfo
                        {
                            Name = "🆕 Recently Added",
                            Id = "recent_x_all",
                            Type = ChannelItemType.Folder,
                            ImageUrl = FolderIcons.RecentlyAdded
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
                            Log($"[YT] Loading handle: {term}");
                            var d = await YouTubeApi.GetChannelDetailsAsync(apiKey, term, true, cancellationToken)
                                .ConfigureAwait(false);
                            Log($"[YT] Handle {term} resolved to id={d.id}, name={d.name}");
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
                                Type = ChannelItemType.Folder,
                                ImageUrl = FolderIcons.Search
                            });
                        }
                    }

                    Log($"[YT] Root level: returning {items.Count} items");
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
                        // Try to use the channel's avatar for prettier sub-folder thumbnails.
                        string? channelThumb = null;
                        try
                        {
                            var d = await YouTubeApi.GetChannelDetailsAsync(
                                apiKey, term, term.StartsWith(HandlePrefix), cancellationToken)
                                .ConfigureAwait(false);
                            channelThumb = d.thumb;
                        }
                        catch { }

                        items.Add(new ChannelItemInfo
                        {
                            Name = "📺 Videos",
                            Id = $"channelvideos{FolderSeparator}{term}",
                            Type = ChannelItemType.Folder,
                            ImageUrl = channelThumb ?? FolderIcons.Videos
                        });
                        items.Add(new ChannelItemInfo
                        {
                            Name = "⚡ Shorts",
                            Id = $"channelshorts{FolderSeparator}{term}",
                            Type = ChannelItemType.Folder,
                            ImageUrl = channelThumb ?? FolderIcons.Shorts
                        });

                        if (config.ShowLiveFolders)
                        {
                            items.Add(new ChannelItemInfo
                            {
                                Name = "🔴 Live & Upcoming",
                                Id = $"channellive{FolderSeparator}{term}",
                                Type = ChannelItemType.Folder,
                                ImageUrl = channelThumb ?? FolderIcons.Live
                            });
                        }

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

                    // ── Categories browser root ──
                    if (type == "categories" && term == "root")
                    {
                        var region = string.IsNullOrWhiteSpace(config.TrendingRegion) ? "US" : config.TrendingRegion.Trim();
                        using var catDoc = await YouTubeApi.GetVideoCategoriesAsync(apiKey, region, cancellationToken)
                            .ConfigureAwait(false);
                        if (catDoc != null && catDoc.RootElement.TryGetProperty("items", out var catItems)
                            && catItems.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var c in catItems.EnumerateArray())
                            {
                                var cid = YouTubeApi.GetString(c, "id");
                                var cname = YouTubeApi.GetNestedString(c, "snippet", "title");
                                if (string.IsNullOrEmpty(cid) || string.IsNullOrEmpty(cname)) continue;
                                // Skip non-assignable categories (no trending available)
                                if (c.TryGetProperty("snippet", out var sn)
                                    && sn.TryGetProperty("assignable", out var ass)
                                    && ass.ValueKind == JsonValueKind.False) continue;
                                items.Add(new ChannelItemInfo
                                {
                                    Name = cname,
                                    Id = $"category{FolderSeparator}{cid}",
                                    Type = ChannelItemType.Folder,
                                    ImageUrl = FolderIcons.ForCategory(cid)
                                });
                            }
                        }
                        if (items.Count == 0) return Msg(items, "No categories available.");
                        return new ChannelItemResult { Items = items, TotalRecordCount = items.Count };
                    }

                    // ── Single category trending ──
                    if (type == "category")
                    {
                        var region = string.IsNullOrWhiteSpace(config.TrendingRegion) ? "US" : config.TrendingRegion.Trim();
                        var catResult = await LoadTrending(apiKey, cancellationToken, region, term).ConfigureAwait(false);
                        ScheduleSortNameFix();
                        return catResult;
                    }

                    // ── Recently Added: newest videos across all saved channels ──
                    if (type == "recent" && term == "all")
                    {
                        var recentResult = await LoadRecentlyAdded(apiKey, config, cancellationToken)
                            .ConfigureAwait(false);
                        ScheduleSortNameFix();
                        return recentResult;
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
                        {
                            // Use uploads playlist (cheap: 1 quota unit) — shorts filtered out after enrichment
                            doc = await YouTubeApi.GetChannelVideosAsync(apiKey, term, pageToken, cancellationToken, config.ChannelSortBy)
                                .ConfigureAwait(false);
                        }
                        else if (type == "channelshorts")
                        {
                            // Use uploads playlist (same as channelvideos) — shorts filtered by REEL_ prefix after enrichment
                            doc = await YouTubeApi.GetChannelVideosAsync(apiKey, term, pageToken, cancellationToken, config.ChannelSortBy)
                                .ConfigureAwait(false);
                        }
                        else if (type == "channellive")
                            doc = await YouTubeApi.GetChannelLiveAsync(apiKey, term, pageToken, cancellationToken)
                                .ConfigureAwait(false);
                        else if (type == "playlist")
                            doc = await YouTubeApi.GetPlaylistVideosAsync(apiKey, term, pageToken, cancellationToken)
                                .ConfigureAwait(false);

                        if (doc == null) break;

                        // Extract videos
                        bool isPlaylistFormat = type == "playlist" || type == "channelvideos" || type == "channelshorts";
                        var tempItems = ExtractVideos(doc, isPlaylistFormat);

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

                        // Post-filter: channelvideos removes shorts/live, channellive keeps only live
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

                    // ── Append upcoming streams to Live folder ──
                    if (type == "channellive" && items.Count < limit)
                    {
                        try
                        {
                            using var upDoc = await YouTubeApi.GetChannelUpcomingAsync(apiKey, term, null, cancellationToken)
                                .ConfigureAwait(false);
                            if (upDoc != null)
                            {
                                var ups = ExtractVideos(upDoc, isPlaylist: false);
                                foreach (var u in ups)
                                {
                                    var rawId = u.Id.StartsWith(LivePrefix) ? u.Id.Substring(LivePrefix.Length) : u.Id;
                                    if (!seenIds.Add(rawId)) continue;
                                    u.Name = u.Name.StartsWith("🔴 LIVE:")
                                        ? u.Name.Replace("🔴 LIVE:", "📅 UPCOMING:")
                                        : "📅 UPCOMING: " + u.Name;
                                    items.Add(u);
                                    if (items.Count >= limit) break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log($"[YT] Upcoming fetch failed: {ex.Message}");
                        }
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
                Log($"[YT] GetChannelItems error: {ex}");
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
                            }

                            // Detect Shorts: only use reliable signals
                            bool isShort = false;
                            JsonElement snipEl = default;
                            bool hasSnippet = detail.TryGetProperty("snippet", out snipEl);

                            // 1. Check tags for exact "shorts" tag (most reliable)
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

                            // 2. #shorts hashtag in title or description (very reliable)
                            if (!isShort && hasSnippet)
                            {
                                var sTitle = YouTubeApi.GetString(snipEl, "title") ?? "";
                                var sDesc = YouTubeApi.GetString(snipEl, "description") ?? "";
                                if (sTitle.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0
                                 || sDesc.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0)
                                    isShort = true;
                            }

                            // 3. Duration ≤ 60s → very likely a Short
                            if (!isShort && ts.HasValue && ts.Value.TotalSeconds > 0 && ts.Value.TotalSeconds <= 60)
                                isShort = true;

                            if (isShort
                                && !batchItem.Id.StartsWith(ReelPrefix)
                                && !batchItem.Id.StartsWith(LivePrefix))
                            {
                                batchItem.Id = ReelPrefix + rawId;
                                batchItem.Name = $"▶ Short: {batchItem.Name}";
                            }

                            // Description + view count + likes + comments
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

                            // Original audio language (for &hl= hint)
                            var origLang = YouTubeApi.GetNestedString(detail, "snippet", "defaultAudioLanguage")
                                        ?? YouTubeApi.GetNestedString(detail, "snippet", "defaultLanguage");

                            // Cache
                            MetaCache[batchItem.Id] = new VideoMeta(
                                overview, premiere, premiere?.Year,
                                ts?.Ticks, batchItem.ImageUrl, DateTime.UtcNow, origLang);
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

        // ── Recently Added: cross-channel newest mix ──
        private static async Task<ChannelItemResult> LoadRecentlyAdded(
            string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            var perChannel = Math.Clamp(config.RecentlyAddedPerChannel, 1, 25);
            var savedItems = (config.SavedItems ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Resolve channel IDs (uses 6h-cached channel-details endpoint, ~free after first load)
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

            foreach (var channelId in channelIds)
            {
                ct.ThrowIfCancellationRequested();
                using var doc = await YouTubeApi.GetChannelVideosAsync(apiKey, channelId, null, ct, "date")
                    .ConfigureAwait(false);
                if (doc == null) continue;

                var items = ExtractVideos(doc, isPlaylist: true);
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

            // Enrich (1u per 50 IDs, 1y cached)
            if (allItems.Count > 0)
            {
                var ids = allItems.Select(i =>
                {
                    var r = i.Id;
                    if (r.StartsWith(LivePrefix)) return r.Substring(LivePrefix.Length);
                    if (r.StartsWith(ReelPrefix)) return r.Substring(ReelPrefix.Length);
                    return r;
                }).ToList();
                await EnrichBatch(apiKey, allItems, ids, ct).ConfigureAwait(false);
                ApplyCachedMeta(allItems);
            }

            // Sort newest first
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

                // Original language for &hl= hint
                var origLang = YouTubeApi.GetNestedString(el, "snippet", "defaultAudioLanguage")
                            ?? YouTubeApi.GetNestedString(el, "snippet", "defaultLanguage");

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
                    ImageUrl = thumb,
                    MediaSources = MakeMediaSources(videoId, isLive, isLive ? null : ts?.Ticks, origLang)
                };

                MetaCache[itemId] = new VideoMeta(overview, premiere, premiere?.Year, ts?.Ticks, thumb, DateTime.UtcNow, origLang);
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
                    ImageUrl = thumb,
                    MediaSources = MakeMediaSources(videoId, isLive, MetaCache.TryGetValue(itemId, out var __m) ? __m.RuntimeTicks : null, MetaCache.TryGetValue(itemId, out var __m2) ? __m2.OriginalLang : null)
                };

                list.Add(info);
            }
            return list;
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

        // SortName fixing extracted to SortNameFixer.cs
        internal static void ScheduleSortNameFix() => SortNameFixer.Schedule();
    }
}
