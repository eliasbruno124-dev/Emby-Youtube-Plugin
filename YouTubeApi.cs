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
        private record CachedResponse(
            string Json,
            long CachedAtMs,
            long ExpiresAtMs,
            string? CacheTag);
        private record DiskCacheHit(string Json, long AgeMs);
        private static readonly ConcurrentDictionary<string, CachedResponse> ResponseCache = new();
        private static readonly ConcurrentDictionary<string, SharedInFlightRequest> InFlightRequests = new();
        private static readonly object CacheMutationLock = new();
        private static HashSet<string>? _supportedContentRegions;
        private static long _cacheGeneration;
        private const int MaxCacheEntries = 200;

        // Memory cache lifetimes. Disk uses the same value per request,
        // capped by the global limit below.
        private const long CacheTtlMs = 15 * 60 * 1000;            // Default: 15 minutes.
        private const long FreshListTtlMs = 6 * 60 * 60 * 1000;       // Trending, uploads, and playlists: 6 hours.
        private const long ChannelDetailsCacheTtlMs = 6 * 60 * 60 * 1000;       // Channel names and thumbnails: 6 hours.
        private const long SearchTtlMs = 24 * 60 * 60 * 1000;           // Search has a separate daily call limit.
        private const long CategoriesTtlMs = 30L * 24 * 60 * 60 * 1000; // Categories rarely change.
        // This response also contains status, live state, and statistics, so it
        // must not be kept for months like immutable metadata.
        private const long VideoDetailTtlMs = 15 * 60 * 1000;

        // YouTube's API terms cap persisted API data at 30 days. Each call
        // gets the smaller of its own TTL and this disk limit.
        private const long DiskCacheTtlMs = 30L * 24 * 60 * 60 * 1000;
        private const string CacheKeySchemaPrefix = "youtube-api-cache-v2|";
        private const string PlaylistCacheTagPrefix = "playlist:";
        private static long _lastDiskCleanupUtcTicks;
        private static int _diskCleanupRunning;

        private static readonly HttpClient Http = new HttpClient(
            YouTubeHttpClientFactory.CreateHandler(
                allowAutoRedirect: true,
                automaticDecompression: DecompressionMethods.All))
        {
            // ResponseHeadersRead completes as soon as the headers arrive, so
            // HttpClient.Timeout would not cover a stalled response body. Each
            // attempt below has its own timeout spanning send, read, and parse.
            Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            DefaultRequestHeaders =
            {
                { "User-Agent", "EmbyYouTubePlugin/1.0" },
                { "Accept", "application/json" }
            }
        };

        private static readonly int[] RetryDelaysMs = { 1500, 4000 };
        private static readonly TimeSpan RequestAttemptTimeout = TimeSpan.FromSeconds(30);

        // Keep bursts gentle so YouTube is less likely to answer with 429s.
        // The gate has to be held for the whole request, not just the
        // bookkeeping below — otherwise we have no real concurrency limit.
        private static readonly SemaphoreSlim ApiGate = new(6, 6);
        private static readonly SemaphoreSlim RequestStartGate = new(1, 1);
        private static long _lastCallTicks = 0;
        private const int MinCallIntervalMs = 250;

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
                await ReserveRequestStartAsync(ct).ConfigureAwait(false);
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

        // One cancellable fetch can serve several callers. A caller may stop
        // waiting without affecting the others; the HTTP work is cancelled only
        // after the final waiter leaves. Abandoning the entry is synchronized so
        // a new waiter can never attach between the zero-waiter check and cancel.
        private sealed class SharedInFlightRequest
        {
            private readonly object _sync = new();
            private readonly CancellationTokenSource _cancellation = new();
            private readonly Lazy<Task<string?>> _work;
            private int _waiters;
            private bool _abandoned;
            private bool _completed;

            public SharedInFlightRequest(
                Func<CancellationToken, Task<string?>> factory,
                Action<SharedInFlightRequest> onCompleted)
            {
                _work = new Lazy<Task<string?>>(
                    async () =>
                    {
                        try
                        {
                            return await factory(_cancellation.Token).ConfigureAwait(false);
                        }
                        finally
                        {
                            lock (_sync)
                                _completed = true;
                            try
                            {
                                onCompleted(this);
                            }
                            finally
                            {
                                _cancellation.Dispose();
                            }
                        }
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public Task<string?> Work => _work.Value;

            public bool TryAddWaiter()
            {
                lock (_sync)
                {
                    if (_abandoned)
                        return false;

                    _waiters++;
                    return true;
                }
            }

            public bool ReleaseWaiterAndAbandonIfUnused()
            {
                lock (_sync)
                {
                    if (_waiters <= 0)
                        return false;

                    _waiters--;
                    if (_waiters != 0 || _completed || _abandoned)
                        return false;

                    _abandoned = true;
                    return true;
                }
            }

            public void Cancel()
            {
                try { _cancellation.Cancel(); }
                catch (ObjectDisposedException) { }
            }
        }

        private static async Task ReserveRequestStartAsync(CancellationToken ct)
        {
            await RequestStartGate.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                while (true)
                {
                    ct.ThrowIfCancellationRequested();
                    long waitMs;
                    lock (_budgetLock)
                    {
                        var now = Environment.TickCount64;
                        while (_requestTimestamps.Count > 0
                               && (now - _requestTimestamps.Peek()) >= BudgetWindowMs)
                        {
                            _requestTimestamps.Dequeue();
                        }

                        var spacingWait = _lastCallTicks == 0
                            ? 0
                            : Math.Max(0, MinCallIntervalMs - (now - _lastCallTicks));
                        var budgetWait = _requestTimestamps.Count < MaxRequestsPerWindow
                            ? 0
                            : Math.Max(1, BudgetWindowMs - (now - _requestTimestamps.Peek()));
                        waitMs = Math.Max(spacingWait, budgetWait);

                        if (waitMs <= 0)
                        {
                            _lastCallTicks = now;
                            _requestTimestamps.Enqueue(now);
                            return;
                        }
                    }

                    Debug.WriteLine($"[YouTubeApi] Request pacing active, waiting {waitMs}ms...");
                    await Task.Delay((int)Math.Min(waitMs, int.MaxValue), ct).ConfigureAwait(false);
                }
            }
            finally
            {
                RequestStartGate.Release();
            }
        }

        private static bool IsTransientError(HttpStatusCode code) =>
            code == HttpStatusCode.TooManyRequests ||
            code == HttpStatusCode.RequestTimeout ||
            code == HttpStatusCode.InternalServerError ||
            code == HttpStatusCode.BadGateway ||
            code == HttpStatusCode.ServiceUnavailable ||
            code == HttpStatusCode.GatewayTimeout;

        private static async Task<JsonDocument> GetJsonAsync(string url, CancellationToken ct)
        {
            HttpStatusCode? lastStatus = null;
            Exception? lastException = null;

            for (int attempt = 0; attempt <= RetryDelaysMs.Length; attempt++)
            {
                int? overrideDelayMs = null;
                var shouldRetry = true;

                try
                {
                    using (await AcquireGateAsync(ct).ConfigureAwait(false))
                    {
                        // Count every dispatched request, including retries and
                        // unsuccessful API responses. Cached reads never reach
                        // this point and therefore do not consume quota.
                        ct.ThrowIfCancellationRequested();
                        QuotaTracker.RecordCall(url);

                        using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                        attemptCts.CancelAfter(RequestAttemptTimeout);
                        var attemptToken = attemptCts.Token;

                        using var resp = await Http.GetAsync(
                                url,
                                HttpCompletionOption.ResponseHeadersRead,
                                attemptToken)
                            .ConfigureAwait(false);

                        lastException = null;
                        if (resp.IsSuccessStatusCode)
                        {
                            await using var stream = await resp.Content
                                .ReadAsStreamAsync(attemptToken).ConfigureAwait(false);
                            return await JsonDocument.ParseAsync(stream, cancellationToken: attemptToken)
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
                        else if (!IsTransientError(lastStatus.Value))
                        {
                            shouldRetry = false;
                        }
                    }
                }
                catch (OperationCanceledException ex) when (ct.IsCancellationRequested)
                {
                    throw new OperationCanceledException(
                        "YouTube API request was canceled.",
                        ex,
                        ct);
                }
                catch (OperationCanceledException ex)
                {
                    // HttpClient timeouts surface as OperationCanceledException
                    // even though the caller's token was not cancelled.
                    lastException = ex;
                    lastStatus = null;
                    Debug.WriteLine($"[YouTubeApi] Request timed out on attempt {attempt + 1}: {ex.Message}");
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    lastStatus = null;
                    Debug.WriteLine($"[YouTubeApi] Transport error on attempt {attempt + 1}: {ex.Message}");
                }
                catch (IOException ex)
                {
                    lastException = ex;
                    lastStatus = null;
                    Debug.WriteLine($"[YouTubeApi] Response stream error on attempt {attempt + 1}: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    lastException = ex;
                    lastStatus = null;
                    Debug.WriteLine($"[YouTubeApi] Invalid JSON on attempt {attempt + 1}: {ex.Message}");
                }

                if (!shouldRetry || attempt == RetryDelaysMs.Length)
                    break;

                var nextDelay = overrideDelayMs ?? RetryDelaysMs[attempt];
                if (nextDelay > 0)
                    await Task.Delay(nextDelay, ct).ConfigureAwait(false);
            }

            var message = lastStatus.HasValue
                ? $"YouTube API returned HTTP {(int)lastStatus.Value}"
                : "YouTube API request failed before a valid response was received";
            throw new HttpRequestException(message, lastException, lastStatus);
        }

        private static async Task<JsonDocument?> TryGetJsonAsync(string url, CancellationToken ct)
        {
            try
            {
                return await GetJsonAsync(url, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] TryGetJsonAsync failed: {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }

        private static async Task<JsonDocument?> TryGetCachedJsonAsync(
            string url,
            CancellationToken ct,
            long? customTtlMs = null,
            string? cacheTag = null)
        {
            ct.ThrowIfCancellationRequested();
            var memTtl = customTtlMs ?? CacheTtlMs;
            var diskTtl = Math.Min(memTtl, DiskCacheTtlMs);
            var now = Environment.TickCount64;

            // First stop: in-memory cache.
            if (ResponseCache.TryGetValue(url, out var cached))
            {
                if (now < cached.ExpiresAtMs)
                {
                    try { return JsonDocument.Parse(cached.Json); }
                    catch (JsonException ex)
                    {
                        Debug.WriteLine($"[YouTubeApi] Dropping invalid memory cache entry: {ex.Message}");
                        ResponseCache.TryRemove(url, out _);
                    }
                }
                else
                {
                    ResponseCache.TryRemove(url, out _);
                }
            }

            // Next stop: on-disk cache.
            var diskReadGeneration = Interlocked.Read(ref _cacheGeneration);
            var diskHit = TryReadDiskCache(url, diskTtl, cacheTag);
            if (diskHit != null)
            {
                // Preserve the original age. Promoting a nearly expired disk
                // entry must not grant it a brand-new full memory lifetime.
                var remainingMs = Math.Max(1, memTtl - diskHit.AgeMs);
                var promoted = false;
                lock (CacheMutationLock)
                {
                    if (Interlocked.Read(ref _cacheGeneration) == diskReadGeneration)
                    {
                        ResponseCache[url] = new CachedResponse(
                            diskHit.Json,
                            now - diskHit.AgeMs,
                            now + remainingMs,
                            cacheTag);
                        EvictCacheIfNeeded();
                        promoted = true;
                    }
                }
                if (promoted)
                    return JsonDocument.Parse(diskHit.Json);
            }

            // Coalesce identical cache misses. A cancelled caller stops waiting
            // without affecting peers, while the final departing waiter cancels
            // the shared HTTP work so abandoned misses cannot occupy the gate.
            var cacheGeneration = Interlocked.Read(ref _cacheGeneration);
            var inFlightKey = GetInFlightKey(url, memTtl, cacheTag, cacheGeneration);
            SharedInFlightRequest pending;
            while (true)
            {
                pending = InFlightRequests.GetOrAdd(
                    inFlightKey,
                    _ => new SharedInFlightRequest(
                        cancellationToken => FetchAndCacheJsonAsync(
                            url,
                            memTtl,
                            cacheTag,
                            cacheGeneration,
                            cancellationToken),
                        completed => RemoveInFlightRequest(inFlightKey, completed)));
                if (pending.TryAddWaiter())
                    break;

                RemoveInFlightRequest(inFlightKey, pending);
            }

            string? freshJson;
            try
            {
                freshJson = await pending.Work.WaitAsync(ct).ConfigureAwait(false);
            }
            finally
            {
                if (pending.ReleaseWaiterAndAbandonIfUnused())
                {
                    RemoveInFlightRequest(inFlightKey, pending);
                    pending.Cancel();
                }
            }
            if (freshJson != null)
                return JsonDocument.Parse(freshJson);

            // If YouTube is unreachable, fall back to the freshest disk copy
            // we have. Stale content beats an empty channel during an outage
            // or after quota is blown.
            var staleReadGeneration = Interlocked.Read(ref _cacheGeneration);
            var staleHit = TryReadDiskCache(url, DiskCacheTtlMs, cacheTag);
            if (staleHit != null)
            {
                Debug.WriteLine("[YouTubeApi] API unavailable, serving stale cache");
                // Keep the stale copy in memory briefly so we retry soon
                // without hitting the disk on every request during an outage.
                var promoted = false;
                lock (CacheMutationLock)
                {
                    if (Interlocked.Read(ref _cacheGeneration) == staleReadGeneration)
                    {
                        var staleCachedAt = Environment.TickCount64;
                        ResponseCache[url] = new CachedResponse(
                            staleHit.Json,
                            staleCachedAt,
                            staleCachedAt + (5 * 60 * 1000),
                            cacheTag);
                        EvictCacheIfNeeded();
                        promoted = true;
                    }
                }
                if (promoted)
                    return JsonDocument.Parse(staleHit.Json);
            }

            return null;
        }

        private static string GetInFlightKey(
            string url,
            long ttlMs,
            string? cacheTag,
            long cacheGeneration) =>
            $"{cacheGeneration}|{ttlMs}|{cacheTag ?? string.Empty}|{url}";

        private static bool RemoveInFlightRequest(
            string inFlightKey,
            SharedInFlightRequest request) =>
            ((ICollection<KeyValuePair<string, SharedInFlightRequest>>)InFlightRequests)
            .Remove(new KeyValuePair<string, SharedInFlightRequest>(inFlightKey, request));

        private static async Task<string?> FetchAndCacheJsonAsync(
            string url,
            long memTtl,
            string? cacheTag,
            long cacheGeneration,
            CancellationToken cancellationToken)
        {
            using var doc = await TryGetJsonAsync(url, cancellationToken).ConfigureAwait(false);
            if (doc == null)
                return null;

            var json = doc.RootElement.GetRawText();
            lock (CacheMutationLock)
            {
                if (Interlocked.Read(ref _cacheGeneration) == cacheGeneration)
                {
                    var cachedAt = Environment.TickCount64;
                    ResponseCache[url] = new CachedResponse(
                        json,
                        cachedAt,
                        cachedAt + memTtl,
                        cacheTag);
                    EvictCacheIfNeeded();
                    WriteDiskCache(url, json, cacheTag);
                }
            }
            return json;
        }

        // ---- disk cache helpers ----

        private static string GetDiskCacheKey(string url)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(CacheKeySchemaPrefix + url));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private static string GetDiskCacheTagKey(string? cacheTag)
        {
            if (string.IsNullOrEmpty(cacheTag))
                return "general";

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(CacheKeySchemaPrefix + "tag|" + cacheTag));
            return Convert.ToHexString(bytes).ToLowerInvariant().Substring(0, 24);
        }

        private static string GetDiskCacheFile(string cacheDir, string url, string? cacheTag) =>
            Path.Combine(
                cacheDir,
                $"{GetDiskCacheTagKey(cacheTag)}-{GetDiskCacheKey(url)}.json");

        private static string GetPlaylistCacheTag(string playlistId) =>
            PlaylistCacheTagPrefix + (playlistId ?? string.Empty).Trim();

        private static DiskCacheHit? TryReadDiskCache(string url, long ttlMs, string? cacheTag)
        {
            string? file = null;
            try
            {
                var cacheDir = Plugin.CachePath;
                if (string.IsNullOrEmpty(cacheDir)) return null;

                MaybeRunDiskCleanup(cacheDir);
                file = GetDiskCacheFile(cacheDir, url, cacheTag);
                if (!File.Exists(file)) return null;

                var lastWrite = File.GetLastWriteTimeUtc(file);
                var ageMs = Math.Max(0, (long)(DateTime.UtcNow - lastWrite).TotalMilliseconds);
                if (ageMs > ttlMs) return null;

                var json = File.ReadAllText(file, Encoding.UTF8);
                // Validate before returning or promoting the entry. A power loss
                // must not leave a permanently unreadable cache file in the path.
                using (var parsed = JsonDocument.Parse(json))
                {
                    if (parsed.RootElement.ValueKind != JsonValueKind.Object)
                        throw new JsonException("Cached YouTube response root is not a JSON object.");
                }
                return new DiskCacheHit(json, ageMs);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[YouTubeApi] Dropping corrupt disk cache entry: {ex.Message}");
                TryDeleteCacheFile(file);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] ReadDiskCache failed: {ex.Message}");
                return null;
            }
        }

        private static void WriteDiskCache(string url, string json, string? cacheTag)
        {
            string? tempFile = null;
            try
            {
                var cacheDir = Plugin.CachePath;
                if (string.IsNullOrEmpty(cacheDir)) return;

                Directory.CreateDirectory(cacheDir);
                var file = GetDiskCacheFile(cacheDir, url, cacheTag);
                tempFile = file + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(tempFile, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                File.Move(tempFile, file, overwrite: true);
                tempFile = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] WriteDiskCache failed: {ex.Message}");
            }
            finally
            {
                TryDeleteCacheFile(tempFile);
            }
        }

        private static void MaybeRunDiskCleanup(string cacheDir)
        {
            var nowTicks = DateTime.UtcNow.Ticks;
            var lastTicks = Interlocked.Read(ref _lastDiskCleanupUtcTicks);
            if (lastTicks != 0 && nowTicks - lastTicks < TimeSpan.TicksPerDay)
                return;
            if (Interlocked.CompareExchange(ref _diskCleanupRunning, 1, 0) != 0)
                return;

            Interlocked.Exchange(ref _lastDiskCleanupUtcTicks, nowTicks);
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
                    var tempCutoff = DateTime.UtcNow.AddDays(-1);
                    foreach (var file in Directory.EnumerateFiles(cacheDir, "*.tmp"))
                    {
                        try
                        {
                            if (File.GetLastWriteTimeUtc(file) < tempCutoff)
                                File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[YouTubeApi] Temp cache cleanup skipped {Path.GetFileName(file)}: {ex.Message}");
                        }
                    }
                    Debug.WriteLine("[YouTubeApi] Disk cache cleanup done.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[YouTubeApi] Disk cache cleanup failed: {ex.Message}");
                }
                finally
                {
                    Interlocked.Exchange(ref _diskCleanupRunning, 0);
                }
            });
        }

        private static void TryDeleteCacheFile(string? file)
        {
            if (string.IsNullOrEmpty(file)) return;
            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] Cache file delete failed: {ex.Message}");
            }
        }

        private static void EvictCacheIfNeeded()
        {
            if (ResponseCache.Count <= MaxCacheEntries) return;
            var now = Environment.TickCount64;

            // First pass: drop expired entries.
            foreach (var kvp in ResponseCache)
            {
                if (now >= kvp.Value.ExpiresAtMs)
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
        /// Clears cached responses associated with a playlist. Only a hash of
        /// the safe playlist tag is persisted in filenames; request URLs and API
        /// keys are never written as cache metadata.
        /// </summary>
        public static void InvalidateCacheContaining(string substring)
        {
            if (string.IsNullOrEmpty(substring)) return;
            try
            {
                lock (CacheMutationLock)
                {
                    try
                    {
                        var playlistTag = GetPlaylistCacheTag(substring);
                        foreach (var kvp in ResponseCache.ToList())
                        {
                            if (string.Equals(kvp.Value.CacheTag, playlistTag, StringComparison.Ordinal)
                                || kvp.Key.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                ResponseCache.TryRemove(kvp.Key, out _);
                                var cacheDir = Plugin.CachePath;
                                if (!string.IsNullOrEmpty(cacheDir))
                                    TryDeleteCacheFile(GetDiskCacheFile(cacheDir, kvp.Key, kvp.Value.CacheTag));
                            }
                        }

                        var diskCacheDir = Plugin.CachePath;
                        if (!string.IsNullOrEmpty(diskCacheDir) && Directory.Exists(diskCacheDir))
                        {
                            var prefix = GetDiskCacheTagKey(playlistTag) + "-";
                            foreach (var file in Directory.EnumerateFiles(diskCacheDir, "*.json"))
                            {
                                if (Path.GetFileName(file).StartsWith(prefix, StringComparison.Ordinal))
                                    TryDeleteCacheFile(file);
                            }
                        }
                    }
                    finally
                    {
                        // Advance only after deletion. A disk reader that starts
                        // while this lock is held must retain the old generation
                        // and fail its promotion check after invalidation ends.
                        Interlocked.Increment(ref _cacheGeneration);
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
            try
            {
                lock (CacheMutationLock)
                {
                    Interlocked.Increment(ref _cacheGeneration);
                    ResponseCache.Clear();
                }
            }
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
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
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

        public static async Task<(string? name, string? thumb, int videoCount, bool lookupSucceeded)>
            GetPlaylistDetailsAsync(string apiKey, string playlistId, CancellationToken ct)
        {
            try
            {
                var url = $"{ApiBase}/playlists?part=snippet,contentDetails&id={Uri.EscapeDataString(playlistId)}&key={Uri.EscapeDataString(apiKey)}";
                using var doc = await TryGetCachedJsonAsync(
                        url,
                        ct,
                        ChannelDetailsCacheTtlMs,
                        GetPlaylistCacheTag(playlistId))
                    .ConfigureAwait(false);
                if (doc == null) return (null, null, 0, false);

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
                    return (name, thumb, count, true);
                }

                return (null, null, 0, true);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeApi] GetPlaylistDetailsAsync failed for '{playlistId}': {ex.Message}");
            }
            return (null, null, 0, false);
        }

        // search.list has its own daily call bucket. Cache each query for a full
        // day so browsing does not repeatedly spend those calls.
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
            // "date" is the default and uses the cheap uploads playlist.
            // Anything else needs search.list with an order parameter.
            var normalized = (sortBy ?? "date").Trim().ToLowerInvariant() switch
            {
                "viewcount" => "viewCount",
                "rating" => "rating",
                "relevance" => "relevance",
                _ => "date"
            };
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
            return await TryGetCachedJsonAsync(
                    url,
                    ct,
                    FreshListTtlMs,
                    GetPlaylistCacheTag(playlistId))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the first up-to-N video IDs from a playlist while bypassing
        /// every cache. Used by the watch-later poll so we always see the live
        /// state of the playlist, not the cached six-hour snapshot.
        /// </summary>
        public static async Task<List<string>> GetPlaylistVideoIdsFreshAsync(
            string apiKey, string playlistId, int maxItems, CancellationToken ct)
        {
            var snapshot = await GetPlaylistSnapshotFreshAsync(apiKey, playlistId, maxItems, ct)
                .ConfigureAwait(false);
            return snapshot.VideoIds;
        }

        public static async Task<(List<string> VideoIds, int TotalResults)> GetPlaylistSnapshotFreshAsync(
            string apiKey, string playlistId, int maxItems, CancellationToken ct)
        {
            if (maxItems <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxItems));

            var ids = new List<string>();
            var seenPageTokens = new HashSet<string>(StringComparer.Ordinal);
            string? pageToken = null;
            var totalResults = -1;
            // 5 pages * 50 = 250 IDs is plenty for change detection.
            for (int page = 0; page < 5 && ids.Count < maxItems; page++)
            {
                if (!seenPageTokens.Add(pageToken ?? string.Empty))
                    throw new InvalidOperationException($"Fresh playlist check returned a repeated page token for '{playlistId}'.");

                var url = $"{ApiBase}/playlistItems?part=contentDetails&playlistId={Uri.EscapeDataString(playlistId)}" +
                          $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";
                if (!string.IsNullOrEmpty(pageToken))
                    url += $"&pageToken={Uri.EscapeDataString(pageToken)}";

                // This is a change detector. Returning a partial list after a
                // failed later page would look like a real playlist change, so
                // any request failure must propagate to the poller.
                try
                {
                    using var doc = await GetJsonAsync(url, ct).ConfigureAwait(false);
                    if (!doc.RootElement.TryGetProperty("items", out var items)
                        || items.ValueKind != JsonValueKind.Array)
                    {
                        throw new JsonException("Playlist response does not contain an items array.");
                    }

                    if (page == 0)
                    {
                        if (!doc.RootElement.TryGetProperty("pageInfo", out var pageInfo)
                            || pageInfo.ValueKind != JsonValueKind.Object
                            || !pageInfo.TryGetProperty("totalResults", out var totalResultsElement)
                            || !totalResultsElement.TryGetInt32(out totalResults))
                        {
                            throw new JsonException("Playlist response does not contain pageInfo.totalResults.");
                        }
                    }

                    foreach (var item in items.EnumerateArray())
                    {
                        var vid = GetNestedString(item, "contentDetails", "videoId");
                        if (!string.IsNullOrEmpty(vid)) ids.Add(vid!);
                        if (ids.Count >= maxItems) break;
                    }

                    pageToken = doc.RootElement.TryGetProperty("nextPageToken", out var ptEl)
                        ? ptEl.GetString()
                        : null;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Fresh playlist check failed for '{playlistId}' on page {page + 1}.",
                        ex);
                }
                if (string.IsNullOrEmpty(pageToken)) break;
            }

            if (totalResults < 0)
                throw new JsonException("Playlist response did not provide a total result count.");

            return (ids, totalResults);
        }

        // Trending videos.

        public static async Task<JsonDocument?> GetTrendingAsync(
            string apiKey, string? regionCode, string? categoryId, CancellationToken ct)
        {
            regionCode = await ResolveContentRegionAsync(apiKey, regionCode, fallback: null, ct)
                .ConfigureAwait(false);
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
            regionCode = await ResolveContentRegionAsync(apiKey, regionCode, fallback: "US", ct)
                .ConfigureAwait(false) ?? "US";
            var url = $"{ApiBase}/videoCategories?part=snippet&regionCode={Uri.EscapeDataString(regionCode)}" +
                      $"&key={Uri.EscapeDataString(apiKey)}";
            // Categories almost never change, so we keep them for the full
            // disk window.
            return await TryGetCachedJsonAsync(url, ct, CategoriesTtlMs).ConfigureAwait(false);
        }

        public static async Task<JsonDocument?> GetI18nRegionsAsync(
            string apiKey,
            CancellationToken ct)
        {
            var url = $"{ApiBase}/i18nRegions?part=snippet&hl=en_US&key={Uri.EscapeDataString(apiKey)}";
            var doc = await TryGetCachedJsonAsync(url, ct, CategoriesTtlMs).ConfigureAwait(false);
            if (doc == null)
                return null;

            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (doc.RootElement.TryGetProperty("items", out var items)
                && items.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in items.EnumerateArray())
                {
                    var code = GetString(item, "id")
                               ?? GetNestedString(item, "snippet", "gl");
                    if (code is { Length: 2 }
                        && code.All(character =>
                            character is >= 'A' and <= 'Z'
                            || character is >= 'a' and <= 'z'))
                        codes.Add(code.ToUpperInvariant());
                }
            }

            if (codes.Count > 0)
                Volatile.Write(ref _supportedContentRegions, codes);
            return doc;
        }

        internal static async Task<string?> ResolveContentRegionAsync(
            string apiKey,
            string? regionCode,
            string? fallback,
            CancellationToken ct)
        {
            var normalized = (regionCode ?? string.Empty).Trim().ToUpperInvariant();
            if (normalized.Length == 0)
                return fallback;

            var supported = Volatile.Read(ref _supportedContentRegions);
            if (supported == null)
            {
                using var regionDocument = await GetI18nRegionsAsync(apiKey, ct).ConfigureAwait(false);
                supported = Volatile.Read(ref _supportedContentRegions);
            }

            // If the validation endpoint itself is temporarily unavailable,
            // preserve the saved value; the normal request can still use a
            // previously valid region. Once validation succeeds, unsupported
            // values deterministically fall back instead of producing empty
            // Trending/Categories folders.
            return supported == null || supported.Contains(normalized)
                ? normalized
                : fallback;
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
            // Availability, live state, and statistics can change quickly.
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
