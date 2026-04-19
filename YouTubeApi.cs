using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public static class YouTubeApi
    {
        private const string ApiBase = "https://www.googleapis.com/youtube/v3";

        // ── Response cache to minimize API calls ──
        private record CachedResponse(string Json, long CachedAtMs);
        private static readonly ConcurrentDictionary<string, CachedResponse> ResponseCache = new();
        private const int MaxCacheEntries = 200;
        private const long CacheTtlMs = 15 * 60 * 1000; // 15 minutes
        private const long ChannelDetailsCacheTtlMs = 6 * 60 * 60 * 1000; // 6 hours

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

        // ── Rate Limiter ──
        private static readonly SemaphoreSlim ApiGate = new(6, 6);
        private static long _lastCallTicks = 0;
        private const int MinCallIntervalMs = 100;

        private static readonly Queue<long> _requestTimestamps = new();
        private static readonly object _budgetLock = new();
        private const int MaxRequestsPerWindow = 90;
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
            var ttl = customTtlMs ?? CacheTtlMs;
            var now = Environment.TickCount64;

            // Check cache
            if (ResponseCache.TryGetValue(url, out var cached)
                && (now - cached.CachedAtMs) < ttl)
            {
                try { return JsonDocument.Parse(cached.Json); }
                catch { ResponseCache.TryRemove(url, out _); }
            }

            var doc = await TryGetJsonAsync(url, ct).ConfigureAwait(false);
            if (doc != null)
            {
                // Store raw JSON in cache
                var json = doc.RootElement.GetRawText();
                ResponseCache[url] = new CachedResponse(json, now);
                EvictCacheIfNeeded();
                // Return a fresh parse (caller will dispose)
                doc.Dispose();
                return JsonDocument.Parse(json);
            }
            return null;
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

        // ── Channel Details ──

        public static async Task<(string? id, string? name, string? thumb, string? uploadsPlaylistId)>
            GetChannelDetailsAsync(string apiKey, string query, bool isHandle, CancellationToken ct)
        {
            try
            {
                string url;
                if (isHandle)
                {
                    // First search for the channel by handle
                    var handle = query.TrimStart('@');
                    url = $"{ApiBase}/search?part=snippet&q=%40{Uri.EscapeDataString(handle)}&type=channel&maxResults=1&key={Uri.EscapeDataString(apiKey)}";
                    using var searchDoc = await TryGetCachedJsonAsync(url, ct, ChannelDetailsCacheTtlMs).ConfigureAwait(false);
                    if (searchDoc == null) return (null, null, null, null);

                    var searchRoot = searchDoc.RootElement;
                    if (searchRoot.TryGetProperty("items", out var searchItems)
                        && searchItems.ValueKind == JsonValueKind.Array
                        && searchItems.GetArrayLength() > 0)
                    {
                        var first = searchItems[0];
                        var channelId = GetNestedString(first, "snippet", "channelId")
                                        ?? GetNestedString(first, "id", "channelId");
                        if (!string.IsNullOrEmpty(channelId))
                            return await GetChannelByIdAsync(apiKey, channelId, ct).ConfigureAwait(false);
                    }
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

        // ── Playlist Details ──

        public static async Task<(string? name, string? thumb, int videoCount)>
            GetPlaylistDetailsAsync(string apiKey, string playlistId, CancellationToken ct)
        {
            try
            {
                var url = $"{ApiBase}/playlists?part=snippet,contentDetails&id={Uri.EscapeDataString(playlistId)}&key={Uri.EscapeDataString(apiKey)}";
                using var doc = await TryGetJsonAsync(url, ct).ConfigureAwait(false);
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

        // ── Search Videos ──

        public static async Task<JsonDocument?> SearchVideosAsync(
            string apiKey, string query, string? pageToken, CancellationToken ct)
        {
            var q = Uri.EscapeDataString(query ?? "");
            var url = $"{ApiBase}/search?part=snippet&q={q}&type=video&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            return await TryGetCachedJsonAsync(url, ct).ConfigureAwait(false);
        }

        // ── Channel Videos (via uploads playlist — costs 1 unit vs 100 for search) ──

        public static async Task<JsonDocument?> GetChannelVideosAsync(
            string apiKey, string channelId, string? pageToken, CancellationToken ct,
            string sortBy = "date")
        {
            // Derive uploads playlist ID: UC... → UU...
            var uploadsPlaylistId = "UU" + channelId.Substring(2);
            return await GetPlaylistVideosAsync(apiKey, uploadsPlaylistId, pageToken, ct)
                .ConfigureAwait(false);
        }

        // ── Channel Live Streams ──

        public static async Task<JsonDocument?> GetChannelLiveAsync(
            string apiKey, string channelId, string? pageToken, CancellationToken ct)
        {
            var url = $"{ApiBase}/search?part=snippet&channelId={Uri.EscapeDataString(channelId)}" +
                      $"&type=video&eventType=live&order=date&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            return await TryGetCachedJsonAsync(url, ct).ConfigureAwait(false);
        }

        // ── Playlist Videos ──

        public static async Task<JsonDocument?> GetPlaylistVideosAsync(
            string apiKey, string playlistId, string? pageToken, CancellationToken ct)
        {
            var url = $"{ApiBase}/playlistItems?part=snippet,contentDetails&playlistId={Uri.EscapeDataString(playlistId)}" +
                      $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(pageToken))
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            return await TryGetCachedJsonAsync(url, ct).ConfigureAwait(false);
        }

        // ── Trending ──

        public static async Task<JsonDocument?> GetTrendingAsync(
            string apiKey, string? regionCode, string? categoryId, CancellationToken ct)
        {
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics" +
                      $"&chart=mostPopular&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
            if (!string.IsNullOrEmpty(regionCode))
                url += $"&regionCode={Uri.EscapeDataString(regionCode)}";
            if (!string.IsNullOrEmpty(categoryId) && categoryId != "0")
                url += $"&videoCategoryId={Uri.EscapeDataString(categoryId)}";
            return await TryGetCachedJsonAsync(url, ct).ConfigureAwait(false);
        }

        // ── Video Details ──

        public static async Task<JsonDocument?> GetVideoDetailsAsync(
            string apiKey, string videoId, CancellationToken ct)
        {
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics,liveStreamingDetails" +
                      $"&id={Uri.EscapeDataString(videoId)}&key={Uri.EscapeDataString(apiKey)}";
            return await TryGetJsonAsync(url, ct).ConfigureAwait(false);
        }

        // ── Batch Video Details (up to 50 IDs) ──

        public static async Task<JsonDocument?> GetVideoDetailsBatchAsync(
            string apiKey, IEnumerable<string> videoIds, CancellationToken ct)
        {
            var ids = string.Join(",", videoIds);
            var url = $"{ApiBase}/videos?part=snippet,contentDetails,statistics,liveStreamingDetails" +
                      $"&id={Uri.EscapeDataString(ids)}&key={Uri.EscapeDataString(apiKey)}";
            return await TryGetCachedJsonAsync(url, ct).ConfigureAwait(false);
        }

        // ── Helpers ──

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
