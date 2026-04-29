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
        private static void Log(string msg) => LogPublic(msg);
        public static void LogPublic(string msg)
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

        // ── Cross-folder dedup ──
        // The same YouTube video may legitimately appear in many folders. To avoid
        // showing it 3-5x in Emby's home-screen "Latest" view we apply asymmetric
        // dedup:
        //   - Channel folders (channel/playlist/watchlater/...): NEVER drop anything,
        //     just remember which video IDs they returned. Channel folders are the
        //     primary view, so they always show their full content.
        //   - Trending / Recently Added: DROP videos already returned by a channel
        //     folder this refresh cycle. They keep their own internal dedup.
        //
        // The seen-set is reset only when the API cache is invalidated (config change)
        // and on a hard timeout (10 min) so a manual / scheduled refresh always sees
        // up-to-date content.
        private static readonly ConcurrentDictionary<string, byte> CrossFolderSeen = new(StringComparer.Ordinal);
        private static DateTime _crossFolderSeenStartedAt = DateTime.UtcNow;
        private static readonly TimeSpan CrossFolderSeenMaxAge = TimeSpan.FromMinutes(10);

        public static void ResetCrossFolderSeen()
        {
            CrossFolderSeen.Clear();
            _crossFolderSeenStartedAt = DateTime.UtcNow;
        }

        private static string StripPrefix(string id)
        {
            if (id.StartsWith(LivePrefix, StringComparison.Ordinal)) return id.Substring(LivePrefix.Length);
            if (id.StartsWith(ReelPrefix, StringComparison.Ordinal)) return id.Substring(ReelPrefix.Length);
            return id;
        }

        // Called by channel folders: remembers which video IDs this folder returned
        // but never drops anything from the input list.
        private static void MarkAsSeen(IEnumerable<ChannelItemInfo> items)
        {
            if ((DateTime.UtcNow - _crossFolderSeenStartedAt) > CrossFolderSeenMaxAge)
                ResetCrossFolderSeen();
            foreach (var it in items)
                CrossFolderSeen.TryAdd(StripPrefix(it.Id), 1);
        }

        // Called by Trending / Categories: drops videos already returned by ANY
        // previously-seeded folder (channel uploads or another aggregator) and
        // then seeds the surviving items so the next aggregator can dedup
        // against this one.
        //
        // The dedup is REQUIRED to keep Emby's home "Latest" row from showing
        // the same video twice — each Channel folder creates its own MediaItem
        // row in Emby's DB, so the same videoId in two folders = two rows = two
        // entries in Latest. Recently Added is intentionally NOT routed through
        // here because it is by definition a subset of saved channels and the
        // dedup would empty it.
        private static void DropIfSeenInChannelFolder(List<ChannelItemInfo> items)
        {
            if ((DateTime.UtcNow - _crossFolderSeenStartedAt) > CrossFolderSeenMaxAge)
                ResetCrossFolderSeen();
            items.RemoveAll(it => CrossFolderSeen.ContainsKey(StripPrefix(it.Id)));
            foreach (var it in items)
                CrossFolderSeen.TryAdd(StripPrefix(it.Id), 1);
        }

        // Pre-seeds the cross-folder seen-set from cached channel uploads so
        // Trending / Recently Added can dedup against channel folders even when
        // Emby happens to fetch them BEFORE the channel folders this cycle.
        // Without this, the first-after-restart Trending/Recent fetch keeps every
        // video, then channel folders later store their own copies → 2 MediaItems
        // for the same video → home "Latest" view shows the video twice.
        // Uses the 6h-cached uploads playlist endpoint, so this is free on warm
        // cache and ~1 quota unit per channel on cold cache.
        private static async Task PreSeedChannelSeenAsync(
            string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            try
            {
                if ((DateTime.UtcNow - _crossFolderSeenStartedAt) > CrossFolderSeenMaxAge)
                    ResetCrossFolderSeen();

                var savedItems = (config.SavedItems ?? "")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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

                // Also pre-seed from Watch Later playlists
                var watchLater = (config.WatchLaterPlaylist ?? "")
                    .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).Where(s => s.Length > 2).ToList();

                async Task SeedFromDocAsync(JsonDocument? doc)
                {
                    await Task.CompletedTask;
                    if (doc == null) return;
                    if (!doc.RootElement.TryGetProperty("items", out var items)
                        || items.ValueKind != JsonValueKind.Array) return;
                    foreach (var el in items.EnumerateArray())
                    {
                        // playlistItems uses snippet.resourceId.videoId; videos.list uses id
                        string? vid = null;
                        if (el.TryGetProperty("snippet", out var sn)
                            && sn.TryGetProperty("resourceId", out var ri)
                            && ri.TryGetProperty("videoId", out var vidEl))
                            vid = vidEl.GetString();
                        if (string.IsNullOrEmpty(vid))
                            vid = YouTubeApi.GetString(el, "id");
                        if (!string.IsNullOrEmpty(vid))
                            CrossFolderSeen.TryAdd(vid!, 1);
                    }
                }

                foreach (var cid in channelIds)
                {
                    ct.ThrowIfCancellationRequested();
                    using var doc = await YouTubeApi.GetChannelVideosAsync(apiKey, cid, null, ct, "date")
                        .ConfigureAwait(false);
                    await SeedFromDocAsync(doc).ConfigureAwait(false);
                }
                foreach (var pid in watchLater)
                {
                    ct.ThrowIfCancellationRequested();
                    using var doc = await YouTubeApi.GetPlaylistVideosAsync(apiKey, pid, null, ct)
                        .ConfigureAwait(false);
                    await SeedFromDocAsync(doc).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Log($"[YT] PreSeedChannelSeenAsync failed: {ex.Message}");
            }
        }

        // ── Rate limiter for enrichment ──
        private static readonly SemaphoreSlim EnrichSemaphore = new(4, 4);
        private const int EnrichDelayMs = 200;
        private const int MaxForegroundEnrich = 0;

        // Playability probe: YouTube's videos.list API does NOT expose every form
        // of geo / partner / sports-league restriction. Items can be marked
        // privacyStatus=public, embeddable=true, with no regionRestriction set,
        // yet still refuse to play in our region (e.g. MLB / FS1 sports streams).
        // The watch-page HTML contains a `playabilityStatus":"OK|UNPLAYABLE|ERROR|LOGIN_REQUIRED"`
        // field that reflects the *actual* playability from the server's IP, so
        // we probe it as a last line of defense after the API checks pass.
        private static readonly System.Net.Http.HttpClient PlayabilityProbeHttp =
            new(new System.Net.Http.HttpClientHandler { AllowAutoRedirect = true })
            { Timeout = TimeSpan.FromSeconds(8) };
        private static readonly ConcurrentDictionary<string, bool> PlayabilityCache = new(StringComparer.Ordinal);

        static YouTubeChannel()
        {
            try
            {
                PlayabilityProbeHttp.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36");
                PlayabilityProbeHttp.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            }
            catch { }
        }

        private static async Task<bool> IsPlayableAsync(string videoId, CancellationToken ct)
        {
            if (PlayabilityCache.TryGetValue(videoId, out var cached)) return cached;
            try
            {
                using var resp = await PlayabilityProbeHttp.GetAsync(
                    $"https://www.youtube.com/watch?v={Uri.EscapeDataString(videoId)}",
                    System.Net.Http.HttpCompletionOption.ResponseContentRead, ct).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode)
                {
                    // Network error — assume playable to avoid false drops
                    return true;
                }
                var html = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                int idx = html.IndexOf("\"playabilityStatus\":{", StringComparison.Ordinal);
                if (idx < 0) return true;
                int statusIdx = html.IndexOf("\"status\":\"", idx, StringComparison.Ordinal);
                if (statusIdx < 0) return true;
                statusIdx += "\"status\":\"".Length;
                int end = html.IndexOf('"', statusIdx);
                if (end < 0) return true;
                var status = html.Substring(statusIdx, end - statusIdx);
                bool playable = string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase);
                PlayabilityCache[videoId] = playable;
                return playable;
            }
            catch
            {
                // Probe failed — assume playable to avoid false drops
                return true;
            }
        }

        // Runs IsPlayableAsync in parallel for every item and removes the unplayable ones.
        private static async Task ProbeAndDropUnplayable(List<ChannelItemInfo> items, CancellationToken ct)
        {
            if (items.Count == 0) return;
            var rawIds = items
                .Select(i =>
                {
                    var r = i.Id;
                    if (r.StartsWith(LivePrefix, StringComparison.Ordinal)) r = r.Substring(LivePrefix.Length);
                    else if (r.StartsWith(ReelPrefix, StringComparison.Ordinal)) r = r.Substring(ReelPrefix.Length);
                    return r;
                })
                .Distinct(StringComparer.Ordinal)
                .ToList();

            var unplayable = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
            using var sem = new SemaphoreSlim(6, 6);
            var tasks = rawIds.Select(async id =>
            {
                await sem.WaitAsync(ct).ConfigureAwait(false);
                try
                {
                    if (!await IsPlayableAsync(id, ct).ConfigureAwait(false))
                    {
                        unplayable.TryAdd(id, 1);
                        Log($"[YT] Dropping unplayable video {id} (watch-page playabilityStatus != OK)");
                    }
                }
                finally { sem.Release(); }
            });
            try { await Task.WhenAll(tasks).ConfigureAwait(false); } catch { }

            if (unplayable.IsEmpty) return;
            items.RemoveAll(i =>
            {
                var r = i.Id;
                if (r.StartsWith(LivePrefix, StringComparison.Ordinal)) r = r.Substring(LivePrefix.Length);
                else if (r.StartsWith(ReelPrefix, StringComparison.Ordinal)) r = r.Substring(ReelPrefix.Length);
                return unplayable.ContainsKey(r);
            });
        }

        private const string LivePrefix = "LIVE_";
        private const string ReelPrefix = "REEL_";
        private const int ReelMaxSeconds = 180;

        private static List<MediaSourceInfo> MakeMediaSources(string videoId, bool isLive = false, long? runTimeTicks = null, string? originalLang = null)
        {
            // Extra parameters (runTimeTicks, originalLang) are accepted for signature
            // compatibility with callers but intentionally NOT applied to MediaSourceInfo:
            //  - YouTube's &hl= is UI-only and does NOT influence audio-track selection.
            //  - Setting RunTimeTicks on the MediaSource makes Emby Web try to direct-play
            //    the watch URL as a raw stream, which hangs forever. Leaving it null lets
            //    the web client recognise the watch URL and fall back to the YouTube
            //    embed/iframe player (the behaviour that worked in v1.14.x and earlier).
            string url = $"https://www.youtube.com/watch?v={videoId}";
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
                }
            };
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
                    // Watch Later (comma-separated list of playlist IDs)
                    var watchLaterRaw = (config.WatchLaterPlaylist ?? "").Trim();
                    if (watchLaterRaw.Length > 2)
                    {
                        var playlistIds = watchLaterRaw
                            .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => s.Length > 2)
                            .Distinct(StringComparer.Ordinal)
                            .ToList();

                        for (int wi = 0; wi < playlistIds.Count; wi++)
                        {
                            var pid = playlistIds[wi];
                            var d = await YouTubeApi.GetPlaylistDetailsAsync(apiKey, pid, cancellationToken)
                                .ConfigureAwait(false);
                            var thumbUrl = !string.IsNullOrEmpty(d.thumb) ? d.thumb : FolderIcons.WatchLater;
                            // Keep the legacy single-playlist label so existing entries don't get renamed
                            var displayName = playlistIds.Count == 1
                                ? "\u2B50 " + (d.name ?? "Watch Later")
                                : "\u2B50 " + (d.name ?? $"Watch Later {wi + 1}");
                            items.Add(new ChannelItemInfo
                            {
                                Name = displayName,
                                Id = $"playlist{FolderSeparator}{pid}",
                                Type = ChannelItemType.Folder,
                                ImageUrl = thumbUrl
                            });
                        }
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
                        if (!config.HideShorts)
                        {
                            items.Add(new ChannelItemInfo
                            {
                                Name = "⚡ Shorts",
                                Id = $"channelshorts{FolderSeparator}{term}",
                                Type = ChannelItemType.Folder,
                                ImageUrl = channelThumb ?? FolderIcons.Shorts
                            });
                        }

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
                        var candidates = new List<(string id, string name)>();
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
                                candidates.Add((cid, cname));
                            }
                        }

                        // Probe each category in parallel: only show categories that
                        // would actually contain at least 1 video AFTER all filters
                        // (region restriction, HideShorts).
                        // Probes try chart=mostPopular first (~free, 6h-cached) and
                        // fall back to a viewCount-sorted search (100u, 6h-cached).
                        // NOTE: deliberately does NOT pre-seed against channel uploads
                        // — that caused categories to vanish whenever a saved channel
                        // happened to dominate that category's trending.
                        var nonEmpty = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
                        bool hideShorts = config.HideShorts;

                        // Local helper: count videos in a doc that survive region/Shorts
                        // filters. Returns true as soon as one survives.
                        bool AnySurvives(JsonDocument doc)
                        {
                            foreach (var v in ExtractTrendingVideos(doc))
                            {
                                if (hideShorts && v.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)) continue;
                                return true;
                            }
                            return false;
                        }

                        using (var probeSem = new SemaphoreSlim(6, 6))
                        {
                            var probes = candidates.Select(async cand =>
                            {
                                await probeSem.WaitAsync(cancellationToken).ConfigureAwait(false);
                                try
                                {
                                    // 1) Cheap chart probe
                                    using (var td = await YouTubeApi.GetTrendingAsync(apiKey, region, cand.id, cancellationToken)
                                        .ConfigureAwait(false))
                                    {
                                        if (td != null && AnySurvives(td))
                                        {
                                            nonEmpty.TryAdd(cand.id, 1);
                                            return;
                                        }
                                    }
                                    // 2) Search fallback: enrich first 50 search hits via
                                    // videos.list so we can apply the same region filter.
                                    using var sd = await YouTubeApi.SearchByCategoryAsync(apiKey, region, cand.id, cancellationToken)
                                        .ConfigureAwait(false);
                                    if (sd == null) return;
                                    var ids = new List<string>();
                                    if (sd.RootElement.TryGetProperty("items", out var sit)
                                        && sit.ValueKind == JsonValueKind.Array)
                                    {
                                        foreach (var el in sit.EnumerateArray())
                                        {
                                            string? vid = null;
                                            if (el.TryGetProperty("id", out var idEl)
                                                && idEl.TryGetProperty("videoId", out var vidEl))
                                                vid = vidEl.GetString();
                                            if (string.IsNullOrEmpty(vid)) continue;
                                            ids.Add(vid!);
                                            if (ids.Count >= 50) break;
                                        }
                                    }
                                    if (ids.Count == 0) return;
                                    using var vd = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, ids, cancellationToken)
                                        .ConfigureAwait(false);
                                    if (vd != null && AnySurvives(vd))
                                        nonEmpty.TryAdd(cand.id, 1);
                                }
                                catch { }
                                finally { probeSem.Release(); }
                            });
                            try { await Task.WhenAll(probes).ConfigureAwait(false); } catch { }
                        }

                        foreach (var cand in candidates)
                        {
                            if (!nonEmpty.ContainsKey(cand.id)) continue;
                            items.Add(new ChannelItemInfo
                            {
                                Name = cand.name,
                                Id = $"category{FolderSeparator}{cand.id}",
                                Type = ChannelItemType.Folder,
                                ImageUrl = FolderIcons.ForCategory(cand.id)
                            });
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

                        // Global: hide shorts everywhere if configured
                        if (config.HideShorts)
                            batch.RemoveAll(item => item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));

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

                    // Channel folders own their videos: just remember the IDs so
                    // Trending / Recently Added can drop duplicates later.
                    MarkAsSeen(items);

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
            // Track every video ID YouTube actually returned. Anything missing after
            // all chunks were processed is unavailable (deleted, private, region-blocked,
            // age-restricted, copyright takedown). We remove those from the batch.
            var foundIds = new HashSet<string>(StringComparer.Ordinal);
            // Videos that the API returned but that are unplayable in Emby:
            //   - status.privacyStatus = "private"
            //   - status.uploadStatus not in ["processed", "uploaded"]
            //     (i.e. "rejected", "deleted", "failed")
            //   - status.embeddable = false (the YouTube iframe player refuses to load)
            //   - duration = 0s and not a live stream (stuck transcode / unavailable)
            // These items are removed from the batch alongside the deleted ones so
            // Emby doesn't keep dead entries with broken thumbnails.
            var unplayableIds = new HashSet<string>(StringComparer.Ordinal);
            // Only IDs from chunks whose API request actually succeeded count as
            // "definitively queried". If a chunk fails (network/5xx/rate-limit), its
            // IDs MUST NOT be treated as missing -- otherwise we would wrongly remove
            // them from the batch, causing Emby to delete their metadata/posters
            // overnight whenever a single chunk transiently fails.
            var queriedIds = new HashSet<string>(StringComparer.Ordinal);
            try
            {
                // YouTube API allows up to 50 IDs per request
                for (int i = 0; i < videoIds.Count; i += 50)
                {
                    var chunkList = videoIds.Skip(i).Take(50).ToList();
                    using var doc = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, chunkList, ct)
                        .ConfigureAwait(false);
                    if (doc == null) continue; // transient failure: keep all items in this chunk
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

                            // ── Unplayable detection ──
                            // Drop videos that YouTube returned but that won't play in
                            // the Emby iframe player (private, rejected, non-embeddable,
                            // stuck processing). Without this they appear in the channel
                            // listing with broken thumbnails and "stream not available"
                            // errors when clicked.
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

                            // Age-restricted videos (ytAgeRestricted) won't play in the
                            // YouTube iframe player without a logged-in YouTube account,
                            // so they're effectively unplayable in Emby.
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

                            // Region-blocked videos: only filter when the user has
                            // explicitly configured a Trending Region. Without an explicit
                            // region we cannot guess the server's geo (and the previous
                            // "DE" fallback was wrongly dropping playable videos for
                            // non-DE users). The check is a cheap JSON lookup.
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

                            // Duration
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
                                // Non-live video with 0s duration = stuck processing or
                                // unavailable. Drop it; it won't play.
                                unplayableIds.Add(rawId);
                                Log($"[YT] Dropping zero-duration non-live video {rawId}");
                                continue;
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

                            // 3. Duration ≤ ReelMaxSeconds → very likely a Short. YouTube
                            // extended Shorts to 3 minutes in late 2024, so the old 60s
                            // threshold missed many Shorts and created inconsistent IDs
                            // (Trending used 180s, EnrichBatch used 60s) → same video
                            // appeared with REEL_ prefix in one folder and without in
                            // another, causing Emby to store two separate MediaItems.
                            if (!isShort && ts.HasValue && ts.Value.TotalSeconds > 0 && ts.Value.TotalSeconds <= ReelMaxSeconds)
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

                // NOTE: The watch-page playability HTTP probe used to run here. It
                // made channel refresh extremely slow (one HTTPS round-trip per
                // video, up to 8s timeout) and frequently false-dropped items when
                // YouTube rate-limited the probe. The fast region/embeddable/
                // private/duration filtering above is enough for the channel
                // listing; the rare unplayable item is a much smaller annoyance
                // than a multi-minute refresh.

                // Drop unavailable videos: only items whose chunk request actually
                // succeeded but whose ID was NOT returned by YouTube are considered
                // unavailable (deleted, private, region-blocked, age-restricted, or
                // copyright takedown). Items whose chunk failed are KEPT so transient
                // API errors don't wipe them from the channel listing (and their
                // posters in Emby's metadata cache).
                // Additionally: items returned by YouTube but flagged as unplayable
                // (private / rejected / non-embeddable / 0-duration non-live) are
                // also removed so they don't leave broken posters in the listing.
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
                        // Only remove if the ID was definitely queried in a successful
                        // chunk and YouTube did not return it.
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

                // Rebuild MediaSources URL with original-language hl= if cache now has it
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

        // ── Recently Added: cross-channel newest mix ──
        private static async Task<ChannelItemResult> LoadRecentlyAdded(
            string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            // Pre-seed seen-set so dedup works regardless of folder fetch order.
            await PreSeedChannelSeenAsync(apiKey, config, ct).ConfigureAwait(false);

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

            if (config.HideShorts)
                allItems.RemoveAll(i => i.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));

            // Sort newest first
            var sorted = allItems
                .OrderByDescending(i => i.PremiereDate ?? i.DateCreated ?? DateTimeOffset.MinValue)
                .ToList();

            if (sorted.Count == 0)
                return Msg(new List<ChannelItemInfo>(), "No videos yet.");

            // NOTE: Previously dropped videos already seen in channel folders.
            // That collapsed Recently Added to empty in practice, because every
            // video here is BY DEFINITION also in a saved channel's uploads.
            // We now keep them and just mark them as seen so other aggregators
            // (Trending, Categories) can still dedup against this folder.
            MarkAsSeen(sorted);

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
            var seenIds = new HashSet<string>(StringComparer.Ordinal);

            // Folders are fully INDEPENDENT and DETERMINISTIC.
            //   - No CrossFolderSeen state: output depends only on the (6h-cached)
            //     YouTube responses, so the same videos stay in the same slots
            //     across refreshes (no "rotation"). The probe in the Categories
            //     root sees the same content the folder will actually build, so
            //     empty categories are correctly hidden.
            //   - Trending root  → chart=mostPopular across 9 buckets, then
            //                      SearchByCategory PAGE 1 fallback to fill 50.
            //   - Category child → SearchByCategory PAGE 1 + PAGE 2 (chart is the
            //                      same data Trending root already shows, so we
            //                      skip it here and use page 2 to give the
            //                      category folder content that's actually
            //                      different from Trending root).
            // Cross-folder duplicates (same video shown in 2+ folders) are tagged
            // via ProviderIds["YouTube"] = videoId on the ChannelItemInfo so Emby
            // can identify them as the same underlying entity.
            var cfg = Plugin.Instance?.Options;
            const int target = 50;
            bool hideShorts = cfg?.HideShorts == true;
            bool isCategoryChild = !string.IsNullOrEmpty(category);

            void TryAdd(ChannelItemInfo v)
            {
                var rawId = StripPrefix(v.Id);
                if (!seenIds.Add(rawId)) return;
                if (hideShorts && v.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)) return;
                allVideos.Add(v);
            }

            try
            {
                if (!isCategoryChild)
                {
                    // ── Trending root ──
                    // 1) chart=mostPopular across 9 buckets (almost free, 6h-cached)
                    string?[] buckets = new string?[] { null, "10", "20", "24", "17", "22", "23", "28", "1" };
                    // All, Music, Gaming, Entertainment, Sports, People, Comedy, Tech, Film
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

                    // 2) SearchByCategory PAGE 1 fallback to fill (100u/bucket cold,
                    //    0u warm). Stops as soon as we hit 50.
                    if (allVideos.Count < target)
                    {
                        foreach (var cat in buckets)
                        {
                            if (allVideos.Count >= target) break;
                            if (string.IsNullOrEmpty(cat)) continue;
                            ct.ThrowIfCancellationRequested();
                            await FillFromSearchAsync(apiKey, region, cat!, pageToken: null, ct, seenIds, TryAdd, target, allVideos)
                                .ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    // ── Category child ──
                    // Use SearchByCategory page 1 + page 2 so the folder content
                    // is genuinely different from Trending root (which uses chart).
                    // Pages are 6h-cached → 200u cold per category, 0u warm.
                    string? nextToken = await FillFromSearchAsync(
                            apiKey, region, category, pageToken: null,
                            ct, seenIds, TryAdd, target, allVideos)
                        .ConfigureAwait(false);

                    if (allVideos.Count < target && !string.IsNullOrEmpty(nextToken))
                    {
                        await FillFromSearchAsync(apiKey, region, category, pageToken: nextToken,
                                ct, seenIds, TryAdd, target, allVideos)
                            .ConfigureAwait(false);
                    }

                    // Last-resort top-up: if search came up short (small category),
                    // add the chart hits for this category so we still try to reach
                    // 50. These will overlap with Trending root, but ProviderIds
                    // lets Emby identify them as the same video.
                    if (allVideos.Count < target)
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

        // Helper: fetch one page of SearchByCategory, enrich via videos.list and
        // append to allVideos via TryAdd. Returns the nextPageToken (or null).
        private static async Task<string?> FillFromSearchAsync(
            string apiKey, string region, string categoryId, string? pageToken,
            CancellationToken ct, HashSet<string> seenIds,
            Action<ChannelItemInfo> tryAdd, int target, List<ChannelItemInfo> allVideos)
        {
            using var sdoc = await YouTubeApi.SearchByCategoryAsync(apiKey, region, categoryId, ct, pageToken)
                .ConfigureAwait(false);
            if (sdoc == null) return null;

            string? nextToken = null;
            if (sdoc.RootElement.TryGetProperty("nextPageToken", out var npt)
                && npt.ValueKind == JsonValueKind.String)
                nextToken = npt.GetString();

            var newIds = new List<string>();
            if (sdoc.RootElement.TryGetProperty("items", out var sitems)
                && sitems.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in sitems.EnumerateArray())
                {
                    string? vid = null;
                    if (el.TryGetProperty("id", out var idEl)
                        && idEl.TryGetProperty("videoId", out var vidEl))
                        vid = vidEl.GetString();
                    if (string.IsNullOrEmpty(vid)) continue;
                    if (seenIds.Contains(vid!)) continue;
                    newIds.Add(vid!);
                    if (newIds.Count >= 50) break;
                }
            }
            if (newIds.Count > 0)
            {
                using var vd = await YouTubeApi.GetVideoDetailsBatchAsync(apiKey, newIds, ct)
                    .ConfigureAwait(false);
                if (vd != null)
                {
                    foreach (var v in ExtractTrendingVideos(vd))
                    {
                        tryAdd(v);
                        if (allVideos.Count >= target) break;
                    }
                }
            }
            return nextToken;
        }

        // ── Extract trending videos (from videos.list which has full details) ──
        private static List<ChannelItemInfo> ExtractTrendingVideos(JsonDocument doc)
        {
            var list = new List<ChannelItemInfo>();
            if (!doc.RootElement.TryGetProperty("items", out var items)
                || items.ValueKind != JsonValueKind.Array)
                return list;

            // Cheap JSON-level region filter: replaces the slow watch-page HTTP
            // probe. Only filters when the user explicitly set a Trending Region
            // (without one we cannot guess the server's geo).
            var serverRegion = (Plugin.Instance?.Options.TrendingRegion ?? "").Trim();

            foreach (var el in items.EnumerateArray())
            {
                var videoId = YouTubeApi.GetString(el, "id");
                if (string.IsNullOrWhiteSpace(videoId)) continue;

                // Skip region-blocked entries before any further work
                if (!string.IsNullOrEmpty(serverRegion)
                    && el.TryGetProperty("contentDetails", out var cdRR)
                    && cdRR.ValueKind == JsonValueKind.Object
                    && cdRR.TryGetProperty("regionRestriction", out var rrEl)
                    && rrEl.ValueKind == JsonValueKind.Object)
                {
                    bool blocked = false;
                    if (rrEl.TryGetProperty("blocked", out var bl)
                        && bl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var b in bl.EnumerateArray())
                            if (string.Equals(b.GetString(), serverRegion, StringComparison.OrdinalIgnoreCase))
                            { blocked = true; break; }
                    }
                    if (!blocked
                        && rrEl.TryGetProperty("allowed", out var al)
                        && al.ValueKind == JsonValueKind.Array)
                    {
                        bool inAllowed = false;
                        foreach (var a in al.EnumerateArray())
                            if (string.Equals(a.GetString(), serverRegion, StringComparison.OrdinalIgnoreCase))
                            { inAllowed = true; break; }
                        if (!inAllowed) blocked = true;
                    }
                    if (blocked) continue;
                }

                var title = YouTubeApi.GetNestedString(el, "snippet", "title") ?? "Untitled";
                var author = YouTubeApi.GetNestedString(el, "snippet", "channelTitle") ?? "Unknown";
                var desc = YouTubeApi.GetNestedString(el, "snippet", "description");
                var pubStr = YouTubeApi.GetNestedString(el, "snippet", "publishedAt");
                var premiere = YouTubeApi.ParsePublishedAt(pubStr);
                var thumb = YouTubeApi.GetStableVideoThumbnailUrl(
                    videoId,
                    YouTubeApi.GetBestThumbnail(el));

                // Original language for &hl= hint
                var origLang = YouTubeApi.GetNestedString(el, "snippet", "defaultAudioLanguage")
                            ?? YouTubeApi.GetNestedString(el, "snippet", "defaultLanguage");

                // Duration
                var duration = YouTubeApi.GetNestedString(el, "contentDetails", "duration");
                var ts = YouTubeApi.ParseDuration(duration);

                // View / like / comment count
                long? viewCount = null;
                long? likeCount = null;
                long? commentCount = null;
                if (el.TryGetProperty("statistics", out var stats))
                {
                    if (long.TryParse(YouTubeApi.GetString(stats, "viewCount"), out var v)) viewCount = v;
                    if (long.TryParse(YouTubeApi.GetString(stats, "likeCount"), out var l)) likeCount = l;
                    if (long.TryParse(YouTubeApi.GetString(stats, "commentCount"), out var c)) commentCount = c;
                }

                var statsParts = new List<string>();
                if (viewCount.HasValue) statsParts.Add($"👁 {viewCount:N0}");
                if (likeCount.HasValue && Plugin.Instance?.Options.ShowLikeCount == true)
                    statsParts.Add($"👍 {likeCount:N0}");
                if (commentCount.HasValue && Plugin.Instance?.Options.ShowCommentCount == true)
                    statsParts.Add($"💬 {commentCount:N0}");
                string statsLine = statsParts.Count > 0 ? string.Join("  ·  ", statsParts) : "";

                string? overview = null;
                if (!string.IsNullOrWhiteSpace(desc))
                    overview = (statsLine.Length > 0 ? statsLine + "\n\n" : "") + desc;
                else if (statsLine.Length > 0)
                    overview = statsLine;

                // Live detection
                bool isLive = false;
                if (el.TryGetProperty("liveStreamingDetails", out var lsd))
                {
                    var concurrentViewers = YouTubeApi.GetString(lsd, "concurrentViewers");
                    if (!string.IsNullOrEmpty(concurrentViewers))
                        isLive = true;
                }

                // Shorts detection: duration ≤ ReelMaxSeconds OR explicit "shorts" tag
                // OR #shorts hashtag in title/description (matches EnrichBatch logic)
                bool isReel = !isLive && ts.HasValue && ts.Value.TotalSeconds > 0 && ts.Value.TotalSeconds <= ReelMaxSeconds;
                if (!isReel && !isLive && el.TryGetProperty("snippet", out var snipForReel))
                {
                    if (snipForReel.TryGetProperty("tags", out var tagsEl)
                        && tagsEl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var tag in tagsEl.EnumerateArray())
                        {
                            var t = tag.GetString();
                            if (t != null && string.Equals(t.Trim(), "shorts", StringComparison.OrdinalIgnoreCase))
                            {
                                isReel = true;
                                break;
                            }
                        }
                    }
                    if (!isReel)
                    {
                        var sTitle = YouTubeApi.GetString(snipForReel, "title") ?? "";
                        var sDesc = YouTubeApi.GetString(snipForReel, "description") ?? "";
                        if (sTitle.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0
                         || sDesc.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0)
                            isReel = true;
                    }
                }

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
                    // Tag every video with its YouTube videoId so the SAME video
                    // appearing in multiple folders (channel + trending + category)
                    // can be identified as one underlying entity by Emby.
                    ProviderIds = new MediaBrowser.Model.Entities.ProviderIdDictionary { ["YouTube"] = videoId },
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
                var thumb = YouTubeApi.GetStableVideoThumbnailUrl(
                    videoId,
                    YouTubeApi.GetBestThumbnail(el));

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
                    ProviderIds = new MediaBrowser.Model.Entities.ProviderIdDictionary { ["YouTube"] = videoId },
                    MediaSources = MakeMediaSources(videoId, isLive,
                        MetaCache.TryGetValue(itemId, out var __m) ? __m.RuntimeTicks : null,
                        __m?.OriginalLang)
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
