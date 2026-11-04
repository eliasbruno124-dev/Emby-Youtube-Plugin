using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using JsonExtensions.Reading;
using YoutubeExplode.Bridge;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Utils;

namespace YoutubeExplode.Videos;

internal class VideoController(HttpClient http)
{
    private string? _visitorData;

    protected HttpClient Http { get; } = http;

    private async ValueTask<string> ResolveVisitorDataAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(_visitorData))
            return _visitorData;

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://www.youtube.com/sw.js_data"
        );

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        request.Headers.Add(
            "User-Agent",
            "com.google.android.youtube/20.10.38 (Linux; U; ANDROID 11) gzip"
        );

        using var response = await Http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        // TODO: move this to a bridge wrapper
        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
        if (jsonString.StartsWith(")]}'"))
            jsonString = jsonString[4..];

        var json = Json.Parse(jsonString);

        // This is just an ordered (but unstructured) blob of data
        var value = json[0][2][0][0][13].GetStringOrNull();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new YoutubeExplodeException("Failed to resolve visitor data.");
        }

        return _visitorData = value;
    }

    private async ValueTask<PlayerResponse> GetPlayerResponseForVisionOsAsync(
        VideoId videoId,
        string visitorData,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://www.youtube.com/youtubei/v1/player"
        );

        request.Content = new StringContent(
            // lang=json
            $$"""
            {
              "videoId": {{Json.Encode(videoId)}},
              "contentCheckOk": false,
              "racyCheckOk": false,
              "context": {
                "client": {
                  "clientName": "VISIONOS",
                  "clientVersion": "1.02",
                  "deviceMake": "Apple",
                  "deviceModel": "RealityDevice17,1",
                  "osName": "visionOS",
                  "osVersion": "26.5.23O471",
                  "visitorData": {{Json.Encode(visitorData)}},
                  "hl": "en",
                  "gl": "US",
                  "utcOffsetMinutes": 0
                }
              }
            }
            """
        );

        request.Headers.Add(
            "User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 15_7_3) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/26.0 Safari/605.1.15"
        );

        using var response = await Http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var playerResponse = PlayerResponse.Parse(
            await response.Content.ReadAsStringAsync(cancellationToken)
        );

        if (!playerResponse.IsAvailable)
            throw new VideoUnavailableException($"Video '{videoId}' is not available.");

        if (!playerResponse.IsPlayable)
            throw new VideoUnplayableException($"Video '{videoId}' is unplayable.");

        return playerResponse;
    }

    public async ValueTask<PlayerResponse> GetPlayerResponseAsync(
        VideoId videoId,
        string? signatureTimestamp = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(signatureTimestamp))
            throw new YoutubeExplodeException("Restricted-client fallback disabled.");
        var visitorData = await ResolveVisitorDataAsync(cancellationToken);
        return await GetPlayerResponseForVisionOsAsync(videoId, visitorData, cancellationToken);
    }

    public async ValueTask<PlayerResponse> GetPlayerResponseAsync(
        VideoId videoId,
        CancellationToken cancellationToken = default
    ) => await GetPlayerResponseAsync(videoId, null, cancellationToken);
}
