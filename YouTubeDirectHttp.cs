using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    internal sealed record YouTubeMediaProbe(int ReceivedBytes, TimeSpan Elapsed);

    // A private, bounded fetcher, never a URL-taking public service. Signed URLs
    // and transport exceptions must not escape through logs or exception chains.
    internal static class YouTubeDirectHttp
    {
        private const int MaximumPayloadBytes = 4 * 1024 * 1024;
        internal const int MaximumMediaPlaylistBytes = 8 * 1024 * 1024;
        private static readonly SemaphoreSlim RequestSlots = new(4, 4);
        private static int _pendingRequests;
        private static readonly HttpClient Client = CreateClient();
        private static readonly HttpClient RangeClient = CreateClient(false);
        private static readonly HashSet<string> AllowedHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
            "User-Agent", "Accept", "Accept-Language", "Sec-Fetch-Mode", "Referer", "Origin"
        };

        internal static Uri ValidateMediaUri(string value)
        {
            var uri = ValidateHttpsUri(value);
            if (!IsMediaHost(uri.Host))
                throw Failure("YouTube media host is not allowed.");
            return uri;
        }

        internal static Uri ValidateCaptionUri(string value)
        {
            var uri = ValidateHttpsUri(value);
            if (IsMediaHost(uri.Host)) return uri;
            if (!IsYouTubeHost(uri.Host) || uri.AbsolutePath != "/api/timedtext")
                throw Failure("YouTube caption host or path is not allowed.");
            return uri;
        }

        internal static Dictionary<string, string> ReadHeaders(JsonElement headers)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headers.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null) return result;
            if (headers.ValueKind != JsonValueKind.Object) throw Failure("Invalid YouTube headers.");
            foreach (var property in headers.EnumerateObject())
            {
                if (result.Count >= 16 || property.Value.ValueKind != JsonValueKind.String
                    || !result.TryAdd(property.Name, property.Value.GetString() ?? ""))
                    throw Failure("Invalid YouTube headers.");
            }
            ValidateHeaders(result);
            return result;
        }

        internal static Task<byte[]> GetBytesAsync(
            Uri uri, IReadOnlyDictionary<string, string> headers, int maximumBytes,
            CancellationToken cancellationToken) =>
            FetchAsync(uri, headers, maximumBytes, cancellationToken, Client);

        // Long VOD playlists repeat large signed URLs for every segment. Only
        // this media-playlist path has the larger envelope: captions, generic
        // payloads and exact byte ranges retain their existing 4 MiB limits.
        internal static Task<byte[]> GetMediaPlaylistBytesAsync(
            Uri uri, IReadOnlyDictionary<string, string> headers, int maximumBytes,
            CancellationToken cancellationToken) =>
            FetchMediaPlaylistAsync(uri, headers, maximumBytes, cancellationToken, Client);

        internal static Dictionary<string, string> CopyHeaders(IReadOnlyDictionary<string, string> headers)
        {
            ValidateHeaders(headers);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers)
                if (!result.TryAdd(header.Key, header.Value)) throw Failure("Invalid YouTube headers.");
            return result;
        }

        internal static Task<byte[]> GetRangeBytesAsync(Uri uri, IReadOnlyDictionary<string, string> headers,
            long offset, int count, long totalLength, CancellationToken cancellationToken) =>
            FetchRangeAsync(uri, headers, offset, count, totalLength, cancellationToken, RangeClient);

        internal const int MaximumProbeBytes = 512 * 1024;
        internal const int MinimumProbeBytes = 64 * 1024;

        internal static Task<YouTubeMediaProbe> ProbeMediaAsync(Uri uri,
            IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken) =>
            FetchProbeAsync(uri, headers, cancellationToken, RangeClient);

        // A startup observation, not a media proxy or a speed-test endpoint.
        // Consume at most a prefix of one ordinary MPEG-TS media segment. The
        // caller supplies the overall playlist+probe deadline; this additional
        // deadline also bounds a future caller that omits one. Partial data at
        // deadline is usable only after a substantial verified media payload.
        private static async Task<YouTubeMediaProbe> FetchProbeAsync(Uri uri,
            IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken, HttpClient client)
        {
            if (uri == null) throw Failure("Invalid YouTube probe URL.");
            var current = ValidateMediaUri(uri.OriginalString);
            ValidateHeaders(headers);
            if (Interlocked.Increment(ref _pendingRequests) > 16)
            {
                Interlocked.Decrement(ref _pendingRequests);
                throw Failure("YouTube probe capacity is unavailable.");
            }
            bool acquired = false;
            int received = 0;
            var prefix = new byte[377];
            var elapsed = Stopwatch.StartNew();
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(TimeSpan.FromMilliseconds(1200));
            bool HasMediaSample() => received >= MinimumProbeBytes
                && prefix[0] == 0x47 && prefix[188] == 0x47 && prefix[376] == 0x47;
            try
            {
                await RequestSlots.WaitAsync(deadline.Token).ConfigureAwait(false);
                acquired = true;
                for (int redirect = 0; redirect <= 3; redirect++)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, current);
                    foreach (var header in headers)
                        if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                            throw Failure("Invalid YouTube headers.");
                    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity"));
                    request.Headers.Range = new RangeHeaderValue(0, MaximumProbeBytes - 1);
                    using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                        deadline.Token).ConfigureAwait(false);
                    if (IsRedirect(response.StatusCode))
                    {
                        if (redirect == 3 || response.Headers.Location == null)
                            throw Failure("YouTube probe redirect limit exceeded.");
                        current = ValidateMediaUri(new Uri(current, response.Headers.Location).AbsoluteUri);
                        continue;
                    }
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                        throw Failure("YouTube media probe was unsuccessful.");
                    if (response.Content.Headers.ContentEncoding.Any(value => !value.Equals("identity", StringComparison.OrdinalIgnoreCase)))
                        throw Failure("YouTube probe representation is encoded.");
                    var type = response.Content.Headers.ContentType?.MediaType;
                    if (!string.Equals(type, "video/mp2t", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(type, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
                        throw Failure("YouTube probe did not return supported media.");
                    if (response.StatusCode == HttpStatusCode.PartialContent)
                    {
                        var range = response.Content.Headers.ContentRange;
                        if (range == null || range.Unit != "bytes" || range.From != 0
                            || !range.To.HasValue || range.To < 0 || range.To >= MaximumProbeBytes
                            || !range.Length.HasValue || range.Length <= range.To
                            || response.Content.Headers.ContentLength != range.To + 1
                            || (range.To != MaximumProbeBytes - 1 && range.To != range.Length - 1))
                            throw Failure("YouTube probe range is invalid.");
                    }
                    using var body = await response.Content.ReadAsStreamAsync(deadline.Token).ConfigureAwait(false);
                    var buffer = new byte[16 * 1024];
                    while (received < MaximumProbeBytes)
                    {
                        int read = await body.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, MaximumProbeBytes - received)),
                            deadline.Token).ConfigureAwait(false);
                        if (read == 0)
                        {
                            if (response.Content.Headers.ContentLength is { } length && received < length)
                                throw Failure("YouTube probe body is incomplete.");
                            break;
                        }
                        if (received < prefix.Length)
                            Array.Copy(buffer, 0, prefix, received, Math.Min(read, prefix.Length - received));
                        received += read;
                        if (received >= prefix.Length
                            && (prefix[0] != 0x47 || prefix[188] != 0x47 || prefix[376] != 0x47))
                            throw Failure("YouTube probe did not return MPEG-TS media.");
                    }
                    if (!HasMediaSample()) throw Failure("YouTube media probe sample is too small.");
                    return new YouTubeMediaProbe(received, elapsed.Elapsed);
                }
                throw Failure("YouTube probe redirect limit exceeded.");
            }
            catch (OperationCanceledException)
            {
                if (HasMediaSample()) return new YouTubeMediaProbe(received, elapsed.Elapsed);
                throw new OperationCanceledException("YouTube media probe timed out or was canceled.", cancellationToken);
            }
            catch (Exception) { throw Failure("YouTube media probe failed."); }
            finally
            {
                if (acquired) RequestSlots.Release();
                Interlocked.Decrement(ref _pendingRequests);
            }
        }

        // Range responses must describe the exact requested bytes of the exact
        // object, not an encoded or silently decompressed representation.
        private static async Task<byte[]> FetchRangeAsync(Uri uri, IReadOnlyDictionary<string, string> headers,
            long offset, int count, long totalLength, CancellationToken cancellationToken, HttpClient client)
        {
            if (uri == null || offset < 0 || count <= 0 || count > MaximumPayloadBytes
                || totalLength <= 0 || offset >= totalLength || count > totalLength - offset)
                throw Failure("Invalid YouTube media range.");
            var current = ValidateMediaUri(uri.OriginalString);
            ValidateHeaders(headers);
            if (Interlocked.Increment(ref _pendingRequests) > 16)
            {
                Interlocked.Decrement(ref _pendingRequests);
                throw new RangeBusyException();
            }
            bool acquired = false;
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(TimeSpan.FromSeconds(20));
            try
            {
                await RequestSlots.WaitAsync(deadline.Token).ConfigureAwait(false);
                acquired = true;
                long end = offset + count - 1;
                for (int redirect = 0; redirect <= 3; redirect++)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, current);
                    foreach (var header in headers)
                        if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                            throw Failure("Invalid YouTube headers.");
                    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity"));
                    request.Headers.Range = new RangeHeaderValue(offset, end);
                    using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                        deadline.Token).ConfigureAwait(false);
                    if (IsRedirect(response.StatusCode))
                    {
                        if (redirect == 3 || response.Headers.Location == null)
                            throw Failure("YouTube redirect limit exceeded.");
                        current = ValidateMediaUri(new Uri(current, response.Headers.Location).AbsoluteUri);
                        continue;
                    }
                    if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
                        throw new RangeForbiddenException(response.StatusCode == HttpStatusCode.TooManyRequests);
                    var range = response.Content.Headers.ContentRange;
                    if (response.StatusCode != HttpStatusCode.PartialContent || range == null
                        || range.Unit != "bytes" || range.From != offset || range.To != end || range.Length != totalLength
                        || response.Content.Headers.ContentLength != count
                        || response.Content.Headers.ContentEncoding.Any(value => !value.Equals("identity", StringComparison.OrdinalIgnoreCase)))
                        throw Failure("YouTube returned an invalid media range.");
                    using var body = await response.Content.ReadAsStreamAsync(deadline.Token).ConfigureAwait(false);
                    var bytes = new byte[count];
                    int received = 0;
                    while (received < count)
                    {
                        int read = await body.ReadAsync(bytes.AsMemory(received, count - received), deadline.Token).ConfigureAwait(false);
                        if (read == 0) throw Failure("YouTube returned an incomplete media range.");
                        received += read;
                    }
                    if (await body.ReadAsync(new byte[1], deadline.Token).ConfigureAwait(false) != 0)
                        throw Failure("YouTube returned an oversized media range.");
                    return bytes;
                }
                throw Failure("YouTube redirect limit exceeded.");
            }
            catch (RangeForbiddenException) { throw; }
            catch (RangeBusyException) { throw; }
            catch (OperationCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException("YouTube media range canceled.", cancellationToken);
                throw Failure("YouTube media range timed out.");
            }
            catch (Exception) { throw Failure("YouTube media range request failed."); }
            finally
            {
                if (acquired) RequestSlots.Release();
                Interlocked.Decrement(ref _pendingRequests);
            }
        }

        internal sealed class RangeForbiddenException : InvalidOperationException
        {
            internal RangeForbiddenException(bool rateLimited = false) : base("YouTube media range was denied.") => RateLimited = rateLimited;
            internal bool RateLimited { get; }
        }

        internal sealed class RangeBusyException : InvalidOperationException
        {
            internal RangeBusyException() : base("YouTube media range capacity is unavailable.") { }
        }

        private static Task<byte[]> FetchAsync(
            Uri uri, IReadOnlyDictionary<string, string> headers, int maximumBytes,
            CancellationToken cancellationToken, HttpClient client) =>
            FetchPayloadAsync(uri, headers, maximumBytes, cancellationToken, client, false);

        private static Task<byte[]> FetchMediaPlaylistAsync(
            Uri uri, IReadOnlyDictionary<string, string> headers, int maximumBytes,
            CancellationToken cancellationToken, HttpClient client) =>
            FetchPayloadAsync(uri, headers, maximumBytes, cancellationToken, client, true);

        private static async Task<byte[]> FetchPayloadAsync(
            Uri uri, IReadOnlyDictionary<string, string> headers, int maximumBytes,
            CancellationToken cancellationToken, HttpClient client, bool mediaPlaylist)
        {
            if (maximumBytes <= 0 || maximumBytes > (mediaPlaylist ? MaximumMediaPlaylistBytes : MaximumPayloadBytes))
                throw Failure("Invalid YouTube response limit.");
            if (uri == null) throw Failure("Invalid YouTube URL.");
            bool isCaption = !mediaPlaylist && IsYouTubeHost(uri.Host);
            Func<string, Uri> validate = isCaption ? ValidateCaptionUri : ValidateMediaUri;
            var current = validate(uri.OriginalString);
            ValidateHeaders(headers);
            if (Interlocked.Increment(ref _pendingRequests) > 16)
            {
                Interlocked.Decrement(ref _pendingRequests);
                throw Failure("YouTube request queue is full.");
            }

            bool acquired = false;
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(TimeSpan.FromSeconds(20));
            try
            {
                await RequestSlots.WaitAsync(deadline.Token).ConfigureAwait(false);
                acquired = true;
                for (int redirect = 0; redirect <= 3; redirect++)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, current);
                    foreach (var header in headers)
                        if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                            throw Failure("Invalid YouTube headers.");
                    using var response = await client.SendAsync(
                        request, HttpCompletionOption.ResponseHeadersRead, deadline.Token).ConfigureAwait(false);
                    if (IsRedirect(response.StatusCode))
                    {
                        if (redirect == 3 || response.Headers.Location == null)
                            throw Failure("YouTube redirect limit exceeded.");
                        current = validate(new Uri(current, response.Headers.Location).AbsoluteUri);
                        continue;
                    }
                    if (!response.IsSuccessStatusCode)
                        throw Failure("YouTube upstream response was unsuccessful.");
                    if (response.Content.Headers.ContentLength > maximumBytes)
                        throw Failure("YouTube response exceeded its size limit.");
                    using var body = await response.Content.ReadAsStreamAsync(deadline.Token).ConfigureAwait(false);
                    using var output = new MemoryStream(Math.Min(maximumBytes, 64 * 1024));
                    var buffer = new byte[16 * 1024];
                    while (true)
                    {
                        int read = await body.ReadAsync(buffer.AsMemory(0,
                            Math.Min(buffer.Length, maximumBytes - (int)output.Length + 1)), deadline.Token)
                            .ConfigureAwait(false);
                        if (read == 0)
                        {
                            var bytes = output.ToArray();
                            if (mediaPlaylist && !HasPlaylistHeader(bytes))
                                throw Failure("YouTube response is not a media playlist.");
                            return bytes;
                        }
                        if (output.Length + read > maximumBytes)
                            throw Failure("YouTube response exceeded its size limit.");
                        output.Write(buffer, 0, read);
                    }
                }
                throw Failure("YouTube redirect limit exceeded.");
            }
            catch (OperationCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException("YouTube request canceled.", cancellationToken);
                throw Failure("YouTube upstream request timed out.");
            }
            catch (Exception)
            {
                // HttpClient exceptions can contain the entire signed request URI.
                throw Failure("YouTube upstream request failed.");
            }
            finally
            {
                if (acquired) RequestSlots.Release();
                Interlocked.Decrement(ref _pendingRequests);
            }
        }

        private static bool HasPlaylistHeader(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
                bytes = bytes.Slice(3);
            return bytes.Length >= 7 && bytes[0] == '#' && bytes[1] == 'E' && bytes[2] == 'X'
                && bytes[3] == 'T' && bytes[4] == 'M' && bytes[5] == '3' && bytes[6] == 'U'
                && (bytes.Length == 7 || bytes[7] == '\r' || bytes[7] == '\n');
        }

        private static Uri ValidateHttpsUri(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 16384
                || value.Any(c => char.IsWhiteSpace(c) || char.IsControl(c))
                || value.Contains('\\')
                || !Uri.TryCreate(value, UriKind.Absolute, out var uri)
                || uri.Scheme != Uri.UriSchemeHttps || uri.Port != 443
                || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Fragment)
                || uri.HostNameType != UriHostNameType.Dns || uri.Host.EndsWith(".", StringComparison.Ordinal)
                || uri.Host.Any(c => c > 127) || IPAddress.TryParse(uri.Host, out _))
                throw Failure("Invalid YouTube HTTPS URL.");
            return uri;
        }

        private static void ValidateHeaders(IReadOnlyDictionary<string, string> headers)
        {
            if (headers == null || headers.Count > 16) throw Failure("Invalid YouTube headers.");
            int total = 0;
            foreach (var header in headers)
            {
                if (!AllowedHeaders.Contains(header.Key) || header.Value == null
                    || header.Value.Length > 4096
                    || header.Value.Any(c => c < 32 || c > 126))
                    throw Failure("Invalid YouTube headers.");
                total += header.Key.Length + header.Value.Length;
                if (total > 8192) throw Failure("Invalid YouTube headers.");
                if (header.Key.Equals("Origin", StringComparison.OrdinalIgnoreCase)
                    || header.Key.Equals("Referer", StringComparison.OrdinalIgnoreCase))
                {
                    var reference = ValidateHttpsUri(header.Value);
                    if (!IsYouTubeHost(reference.Host)) throw Failure("Invalid YouTube reference header.");
                    if (header.Key.Equals("Origin", StringComparison.OrdinalIgnoreCase)
                        && (reference.AbsolutePath != "/" || reference.Query.Length != 0))
                        throw Failure("Invalid YouTube origin header.");
                }
                if (header.Key.Equals("Sec-Fetch-Mode", StringComparison.OrdinalIgnoreCase)
                    && header.Value != "cors" && header.Value != "no-cors"
                    && header.Value != "same-origin" && header.Value != "navigate")
                    throw Failure("Invalid YouTube fetch mode.");
            }
        }

        private static bool IsMediaHost(string host) =>
            host.Equals("googlevideo.com", StringComparison.OrdinalIgnoreCase)
            || host.EndsWith(".googlevideo.com", StringComparison.OrdinalIgnoreCase);

        private static bool IsYouTubeHost(string host) =>
            host.Equals("www.youtube.com", StringComparison.OrdinalIgnoreCase)
            || host.Equals("youtube.com", StringComparison.OrdinalIgnoreCase);

        private static bool IsRedirect(HttpStatusCode code) =>
            code is HttpStatusCode.MovedPermanently or HttpStatusCode.Found or HttpStatusCode.SeeOther
                or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect;

        private static HttpClient CreateClient(bool allowCompression = true)
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                UseProxy = false,
                UseCookies = false,
                AutomaticDecompression = allowCompression ? DecompressionMethods.GZip | DecompressionMethods.Deflate : DecompressionMethods.None,
                ConnectTimeout = TimeSpan.FromSeconds(8),
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                MaxConnectionsPerServer = 4,
                MaxResponseHeadersLength = 32,
                ConnectCallback = ConnectPublicAddressAsync
            };
            return new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };
        }

        // Managed metadata resolution owns its own strict endpoint, body and
        // denial policy, while reusing the same public-address-pinned sockets.
        // Never change the existing exact media-range transport contract here.
        internal static HttpClient CreateResolverClient() => CreateClient(allowCompression: false);

        private static async ValueTask<Stream> ConnectPublicAddressAsync(
            SocketsHttpConnectionContext context, CancellationToken cancellationToken)
        {
            var endpoint = context.DnsEndPoint;
            if (endpoint.Port != 443 || (!IsMediaHost(endpoint.Host) && !IsYouTubeHost(endpoint.Host)))
                throw Failure("YouTube connection target is not allowed.");
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(endpoint.Host)
                    .WaitAsync(cancellationToken).ConfigureAwait(false);
                ValidateAddresses(addresses);
                foreach (var address in addresses.OrderBy(a => a.AddressFamily == AddressFamily.InterNetwork ? 0 : 1))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                    { NoDelay = true };
                    try
                    {
                        using var attempt = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        attempt.CancelAfter(TimeSpan.FromSeconds(2));
                        await socket.ConnectAsync(new IPEndPoint(address, endpoint.Port), attempt.Token).ConfigureAwait(false);
                        // Connect to exactly the validated address. HttpClient still
                        // performs TLS using the original hostname and normal checks.
                        return new NetworkStream(socket, ownsSocket: true);
                    }
                    catch (Exception)
                    {
                        socket.Dispose();
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                throw Failure("YouTube connection failed.");
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("YouTube connection canceled.", cancellationToken);
            }
            catch (Exception)
            {
                throw Failure("YouTube connection failed.");
            }
        }

        internal static bool IsPublicAddress(IPAddress address)
        {
            if (IPAddress.IsLoopback(address) || address.IsIPv4MappedToIPv6) return false;
            var bytes = address.GetAddressBytes();
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                return bytes[0] != 0 && bytes[0] != 10 && bytes[0] != 127 && bytes[0] < 224
                    && !(bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127)
                    && !(bytes[0] == 169 && bytes[1] == 254)
                    && !(bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                    && !(bytes[0] == 192 && (bytes[1] == 0 || bytes[1] == 168
                        || (bytes[1] == 88 && bytes[2] == 99)))
                    && !(bytes[0] == 198 && (bytes[1] is 18 or 19
                        || (bytes[1] == 51 && bytes[2] == 100)))
                    && !(bytes[0] == 203 && bytes[1] == 0 && bytes[2] == 113);
            }
            if (address.AddressFamily != AddressFamily.InterNetworkV6 || address.ScopeId != 0
                || (bytes[0] & 0xe0) != 0x20) return false;
            // Exclude documentation, transition and special-purpose allocations.
            if (bytes[0] == 0x20 && bytes[1] == 0x02) return false;
            if (bytes[0] == 0x20 && bytes[1] == 0x01
                && ((bytes[2] == 0 && (bytes[3] < 0x30))
                    || (bytes[2] == 0x0d && bytes[3] == 0xb8))) return false;
            if (bytes[0] == 0x3f && bytes[1] == 0xff) return false;
            return true;
        }

        private static void ValidateAddresses(IPAddress[] addresses)
        {
            if (addresses.Length == 0 || addresses.Length > 32 || addresses.Any(a => !IsPublicAddress(a)))
                throw Failure("YouTube DNS returned a non-public address.");
        }

        private static InvalidOperationException Failure(string message) => new(message);
    }
}
