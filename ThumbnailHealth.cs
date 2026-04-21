using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    /// <summary>
    /// Tracks the health of YouTube video thumbnail URLs and produces the best
    /// known-good URL for a given video.
    ///
    /// Why this exists:
    ///   Emby fetches thumbnail URLs lazily and caches the result on disk. If
    ///   a thumbnail returns 404 (deleted/private/region-blocked video) or a
    ///   transient CDN failure, Emby caches a broken/empty image and the
    ///   poster stays broken until the URL changes. We therefore probe each
    ///   URL ourselves, remember the result (in memory + on disk so it
    ///   survives restarts), and substitute a guaranteed-fetchable fallback
    ///   for videos that we know will return broken images.
    /// </summary>
    internal static class ThumbnailHealth
    {
        // Quality fallback chain. mqdefault is YouTube's most universally
        // available size, but we still chain through the other guaranteed
        // sizes before giving up.
        private static readonly string[] QualityChain = { "mqdefault", "hqdefault", "default" };

        // Final fallback when every YouTube thumb is broken — uses the
        // FolderIcons CDN (jsdelivr) which is highly available.
        private static readonly string FallbackUrl = FolderIcons.Videos;

        private enum Health : byte { Unknown = 0, Good = 1, Broken = 2 }

        private sealed record Entry(Health Status, long CheckedAtUtcMs);

        // Re-validate "good" entries every 7 days, "broken" entries every 30
        // days (so a temporarily-broken video can recover, but we don't
        // hammer i.ytimg.com for permanently dead ones).
        private static readonly long GoodTtlMs = (long)TimeSpan.FromDays(7).TotalMilliseconds;
        private static readonly long BrokenTtlMs = (long)TimeSpan.FromDays(30).TotalMilliseconds;

        // Key = "<videoId>|<quality>" → health
        private static readonly ConcurrentDictionary<string, Entry> Cache = new();

        // Tracks videoIds that already have an inflight validation, to avoid
        // probing the same one repeatedly within a single refresh cycle.
        private static readonly ConcurrentDictionary<string, byte> InFlight = new();

        // Limit concurrent HEAD requests so we don't accidentally hammer
        // i.ytimg.com when a large playlist refreshes.
        private static readonly SemaphoreSlim Gate = new(4, 4);

        private static readonly HttpClient Http = new HttpClient(
            new SocketsHttpHandler
            {
                AllowAutoRedirect = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                ConnectTimeout = TimeSpan.FromSeconds(5)
            })
        {
            Timeout = TimeSpan.FromSeconds(8),
            DefaultRequestHeaders = { { "User-Agent", "EmbyYouTubePlugin/1.0" } }
        };

        private static int _diskLoaded = 0;
        private const string DiskFileName = "thumb-health.json";
        private static readonly object DiskLock = new();
        private static long _lastFlushMs = 0;
        private static readonly long FlushIntervalMs = 60_000; // throttle disk writes to once per minute

        // ── Public API ──────────────────────────────────────────────

        /// <summary>
        /// Returns the best known-good thumbnail URL for the given video.
        /// If we know the preferred URL is broken, falls back through the
        /// quality chain, ultimately returning a guaranteed-available
        /// fallback image so Emby never receives a URL we know to be 404.
        /// </summary>
        public static string ResolveUrl(string? videoId, string? apiUrl)
        {
            EnsureLoaded();

            if (string.IsNullOrWhiteSpace(videoId))
                return string.IsNullOrWhiteSpace(apiUrl) ? FallbackUrl : StripQuery(apiUrl!);

            // Walk the quality chain and return the first one that's not
            // proven-broken. Unknown counts as "use it" — we'll validate in
            // the background.
            foreach (var quality in QualityChain)
            {
                if (GetStatus(videoId!, quality) != Health.Broken)
                    return BuildUrl(videoId!, quality);
            }

            // Everything is known-broken → use the safe fallback.
            return FallbackUrl;
        }

        /// <summary>
        /// Fire-and-forget: asynchronously HEAD-checks the preferred
        /// thumbnail and (if needed) the fallbacks for the given video.
        /// Safe to call repeatedly; deduplicates inflight checks and
        /// honors per-entry TTL.
        /// </summary>
        public static void EnqueueValidation(string? videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId)) return;
            EnsureLoaded();

            // Skip if we already have a fresh status for the preferred quality.
            if (GetStatus(videoId!, QualityChain[0]) != Health.Unknown)
                return;

            if (!InFlight.TryAdd(videoId!, 1)) return;

            _ = Task.Run(async () =>
            {
                try
                {
                    foreach (var quality in QualityChain)
                    {
                        var url = BuildUrl(videoId!, quality);
                        var ok = await ProbeAsync(url).ConfigureAwait(false);
                        SetStatus(videoId!, quality, ok ? Health.Good : Health.Broken);
                        // First good one wins — leave the rest as Unknown.
                        if (ok) break;
                    }
                    MaybeFlushToDisk();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ThumbnailHealth] Probe failed for {videoId}: {ex.GetType().Name}: {ex.Message}");
                }
                finally
                {
                    InFlight.TryRemove(videoId!, out _);
                }
            });
        }

        // ── Internals ───────────────────────────────────────────────

        private static string BuildUrl(string videoId, string quality)
            => $"https://i.ytimg.com/vi/{videoId}/{quality}.jpg";

        private static string StripQuery(string url)
        {
            var i = url.IndexOf('?');
            return i < 0 ? url : url.Substring(0, i);
        }

        private static Health GetStatus(string videoId, string quality)
        {
            var key = videoId + "|" + quality;
            if (!Cache.TryGetValue(key, out var entry))
                return Health.Unknown;

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var age = now - entry.CheckedAtUtcMs;
            var ttl = entry.Status == Health.Good ? GoodTtlMs : BrokenTtlMs;
            if (age > ttl)
            {
                Cache.TryRemove(key, out _);
                return Health.Unknown;
            }
            return entry.Status;
        }

        private static void SetStatus(string videoId, string quality, Health status)
        {
            var key = videoId + "|" + quality;
            Cache[key] = new Entry(status, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

        private static async Task<bool> ProbeAsync(string url)
        {
            await Gate.WaitAsync().ConfigureAwait(false);
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Head, url);
                using var resp = await Http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                if (resp.StatusCode == HttpStatusCode.NotFound) return false;
                if (!resp.IsSuccessStatusCode)
                {
                    // Treat transient errors (5xx, 429) as "unknown" by
                    // returning true — we'd rather optimistically use the URL
                    // than incorrectly mark it broken. The next refresh will
                    // re-probe.
                    return resp.StatusCode != HttpStatusCode.Gone
                        && resp.StatusCode != HttpStatusCode.Forbidden;
                }

                // Some "unavailable" placeholders are served with HTTP 200
                // and a fixed tiny size. The 120x90 placeholder is roughly
                // ~1.7 KB. If Content-Length is suspiciously small for the
                // requested size, treat as broken for the larger qualities.
                var len = resp.Content.Headers.ContentLength;
                if (len.HasValue && len.Value > 0 && len.Value < 1500
                    && (url.Contains("/mqdefault.jpg") || url.Contains("/hqdefault.jpg")))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                // Network/DNS errors are transient — don't mark broken.
                return true;
            }
            finally
            {
                Gate.Release();
            }
        }

        // ── Disk persistence ────────────────────────────────────────

        private static string? GetDiskPath()
        {
            var dir = Plugin.CachePath;
            if (string.IsNullOrEmpty(dir)) return null;
            return Path.Combine(dir, DiskFileName);
        }

        private static void EnsureLoaded()
        {
            if (Interlocked.CompareExchange(ref _diskLoaded, 1, 0) != 0) return;
            try
            {
                var path = GetDiskPath();
                if (path == null || !File.Exists(path)) return;

                var json = File.ReadAllText(path, Encoding.UTF8);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != JsonValueKind.Object) return;

                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (prop.Value.ValueKind != JsonValueKind.Object) continue;
                    var status = (Health)prop.Value.GetProperty("s").GetByte();
                    var checkedAt = prop.Value.GetProperty("t").GetInt64();
                    var ttl = status == Health.Good ? GoodTtlMs : BrokenTtlMs;
                    if (now - checkedAt > ttl) continue; // expired — skip
                    Cache[prop.Name] = new Entry(status, checkedAt);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ThumbnailHealth] Failed to load disk cache: {ex.Message}");
            }
        }

        private static void MaybeFlushToDisk()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var last = Interlocked.Read(ref _lastFlushMs);
            if (now - last < FlushIntervalMs) return;
            if (Interlocked.CompareExchange(ref _lastFlushMs, now, last) != last) return;

            try
            {
                var path = GetDiskPath();
                if (path == null) return;

                lock (DiskLock)
                {
                    using var ms = new MemoryStream();
                    using (var writer = new Utf8JsonWriter(ms))
                    {
                        writer.WriteStartObject();
                        foreach (var kv in Cache)
                        {
                            writer.WriteStartObject(kv.Key);
                            writer.WriteNumber("s", (byte)kv.Value.Status);
                            writer.WriteNumber("t", kv.Value.CheckedAtUtcMs);
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                    var tmp = path + ".tmp";
                    File.WriteAllBytes(tmp, ms.ToArray());
                    // Atomic replace: handles existing file in a single OS
                    // call so we don't lose the cache if the process crashes
                    // between delete and move.
                    File.Move(tmp, path, overwrite: true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ThumbnailHealth] Failed to flush disk cache: {ex.Message}");
            }
        }
    }
}
