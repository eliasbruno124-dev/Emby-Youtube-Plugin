using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using JsonExtensions.Reading;
using PowerKit.Extensions;
using YoutubeExplode.Utils;

namespace YoutubeExplode.Bridge;

internal partial class PlayerResponse(JsonElement content)
{
    internal JsonElement Snapshot => content;


    private JsonElement? Playability => content.GetPropertyOrNull("playabilityStatus");

    private string? PlayabilityStatus =>
        Playability?.GetPropertyOrNull("status")?.GetStringOrNull();

    public string? PlayabilityError => Playability?.GetPropertyOrNull("reason")?.GetStringOrNull();

    public bool IsAvailable =>
        !string.Equals(PlayabilityStatus, "error", StringComparison.OrdinalIgnoreCase)
        && Details is not null;

    public bool IsPlayable =>
        string.Equals(PlayabilityStatus, "ok", StringComparison.OrdinalIgnoreCase);

    private JsonElement? Details => content.GetPropertyOrNull("videoDetails");

    public string? Title => Details?.GetPropertyOrNull("title")?.GetStringOrNull();

    public string? ChannelId => Details?.GetPropertyOrNull("channelId")?.GetStringOrNull();

    public string? Author => Details?.GetPropertyOrNull("author")?.GetStringOrNull();

    public DateTimeOffset? UploadDate =>
        content
            .GetPropertyOrNull("microformat")
            ?.GetPropertyOrNull("playerMicroformatRenderer")
            ?.GetPropertyOrNull("uploadDate")
            ?.GetDateTimeOffset();

    public TimeSpan? Duration =>
        Details
            ?.GetPropertyOrNull("lengthSeconds")
            ?.GetStringOrNull()
            ?.Pipe(s => ManagedNumeric.ParseDoubleOrNull(s, CultureInfo.InvariantCulture))
            ?.Pipe(TimeSpan.FromSeconds);

    public IReadOnlyList<ThumbnailData> Thumbnails =>
        Details
            ?.GetPropertyOrNull("thumbnail")
            ?.GetPropertyOrNull("thumbnails")
            ?.EnumerateArrayOrNull()
            ?.Select(j => new ThumbnailData(j))
            .ToArray()
        ?? [];

    public IReadOnlyList<string> Keywords =>
        Details
            ?.GetPropertyOrNull("keywords")
            ?.EnumerateArrayOrNull()
            ?.Select(j => j.GetStringOrNull())
            .WhereNotNull()
            .ToArray()
        ?? [];

    public string? Description => Details?.GetPropertyOrNull("shortDescription")?.GetStringOrNull();

    public long? ViewCount =>
        Details
            ?.GetPropertyOrNull("viewCount")
            ?.GetStringOrNull()
            ?.Pipe(s => ManagedNumeric.ParseLongOrNull(s, CultureInfo.InvariantCulture));

    public string? PreviewVideoId =>
        Playability
            ?.GetPropertyOrNull("errorScreen")
            ?.GetPropertyOrNull("playerLegacyDesktopYpcTrailerRenderer")
            ?.GetPropertyOrNull("trailerVideoId")
            ?.GetStringOrNull()
        ?? Playability
            ?.GetPropertyOrNull("errorScreen")
            ?.GetPropertyOrNull("ypcTrailerRenderer")
            ?.GetPropertyOrNull("playerVars")
            ?.GetStringOrNull()
            ?.Pipe(UrlEx.GetQueryParameters)
            .GetValueOrDefault("video_id")
        ?? Playability
            ?.GetPropertyOrNull("errorScreen")
            ?.GetPropertyOrNull("ypcTrailerRenderer")
            ?.GetPropertyOrNull("playerResponse")
            ?.GetStringOrNull()
            ?
            // YouTube uses weird base64-like encoding here that I don't know how to deal with.
            // It's supposed to have JSON inside, but if extracted as is, it contains garbage.
            // Luckily, some of the text gets decoded correctly, which is enough for us to
            // extract the preview video ID using regex.
            .Replace('-', '+')
            .Replace('_', '/')
            .Pipe(Convert.FromBase64String)
            .Pipe(Encoding.UTF8.GetString)
            .Pipe(s => Regex.Match(s, @"video_id=(.{11})").Groups[1].Value)
            .NullIfWhiteSpace();

    private JsonElement? StreamingData => content.GetPropertyOrNull("streamingData");

    public string? DashManifestUrl =>
        StreamingData?.GetPropertyOrNull("dashManifestUrl")?.GetStringOrNull();

    public string? HlsManifestUrl =>
        StreamingData?.GetPropertyOrNull("hlsManifestUrl")?.GetStringOrNull();

    public IReadOnlyList<IStreamData> Streams
    {
        get
        {
            var result = new List<IStreamData>();

            var muxedStreams = StreamingData
                ?.GetPropertyOrNull("formats")
                ?.EnumerateArrayOrNull()
                ?.Select(j => new StreamData(j));

            if (muxedStreams is not null)
                result.AddRange(muxedStreams);

            var adaptiveStreams = StreamingData
                ?.GetPropertyOrNull("adaptiveFormats")
                ?.EnumerateArrayOrNull()
                ?.Select(j => new StreamData(j));

            if (adaptiveStreams is not null)
                result.AddRange(adaptiveStreams);

            return result;
        }
    }

