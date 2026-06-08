using MediaBrowser.Controller.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel
    {
        private static readonly HttpClient ShortsHttp = CreateShortsHttp();
        private const string ShortsBrowserUserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

        // Pre-accepted YouTube consent cookie. Without it, EU/UK requests get
        // bounced to consent.youtube.com which makes the redirect probe
        // useless — we can't tell if YouTube would have served the Short or
        // sent us to /watch.
        private const string ShortsConsentCookie = "SOCS=CAESEwgDEgk2NzQwODc2NzAaAmRlIAEaBgiAtbGwBg";

        // /shorts/<channel> lists actual shorts inside JSON structures that
        // are unique to Shorts: "reelItemRenderer", "reelWatchEndpoint" and
        // "shortsLockupViewModel", plus the literal /shorts/<id> URL form.
        // We deliberately do NOT match bare "videoId":"..." — that would
        // also pull in header / related / recommendation entries (regular
        // long-form videos) and gave us false positives.
        private static readonly Regex ShortsVideoIdRegex = new(
            @"(?:reelItemRenderer|reelWatchEndpoint|shortsLockupViewModel)[^{}]{0,400}?\\?[""']videoId\\?[""']\s*:\s*\\?[""']([A-Za-z0-9_-]{11})\\?[""']" +
            @"|(?:/|\\/|%2F)shorts(?:/|\\/|%2F)([A-Za-z0-9_-]{11})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static HttpClient CreateShortsHttp()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                ConnectTimeout = TimeSpan.FromSeconds(10),
                PooledConnectionLifetime = TimeSpan.FromMinutes(10)
            };

            var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", ShortsBrowserUserAgent);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9,de;q=0.8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Pragma", "no-cache");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", ShortsConsentCookie);
            return client;
        }

        // Separate client without auto-redirect so we can inspect the
        // 303/302 Location header directly. Going through auto-redirect
        // hides whether YouTube redirected /shorts/<id> to /watch (clearly
        // not a Short) or to a consent dialog (ambiguous).
        private static readonly HttpClient ShortsProbeHttp = CreateShortsProbeHttp();

        private static HttpClient CreateShortsProbeHttp()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                ConnectTimeout = TimeSpan.FromSeconds(10),
                PooledConnectionLifetime = TimeSpan.FromMinutes(10)
            };
            var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(10) };
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", ShortsBrowserUserAgent);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9,de;q=0.8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", ShortsConsentCookie);
            return client;
        }

        private static readonly ConcurrentDictionary<string, (bool IsShort, DateTime CachedAt)> ShortsUrlProbeCache =
            new(StringComparer.Ordinal);
        private static readonly TimeSpan ShortsUrlProbeTtl = TimeSpan.FromDays(1);
        private const int ShortsUrlProbeCacheMaxEntries = 5000;
        private static int _probeCacheLoaded;
        private static long _probeCacheLastWriteTicks;

        private static string ShortsProbeCachePath =>
            System.IO.Path.Combine(
                Plugin.CachePath ?? System.IO.Path.GetTempPath(),
                "shorts-probe-cache.json");

        // Loaded once per process. Persisting it across restarts means a user's
        // first refresh after an upgrade doesn't have to re-probe every Short.
        internal static void LoadShortsProbeCache()
        {
            if (Interlocked.CompareExchange(ref _probeCacheLoaded, 1, 0) != 0) return;
            try
            {
                var path = ShortsProbeCachePath;
                if (!System.IO.File.Exists(path)) return;
                var json = System.IO.File.ReadAllText(path);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object) return;

                var cutoff = DateTime.UtcNow - ShortsUrlProbeTtl;
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (prop.Value.ValueKind != System.Text.Json.JsonValueKind.Object) continue;
                    if (!prop.Value.TryGetProperty("isShort", out var isShortEl)) continue;
                    if (!prop.Value.TryGetProperty("cachedAt", out var cachedAtEl)) continue;
                    if (!cachedAtEl.TryGetInt64(out var ts)) continue;
                    var cachedAt = new DateTime(ts, DateTimeKind.Utc);
                    if (cachedAt < cutoff) continue;
                    ShortsUrlProbeCache[prop.Name] = (isShortEl.GetBoolean(), cachedAt);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeChannel] LoadShortsProbeCache failed: {ex.Message}");
            }
        }

        // Throttled to once a minute so a busy refresh doesn't keep rewriting
        // the file.
        internal static void PersistShortsProbeCache()
        {
            var now = Environment.TickCount64;
            var last = Interlocked.Read(ref _probeCacheLastWriteTicks);
            if (now - last < 60_000) return;
            if (Interlocked.CompareExchange(ref _probeCacheLastWriteTicks, now, last) != last) return;

            try
            {
                var path = ShortsProbeCachePath;
                var dir = System.IO.Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    System.IO.Directory.CreateDirectory(dir);

                using var ms = new System.IO.MemoryStream();
                using (var writer = new System.Text.Json.Utf8JsonWriter(ms))
                {
                    writer.WriteStartObject();
                    foreach (var kvp in ShortsUrlProbeCache)
                    {
                        writer.WriteStartObject(kvp.Key);
                        writer.WriteBoolean("isShort", kvp.Value.IsShort);
                        writer.WriteNumber("cachedAt", kvp.Value.CachedAt.Ticks);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                }
                System.IO.File.WriteAllBytes(path, ms.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeChannel] PersistShortsProbeCache failed: {ex.Message}");
            }
        }

        // The redirect itself is the only signal that's always reliable.
        // Metadata tags can be missing, but the /shorts/<id> URL either stays
        // a Short or 30x's to /watch.
        //
        // We check the redirect status directly instead of letting the client
        // follow it. With AllowAutoRedirect=true a consent dialog ends up as
        // the final URI and the old code defaulted to "not Short", which let
        // some Shorts slip into the videos folder. Looking at the Location
        // header explicitly tells us:
        //   - HTTP 200             → /shorts/<id> served → Short
        //   - 30x to /watch?v=<id> → regular video
        //   - 30x to consent/login → unclear, fall back to a HEAD on
        //                            /watch?v=<id> and check if YouTube
        //                            bounces it back to /shorts/
        internal static async Task<bool> IsShortByUrlProbeAsync(string videoId, CancellationToken ct)
        {
            if (ShortsUrlProbeCache.TryGetValue(videoId, out var entry)
                && (DateTime.UtcNow - entry.CachedAt) < ShortsUrlProbeTtl)
                return entry.IsShort;

            try
            {
                bool? result = await ProbeShortsRedirectAsync(
                        $"https://www.youtube.com/shorts/{videoId}", expectShorts: true, ct)
                    .ConfigureAwait(false);

                if (!result.HasValue)
                {
                    // Consent / unrelated redirect: flip the question and
                    // hit /watch?v=<id> to see if YouTube bounces it onto
                    // /shorts/<id>.
                    result = await ProbeShortsRedirectAsync(
                            $"https://www.youtube.com/watch?v={videoId}", expectShorts: false, ct)
                        .ConfigureAwait(false);
                }

                var isShort = result ?? false;
                ShortsUrlProbeCache[videoId] = (isShort, DateTime.UtcNow);
                Log($"[YT] Shorts URL probe for {videoId}: isShort={isShort}");
                EvictShortsUrlProbeCache();
                return isShort;
            }
            catch (Exception ex)
            {
                Log($"[YT] Shorts URL probe failed for {videoId}: {ex.Message}");
                return false;
            }
        }

        // Returns true/false when the response is clear, or null when YouTube
        // sent us somewhere unrelated (consent, login, etc).
        //   expectShorts=true  → starting URL is /shorts/<id>;
        //                        200 means Short, redirect to /watch means not.
        //   expectShorts=false → starting URL is /watch?v=<id>;
        //                        redirect to /shorts means Short, anything else
        //                        (200 on /watch, redirect to consent) means not.
        private static async Task<bool?> ProbeShortsRedirectAsync(string url, bool expectShorts, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Head, url);
            req.Headers.Referrer = new Uri("https://www.youtube.com/");

            using var resp = await ShortsProbeHttp.SendAsync(
                    req, HttpCompletionOption.ResponseHeadersRead, ct)
                .ConfigureAwait(false);

            var status = (int)resp.StatusCode;

            if (expectShorts)
            {
                if (status == 200) return true;
                if (status >= 300 && status < 400)
                {
                    var loc = resp.Headers.Location?.ToString() ?? "";
                    if (loc.Contains("/watch", StringComparison.OrdinalIgnoreCase))
                        return false;
                    if (loc.Contains("/shorts/", StringComparison.OrdinalIgnoreCase))
                        return true;
                    return null; // consent/login — ambiguous
                }
                return null;
            }
            else
            {
                if (status >= 300 && status < 400)
                {
                    var loc = resp.Headers.Location?.ToString() ?? "";
                    if (loc.Contains("/shorts/", StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
        }

        private static void EvictShortsUrlProbeCache()
        {
            if (ShortsUrlProbeCache.Count <= ShortsUrlProbeCacheMaxEntries)
                return;

            var now = DateTime.UtcNow;
            foreach (var kvp in ShortsUrlProbeCache)
            {
                if ((now - kvp.Value.CachedAt) > ShortsUrlProbeTtl)
                    ShortsUrlProbeCache.TryRemove(kvp.Key, out _);
            }

            if (ShortsUrlProbeCache.Count > ShortsUrlProbeCacheMaxEntries)
            {
                var oldest = ShortsUrlProbeCache
                    .OrderBy(kvp => kvp.Value.CachedAt)
                    .Take(ShortsUrlProbeCache.Count - ShortsUrlProbeCacheMaxEntries)
                    .Select(kvp => kvp.Key)
                    .ToList();
                foreach (var key in oldest)
                    ShortsUrlProbeCache.TryRemove(key, out _);
            }
        }

        private static async Task<HashSet<string>> GetChannelShortVideoIdsAsync(string channelId, CancellationToken cancellationToken)
        {
            if (!IsChannelId(channelId))
                return new HashSet<string>(StringComparer.Ordinal);

            if (ShortsPageCache.TryGetValue(channelId, out var cached)
                && (DateTime.UtcNow - cached.CachedAt) < (cached.VideoIds.Count == 0 ? ShortsPageEmptyCacheTtl : ShortsPageCacheTtl))
                return new HashSet<string>(cached.VideoIds, StringComparer.Ordinal);

            var ids = new HashSet<string>(StringComparer.Ordinal);

            try
            {
                var url = $"https://www.youtube.com/channel/{channelId}/shorts";
                var html = await GetShortsPageHtmlAsync(url, cancellationToken).ConfigureAwait(false);
                var usedExternalFallback = false;
                if (!HasShortsMarkers(html))
                {
                    var fallbackHtml = await TryGetShortsPageHtmlWithExternalToolAsync(url, cancellationToken)
                        .ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(fallbackHtml))
                    {
                        html = fallbackHtml;
                        usedExternalFallback = true;
                    }
                }

                foreach (Match match in ShortsVideoIdRegex.Matches(html))
                {
                    for (var groupIndex = 1; groupIndex < match.Groups.Count; groupIndex++)
                    {
                        var value = match.Groups[groupIndex].Value;
                        if (!string.IsNullOrEmpty(value))
                            ids.Add(value);
                    }
                }

                ShortsPageCache[channelId] = new ShortsPageCacheEntry(ids, DateTime.UtcNow);
                if (ids.Count == 0)
                {
                    var hasVideoIdMarker = html.IndexOf("\"videoId\":\"", StringComparison.Ordinal) >= 0
                                        || html.IndexOf("\\\"videoId\\\":\\\"", StringComparison.Ordinal) >= 0;
                    var hasShortsPathMarker = html.IndexOf("/shorts/", StringComparison.Ordinal) >= 0
                                           || html.IndexOf("\\/shorts\\/", StringComparison.Ordinal) >= 0;
                    Log($"[YT] Shorts page probe for {channelId}: 0 ids (html={html.Length}, videoIdMarker={hasVideoIdMarker}, shortsPathMarker={hasShortsPathMarker}, externalFallback={usedExternalFallback})");
                }
                else
                {
                    Log($"[YT] Shorts page probe for {channelId}: {ids.Count} ids (externalFallback={usedExternalFallback})");
                }
            }
            catch (Exception ex)
            {
                Log($"[YT] Shorts page probe failed for {channelId}: {ex.Message}");
                if (ShortsPageCache.TryGetValue(channelId, out var stale))
                    return new HashSet<string>(stale.VideoIds, StringComparer.Ordinal);
            }

            return ids;
        }

        private static bool HasShortsMarkers(string html) =>
            html.IndexOf("\"videoId\":\"", StringComparison.Ordinal) >= 0
            || html.IndexOf("\\\"videoId\\\":\\\"", StringComparison.Ordinal) >= 0
            || html.IndexOf("/shorts/", StringComparison.Ordinal) >= 0
            || html.IndexOf("\\/shorts\\/", StringComparison.Ordinal) >= 0;

        private static async Task<string> GetShortsPageHtmlAsync(string url, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Referrer = new Uri("https://www.youtube.com/");

            using var response = await ShortsHttp.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task<string?> TryGetShortsPageHtmlWithExternalToolAsync(
            string url,
            CancellationToken cancellationToken)
        {
            foreach (var tool in new[] { "curl", "wget" })
            {
                try
                {
                    var html = await RunShortsFetchToolAsync(tool, url, cancellationToken).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(html) && HasShortsMarkers(html))
                        return html;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[YouTubeChannel] Shorts {tool} fallback failed: {ex.Message}");
                }
            }

            return null;
        }

        private static async Task<string?> RunShortsFetchToolAsync(
            string tool,
            string url,
            CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = tool,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            if (string.Equals(tool, "curl", StringComparison.OrdinalIgnoreCase))
            {
                startInfo.ArgumentList.Add("-L");
                startInfo.ArgumentList.Add("-sS");
                startInfo.ArgumentList.Add("--max-time");
                startInfo.ArgumentList.Add("15");
                startInfo.ArgumentList.Add("-A");
                startInfo.ArgumentList.Add(ShortsBrowserUserAgent);
                AddCurlHeader(startInfo, "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                AddCurlHeader(startInfo, "Accept-Language: en-US,en;q=0.9,de;q=0.8");
                AddCurlHeader(startInfo, "Cache-Control: no-cache");
                AddCurlHeader(startInfo, "Pragma: no-cache");
                AddCurlHeader(startInfo, "Referer: https://www.youtube.com/");
                AddCurlHeader(startInfo, "Cookie: " + ShortsConsentCookie);
                startInfo.ArgumentList.Add(url);
            }
            else
            {
                startInfo.ArgumentList.Add("-q");
                startInfo.ArgumentList.Add("-O");
                startInfo.ArgumentList.Add("-");
                startInfo.ArgumentList.Add("-U");
                startInfo.ArgumentList.Add(ShortsBrowserUserAgent);
                AddWgetHeader(startInfo, "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                AddWgetHeader(startInfo, "Accept-Language: en-US,en;q=0.9,de;q=0.8");
                AddWgetHeader(startInfo, "Cache-Control: no-cache");
                AddWgetHeader(startInfo, "Pragma: no-cache");
                AddWgetHeader(startInfo, "Referer: https://www.youtube.com/");
                AddWgetHeader(startInfo, "Cookie: " + ShortsConsentCookie);
                startInfo.ArgumentList.Add(url);
            }

            using var process = Process.Start(startInfo);
            if (process == null)
                return null;

            // net6.0 has no CancellationToken overload of ReadToEndAsync; the
            // WaitForExitAsync below carries the cancellation, and the reads
            // complete when the process exits.
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            var stdout = await stdoutTask.ConfigureAwait(false);
            await stderrTask.ConfigureAwait(false);

            return process.ExitCode == 0 ? stdout : null;
        }

        private static void AddCurlHeader(ProcessStartInfo startInfo, string header)
        {
            startInfo.ArgumentList.Add("-H");
            startInfo.ArgumentList.Add(header);
        }

        private static void AddWgetHeader(ProcessStartInfo startInfo, string header)
        {
            startInfo.ArgumentList.Add("--header");
            startInfo.ArgumentList.Add(header);
        }

        private static void ApplyShortsPageMatches(List<ChannelItemInfo> batch, HashSet<string> shortsVideoIds)
        {
            if (shortsVideoIds.Count == 0)
                return;

            foreach (var item in batch)
            {
                if (item.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)
                    || item.Id.StartsWith(LivePrefix, StringComparison.Ordinal))
                    continue;

                var rawId = StripPrefix(item.Id);
                if (!shortsVideoIds.Contains(rawId))
                    continue;

                item.Id = ReelPrefix + rawId;
                if (!item.Name.StartsWith("Short:", StringComparison.Ordinal)
                    && !item.Name.StartsWith("▶ Short:", StringComparison.Ordinal))
                    item.Name = $"▶ Short: {item.Name}";
            }
        }
    }
}
