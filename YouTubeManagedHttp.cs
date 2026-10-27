using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    internal enum YouTubeManagedFailureKind
    {
        INVALID_REQUEST, INVALID_RESPONSE, HTTP_DENIED, VERIFICATION_REQUIRED, VIDEO_RESTRICTED,
        HTTP_FAILED, SIZE_LIMIT, REQUEST_BUDGET, CANCELED, TRANSPORT_FAILED
    }

    internal sealed class YouTubeManagedResolutionException : InvalidOperationException
    {
        internal YouTubeManagedResolutionException(YouTubeManagedFailureKind kind, string stage = "SNAPSHOT")
            : base("YouTube managed resolution failed (" + kind + ").") { Kind = kind; Stage = stage; }
        internal YouTubeManagedFailureKind Kind { get; }
        internal string Stage { get; }
    }

    // Per-resolution HTTP boundary. It does not accept arbitrary URLs or expose
    // payloads, identities, cookies or transport exception chains to callers.
    internal sealed class YouTubeManagedHttp : HttpMessageHandler
    {
        internal const int MaximumRequests = 4;
        internal const int MaximumResponseBytes = 4 * 1024 * 1024;
        internal const int MaximumTotalBytes = 8 * 1024 * 1024;
        private readonly HttpClient _transport;
        private readonly string _videoId;
        private readonly int _maximumRequests;
        private readonly HashSet<string> _usedEndpoints = new(StringComparer.Ordinal);
        private bool _admitted;
        private bool _terminal;
        private int _requests;
        private long _responseBytes;
        private string _stage = "REQUEST";

        internal YouTubeManagedHttp(string videoId, HttpClient transport, int maximumRequests = MaximumRequests)
        { _videoId = videoId; _transport = transport; _maximumRequests = maximumRequests; }

        internal bool Terminal => _terminal;
        internal int RequestCount => _requests;
        internal long ResponseBytes => _responseBytes;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_terminal || YouTubeRequestBackoff.IsBlocked()) throw Failure(YouTubeManagedFailureKind.HTTP_DENIED);
            try
            {
                string endpoint = ValidateRequest(request);
                _stage = endpoint.ToUpperInvariant() + "_REQUEST";
                if (++_requests > _maximumRequests || !_usedEndpoints.Add(endpoint))
                    throw Failure(YouTubeManagedFailureKind.REQUEST_BUDGET);
                using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                deadline.CancelAfter(TimeSpan.FromSeconds(endpoint == "watch" ? 4 : 15));
                using var forwarded = new HttpRequestMessage(request.Method, request.RequestUri) { Version = HttpVersion.Version11 };
                foreach (var header in request.Headers)
                {
                    if (header.Key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase)
                        || header.Key.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                        forwarded.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                forwarded.Headers.TryAddWithoutValidation("Accept-Encoding", "identity");
                forwarded.Headers.TryAddWithoutValidation("Origin", "https://www.youtube.com");
                if (request.Content != null)
                {
                    byte[] content = await request.Content.ReadAsByteArrayAsync(deadline.Token).ConfigureAwait(false);
                    if (content.Length > 16 * 1024) throw Failure(YouTubeManagedFailureKind.SIZE_LIMIT);
                    ValidatePlayerRequest(content);
                    forwarded.Content = new ByteArrayContent(content);
                    forwarded.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }
                Task<HttpResponseMessage>? sent = null;
                _stage = endpoint.ToUpperInvariant() + "_SEND";
                if (!_admitted)
                {
                    await YouTubeRequestBackoff.StartAsync(() =>
                    {
                        sent = _transport.SendAsync(forwarded, HttpCompletionOption.ResponseHeadersRead, deadline.Token);
                        _admitted = true;
                        return true;
                    }, deadline.Token).ConfigureAwait(false);
                }
                else
                {
                    if (_terminal || YouTubeRequestBackoff.IsBlocked()) throw Failure(YouTubeManagedFailureKind.HTTP_DENIED);
                    sent = _transport.SendAsync(forwarded, HttpCompletionOption.ResponseHeadersRead, deadline.Token);
                }
                using var response = await sent!.ConfigureAwait(false);
                _stage = endpoint.ToUpperInvariant() + "_HEADERS";
                if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
                    Deny(verification: false);
                if (!response.IsSuccessStatusCode) throw Failure(YouTubeManagedFailureKind.HTTP_FAILED);
                // No redirect following, encoded responses or silently expanded bodies.
                if (response.Content.Headers.ContentEncoding.Any(value => !value.Equals("identity", StringComparison.OrdinalIgnoreCase)))
                    throw Failure(YouTubeManagedFailureKind.INVALID_RESPONSE);
                string? mediaType = response.Content.Headers.ContentType?.MediaType;
                if (endpoint == "watch" ? mediaType != "text/html" : mediaType is not ("application/json" or "text/javascript" or "application/javascript"))
                    throw Failure(YouTubeManagedFailureKind.INVALID_RESPONSE);
                int maximum = endpoint == "visitor" ? 1024 * 1024 : MaximumResponseBytes;
                if (response.Content.Headers.ContentLength is long announced && (announced < 0 || announced > maximum))
                    throw Failure(YouTubeManagedFailureKind.SIZE_LIMIT);
                using var body = new MemoryStream();
                _stage = endpoint.ToUpperInvariant() + "_BODY";
                using var input = await response.Content.ReadAsStreamAsync(deadline.Token).ConfigureAwait(false);
                var buffer = new byte[16 * 1024];
                while (true)
                {
                    int read = await input.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, maximum + 1 - (int)body.Length)), deadline.Token).ConfigureAwait(false);
                    if (read == 0) break;
                    body.Write(buffer, 0, read);
                    _responseBytes = checked(_responseBytes + read);
                    if (body.Length > maximum || _responseBytes > MaximumTotalBytes) throw Failure(YouTubeManagedFailureKind.SIZE_LIMIT);
                }
                if (body.Length == 0 || response.Content.Headers.ContentLength is long length && length != body.Length)
                    throw Failure(YouTubeManagedFailureKind.INVALID_RESPONSE);
                byte[] bytes = body.ToArray();
                if (endpoint == "player") { _stage = "PLAYER_VALIDATE"; ValidatePlayerResponse(bytes); }
                var bounded = new HttpResponseMessage(response.StatusCode)
                { RequestMessage = request, Content = new ByteArrayContent(bytes) };
                bounded.Content.Headers.ContentType = response.Content.Headers.ContentType;
                return bounded;
            }
            catch (YouTubeManagedResolutionException) { throw; }
            catch (OperationCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException("YouTube managed request canceled.", cancellationToken);
                throw Failure(YouTubeManagedFailureKind.CANCELED);
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            { throw Failure(YouTubeManagedFailureKind.TRANSPORT_FAILED); }
        }

        private string ValidateRequest(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            if (uri == null || uri.Scheme != "https" || uri.Port != 443 || uri.Host != "www.youtube.com"
                || uri.UserInfo.Length != 0 || uri.Fragment.Length != 0 || uri.OriginalString.Any(char.IsWhiteSpace)
                || uri.OriginalString.Contains('\\')) throw Failure(YouTubeManagedFailureKind.INVALID_REQUEST);
            if (request.Method == HttpMethod.Get && request.Content == null && uri.AbsolutePath == "/sw.js_data" && uri.Query.Length == 0)
                return "visitor";
            if (request.Method == HttpMethod.Post && request.Content != null && uri.AbsolutePath == "/youtubei/v1/player" && uri.Query.Length == 0)
                return "player";
            if (request.Method == HttpMethod.Get && request.Content == null && uri.AbsolutePath == "/watch" && uri.Query == "?v=" + _videoId)
                return "watch";
            throw Failure(YouTubeManagedFailureKind.INVALID_REQUEST);
        }

        private void ValidatePlayerRequest(byte[] content)
        {
            using var document = JsonDocument.Parse(content, new JsonDocumentOptions { MaxDepth = 16 });
            var value = document.RootElement;
            if (value.GetProperty("videoId").GetString() != _videoId
                || value.GetProperty("context").GetProperty("client").GetProperty("clientName").GetString() != "VISIONOS"
                || value.GetProperty("contentCheckOk").ValueKind != JsonValueKind.False
                || value.GetProperty("racyCheckOk").ValueKind != JsonValueKind.False)
                throw Failure(YouTubeManagedFailureKind.INVALID_REQUEST);
        }

        private void ValidatePlayerResponse(byte[] content)
        {
            using var document = JsonDocument.Parse(content, new JsonDocumentOptions { MaxDepth = 48 });
            var root = document.RootElement;
            if (!root.TryGetProperty("playabilityStatus", out var status) || !status.TryGetProperty("status", out var field)
                || field.ValueKind != JsonValueKind.String) throw Failure(YouTubeManagedFailureKind.INVALID_RESPONSE);
            if (field.GetString() != "OK")
            {
                // A removed, age/account/country-restricted video is terminal
                // for this flight, not evidence that unrelated videos are rate
                // limited. Only an explicit bot challenge engages shared cooldown.
                bool bot = status.TryGetProperty("reason", out var reason) && reason.ValueKind == JsonValueKind.String
                    && (reason.GetString()?.IndexOf("not a bot", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;
                Deny(verification: bot, globalCooldown: bot);
            }
            if (!root.TryGetProperty("videoDetails", out var video) || !video.TryGetProperty("videoId", out var id)
                || id.ValueKind != JsonValueKind.String || id.GetString() != _videoId)
                throw Failure(YouTubeManagedFailureKind.INVALID_RESPONSE);
        }

        private void Deny(bool verification, bool globalCooldown = true)
        {
            if (!_terminal) { _terminal = true; if (globalCooldown) YouTubeRequestBackoff.Record(verification); }
            throw Failure(!globalCooldown ? YouTubeManagedFailureKind.VIDEO_RESTRICTED
                : verification ? YouTubeManagedFailureKind.VERIFICATION_REQUIRED : YouTubeManagedFailureKind.HTTP_DENIED);
        }

        private YouTubeManagedResolutionException Failure(YouTubeManagedFailureKind kind) => new(kind, _stage);
        protected override void Dispose(bool disposing) { if (disposing) _transport.Dispose(); base.Dispose(disposing); }
    }
}