    public IReadOnlyList<ClosedCaptionTrackData> ClosedCaptionTracks =>
        content
            .GetPropertyOrNull("captions")
            ?.GetPropertyOrNull("playerCaptionsTracklistRenderer")
            ?.GetPropertyOrNull("captionTracks")
            ?.EnumerateArrayOrNull()
            ?.Select(j => new ClosedCaptionTrackData(j))
            .ToArray()
        ?? [];
}

internal partial class PlayerResponse
{
    public class ClosedCaptionTrackData(JsonElement content)
    {

        public string? Url => content.GetPropertyOrNull("baseUrl")?.GetStringOrNull();

        public string? LanguageCode => content.GetPropertyOrNull("languageCode")?.GetStringOrNull();

        public string? LanguageName =>
            content.GetPropertyOrNull("name")?.GetPropertyOrNull("simpleText")?.GetStringOrNull()
            ?? content
                .GetPropertyOrNull("name")
                ?.GetPropertyOrNull("runs")
                ?.EnumerateArrayOrNull()
                ?.Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
                .WhereNotNull()
                .Pipe(string.Concat);

        public bool IsAutoGenerated =>
            content
                .GetPropertyOrNull("vssId")
                ?.GetStringOrNull()
                ?.StartsWith("a.", StringComparison.OrdinalIgnoreCase)
            ?? false;
    }
}

internal partial class PlayerResponse
{
    public class StreamData(JsonElement content) : IStreamData
    {

        public int? Itag => content.GetPropertyOrNull("itag")?.GetInt32OrNull();

        private IReadOnlyDictionary<string, string>? CipherData =>
            content.GetPropertyOrNull("cipher")?.GetStringOrNull()?.Pipe(UrlEx.GetQueryParameters)
            ?? content
                .GetPropertyOrNull("signatureCipher")
                ?.GetStringOrNull()
                ?.Pipe(UrlEx.GetQueryParameters);

        public string? Url =>
            content.GetPropertyOrNull("url")?.GetStringOrNull()
            ?? CipherData?.GetValueOrDefault("url");

        public string? Signature => CipherData?.GetValueOrDefault("s");

        public string? SignatureParameter => CipherData?.GetValueOrDefault("sp");

        public long? ContentLength =>
            content
                .GetPropertyOrNull("contentLength")
                ?.GetStringOrNull()
                ?.Pipe(s => ManagedNumeric.ParseLongOrNull(s, CultureInfo.InvariantCulture))
            ?? Url?.Pipe(s => UrlEx.TryGetQueryParameterValue(s, "clen"))
                ?.NullIfWhiteSpace()
                ?.Pipe(s => ManagedNumeric.ParseLongOrNull(s, CultureInfo.InvariantCulture));

        public long? Bitrate => content.GetPropertyOrNull("bitrate")?.GetInt64OrNull();

        private string? MimeType => content.GetPropertyOrNull("mimeType")?.GetStringOrNull();

        public string? Container => MimeType?.SubstringUntil(";").SubstringAfter("/");

        private bool IsAudioOnly =>
            MimeType?.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ?? false;

        public string? Codecs => MimeType?.SubstringAfter("codecs=\"").SubstringUntil("\"");

        public string? AudioCodec =>
            IsAudioOnly ? Codecs : Codecs?.SubstringAfter(", ").NullIfWhiteSpace();

        public string? AudioLanguageCode =>
            content
                .GetPropertyOrNull("audioTrack")
                ?.GetPropertyOrNull("id")
                ?.GetStringOrNull()
                ?.SubstringUntil(".");

        public string? AudioLanguageName =>
            content
                .GetPropertyOrNull("audioTrack")
                ?.GetPropertyOrNull("displayName")
                ?.GetStringOrNull();

        public bool? IsAudioLanguageDefault =>
            content
                .GetPropertyOrNull("audioTrack")
                ?.GetPropertyOrNull("audioIsDefault")
                ?.GetBooleanOrNull();

        public string? VideoCodec
        {
            get
            {
                var codec = IsAudioOnly ? null : Codecs?.SubstringUntil(", ").NullIfWhiteSpace();

                // "unknown" value indicates av01 codec
                if (string.Equals(codec, "unknown", StringComparison.OrdinalIgnoreCase))
                    return "av01.0.05M.08";

                return codec;
            }
        }

        public string? VideoQualityLabel =>
            content.GetPropertyOrNull("qualityLabel")?.GetStringOrNull();

        public int? VideoWidth => content.GetPropertyOrNull("width")?.GetInt32OrNull();

        public int? VideoHeight => content.GetPropertyOrNull("height")?.GetInt32OrNull();

        public bool IsVideoUpscaled =>
            // xtags is a base64-encoded protobuf map<string, string>.
            // Streams upscaled with YouTube's Super Resolution feature carry the entry {"sr": "1"}.
            content
                .GetPropertyOrNull("xtags")
                ?.GetStringOrNull()
                ?.NullIfWhiteSpace()
                ?.Pipe(Protobuf.TryDeserializeMap)
                ?.GetValueOrDefault("sr") == "1";

        public int? VideoFramerate => content.GetPropertyOrNull("fps")?.GetInt32OrNull();
    }
}

internal partial class PlayerResponse
{
    public static PlayerResponse Parse(string raw) => new(Json.Parse(raw));
}
