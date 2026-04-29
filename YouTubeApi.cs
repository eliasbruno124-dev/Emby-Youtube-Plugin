using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public static class YouTubeApi
    {
        private const string ApiBase = "https://www.googleapis.com/youtube/v3";

        // Response cache: helps minimize API calls by storing recent responses.
        private record CachedResponse(string Json, long CachedAtMs);
        private static readonly ConcurrentDictionary<string, CachedResponse> ResponseCache = new();
        private const int MaxCacheEntries = 200;

        // How long to keep things in memory before checking disk again (memory TTLs)
        private const long CacheTtlMs = 15 * 60 * 1000;            // 15 min default
        private const long FreshListTtlMs = 6 * 60 * 60 * 1000;       // 6 h: trending, uploads
        private const long ChannelDetailsCacheTtlMs = 6 * 60 * 60 * 1000;       // 6 h
        private const long LiveSearchTtlMs = 12 * 60 * 60 * 1000;       // 12 h: live/upcoming (search = 100u!)
        private const long CategoriesTtlMs = 30L * 24 * 60 * 60 * 1000; // 30 d: video categories almost never change
        private const long VideoDetailTtlMs = 365L * 24 * 60 * 60 * 1000; // 1 y: video metadata is immutable

        // Disk TTL is capped at 30 days (per YouTube ToS). For each call, we use the smaller of the requested TTL or 30 days, so "fresh" lists aren't served stale from disk.
        private const long DiskCacheTtlMs = 30L * 24 * 60 * 60 * 1000;
        private static int _diskCleanupDone = 0;

        private static readonly HttpClient Http = new HttpClient(
            new SocketsHttpHandler
            {
                AllowAutoRedirect = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                ConnectTimeout = TimeSpan.FromSeconds(10),
            })
        {
            Timeout = TimeSpan.FromSeconds(30),
            DefaultRequestHeaders =
            {
                { "User-Agent", "EmbyYouTubePlugin/1.0" },
                { "Accept", "application/json" }
            }
        };

        private static readonly int[] RetryDelaysMs = { 1500, 4000 };

        // Rate limiter: keeps us from hitting YouTube's request limits too quickly.
        private static readonly SemaphoreSlim ApiGate = new(6, 6);
        private static long _lastCallTicks = 0;
        private const int MinCallIntervalMs = 100;

        private static readonly Queue<long> _requestTimestamps = new();
        private static readonly object _budgetLock = new();
        // Hard cap: 240 requests per 60 seconds. YouTube's soft limit is higher, but staying under 4 requests per second helps avoid 429 errors.
        private const int MaxRequestsPerWindow = 240;
        private const int BudgetWindowMs = 60_000;

        private static async Task ThrottleAsync(CancellationToken ct)
        {
            await ApiGate.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                await EnforceBudgetAsync(ct).ConfigureAwait(false);

                var now = Environment.TickCount64;
                var last = Interlocked.Read(ref _lastCallTicks);
                var elapsed = now - last;
                if (elapsed < MinCallIntervalMs)
                {
                    await Task.Delay((int)(MinCallIntervalMs - elapsed), ct)
                        .ConfigureAwait(false);
                }
                Interlocked.Exchange(ref _lastCallTicks, Environment.TickCount64);

                lock (_budgetLock)
                    _requestTimestamps.Enqueue(Environment.TickCount64);
            }
            finally
            {
                ApiGate.Release();
            }
        }

        private static async Task EnforceBudgetAsync(CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                lock (_budgetLock)
                {
                    var now = Environment.TickCount64;
                    while (_requestTimestamps.Count > 0
                           && (now - _requestTimestamps.Peek()) > BudgetWindowMs)
                        _requestTimestamps.Dequeue();

                    if (_requestTimestamps.Count < MaxRequestsPerWindow)
                        return;
                }
                Debug.WriteLine("[YouTubeApi] Request budget exhausted, waiting 3s...");
                await Task.Delay(3000, ct).ConfigureAwait(false);
            }
        }

        private static bool IsTransientError(HttpStatusCode code) =>
            code == HttpStatusCode.TooManyRequests ||
            code == HttpStatusCode.InternalServerError ||
            code == HttpStatusCode.BadGateway ||
            code == HttpStatusCode.ServiceUnavailable ||
            code == HttpStatusCode.GatewayTimeout;

        private static async Task<JsonDocument> GetJsonAsync(string url, CancellationToken ct)
        {
            HttpStatusCode lastStatus = 0;

            for (int attempt = 0; attempt <= RetryDelaysMs.Length; attempt++)
            {
                if (attempt > 0)
                    await Task.Delay(RetryDelaysMs[attempt - 1], ct).ConfigureAwait(false);

                await ThrottleAsync(ct).ConfigureAwait(false);

                using var resp = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct)
                    .ConfigureAwait(false);

                if (resp.IsSuccessStatusCode)
                {
                    await using var stream = await resp.Content
                        .ReadAsStreamAsync(ct).ConfigureAwait(false);
                    return await JsonDocument.ParseAsync(stream, cancellationToken: ct)
                        .ConfigureAwait(false);
                }

                lastStatus = resp.StatusCode;

                if (lastStatus == HttpStatusCode.TooManyRequests)
                {
                    int baseDelay = 10_000 * (1 << attempt);
                    var retryAfter = Math.Min(baseDelay, 60_000);
                    Debug.WriteLine($"[YouTubeApi] Rate limited (429), attempt {attempt}, waiting {retryAfter}ms...");
                    await Task.Delay(retryAfter, ct).ConfigureAwait(false);
                    continue;
                }

                if (!IsTransientError(lastStatus))
                    break;
            }

            throw new HttpRequestException($"YouTube API returned HTTP {(int)lastStatus}");
        }

        private static async Task<JsonDocument?> TryGetJsonAsync(string url, CancellationToken ct)
        {
            try
            {
                return await GetJsonAsync(url, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] TryGetJsonAsync failed: {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }

        private static async Task<JsonDocument?> TryGetCachedJsonAsync(
            string url, CancellationToken ct, long? customTtlMs = null)
        {
            var memTtl = customTtlMs ?? CacheTtlMs;
            // Disk TTL matches memory TTL (but never more than 30 days), so "fresh" lists aren't served stale from disk after memory expires.
            var diskTtl = Math.Min(memTtl, DiskCacheTtlMs);
            var now = Environment.TickCount64;

            // First level: in-memory cache
            if (ResponseCache.TryGetValue(url, out var cached)
                && (now - cached.CachedAtMs) < memTtl)
            {
                try { return JsonDocument.Parse(cached.Json); }
                catch { ResponseCache.TryRemove(url, out _); }
            }

            // Second level: disk cache (per-call TTL, max 30 days)
            var diskJson = TryReadDiskCache(url, diskTtl);
            if (diskJson != null)
            {
                // If we hit disk cache, refresh in-memory cache from disk
                ResponseCache[url] = new CachedResponse(diskJson, now);
                EvictCacheIfNeeded();
                return JsonDocument.Parse(diskJson);
            }

            // Third: make the actual API call (this counts against your quota)
            var doc = await TryGetJsonAsync(url, ct).ConfigureAwait(false);
            if (doc != null)
            {
                QuotaTracker.RecordCall(url);
                var json = doc.RootElement.GetRawText();
                ResponseCache[url] = new CachedResponse(json, now);
                EvictCacheIfNeeded();
                WriteDiskCache(url, json);
                doc.Dispose();
                return JsonDocument.Parse(json);
            }

            // Fourth: if the API is unavailable (quota exhausted, network error, etc.), serve the most recent disk-cached response, no matter how old it is.
            // This way, users still see channel content instead of an empty list when the daily quota runs out.
            var staleJson = TryReadDiskCache(url, long.MaxValue);
            if (staleJson != null)
            {
                Debug.WriteLine("[YouTubeApi] API unavailable, serving stale cache");
                // Cache in memory with a 5-minute TTL so we retry the API soon
                // but avoid hitting disk on every request during the outage.
                var shortTtlAnchor = now - memTtl + (5 * 60 * 1000);
                ResponseCache[url] = new CachedResponse(staleJson, shortTtlAnchor);
                EvictCacheIfNeeded();
                return JsonDocument.Parse(staleJson);
            }

            return null;
        }

        // Disk cache helpers

        private static string GetDiskCacheKey(string url)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(url));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private static string? TryReadDiskCache(string url, long ttlMs)
        {
            try
            {
                var cacheDir = Plugin.CachePath;
                if (string.IsNullOrEmpty(cacheDir)) return null;

                RunDiskCleanupOnce(cacheDir);

                var key = GetDiskCacheKey(url);
                var file = Path.Combine(cacheDir, key + ".json");

                // Backward compatibility: older plugin versions used 32-character (truncated) keys.
                // If the 64-character file is missing, check for the legacy file and rename it so the next WriteDiskCache uses the right name.
                if (!File.Exists(file))
                {
                    var legacyKey = key.Substring(0, 32);
                    var legacyFile = Path.Combine(cacheDir, legacyKey + ".json");
                    if (File.Exists(legacyFile))
                    {
                        try { File.Move(legacyFile, file); } catch { file = legacyFile; }
                    }
                    else
                    {
                        return null;
                    }
                }

                var lastWrite = File.GetLastWriteTimeUtc(file);
                var ageMs = (long)(DateTime.UtcNow - lastWrite).TotalMilliseconds;
                // Per-call TTL (set by the caller). Don't delete the file just because it's stale for this call—30-day cleanup will handle deletion.
                if (ageMs > ttlMs) return null;

                return File.ReadAllText(file, Encoding.UTF8);
            }
            catch
            {
                return null;
            }
        }

        private static void WriteDiskCache(string url, string json)
        {
            try
            {
                var cacheDir = Plugin.CachePath;
                if (string.IsNullOrEmpty(cacheDir)) return;

                var file = Path.Combine(cacheDir, GetDiskCacheKey(url) + ".json");
                File.WriteAllText(file, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] WriteDiskCache failed: {ex.Message}");
            }
        }

        private static void RunDiskCleanupOnce(string cacheDir)
        {
            if (Interlocked.CompareExchange(ref _diskCleanupDone, 1, 0) != 0) return;
            Task.Run(() =>
            {
                try
                {
                    var cutoff = DateTime.UtcNow.AddMilliseconds(-DiskCacheTtlMs);
                    foreach (var file in Directory.EnumerateFiles(cacheDir, "*.json"))
                    {
                        try
                        {
                            if (File.GetLastWriteTimeUtc(file) < cutoff)
                                File.Delete(file);
                        }
                        catch { }
                    }
                    Debug.WriteLine("[YouTubeApi] Disk cache cleanup done.");
                }
                catch { }
            });
        }

        private static void EvictCacheIfNeeded()
        {
            if (ResponseCache.Count <= MaxCacheEntries) return;
            var oldest = new List<string>();
            var now = Environment.TickCount64;
            foreach (var kvp in ResponseCache)
            {
                if ((now - kvp.Value.CachedAtMs) > CacheTtlMs)
                    oldest.Add(kvp.Key);
            }
            foreach (var key in oldest)
                ResponseCache.TryRemove(key, out _);
        }

        /// <summary>
        /// Invalidate all cached entries (memory + disk) whose URL contains the given substring.
        /// Used by the Watch Later poll: when the playlist contents change, we must drop the
        /// 6h-cached playlistItems so the user sees the fresh list.
        /// </summary>
        public static void InvalidateCacheContaining(string substring)
        {
            if (string.IsNullOrEmpty(substring)) return;
            try
            {
                foreach (var key in ResponseCache.Keys.ToList())
                {
                    if (key.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ResponseCache.TryRemove(key, out _);
                        try
                        {
                            var cacheDir = Plugin.CachePath;
                            if (!string.IsNullOrEmpty(cacheDir))
                            {
                                var file = Path.Combine(cacheDir, GetDiskCacheKey(key) + ".json");
                                if (File.Exists(file)) File.Delete(file);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] InvalidateCacheContaining failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Drop the entire memory response cache. Used after the user changes the API
        /// key or saved channels so the next channel refresh re-fetches with the new
        /// configuration instead of serving stale data.
        /// </summary>
        public static void InvalidateAllCache()
        {
            try { ResponseCache.Clear(); }
            catch (Exception ex) { Debug.WriteLine($"[YouTubeApi] InvalidateAllCache failed: {ex.Message}"); }
        }

        // Channel details

        public static async Task<(string? id, string? name, string? thumb, string? uploadsPlaylistId)>
            GetChannelDetailsAsync(string apiKey, string query, bool isHandle, CancellationToken ct)
        {
            try
            {
                if (isHandle)
                {
                    // Use channels?forHandle= (costs 1 unit) instead of search (which costs 100 units)
                    var handle = query.TrimStart('@');
                    var url = $"{ApiBase}/channels?part=snippet,contentDetails&forHandle={Uri.EscapeDataString(handle)}&key={Uri.EscapeDataString(apiKey)}";
                    using var doc = await TryGetCachedJsonAsync(url, ct, ChannelDetailsCacheTtlMs).ConfigureAwait(false);
                    if (doc == null) return (null, null, null, null);

                    var root = doc.RootElement;
                    if (root.TryGetProperty("items", out var items2)
                        && items2.ValueKind == JsonValueKind.Array
                        && items2.GetArrayLength() > 0)
                    {
                        var ch = items2[0];
                        var channelId = GetString(ch, "id");
                        var name = GetNestedString(ch, "snippet", "title");
                        var thumb = GetBestThumbnail(ch);
                        string? uploadsId = null;
                        if (ch.TryGetProperty("contentDetails", out var cd)
                            && cd.TryGetProperty("relatedPlaylists", out var rp))
                        {
                            uploadsId = GetString(rp, "uploads");
                        }
                        return (channelId, name, thumb, uploadsId);
                    }
                    return (null, null, null, null);
                }
                else
                {
                    return await GetChannelByIdAsync(apiKey, query, ct).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] GetChannelDetailsAsync failed for '{query}': {ex.Message}");
            }
            return (null, null, null, null);
        }

        private static async Task<(string? id, string? name, string? thumb, string? uploadsPlaylistId)>
            GetChannelByIdAsync(string apiKey, string channelId, CancellationToken ct)
        {
            var url = $"{ApiBase}/channels?part=snippet,contentDetails&id={Uri.EscapeDataString(channelId)}&key={Uri.EscapeDataString(apiKey)}";
            using var doc = await TryGetCachedJsonAsync(url, ct, ChannelDetailsCacheTtlMs).ConfigureAwait(false);
            if (doc == null) return (channelId, null, null, null);

            var root = doc.RootElement;
            if (root.TryGetProperty("items", out var items)
                && items.ValueKind == JsonValueKind.Array
                && items.GetArrayLength() > 0)
            {
                var ch = items[0];
                var name = GetNestedString(ch, "snippet", "title");
                var thumb = GetBestThumbnail(ch);
                string? uploadsId = null;
                if (ch.TryGetProperty("contentDetails", out var cd)
                    && cd.TryGetProperty("relatedPlaylists", out var rp))
                {
                    uploadsId = GetString(rp, "uploads");
                }
                return (channelId, name, thumb, uploadsId);
            }
            return (channelId, null, null, null);
        }

        // Playlist details

        public static async Task<(string? name, string? thumb, int videoCount)>
            GetPlaylistDetailsAsync(string apiKey, string playlistId, CancellationToken ct)
        {
            try
            {
                var url = $"{ApiBase}/playlists?part=snippet,contentDetails&id={Uri.EscapeDataString(playlistId)}&key={Uri.EscapeDataString(apiKey)}";
                using var doc = await TryGetCachedJsonAsync(url, ct, ChannelDetailsCacheTtlMs).ConfigureAwait(false);
                if (doc == null) return (null, null, 0);

                var root = doc.RootElement;
                if (root.TryGetProperty("items", out var items)
                    && items.ValueKind == JsonValueKind.Array
                    && items.GetArrayLength() > 0)
                {
                    var pl = items[0];
                    var name = GetNestedString(pl, "snippet", "title");
                    var thumb = GetBestThumbnail(pl);
                    int count = 0;
                    if (pl.TryGetProperty("contentDetails", out var cd)
                        && cd.TryGetProperty("itemCount", out var ic)
                        && ic.TryGetInt32(out var n))
                        count = n;
                    return (name, thumb, count);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] GetPlaylistDetailsAsync failed for '{playlistId}': {ex.Message}");
            }
            return (null, null, 0);
        }

        // Search videos

        public static async Task<JsonDocument?> SearchVideosAsync(
            string apiKey, string query, string? pageToken, CancellationToken ct)
        {
            var q = Uri.EscapeDataString(query ?? "");
            var url = $"{ApiBase}/search?part=snippet&q={q}&type=video&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            // Search costs 100 units—cache for 6 hours
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        // Channel videos (via uploads playlist—costs 1 unit instead of 100 for search)

        public static async Task<JsonDocument?> GetChannelVideosAsync(
            string apiKey, string channelId, string? pageToken, CancellationToken ct,
            string sortBy = "date")
        {
            // Derive uploads playlist ID: UC... → UU...
            var uploadsPlaylistId = "UU" + channelId.Substring(2);
            return await GetPlaylistVideosAsync(apiKey, uploadsPlaylistId, pageToken, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Search channel videos by duration (short, medium, or long).
        /// "short" means Shorts, "medium" and "long" are regular videos.
        /// Uses search.list (costs 100 quota units per call).
        /// </summary>
        public static async Task<JsonDocument?> SearchChannelByDurationAsync(
            string apiKey, string channelId, string videoDuration,
            string? pageToken, CancellationToken ct, string order = "date")
        {
            var url = $"{ApiBase}/search?part=snippet&channelId={Uri.EscapeDataString(channelId)}" +
                      $"&type=video&videoDuration={Uri.EscapeDataString(videoDuration)}" +
                      $"&order={Uri.EscapeDataString(NormalizeSortBy(order))}" +
                      $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            // Search costs 100 units—cache for 6 hours
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        // Channel live and upcoming streams

        public static async Task<JsonDocument?> GetChannelLiveAsync(
            string apiKey, string channelId, string? pageToken, CancellationToken ct)
        {
            var url = $"{ApiBase}/search?part=snippet&channelId={Uri.EscapeDataString(channelId)}" +
                      $"&type=video&eventType=live&order=date&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            // Live search costs 100 units, changes rarely—cache for 12 hours
            return await TryGetCachedJsonAsync(url, ct, LiveSearchTtlMs).ConfigureAwait(false);
        }

        public static async Task<JsonDocument?> GetChannelUpcomingAsync(
            string apiKey, string channelId, string? pageToken, CancellationToken ct)
        {
            var url = $"{ApiBase}/search?part=snippet&channelId={Uri.EscapeDataString(channelId)}" +
                      $"&type=video&eventType=upcoming&order=date&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            // Upcoming search costs 100 units—cache for 12 hours
            return await TryGetCachedJsonAsync(url, ct, LiveSearchTtlMs).ConfigureAwait(false);
        }

        // Playlist videos

        public static async Task<JsonDocument?> GetPlaylistVideosAsync(
            string apiKey, string playlistId, string? pageToken, CancellationToken ct)
        {
            var url = $"{ApiBase}/playlistItems?part=snippet,contentDetails&playlistId={Uri.EscapeDataString(playlistId)}" +
                      $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            // Uploads and playlists update often (costs 1 unit)—cache for 6 hours
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        // Trending videos

        public static async Task<JsonDocument?> GetTrendingAsync(
            string apiKey, string? regionCode, string? categoryId, CancellationToken ct)
        {
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics" +
                      $"&chart=mostPopular&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(regionCode))
                url += $"&regionCode={Uri.EscapeDataString(regionCode)}";
            if (!string.IsNullOrEmpty(categoryId) && categoryId != "0")
                url += $"&videoCategoryId={Uri.EscapeDataString(categoryId)}";
            // Trending changes every hour, but we cache for 6 hours to save quota
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        // Video categories (for the Categories browser)

        public static async Task<JsonDocument?> GetVideoCategoriesAsync(
            string apiKey, string regionCode, CancellationToken ct)
        {
            var url = $"{ApiBase}/videoCategories?part=snippet&regionCode={Uri.EscapeDataString(regionCode)}" +
                      $"&key={Uri.EscapeDataString(apiKey)}";
            // Categories almost never change—cache for 30 days
            return await TryGetCachedJsonAsync(url, ct, CategoriesTtlMs).ConfigureAwait(false);
        }

        // Search videos by category (Trending in Category, no search query)

        public static async Task<JsonDocument?> GetTrendingByCategoryAsync(
            string apiKey, string regionCode, string categoryId, CancellationToken ct)
        {
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics" +
                      $"&chart=mostPopular&maxResults=50&regionCode={Uri.EscapeDataString(regionCode)}" +
                      $"&videoCategoryId={Uri.EscapeDataString(categoryId)}&key={Uri.EscapeDataString(apiKey)}";
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        // Search by category (popular videos in a category, costs 100 units)
        // Used to supplement chart=mostPopular, which often returns only a few videos for some categories. Cached for 6 hours to keep quota usage reasonable.
        public static async Task<JsonDocument?> SearchByCategoryAsync(
            string apiKey, string regionCode, string categoryId, CancellationToken ct,
            string? pageToken = null)
        {
            // Restrict to the last 30 days so we get current popular uploads instead of all-time top hits (which overlap a lot with chart=mostPopular and limit variety).
            // Important: round to the UTC day boundary so the URL stays stable for 24 hours—otherwise, the per-millisecond timestamp would break the 6-hour disk cache and every call would cost 100 units.
            var publishedAfter = DateTime.UtcNow.Date.AddDays(-30)
                .ToString("yyyy-MM-ddTHH:mm:ssZ");
            var url = $"{ApiBase}/search?part=snippet&type=video&order=viewCount" +
                      $"&maxResults=50&videoCategoryId={Uri.EscapeDataString(categoryId)}" +
                      $"&publishedAfter={Uri.EscapeDataString(publishedAfter)}" +
                      $"&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(regionCode))
                url += $"&regionCode={Uri.EscapeDataString(regionCode)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        // Batch video details (up to 50 IDs at once)

        public static async Task<JsonDocument?> GetVideoDetailsBatchAsync(
            string apiKey, IEnumerable<string> videoIds, CancellationToken ct)
        {
            var ids = string.Join(",", videoIds);
            // The status part lets us filter out private, rejected, or non-embeddable videos that would otherwise show up in the channel listing as broken posters.
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics,liveStreamingDetails,status" +
                      $"&id={Uri.EscapeDataString(ids)}&key={Uri.EscapeDataString(apiKey)}";
            // Video metadata doesn't change—cache for 1 year (disk cache capped at 30 days)
            return await TryGetCachedJsonAsync(url, ct, VideoDetailTtlMs).ConfigureAwait(false);
        }

        // Helper methods

        private static string NormalizeSortBy(string sortBy)
        {
            return (sortBy ?? "").Trim().ToLowerInvariant() switch
            {
                "date" => "date",
                "newest" => "date",
                "viewcount" => "viewCount",
                "popular" => "viewCount",
                "rating" => "rating",
                "relevance" => "relevance",
                "oldest" => "date",
                _ => "date"
            };
        }

        public static string? GetString(JsonElement el, string name)
        {
            if (el.ValueKind != JsonValueKind.Object) return null;
            if (!el.TryGetProperty(name, out var p)) return null;
            return p.ValueKind == JsonValueKind.String ? p.GetString() : p.ToString();
        }

        public static int? GetInt(JsonElement el, string name)
        {
            if (el.ValueKind != JsonValueKind.Object) return null;
            if (!el.TryGetProperty(name, out var p)) return null;
            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var i)) return i;
            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var s)) return s;
            return null;
        }

        public static long? GetLong(JsonElement el, string name)
        {
            if (el.ValueKind != JsonValueKind.Object) return null;
            if (!el.TryGetProperty(name, out var p)) return null;
            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt64(out var i)) return i;
            if (p.ValueKind == JsonValueKind.String && long.TryParse(p.GetString(), out var s)) return s;
            return null;
        }

        public static string? GetNestedString(JsonElement el, string parent, string child)
        {
            if (el.TryGetProperty(parent, out var p) && p.ValueKind == JsonValueKind.Object)
                return GetString(p, child);
            return null;
        }

        public static string? GetBestThumbnail(JsonElement el)
        {
            if (!el.TryGetProperty("snippet", out var snippet)) return null;
            if (!snippet.TryGetProperty("thumbnails", out var thumbs)) return null;
            if (thumbs.ValueKind != JsonValueKind.Object) return null;

            // Prefer: maxres > high > medium > default
            foreach (var quality in new[] { "maxres", "high", "medium", "default" })
            {
                if (thumbs.TryGetProperty(quality, out var t))
                {
                    var url = GetString(t, "url");
                    if (!string.IsNullOrEmpty(url)) return url;
                }
            }
            return null;
        }

        public static string GetStableVideoThumbnailUrl(string videoId, string? preferredUrl)
        {
            // Always use mqdefault.jpg—it's the only thumbnail size YouTube guarantees for every video (including upcoming streams, brand-new uploads, and videos without maxres/hqdefault yet).
            // hqdefault and maxresdefault can return 404 for many edge cases.
            if (string.IsNullOrWhiteSpace(videoId))
                return preferredUrl ?? string.Empty;
            return $"https://i.ytimg.com/vi/{videoId}/mqdefault.jpg";
        }

        /// <summary>
        /// Parses ISO 8601 duration (PT1H2M3S) to TimeSpan.
        /// </summary>
        public static TimeSpan? ParseDuration(string? duration)
        {
            if (string.IsNullOrEmpty(duration)) return null;
            try
            {
                return System.Xml.XmlConvert.ToTimeSpan(duration);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parses a YouTube publishedAt string (ISO 8601) to DateTime.
        /// </summary>
        public static DateTime? ParsePublishedAt(string? publishedAt)
        {
            if (string.IsNullOrEmpty(publishedAt)) return null;
            if (DateTime.TryParse(publishedAt, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                    out var dt))
                return dt;
            return null;
        }
    }
}
