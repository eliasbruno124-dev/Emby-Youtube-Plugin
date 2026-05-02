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

        // Hold onto recent API responses so we don't ask YouTube for the same
        // data more often than needed.
        private record CachedResponse(string Json, long CachedAtMs);
        private static readonly ConcurrentDictionary<string, CachedResponse> ResponseCache = new();
        private const int MaxCacheEntries = 200;

        // Memory cache lifetimes. Disk uses the same value per request,
        // capped by the global limit below.
        private const long CacheTtlMs = 15 * 60 * 1000;            // Default: 15 minutes.
        private const long FreshListTtlMs = 6 * 60 * 60 * 1000;       // Trending, uploads, and playlists: 6 hours.
        private const long ChannelDetailsCacheTtlMs = 6 * 60 * 60 * 1000;       // Channel names and thumbnails: 6 hours.
        private const long SearchTtlMs = 24 * 60 * 60 * 1000;           // Search is expensive, so keep it for a day.
        private const long CategoriesTtlMs = 30L * 24 * 60 * 60 * 1000; // Categories rarely change.
        private const long VideoDetailTtlMs = 365L * 24 * 60 * 60 * 1000; // Video details are mostly stable.

        // YouTube's API terms cap persisted API data at 30 days. Each call
        // gets the smaller of its own TTL and this disk limit.
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

        // Keep bursts gentle so YouTube is less likely to answer with 429s.
        // The gate has to be held for the whole request, not just the
        // bookkeeping below — otherwise we have no real concurrency limit.
        private static readonly SemaphoreSlim ApiGate = new(6, 6);
        private static long _lastCallTicks = 0;
        private const int MinCallIntervalMs = 100;

        private static readonly Queue<long> _requestTimestamps = new();
        private static readonly object _budgetLock = new();
        // Hard local cap: 240 requests per minute. Below ~4 req/s keeps
        // refreshes polite and predictable.
        private const int MaxRequestsPerWindow = 240;
        private const int BudgetWindowMs = 60_000;

        // Acquires the gate and applies the per-call spacing rules. The caller
        // must Release the gate when the HTTP request finishes — enforced via
        // the IDisposable returned (use it with a using statement).
        private static async Task<IDisposable> AcquireGateAsync(CancellationToken ct)
        {
            await ApiGate.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                await EnforceBudgetAsync(ct).ConfigureAwait(false);

                var now = Environment.TickCount64;
                var last = Interlocked.Read(ref _lastCallTicks);
                var elapsed = now - last;
                if (elapsed < MinCallIntervalMs)
                    await Task.Delay((int)(MinCallIntervalMs - elapsed), ct).ConfigureAwait(false);
                Interlocked.Exchange(ref _lastCallTicks, Environment.TickCount64);

                lock (_budgetLock)
                    _requestTimestamps.Enqueue(Environment.TickCount64);
            }
            catch
            {
                ApiGate.Release();
                throw;
            }
            return new GateLease();
        }

        private sealed class GateLease : IDisposable
        {
            private int _disposed;
            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
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
                int? overrideDelayMs = null;

                using (await AcquireGateAsync(ct).ConfigureAwait(false))
                using (var resp = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
                {
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
                        // Honor Retry-After when present, otherwise back off
                        // exponentially. Capped at 60s.
                        var ra = resp.Headers.RetryAfter;
                        int delay;
                        if (ra?.Delta is TimeSpan delta && delta > TimeSpan.Zero)
                            delay = (int)Math.Min(delta.TotalMilliseconds, 60_000);
                        else if (ra?.Date is DateTimeOffset when)
                            delay = (int)Math.Min(Math.Max((when - DateTimeOffset.UtcNow).TotalMilliseconds, 0), 60_000);
                        else
                            delay = Math.Min(10_000 * (1 << attempt), 60_000);
                        overrideDelayMs = delay;
                        Debug.WriteLine($"[YouTubeApi] Rate limited (429), attempt {attempt}, waiting {delay}ms...");
                    }
                    else if (!IsTransientError(lastStatus))
                    {
                        break;
                    }
                }

                if (attempt == RetryDelaysMs.Length) break;

                var nextDelay = overrideDelayMs ?? RetryDelaysMs[attempt];
                if (nextDelay > 0)
                    await Task.Delay(nextDelay, ct).ConfigureAwait(false);
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
            // Disk follows the same freshness window as memory, capped by the
            // global limit below.
            // 30-day cap applied.
            var diskTtl = Math.Min(memTtl, DiskCacheTtlMs);
            var now = Environment.TickCount64;

            // First stop: in-memory cache.
            if (ResponseCache.TryGetValue(url, out var cached)
                && (now - cached.CachedAtMs) < memTtl)
            {
                try { return JsonDocument.Parse(cached.Json); }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"[YouTubeApi] Dropping invalid memory cache entry: {ex.Message}");
                    ResponseCache.TryRemove(url, out _);
                }
            }

            // Next stop: on-disk cache.
            var diskJson = TryReadDiskCache(url, diskTtl);
            if (diskJson != null)
            {
                // Promote the disk hit back into memory for the next caller.
                ResponseCache[url] = new CachedResponse(diskJson, now);
                EvictCacheIfNeeded();
                return JsonDocument.Parse(diskJson);
            }

            // Last normal option: actually call YouTube. This is where we
            // spend quota.
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

            // If YouTube is unreachable, fall back to the freshest disk copy
            // we have. Stale content beats an empty channel during an outage
            // or after quota is blown.
            var staleJson = TryReadDiskCache(url, long.MaxValue);
            if (staleJson != null)
            {
                Debug.WriteLine("[YouTubeApi] API unavailable, serving stale cache");
                // Keep the stale copy in memory briefly so we retry soon
                // without hitting the disk on every request during an outage.
                var shortTtlAnchor = now - memTtl + (5 * 60 * 1000);
                ResponseCache[url] = new CachedResponse(staleJson, shortTtlAnchor);
                EvictCacheIfNeeded();
                return JsonDocument.Parse(staleJson);
            }

            return null;
        }

        // ---- disk cache helpers ----

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

                // Older versions used a 32-character cache key. Pick it up
                // once and move it over to the current 64-char key.
                if (!File.Exists(file))
                {
                    var legacyKey = key.Substring(0, 32);
                    var legacyFile = Path.Combine(cacheDir, legacyKey + ".json");
                    if (File.Exists(legacyFile))
                    {
                        try { File.Move(legacyFile, file); }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[YouTubeApi] Legacy disk cache rename failed: {ex.Message}");
                            file = legacyFile;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                var lastWrite = File.GetLastWriteTimeUtc(file);
                var ageMs = (long)(DateTime.UtcNow - lastWrite).TotalMilliseconds;
                // This call may need fresher data, but the file can still
                // serve as a stale fallback until the 30-day cleanup wipes it.
                if (ageMs > ttlMs) return null;

                return File.ReadAllText(file, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] ReadDiskCache failed: {ex.Message}");
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
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[YouTubeApi] Disk cache cleanup skipped {Path.GetFileName(file)}: {ex.Message}");
                        }
                    }
                    Debug.WriteLine("[YouTubeApi] Disk cache cleanup done.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[YouTubeApi] Disk cache cleanup failed: {ex.Message}");
                }
            });
        }

        private static void EvictCacheIfNeeded()
        {
            if (ResponseCache.Count <= MaxCacheEntries) return;
            var now = Environment.TickCount64;

            // First pass: drop expired entries.
            foreach (var kvp in ResponseCache)
            {
                if ((now - kvp.Value.CachedAtMs) > CacheTtlMs)
                    ResponseCache.TryRemove(kvp.Key, out _);
            }

            // Still over the cap (everything fresh): drop the oldest by
            // CachedAtMs so memory doesn't grow forever.
            if (ResponseCache.Count > MaxCacheEntries)
            {
                var overflow = ResponseCache.Count - MaxCacheEntries;
                var oldest = ResponseCache
                    .OrderBy(kvp => kvp.Value.CachedAtMs)
                    .Take(overflow)
                    .Select(kvp => kvp.Key)
                    .ToList();
                foreach (var key in oldest)
                    ResponseCache.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Clears cached responses whose URL contains the given text.
        /// The Watch Later poll uses this when a playlist changes, so the next
        /// refresh does not keep showing the old six-hour playlist cache.
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
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[YouTubeApi] Disk cache delete failed: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] InvalidateCacheContaining failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Drops the in-memory response cache after settings change.
        /// Disk cache remains available for normal TTL-based reuse and stale
        /// fallback behavior.
        /// </summary>
        public static void InvalidateAllCache()
        {
            try { ResponseCache.Clear(); }
            catch (Exception ex) { Debug.WriteLine($"[YouTubeApi] InvalidateAllCache failed: {ex.Message}"); }
        }

        // Channel details.

        public static async Task<(string? id, string? name, string? thumb, string? uploadsPlaylistId)>
            GetChannelDetailsAsync(string apiKey, string query, bool isHandle, CancellationToken ct)
        {
            try
            {
                if (isHandle)
                {
                    // Resolve handles via channels?forHandle instead of
                    // search.list — cheaper and more exact.
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

        // Playlist details.

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

        // Search is the expensive path: search.list costs 100 quota units.
        // Cache each query for a full day.
        public static async Task<JsonDocument?> SearchVideosAsync(
            string apiKey, string query, string? pageToken, CancellationToken ct)
        {
            var q = Uri.EscapeDataString(query ?? "");
            var url = $"{ApiBase}/search?part=snippet&q={q}&type=video&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            return await TryGetCachedJsonAsync(url, ct, SearchTtlMs).ConfigureAwait(false);
        }

        // Channel uploads come from the uploads playlist (1 quota unit) instead
        // of search.list.

        public static async Task<JsonDocument?> GetChannelVideosAsync(
            string apiKey, string channelId, string? pageToken, CancellationToken ct,
            string sortBy = "date")
        {
            // "date" is the default and uses the cheap uploads playlist (1 unit).
            // Anything else needs search.list with an order param (100 units).
            var normalized = (sortBy ?? "date").Trim().ToLowerInvariant();
            if (normalized == "date" || string.IsNullOrEmpty(normalized))
            {
                var uploadsPlaylistId = "UU" + channelId.Substring(2);
                return await GetPlaylistVideosAsync(apiKey, uploadsPlaylistId, pageToken, ct)
                    .ConfigureAwait(false);
            }

            var url = $"{ApiBase}/search?part=snippet&channelId={Uri.EscapeDataString(channelId)}" +
                      $"&type=video&maxResults=50&order={Uri.EscapeDataString(normalized)}" +
                      $"&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            return await TryGetCachedJsonAsync(url, ct, SearchTtlMs).ConfigureAwait(false);
        }

        // Playlist videos.

        public static async Task<JsonDocument?> GetPlaylistVideosAsync(
            string apiKey, string playlistId, string? pageToken, CancellationToken ct)
        {
            var url = $"{ApiBase}/playlistItems?part=snippet,contentDetails&playlistId={Uri.EscapeDataString(playlistId)}" +
                      $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            // Uploads/playlists are cheap but noisy, so 6h hits a good middle
            // ground for normal browsing.
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the first up-to-N video IDs from a playlist while bypassing
        /// every cache. Used by the watch-later poll so we always see the live
        /// state of the playlist, not the cached six-hour snapshot.
        /// </summary>
        public static async Task<List<string>> GetPlaylistVideoIdsFreshAsync(
            string apiKey, string playlistId, int maxItems, CancellationToken ct)
        {
            var ids = new List<string>();
            string? pageToken = null;
            // 5 pages * 50 = 250 IDs is plenty for change detection.
            for (int page = 0; page < 5 && ids.Count < maxItems; page++)
            {
                var url = $"{ApiBase}/playlistItems?part=contentDetails&playlistId={Uri.EscapeDataString(playlistId)}" +
                          $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
                if (!string.IsNullOrEmpty(pageToken))
                    url += $"&pageToken={Uri.EscapeDataString(pageToken)}";

                using var doc = await TryGetJsonAsync(url, ct).ConfigureAwait(false);
                if (doc == null) break;
                QuotaTracker.RecordCall(url);

                if (doc.RootElement.TryGetProperty("items", out var items)
                    && items.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        var vid = GetNestedString(item, "contentDetails", "videoId");
                        if (!string.IsNullOrEmpty(vid)) ids.Add(vid!);
                        if (ids.Count >= maxItems) break;
                    }
                }

                pageToken = doc.RootElement.TryGetProperty("nextPageToken", out var ptEl)
                    ? ptEl.GetString()
                    : null;
                if (string.IsNullOrEmpty(pageToken)) break;
            }
            return ids;
        }

        // Trending videos.

        public static async Task<JsonDocument?> GetTrendingAsync(
            string apiKey, string? regionCode, string? categoryId, CancellationToken ct)
        {
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics" +
                      $"&chart=mostPopular&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(regionCode))
                url += $"&regionCode={Uri.EscapeDataString(regionCode)}";
            if (!string.IsNullOrEmpty(categoryId) && categoryId != "0")
                url += $"&videoCategoryId={Uri.EscapeDataString(categoryId)}";
            // Trending shifts often, but a 6h cache keeps quota usage calm.
            return await TryGetCachedJsonAsync(url, ct, FreshListTtlMs).ConfigureAwait(false);
        }

        // Video categories for the Categories browser.

        public static async Task<JsonDocument?> GetVideoCategoriesAsync(
            string apiKey, string regionCode, CancellationToken ct)
        {
            var url = $"{ApiBase}/videoCategories?part=snippet&regionCode={Uri.EscapeDataString(regionCode)}" +
                      $"&key={Uri.EscapeDataString(apiKey)}";
            // Categories almost never change, so we keep them for the full
            // disk window.
            return await TryGetCachedJsonAsync(url, ct, CategoriesTtlMs).ConfigureAwait(false);
        }

        // Batch video details, up to 50 IDs per request.

        public static async Task<JsonDocument?> GetVideoDetailsBatchAsync(
            string apiKey, IEnumerable<string> videoIds, CancellationToken ct)
        {
            var ids = string.Join(",", videoIds);
            // The status part lets us drop private, rejected and
            // non-embeddable videos before they show up as broken items.
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics,liveStreamingDetails,status" +
                      $"&id={Uri.EscapeDataString(ids)}&key={Uri.EscapeDataString(apiKey)}";
            // Metadata is stable; disk persistence is still capped at 30 days.
            return await TryGetCachedJsonAsync(url, ct, VideoDetailTtlMs).ConfigureAwait(false);
        }

        // Helper methods.

        public static string? GetString(JsonElement el, string name)
        {
            if (el.ValueKind != JsonValueKind.Object) return null;
            if (!el.TryGetProperty(name, out var p)) return null;
            return p.ValueKind == JsonValueKind.String ? p.GetString() : p.ToString();
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

            // Pick the sharpest thumbnail YouTube hands us.
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
            // mqdefault.jpg is the safest direct thumbnail URL. Larger
            // variants 404 a lot for fresh uploads, upcoming streams or
            // older videos.
            if (string.IsNullOrWhiteSpace(videoId))
                return preferredUrl ?? string.Empty;
            return $"https://i.ytimg.com/vi/{videoId}/mqdefault.jpg";
        }

        /// <summary>
        /// Parses a YouTube ISO 8601 duration, such as PT1H2M3S.
        /// </summary>
        public static TimeSpan? ParseDuration(string? duration)
        {
            if (string.IsNullOrEmpty(duration)) return null;
            try
            {
                return System.Xml.XmlConvert.ToTimeSpan(duration);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] ParseDuration failed for '{duration}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parses YouTube's publishedAt timestamp.
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
