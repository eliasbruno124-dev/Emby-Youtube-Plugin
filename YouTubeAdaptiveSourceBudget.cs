using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    internal sealed record YouTubeAdaptiveSelection(JsonElement Video, JsonElement? Audio);

    // A bounded startup observation of the server-to-CDN route, not continuous
    // ABR, a measurement of the client connection or a stall-free guarantee.
    // Only the exact prefix of one real HTTPS representation is sampled.
    internal static class YouTubeAdaptiveSourceBudget
    {
        internal const int MaximumProbeBytes = 512 * 1024;
        internal const int MinimumProbeBytes = 64 * 1024;
        internal const int ProbeDeadlineMilliseconds = 1800;
        internal const int MaximumCacheEntries = 64;
        internal const int MaximumPendingProbes = 16;
        internal const int MaximumParallelProbes = 2;
        internal const int FallbackMaximumHeight = 480;
        internal const double ThroughputHeadroom = 0.60;
        private const long MaximumFileBytes = 1L << 40;
        private static readonly object Gate = new();
        private static readonly Dictionary<string, Task<Observation>> Cache = new(StringComparer.Ordinal);
        private static readonly SemaphoreSlim ProbeSlots = new(MaximumParallelProbes, MaximumParallelProbes);
        private static int _pendingProbes;
        private static Func<long> _clockMilliseconds = () => Environment.TickCount64;
        private static Func<Uri, IReadOnlyDictionary<string, string>, long, int, long, CancellationToken, Task<byte[]>> _fetchRange =
            YouTubeDirectHttp.GetRangeBytesAsync;

        private sealed record Input(JsonElement Format, Uri Uri, long Length, int Height, string VideoCodec,
            string AudioCodec, double? Bitrate, Dictionary<string, string> Headers);
        private sealed record Choice(Input Video, Input? Audio, double? Bitrate);
        private sealed record ProbeInput(Uri Uri, long Length, IReadOnlyDictionary<string, string> Headers);
        private sealed record Observation(double? Bitrate, bool Denied, long ExpiresAt);

        internal static async Task<YouTubeAdaptiveSelection> SelectAsync(YouTubeResolvedVideo resolved,
            IReadOnlyDictionary<string, string> headers, int maximumHeight, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (resolved.ExpiresUtc <= DateTime.UtcNow.AddSeconds(5)
                || resolved.VideoId is not { Length: 11 }
                || resolved.VideoId.Any(c => !YouTubeDirectPlayback.IsVideoIdCharacter(c))
                || Text(resolved.Metadata, "id") != resolved.VideoId
                || !resolved.Metadata.TryGetProperty("formats", out var formats)
                || formats.ValueKind != JsonValueKind.Array || formats.GetArrayLength() is 0 or > 1000)
                throw Failure();
            maximumHeight = YouTubeDirectQualityPolicy.ClampHeight(maximumHeight);
            var commonHeaders = YouTubeDirectHttp.CopyHeaders(headers);
            var duration = Number(resolved.Metadata, "duration");
            if (duration is not (> 0 and <= 315360000)) duration = null;
            var unique = formats.EnumerateArray().GroupBy(f => Text(f, "format_id"), StringComparer.Ordinal)
                .Where(group => group.Count() == 1).Select(group => group.First());
            var inputs = unique.Select(format => ReadInput(format, commonHeaders, duration, resolved.ExpiresUtc))
                .Where(input => input != null).Cast<Input>().ToArray();
            var audio = inputs.Where(input => input.VideoCodec.Length == 0 && input.AudioCodec == "aac")
                .OrderByDescending(input => Number(input.Format, "language_preference") ?? 0)
                .ThenByDescending(input => input.Bitrate ?? 0)
                .ThenBy(input => Text(input.Format, "format_id"), StringComparer.Ordinal).FirstOrDefault();
            var choices = inputs.Where(input => input.Height > 0 && input.Height <= maximumHeight
                    && input.VideoCodec.Length > 0 && (input.AudioCodec == "aac" || input.AudioCodec.Length == 0 && audio != null))
                .Select(input => new Choice(input, input.AudioCodec.Length == 0 ? audio : null,
                    input.AudioCodec.Length == 0 ? input.Bitrate + audio!.Bitrate : input.Bitrate)).ToArray();
            if (choices.Length == 0) throw Failure();
            // Prefer broadly native AVC/AAC over CPU-expensive AV1 even when an
            // AV1 representation advertises a higher resolution or frame rate.
            int codec = choices.Min(choice => CodecPreference(choice.Video.VideoCodec));
            var eligible = choices.Where(choice => CodecPreference(choice.Video.VideoCodec) == codec)
                .OrderByDescending(choice => choice.Video.Height)
                .ThenByDescending(choice => Number(choice.Video.Format, "fps") is > 0 and <= YouTubeDirectQualityPolicy.MaximumFrameRate)
                .ThenByDescending(choice => Number(choice.Video.Format, "fps") ?? 0)
                .ThenBy(choice => choice.Audio != null)
                .ThenBy(choice => choice.Bitrate ?? double.MaxValue)
                .ThenBy(choice => Text(choice.Video.Format, "format_id"), StringComparer.Ordinal).ToArray();
            var lowest = eligible.OrderBy(choice => choice.Video.Height)
                .ThenBy(choice => choice.Bitrate ?? double.MaxValue).First();
            var fallback = eligible.FirstOrDefault(choice => choice.Video.Height <= Math.Min(maximumHeight, FallbackMaximumHeight)) ?? lowest;
            // Unknown representation costs cannot justify a high source. Exact
            // length/duration is a real average, never a promised peak bitrate.
            var preferred = eligible[0];
            var observation = await GetObservation(preferred.Video).WaitAsync(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (observation.Denied) throw Failure();
            var selected = observation.Bitrate is { } budget
                ? eligible.FirstOrDefault(choice => choice.Bitrate is > 0
                        && choice.Bitrate <= Math.Min(budget, YouTubeDirectQualityPolicy.MaximumStreamingBitrate))
                    ?? eligible.Where(choice => choice.Bitrate is > 0).OrderBy(choice => choice.Bitrate)
                        .ThenBy(choice => choice.Video.Height).FirstOrDefault() ?? fallback
                : fallback;
            // Do not register an object that expired while its shared sample was
            // queued, even if the route's short observation remains useful.
            if (resolved.ExpiresUtc <= DateTime.UtcNow.AddSeconds(5)
                || ReadInput(selected.Video.Format, commonHeaders, duration, resolved.ExpiresUtc) == null
                || selected.Audio != null && ReadInput(selected.Audio.Format, commonHeaders, duration, resolved.ExpiresUtc) == null)
                throw Failure();
            return new YouTubeAdaptiveSelection(selected.Video.Format, selected.Audio?.Format);
        }

        private static Input? ReadInput(JsonElement format, IReadOnlyDictionary<string, string> headers,
            double? duration, DateTime expiresUtc)
        {
            if (format.ValueKind != JsonValueKind.Object || Text(format, "protocol") != "https") return null;
            string id = Text(format, "format_id"), extension = Text(format, "ext");
            if (id.Length is 0 or > 80 || id.Any(c => !(c is >= 'A' and <= 'Z' or >= 'a' and <= 'z'
                    or >= '0' and <= '9' or '_' or '.' or '-')) || extension is not ("mp4" or "m4a")
                || !format.TryGetProperty("filesize", out var size) || size.ValueKind != JsonValueKind.Number
                || !size.TryGetInt64(out long length) || length <= 0 || length > MaximumFileBytes) return null;
            string video = Text(format, "vcodec"), audio = Text(format, "acodec");
            string videoCodec = video == "none" ? "" : YouTubeDirectManifest.VideoCodec(video) ?? "?";
            string audioCodec = audio == "none" ? "" : YouTubeDirectManifest.AudioCodec(audio) ?? "?";
            if (videoCodec == "?" || audioCodec == "?" || videoCodec.Length == 0 && audioCodec.Length == 0) return null;
            int? height = Integer(format, "height"), width = Integer(format, "width");
            if (videoCodec.Length > 0 && (height is not (> 0 and <= 16384) || width is not (> 0 and <= 16384))) return null;
            foreach (string field in new[] { "width", "height", "audio_channels", "asr" })
                if (format.TryGetProperty(field, out var value) && value.ValueKind != JsonValueKind.Null
                    && (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out var _))) return null;
            if (Text(format, "language").Length > 80 || video.Length > 128 || audio.Length > 128) return null;
            var uri = YouTubeDirectHttp.ValidateMediaUri(Text(format, "url"));
            long? declared = null, expiry = null;
            foreach (string part in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var pieces = part.Split('=', 2);
                string name = Uri.UnescapeDataString(pieces[0]);
                if (name is not ("clen" or "expire")) continue;
                if (pieces[0] != name || pieces.Length != 2 || pieces[1].Length is 0 or > 19
                    || pieces[1].Any(c => c is < '0' or > '9')
                    || !long.TryParse(pieces[1], NumberStyles.None, CultureInfo.InvariantCulture, out long parsed)) return null;
                if (name == "clen")
                {
                    if (declared.HasValue || pieces[1][0] == '0') return null;
                    declared = parsed;
                }
                else
                {
                    if (expiry.HasValue) return null;
                    expiry = parsed;
                }
            }
            if (declared != length) return null;
            try { if (expiry.HasValue) expiresUtc = DateTimeOffset.FromUnixTimeSeconds(expiry.Value).UtcDateTime.AddSeconds(-120); }
            catch (ArgumentOutOfRangeException) { return null; }
            if (expiresUtc <= DateTime.UtcNow.AddSeconds(5)) return null;
            var safeHeaders = YouTubeDirectHttp.CopyHeaders(headers);
            if (format.TryGetProperty("http_headers", out var local))
                foreach (var header in YouTubeDirectHttp.ReadHeaders(local)) safeHeaders[header.Key] = header.Value;
            safeHeaders = YouTubeDirectHttp.CopyHeaders(safeHeaders);
            // Use the larger reported/length-derived rate. Missing or malformed
            // optional bitrate never turns into unlimited available bandwidth.
            double? bitrate = Number(format, "tbr");
            bitrate = bitrate is > 0 and < 2147483 ? bitrate * 1000 : null;
            if (duration.HasValue) bitrate = Math.Max(bitrate ?? 0, length * 8.0 / duration.Value);
            return new Input(format, uri, length, height ?? 0, videoCodec, audioCodec, bitrate, safeHeaders);
        }

        private static Task<Observation> GetObservation(Input input)
        {
            string route = input.Uri.Host.ToLowerInvariant() + "/"
                + string.Join('/', input.Uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).Take(3));
            string identity = route + "\n" + string.Join("\n", input.Headers.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .Select(pair => pair.Key.ToLowerInvariant() + ":" + pair.Value));
            var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
            lock (Gate)
            {
                long now = _clockMilliseconds();
                foreach (var entry in Cache.Where(entry => entry.Value.IsCompletedSuccessfully && entry.Value.Result.ExpiresAt <= now).ToArray())
                    Cache.Remove(entry.Key);
                if (Cache.TryGetValue(key, out var existing)) return existing;
                if (Cache.Count >= MaximumCacheEntries)
                {
                    var oldest = Cache.Where(entry => entry.Value.IsCompletedSuccessfully)
                        .OrderBy(entry => entry.Value.Result.ExpiresAt).FirstOrDefault();
                    if (oldest.Key == null) return Task.FromResult(Fallback());
                    Cache.Remove(oldest.Key);
                }
                Task<Observation> pending;
                if (_pendingProbes >= MaximumPendingProbes) pending = Task.FromResult(Fallback());
                else
                {
                    Interlocked.Increment(ref _pendingProbes);
                    // A completed cached task must never retain the JsonElement
                    // and, through it, the resolver's entire metadata document.
                    pending = MeasureAsync(new ProbeInput(input.Uri, input.Length, input.Headers));
                }
                Cache.Add(key, pending);
                return pending;
            }
        }

        private static async Task<Observation> MeasureAsync(ProbeInput input)
        {
            bool acquired = false;
            using var deadline = new CancellationTokenSource(ProbeDeadlineMilliseconds);
            try
            {
                await Task.Yield(); // Publish the shared task before completing.
                await ProbeSlots.WaitAsync(deadline.Token).ConfigureAwait(false);
                acquired = true;
                int count = (int)Math.Min(MaximumProbeBytes, input.Length);
                if (count < MinimumProbeBytes) return Fallback();
                var watch = Stopwatch.StartNew();
                var bytes = await _fetchRange(input.Uri, input.Headers, 0, count, input.Length, deadline.Token)
                    .WaitAsync(deadline.Token).ConfigureAwait(false);
                watch.Stop();
                if (bytes == null || bytes.Length != count || deadline.IsCancellationRequested) return Fallback();
                double rate = bytes.Length * 8.0 / Math.Max(0.1, watch.Elapsed.TotalSeconds) * ThroughputHeadroom;
                return new Observation(rate, false, _clockMilliseconds() + 90_000);
            }
            catch (YouTubeDirectHttp.RangeForbiddenException) { return new Observation(null, true, _clockMilliseconds() + 15_000); }
            catch (Exception error) when (error is not OutOfMemoryException) { return Fallback(); }
            finally
            {
                if (acquired) ProbeSlots.Release();
                Interlocked.Decrement(ref _pendingProbes);
            }
        }

        private static Observation Fallback() => new(null, false, _clockMilliseconds() + 15_000);
        private static int CodecPreference(string codec) => codec switch { "h264" => 0, "hevc" => 1, "vp9" => 2, _ => 3 };
        private static string Text(JsonElement value, string field) => value.ValueKind == JsonValueKind.Object
            && value.TryGetProperty(field, out var item) && item.ValueKind == JsonValueKind.String ? item.GetString() ?? "" : "";
        private static double? Number(JsonElement value, string field) => value.ValueKind == JsonValueKind.Object
            && value.TryGetProperty(field, out var item) && item.ValueKind == JsonValueKind.Number
            && item.TryGetDouble(out var number) && double.IsFinite(number) ? number : null;
        private static int? Integer(JsonElement value, string field) => value.ValueKind == JsonValueKind.Object
            && value.TryGetProperty(field, out var item) && item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var number) ? number : null;
        private static InvalidOperationException Failure() => new("No verified YouTube adaptive source is available.");
    }
}
