using MediaBrowser.Controller.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel
    {
        internal enum ShortsClassification
        {
            Unknown = 0,
            Regular = 1,
            Short = 2
        }

        private static readonly HttpClient ShortsHttp = CreateShortsHttp();
        private const int ShortsProbeTimeoutSeconds = 10;
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
            var handler = YouTubeHttpClientFactory.CreateHandler(
                allowAutoRedirect: true,
                automaticDecompression: DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli);

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
            var handler = YouTubeHttpClientFactory.CreateHandler(
                allowAutoRedirect: false,
                automaticDecompression: DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli);
            var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(ShortsProbeTimeoutSeconds) };
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
        private const int ShortsProbeDocumentPrefixBytes = 80 * 1024;
        private static readonly byte[] ShortsWatchBootstrapMarker =
            Encoding.UTF8.GetBytes("\"WEB_PLAYER_CONTEXT_CONFIGS\"");
        private static readonly SemaphoreSlim ShortsUrlProbeConcurrency = new(8, 8);
        private static readonly object ShortsProbeCircuitLock = new();
        private static readonly TimeSpan ShortsProbeFailureWindow = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan ShortsProbeCooldown = TimeSpan.FromMinutes(1);
        private const int ShortsProbeFailureThreshold = 8;
        private static DateTime _shortsProbeFailureWindowStartedUtc = DateTime.MinValue;
        private static DateTime _shortsProbeCooldownUntilUtc = DateTime.MinValue;
        private static int _shortsProbeAmbiguousCount;
        private static int _probeCacheLoaded;
        private static long _probeCacheLastWriteTicks;

        private static string ShortsProbeCachePath =>
            System.IO.Path.Combine(
                Plugin.CachePath ?? System.IO.Path.GetTempPath(),
                "shorts-probe-cache-v3.json");

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

        // Only the /shorts/<id> endpoint is authoritative. A successful response
        // is accepted only after its YouTube watch bootstrap confirms the exact
        // Shorts URL; an explicit redirect to /watch is a regular video. Consent,
        // block, rate-limit and network outcomes remain Unknown so the caller can
        // fail closed when the user disabled Shorts.
        internal static async Task<ShortsClassification> IsShortByUrlProbeAsync(
            string videoId,
            CancellationToken ct)
        {
            if (ShortsUrlProbeCache.TryGetValue(videoId, out var entry)
                && (DateTime.UtcNow - entry.CachedAt) < ShortsUrlProbeTtl)
            {
                return entry.IsShort
                    ? ShortsClassification.Short
                    : ShortsClassification.Regular;
            }

            if (IsShortsProbeCircuitOpen())
                return ShortsClassification.Unknown;

            await ShortsUrlProbeConcurrency.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                // A concurrent request may have populated the cache while this
                // call waited for the global probe slot.
                if (ShortsUrlProbeCache.TryGetValue(videoId, out entry)
                    && (DateTime.UtcNow - entry.CachedAt) < ShortsUrlProbeTtl)
                {
                    return entry.IsShort
                        ? ShortsClassification.Short
                        : ShortsClassification.Regular;
                }

                if (IsShortsProbeCircuitOpen())
                    return ShortsClassification.Unknown;

                var url = $"https://www.youtube.com/shorts/{videoId}";
                var headProbe = await ProbeShortsRedirectAsync(url, HttpMethod.Head, ct)
                    .ConfigureAwait(false);
                var result = headProbe.Classification;

                if (result == ShortsClassification.Unknown && headProbe.ShouldRetryWithGet)
                {
                    // Some intermediaries handle HEAD differently. Retry the
                    // same authoritative URL with GET and inspect only a bounded
                    // document prefix for YouTube's watch bootstrap.
                    var getProbe = await ProbeShortsRedirectAsync(url, HttpMethod.Get, ct)
                        .ConfigureAwait(false);
                    result = getProbe.Classification;
                }

                if (result == ShortsClassification.Unknown)
                {
                    RegisterAmbiguousShortsProbe();
                    Log($"[YT] Shorts URL probe for {videoId}: ambiguous response, not cached");
                    return ShortsClassification.Unknown;
                }

                var isShort = result == ShortsClassification.Short;
                ShortsUrlProbeCache[videoId] = (isShort, DateTime.UtcNow);
                Log($"[YT] Shorts URL probe for {videoId}: classification={result}");
                EvictShortsUrlProbeCache();
                return result;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                RegisterAmbiguousShortsProbe();
                Log($"[YT] Shorts URL probe failed for {videoId}: {ex.Message}");
                return ShortsClassification.Unknown;
            }
            finally
            {
                ShortsUrlProbeConcurrency.Release();
            }
        }

        internal static bool IsShortsProbeCircuitOpen()
        {
            lock (ShortsProbeCircuitLock)
            {
                var now = DateTime.UtcNow;
                if (_shortsProbeCooldownUntilUtc > now)
                    return true;

                if (_shortsProbeCooldownUntilUtc != DateTime.MinValue)
                {
                    _shortsProbeCooldownUntilUtc = DateTime.MinValue;
                    _shortsProbeFailureWindowStartedUtc = DateTime.MinValue;
                    _shortsProbeAmbiguousCount = 0;
                }

                return false;
            }
        }

        private static void RegisterAmbiguousShortsProbe()
        {
            var opened = false;
            lock (ShortsProbeCircuitLock)
            {
                var now = DateTime.UtcNow;
                if (_shortsProbeFailureWindowStartedUtc == DateTime.MinValue
                    || now - _shortsProbeFailureWindowStartedUtc > ShortsProbeFailureWindow)
                {
                    _shortsProbeFailureWindowStartedUtc = now;
                    _shortsProbeAmbiguousCount = 0;
                }

                _shortsProbeAmbiguousCount++;
                if (_shortsProbeAmbiguousCount >= ShortsProbeFailureThreshold
                    && _shortsProbeCooldownUntilUtc <= now)
                {
                    _shortsProbeCooldownUntilUtc = now + ShortsProbeCooldown;
                    opened = true;
                }
            }

            if (opened)
                Log("[YT] Shorts URL probes temporarily paused for 1 minute after repeated ambiguous responses.");
        }

        internal static ShortsClassification ClassifyShortsProbeResponse(
            int status,
            Uri requestUri,
            Uri? location)
        {
            if (status >= 300 && status < 400 && location != null)
            {
                var resolved = location.IsAbsoluteUri
                    ? location
                    : new Uri(requestUri, location);
                var path = resolved.AbsolutePath;
                if (string.Equals(path, "/watch", StringComparison.OrdinalIgnoreCase))
                    return ShortsClassification.Regular;
                if (path.StartsWith("/shorts/", StringComparison.OrdinalIgnoreCase))
                    return ShortsClassification.Short;
            }

            return ShortsClassification.Unknown;
        }

        private static async Task<(ShortsClassification Classification, bool ShouldRetryWithGet)> ProbeShortsRedirectAsync(
            string url,
            HttpMethod method,
            CancellationToken ct)
        {
            using var requestCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            requestCts.CancelAfter(TimeSpan.FromSeconds(ShortsProbeTimeoutSeconds));
            var requestToken = requestCts.Token;

            using var req = new HttpRequestMessage(method, url);
            req.Headers.Referrer = new Uri("https://www.youtube.com/");

            using var resp = await ShortsProbeHttp.SendAsync(
                    req, HttpCompletionOption.ResponseHeadersRead, requestToken)
                .ConfigureAwait(false);

            var result = ClassifyShortsProbeResponse(
                (int)resp.StatusCode,
                req.RequestUri!,
                resp.Headers.Location);

            if (result != ShortsClassification.Unknown
                || resp.StatusCode != HttpStatusCode.OK
                || method == HttpMethod.Head)
            {
                var shouldRetryWithGet = method == HttpMethod.Head
                    && (resp.StatusCode == HttpStatusCode.OK
                        || resp.StatusCode == HttpStatusCode.MethodNotAllowed
                        || resp.StatusCode == HttpStatusCode.NotImplemented);
                return (result, shouldRetryWithGet);
            }

            var documentResult = await ClassifyShortsDocumentAsync(
                    resp, req.RequestUri!, requestToken)
                .ConfigureAwait(false);
            return (documentResult, false);
        }

        private static async Task<ShortsClassification> ClassifyShortsDocumentAsync(
            HttpResponseMessage response,
            Uri requestUri,
            CancellationToken ct)
        {
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (!string.Equals(mediaType, "text/html", StringComparison.OrdinalIgnoreCase))
                return ShortsClassification.Unknown;

            using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            var buffer = new byte[ShortsProbeDocumentPrefixBytes];
            var total = 0;
            while (total < buffer.Length)
            {
                var read = await stream.ReadAsync(
                        buffer.AsMemory(total, buffer.Length - total), ct)
                    .ConfigureAwait(false);
                if (read == 0)
                    break;
                total += read;
            }

            if (total == 0)
                return ShortsClassification.Unknown;

            var expectedUrl = requestUri.GetLeftPart(UriPartial.Path);
            var expectedUrlMarker = Encoding.UTF8.GetBytes(
                $"\"originalUrl\":\"{expectedUrl}\"");
            var hasExpectedUrl = ContainsBytes(buffer, total, expectedUrlMarker);
            var hasWatchBootstrap = ContainsBytes(
                buffer, total, ShortsWatchBootstrapMarker);

            return hasExpectedUrl && hasWatchBootstrap
                ? ShortsClassification.Short
                : ShortsClassification.Unknown;
        }

        private static bool ContainsBytes(byte[] buffer, int count, byte[] marker) =>
            buffer.AsSpan(0, count).IndexOf(marker) >= 0;

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

        private static async Task<ChannelPageProbeResult> GetChannelShortVideoIdsAsync(
            string channelId,
            CancellationToken cancellationToken)
        {
            if (!IsChannelId(channelId))
                return new ChannelPageProbeResult(new HashSet<string>(StringComparer.Ordinal), true);

            if (ShortsPageCache.TryGetValue(channelId, out var cached)
                && (DateTime.UtcNow - cached.CachedAt) < (cached.VideoIds.Count == 0 ? ShortsPageEmptyCacheTtl : ShortsPageCacheTtl))
            {
                return new ChannelPageProbeResult(
                    new HashSet<string>(cached.VideoIds, StringComparer.Ordinal),
                    true);
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);

            try
            {
                var url = $"https://www.youtube.com/channel/{channelId}/shorts";
                var html = await GetShortsPageHtmlAsync(url, cancellationToken).ConfigureAwait(false);
                var usedExternalFallback = false;
                if (!IsUsableShortsPage(html))
                {
                    var fallbackHtml = await TryGetShortsPageHtmlWithExternalToolAsync(url, cancellationToken)
                        .ConfigureAwait(false);
                    if (IsUsableShortsPage(fallbackHtml))
                    {
                        html = fallbackHtml!;
                        usedExternalFallback = true;
                    }
                }

                if (!IsUsableShortsPage(html))
                    throw new InvalidOperationException("YouTube returned a consent, block, or incomplete Shorts page.");

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

                return new ChannelPageProbeResult(ids, true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log($"[YT] Shorts page probe failed for {channelId}: {ex.Message}");
                if (ShortsPageCache.TryGetValue(channelId, out var stale))
                {
                    return new ChannelPageProbeResult(
                        new HashSet<string>(stale.VideoIds, StringComparer.Ordinal),
                        false);
                }
                return new ChannelPageProbeResult(ids, false);
            }
        }

        private static bool IsUsableShortsPage(string? html)
        {
            if (string.IsNullOrWhiteSpace(html) || html.Length < 512)
                return false;

            return html.IndexOf("ytInitialData", StringComparison.Ordinal) >= 0
                && (html.IndexOf("channelMetadataRenderer", StringComparison.Ordinal) >= 0
                    || html.IndexOf("tabRenderer", StringComparison.Ordinal) >= 0
                    || html.IndexOf("reelItemRenderer", StringComparison.Ordinal) >= 0
                    || html.IndexOf("shortsLockupViewModel", StringComparison.Ordinal) >= 0);
        }

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
                    if (!string.IsNullOrWhiteSpace(html))
                        return html;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
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
                startInfo.ArgumentList.Add("--timeout=15");
                startInfo.ArgumentList.Add("--tries=1");
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

            try
            {
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
                catch { }
                throw;
            }
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
