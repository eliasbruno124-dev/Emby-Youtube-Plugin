using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel
    {
        public Task<IEnumerable<MediaSourceInfo>> GetChannelItemMediaInfo(string id, CancellationToken cancellationToken)
        {
            var live = id?.StartsWith(LivePrefix, StringComparison.Ordinal) == true;
            var videoId = live ? id!.Substring(LivePrefix.Length)
                : id?.StartsWith(ReelPrefix, StringComparison.Ordinal) == true ? id.Substring(ReelPrefix.Length) : id ?? string.Empty;
            if (videoId.Length != 11 || videoId.Any(c => !YouTubeDirectPlayback.IsVideoIdCharacter(c)))
                throw new ArgumentException("Invalid YouTube channel item.");
            if (!YouTubeDirectPlayback.Enabled)
                return Task.FromResult<IEnumerable<MediaSourceInfo>>(MakeMediaSources(videoId, live));
            // A failed resolver must throw, never return an empty result that
            // could make Emby fall back to the persisted YouTube watch page.
            return YouTubeDirectPlayback.GetMediaSourcesAsync(videoId, cancellationToken);
        }
    }

    internal static class YouTubeDirectPlayback
    {
        private static readonly SemaphoreSlim ManifestGate = new(1, 1);
        private const int MaximumManifestFiles = 128 * (YouTubeAudioTracks.MaximumTracks + 1);
        private const long MaximumManifestBytes = 64L * 1024 * 1024;
        private static readonly TimeSpan ManifestLifetime = TimeSpan.FromHours(12);
        // A private IO seam, not a configurable endpoint. Tests substitute only
        // the fetch; URI, parser, budget, source and native seek code stay real.
        private static Func<Uri, IReadOnlyDictionary<string, string>, int, CancellationToken, Task<byte[]>> _fetchMaster =
            YouTubeDirectHttp.GetBytesAsync;
        internal static bool Enabled => Plugin.Instance?.Options.DirectPlaybackEnabled == true;
        internal static bool IsVideoIdCharacter(char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '-';

        internal static async Task<IEnumerable<MediaSourceInfo>> GetMediaSourcesAsync(string videoId, CancellationToken cancellationToken)
        {
            PluginEntryPoint.SuppressLegacyPlaybackForDirectMode();
            var stage = "resolver";
            try
            {
                var resolved = await YouTubeStreamResolver.ResolveAsync(videoId, cancellationToken).ConfigureAwait(false);
                stage = "format selection";
                if (!Enabled) throw new InvalidOperationException("Direct playback mode changed; retry playback.");
                var metadata = resolved.Metadata;
                if (resolved.ExpiresUtc <= DateTime.UtcNow.AddSeconds(5))
                    throw new InvalidOperationException("Direct playback metadata has expired.");
                if (!metadata.TryGetProperty("formats", out var formats) || formats.ValueKind != JsonValueKind.Array
                    || formats.GetArrayLength() > 1000)
                    throw new InvalidOperationException("No direct playback formats available.");
                var maximumHeight = YouTubeDirectQualityPolicy.ClampHeight(Plugin.Instance?.Options.DirectPlaybackMaxHeight);
                var candidates = formats.EnumerateArray().Where(f => Integer(f, "height") is > 0
                    && Integer(f, "height") <= maximumHeight && Text(f, "vcodec") is not ("" or "none")).ToArray();
                var hls = candidates.Where(f => Text(f, "protocol").StartsWith("m3u8", StringComparison.Ordinal)
                    && Text(f, "manifest_url").Length > 0 && Integer(f, "height") <= 1080
                    && YouTubeDirectManifest.VideoCodec(Text(f, "vcodec")) == "h264")
                    .OrderByDescending(f => Integer(f, "height")).ThenByDescending(f => Number(f, "fps") ?? 0).ToArray();
                var headers = Headers(metadata);
                MediaSourceInfo source;
                // Managed player responses expose a real master URL separately,
                // not fabricated per-height HLS formats. Once advertised, a bad
                // or denied master must fail, never trigger another route.
                Uri? masterUri = null;
                bool managedMaster = metadata.TryGetProperty("hls_manifest_url", out var masterValue)
                    && masterValue.ValueKind != JsonValueKind.Null;
                if (managedMaster)
                {
                    if (masterValue.ValueKind != JsonValueKind.String || string.IsNullOrEmpty(masterValue.GetString()))
                        throw new InvalidOperationException("Invalid direct HLS master metadata.");
                    masterUri = YouTubeDirectHttp.ValidateMediaUri(masterValue.GetString()!);
                }
                else if (hls.Length > 0)
                {
                    // Retain the previous input schema for compatibility, but
                    // all real video/audio eligibility is checked in the master.
                    foreach (var header in Headers(hls[0])) headers[header.Key] = header.Value;
                    masterUri = YouTubeDirectHttp.ValidateMediaUri(Text(hls[0], "manifest_url"));
                }
                if (masterUri != null)
                {
                    stage = "HLS input compatibility";
                    YouTubeDirectSeekCompatibility.RequireInstalled();
                    stage = "HLS master fetch";
                    var bytes = await _fetchMaster(masterUri, headers,
                        YouTubeDirectManifest.MaximumMasterBytes, cancellationToken).ConfigureAwait(false);
                    if (bytes.Length > YouTubeDirectManifest.MaximumMasterBytes)
                        throw new InvalidOperationException("HLS master exceeds its limit.");
                    var master = new UTF8Encoding(false, true).GetString(bytes);
                    stage = "HLS master selection";
                    var advertised = YouTubeDirectManifest.Select(master, masterUri, maximumHeight,
                        YouTubeDirectHttp.ValidateMediaUri, requiredVideoCodec: "h264");
                    // Unsupported higher codecs are not eligible on this runtime.
                    // A missing advertised compatible quality is still an error,
                    // never a reason to hide master corruption with a downgrade.
                    if (!managedMaster && advertised.Height < Integer(hls[0], "height"))
                        throw new InvalidOperationException("HLS master does not contain the selected source quality.");
                    stage = "HLS source budget";
                    var selected = await YouTubeSourceBudget.SelectAsync(master, masterUri, headers,
                        maximumHeight, cancellationToken).ConfigureAwait(false);
                    var manifestPath = await StoreManifestAsync(selected.VideoContent ?? selected.Content, cancellationToken).ConfigureAwait(false);
                    stage = "HLS source metadata";
                    source = new MediaSourceInfo
                    {
                        Id = SourceId(videoId), Path = manifestPath, Protocol = MediaProtocol.File,
                        Container = "hls", Name = "YouTube Auto (bis " + selected.Height.ToString(CultureInfo.InvariantCulture) + "p)",
                        Bitrate = selected.Bitrate, MediaStreams = new List<MediaStream>
                        {
                            new() { Type = MediaStreamType.Video, Index = 0, Codec = selected.VideoCodec,
                                Width = selected.Width, Height = selected.Height, AverageFrameRate = selected.FrameRate }
                        }
                    };
                    foreach (var track in selected.AudioTracks)
                        source.MediaStreams.Add(new MediaStream
                        {
                            Type = MediaStreamType.Audio, Index = YouTubeAudioTracks.Index(track.Key), Codec = selected.AudioCodec,
                            IsExternal = true, Protocol = MediaProtocol.File,
                            Path = await StoreManifestAsync(track.Content, cancellationToken).ConfigureAwait(false),
                            Channels = track.Channels, Language = track.Language, IsDefault = track.IsDefault,
                            Title = track.Name, DisplayTitle = track.Name
                        });
                    if (selected.AudioTracks.Count == 0)
                        source.MediaStreams.Add(new MediaStream { Type = MediaStreamType.Audio, Index = 1,
                            Codec = selected.AudioCodec, Channels = selected.AudioChannels,
                            Language = selected.AudioLanguage, IsDefault = true });
                }
                else
                {
                    stage = "adaptive source budget";
                    var selected = await YouTubeAdaptiveSourceBudget.SelectAsync(resolved, headers, maximumHeight, cancellationToken).ConfigureAwait(false);
                    var video = selected.Video;
                    var audio = selected.Audio;
                    if (audio.HasValue)
                    {
                        stage = "separate input compatibility";
                        YouTubeDirectSeekCompatibility.RequireInstalled();
                    }
                    var videoHeaders = new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
                    foreach (var header in Headers(video)) videoHeaders[header.Key] = header.Value;
                    stage = "adaptive range input registration";
                    var videoPath = await YouTubeDirectRangeProxy.RegisterAsync(resolved, video, videoHeaders, cancellationToken).ConfigureAwait(false);
                    var audioStreams = new List<MediaStream>();
                    foreach (var track in selected.AudioTracks)
                    {
                        var audioHeaders = new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
                        foreach (var header in Headers(track)) audioHeaders[header.Key] = header.Value;
                        var audioPath = RequireOwnedMediaUri(await YouTubeDirectRangeProxy.RegisterAsync(resolved, track, audioHeaders, cancellationToken).ConfigureAwait(false));
                        audioStreams.Add(new MediaStream
                        {
                            Type = MediaStreamType.Audio, Index = YouTubeAudioTracks.Index(YouTubeAudioTracks.Key(track)),
                            Codec = "aac", IsExternal = true, Path = audioPath, Protocol = MediaProtocol.Http,
                            Channels = PositiveInteger(track, "audio_channels"), SampleRate = PositiveInteger(track, "asr"),
                            BitRate = BitsPerSecond(Number(track, "tbr")), Language = Text(track, "language"),
                            IsDefault = audioStreams.Count == 0, Title = YouTubeAudioTracks.Title(track), DisplayTitle = YouTubeAudioTracks.Title(track)
                        });
                    }
                    // Upstream headers remain in each private registry entry,
                    // never on native loopback requests or the other input.
                    headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    if (audioStreams.Count == 0)
                        audioStreams.Add(new MediaStream { Type = MediaStreamType.Audio, Index = 1, Codec = "aac",
                            Channels = PositiveInteger(video, "audio_channels"), SampleRate = PositiveInteger(video, "asr"),
                            BitRate = BitsPerSecond(Number(video, "abr")), Language = Text(video, "language"),
                            IsDefault = true, Title = YouTubeAudioTracks.Title(video), DisplayTitle = YouTubeAudioTracks.Title(video) });
                    source = new MediaSourceInfo
                    {
                        Id = SourceId(videoId), Path = RequireOwnedMediaUri(videoPath),
                        Protocol = MediaProtocol.Http, Container = Text(video, "ext"),
                        Name = "YouTube Auto (bis " + Integer(video, "height").ToString(CultureInfo.InvariantCulture) + "p)",
                        Bitrate = BitsPerSecond(Number(video, "tbr") + (audio.HasValue ? Number(audio.Value, "tbr") : 0)),
                        MediaStreams = new List<MediaStream>
                        {
                            new() { Type = MediaStreamType.Video, Index = 0,
                                Codec = YouTubeDirectManifest.VideoCodec(Text(video, "vcodec")), Width = Integer(video, "width"),
                                Height = Integer(video, "height"), AverageFrameRate = (float?)Number(video, "fps"),
                                BitRate = BitsPerSecond(Number(video, Text(video, "acodec") == "none" ? "tbr" : "vbr")) }
                        }
                    };
                    source.MediaStreams.AddRange(audioStreams);
                }
                cancellationToken.ThrowIfCancellationRequested();
                if (!Enabled || resolved.ExpiresUtc <= DateTime.UtcNow.AddSeconds(5))
                    throw new InvalidOperationException("Direct playback metadata is no longer available.");
                var isLive = Text(metadata, "live_status") == "is_live"
                    || metadata.TryGetProperty("is_live", out var live) && live.ValueKind == JsonValueKind.True;
                source.IsRemote = true;
                source.IsInfiniteStream = isLive;
                var duration = Number(metadata, "duration");
                source.RunTimeTicks = !isLive && duration is > 0 and < 315360000
                    ? (long)(duration.Value * TimeSpan.TicksPerSecond) : null;
                source.Chapters = isLive ? Array.Empty<ChapterInfo>() : YouTubeDirectMetadata.GetChapters(videoId);
                // Emby's original.hls route serves this private master as-is.
                // That bypasses its start offset and subtitle timestamp map;
                // native clients can then restart at zero after a valid resume.
                // Always use the server's native playback timeline, also for a
                // combined HLS input. No client-side corrective seek is needed.
                source.SupportsDirectPlay = false;
                source.SupportsDirectStream = false;
                source.SupportsTranscoding = true;
                // Probing the primary video-only URL cannot discover external
                // audio and could replace the complete advertised stream list.
                source.SupportsProbing = !source.MediaStreams.Any(s => s.Type == MediaStreamType.Audio && s.IsExternal);
                source.RequiresOpening = false;
                source.RequiresClosing = false;
                source.RequiresLooping = false;
                source.RequiredHttpHeaders = headers;
                source.DefaultAudioStreamIndex = source.MediaStreams.First(s => s.Type == MediaStreamType.Audio).Index;
                stage = "subtitle preparation";
                var occupiedIndices = source.MediaStreams.Select(stream => stream.Index).ToHashSet();
                source.MediaStreams.AddRange((await YouTubeDirectSubtitles.GetStreamsAsync(resolved, 2, cancellationToken).ConfigureAwait(false))
                    .Where(stream => occupiedIndices.Add(stream.Index)));
                // Captions are available, but YouTube starts without them. The
                // PlaybackInfo prefix also makes Off explicit before Emby's
                // automatic language selection; manual track requests survive.
                source.DefaultSubtitleStreamIndex = -1;
                return new[] { source };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception error)
            {
                // Exception messages from network/parser/process libraries can
                // contain signed URLs. Do not pass them to Emby's API or logs.
                YouTubeChannel.LogPublic("[YT] Direct source preparation failed at " + stage + " (" + error.GetType().Name + ").");
                throw new InvalidOperationException("YouTube direct playback could not prepare a valid audio/video source. No YouTube-player fallback was used.");
            }
        }

        internal static object RequireDirectPlaybackInfo(object response, string videoId)
        {
            // Emby catches provider exceptions and still returns static sources.
            // Never allow that recovery path to expose a stored watch URL to a
            // direct-mode client. Keep native track/start/encoding decisions.
            var property = response.GetType().GetProperty("MediaSources", BindingFlags.Public | BindingFlags.Instance);
            var original = property?.GetValue(response) as IEnumerable;
            var sources = original?.Cast<object>().OfType<MediaSourceInfo>()
                .Where(s => string.Equals(s.Id, SourceId(videoId), StringComparison.Ordinal)
                    && s.MediaStreams?.Any(t => t.Type == MediaStreamType.Video) == true
                    && s.MediaStreams.Any(t => t.Type == MediaStreamType.Audio)).ToArray();
            if (property == null || sources == null || sources.Length == 0)
                throw new InvalidOperationException("YouTube direct playback is unavailable. No YouTube-player fallback was used.");
            if (property.CanWrite && property.PropertyType.IsAssignableFrom(typeof(MediaSourceInfo[])))
                property.SetValue(response, sources);
            else if (property.CanWrite && property.PropertyType.IsAssignableFrom(typeof(List<MediaSourceInfo>)))
                property.SetValue(response, sources.ToList());
            else if (original is IList list && !list.IsReadOnly && !list.IsFixedSize)
            {
                for (var i = list.Count - 1; i >= 0; i--)
                    if (list[i] is not MediaSourceInfo source || !sources.Contains(source)) list.RemoveAt(i);
            }
            else throw new InvalidOperationException("Native direct source filtering is unavailable.");
            return response;
        }

        internal static string SourceId(string videoId) => "youtube-direct-" + videoId;
        internal static string RequireOwnedMediaUri(string value)
        {
            // Host/port checks alone cannot prove ownership of a loopback URL.
            // Only the active adapter registry can authorize this capability.
            if (!YouTubeDirectRangeProxy.IsOwnedUri(value))
                throw new InvalidOperationException("Native YouTube range input is not registered.");
            return value;
        }
        private static string Text(JsonElement value, string name) => value.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() ?? "" : "";
        private static double? Number(JsonElement value, string name) => value.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.Number && p.TryGetDouble(out var number) && double.IsFinite(number) ? number : null;
        private static int Integer(JsonElement value, string name) => value.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var n) ? n : 0;
        private static int? PositiveInteger(JsonElement value, string name)
        {
            var number = Integer(value, name);
            return number > 0 ? number : null;
        }
        private static int? BitsPerSecond(double? kbps) => kbps is > 0 and < 2147483 ? (int)Math.Ceiling(kbps.Value * 1000) : null;
        private static Dictionary<string, string> Headers(JsonElement value) => YouTubeDirectHttp.ReadHeaders(value.TryGetProperty("http_headers", out var h) ? h : default);

        private static async Task<string> StoreManifestAsync(string content, CancellationToken cancellationToken)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            if (bytes.Length > YouTubeDirectManifest.MaximumMasterBytes) throw new InvalidOperationException("Filtered master exceeds limit.");
            var root = Plugin.CachePath ?? Plugin.DataPath;
            if (string.IsNullOrWhiteSpace(root)) throw new InvalidOperationException("Plugin cache is unavailable.");
            var directory = Path.Combine(root, "youtube-direct-manifests");
            var name = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant() + ".m3u8";
            await ManifestGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                Directory.CreateDirectory(directory);
                if (new DirectoryInfo(directory).LinkTarget != null) throw new InvalidOperationException("Unsafe manifest directory.");
                RestrictAccess(directory, 448); //0700: signed upstream URLs remain server-private.
                var path = Path.Combine(directory, name);
                if (File.Exists(path))
                {
                    if (new FileInfo(path).LinkTarget != null) throw new InvalidOperationException("Unsafe manifest file.");
                    File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
                    return path;
                }
                var files = Directory.EnumerateFiles(directory).Where(IsOwnedManifest).Select(p => new FileInfo(p)).ToArray();
                foreach (var file in files.Where(f => f.LinkTarget == null && DateTime.UtcNow - f.LastWriteTimeUtc > ManifestLifetime)) file.Delete();
                // More language wrappers must not reduce the former video
                // capacity or expand its absolute 128 x 512KiB byte envelope.
                // Count every entry, including unknown files; never traverse a
                // link or nested directory while assessing the private cache.
                int fileCount = 0;
                long totalBytes = bytes.Length;
                foreach (var entry in Directory.EnumerateFileSystemEntries(directory))
                {
                    var file = new FileInfo(entry);
                    if (file.LinkTarget != null || (file.Attributes & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != 0)
                        throw new InvalidOperationException("Unsafe manifest cache entry.");
                    if (++fileCount >= MaximumManifestFiles || file.Length > MaximumManifestBytes - totalBytes)
                        throw new InvalidOperationException("Direct manifest cache is full; retry later.");
                    totalBytes += file.Length;
                }
                var temporary = Path.Combine(directory, name + "." + Guid.NewGuid().ToString("N") + ".tmp");
                try
                {
                    using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        RestrictAccess(temporary, 384); //0600
                        // Tiny bounded manifest; no network/media I/O under this lock.
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush(flushToDisk: true);
                    }
                    File.Move(temporary, path);
                }
                finally { if (File.Exists(temporary)) File.Delete(temporary); }
                return path;
            }
            finally { ManifestGate.Release(); }
        }

        private static bool IsOwnedManifest(string path)
        {
            var name = Path.GetFileName(path);
            return name.Length == 69 && name.EndsWith(".m3u8", StringComparison.Ordinal)
                && name.Take(64).All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f');
        }

        internal static string RequireOwnedManifestPath(string path)
        {
            var root = Plugin.CachePath ?? Plugin.DataPath;
            if (string.IsNullOrWhiteSpace(root) || !Path.IsPathRooted(path) || !IsOwnedManifest(path))
                throw new InvalidOperationException("Invalid direct manifest identity.");
            var directory = Path.GetFullPath(Path.Combine(root, "youtube-direct-manifests"));
            var fullPath = Path.GetFullPath(path);
            var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var parent = new DirectoryInfo(directory);
            var file = new FileInfo(fullPath);
            if (!string.Equals(Path.GetDirectoryName(fullPath), directory, comparison)
                || !parent.Exists || parent.LinkTarget != null || !file.Exists || file.LinkTarget != null
                || file.Length > YouTubeDirectManifest.MaximumMasterBytes)
                throw new InvalidOperationException("Invalid direct manifest location.");
            return fullPath;
        }
        [DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
        private static extern int Chmod(string path, uint mode);
        private static void RestrictAccess(string path, uint mode)
        {
            if (!OperatingSystem.IsWindows() && Chmod(path, mode) != 0)
                throw new IOException("Cannot protect private manifest cache.");
        }
    }

    // Emby4.10.2 creates external audio inputs in MapStreams, but does not
    // copy the primary input's start offset to them. Keep this compatibility
    // hook local to our validated separate-input sources. All ordinary media,
    // passes through unchanged. The owned local HLS path additionally needs a
    // narrow protocol whitelist so its validated HTTPS playlists can be read.
    internal static class YouTubeDirectSeekCompatibility
    {
        private const string HarmonyId = "emby.youtubeplugin.direct-input-seek";
        private const string SourcePrefix = "youtube-direct-";
        private static readonly object Sync = new();
        private static object? _harmony;
        private static Type? _harmonyType;
        private static Binding? _binding;

        private sealed record Binding(PropertyInfo Source, PropertyInfo OutputCodec,
            PropertyInfo Input0, PropertyInfo Inputs, PropertyInfo Url,
            PropertyInfo Options, PropertyInfo Protocol, Type HttpType,
            PropertyInfo[] HttpStrings, PropertyInfo[] InputOptions,
            PropertyInfo ResolutionWidth, PropertyInfo ResolutionHeight, PropertyInfo SupportedCodecs,
            PropertyInfo OriginalAudioStream, PropertyInfo BaseRequest, PropertyInfo StaticRequest,
            MethodInfo TargetFrameRate, PropertyInfo Output0, PropertyInfo OutputVideo,
            PropertyInfo VideoOptions, PropertyInfo OutputFrameRate, PropertyInfo SelectedAudioStreams);

        internal static void RequireInstalled()
        {
            Install();
            if (_binding == null)
                throw new InvalidOperationException("Native direct-input compatibility is unavailable.");
        }

        internal static void Install()
        {
            lock (Sync)
            {
                if (_binding != null) return;
                var stage = "native types";
                try
                {
                    const string encoder = "Emby.Server.MediaEncoding.Encoder.";
                    var builder = FindType(encoder + "TranscodingCommandBuilder");
                    var state = FindType(encoder + "EncodingJobInfo");
                    var command = FindType("Emby.Ffmpeg.Model.FfmpegCommand");
                    var input = FindType("Emby.Ffmpeg.Model.Input.InputFile");
                    var output = FindType("Emby.Ffmpeg.Model.Output.OutputFile");
                    var http = FindType("Emby.Ffmpeg.Lib.InputProtocols.http");
                    var resolution = FindType(encoder + "ResolutionOptions");
                    if (builder.Assembly.GetName().Version != new Version(4, 10, 2, 0))
                        throw new InvalidOperationException();
                    var map = ExactStaticMethod(builder, "MapStreams", state, command, input, output);
                    var setup = ExactStaticMethod(builder, "SetupInput", command, state);
                    var resolutionLimits = ExactStaticMethod(FindType(encoder + "EncodingHelpers"),
                        "ApplyCodecResolutionLimits", state, typeof(int?), typeof(int?), resolution);
                    stage = "audio signatures";
                    var outputStream = FindType(encoder + "OutputStreamInfo");
                    stage = "audio channel signature";
                    var audioChannels = ExactStaticReturning(typeof(int?), FindType(encoder + "EncodingHelpers"),
                        "GetNumAudioChannelsParam", state, outputStream, typeof(string));
                    stage = "audio selector signature";
                    // Enumerating this signature resolves its lazily loaded
                    // interface assembly before binding. Do not depend on that
                    // assembly having been touched by an earlier playback.
                    var audioSelector = FindType(encoder + "EncodingHelpers")
                        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        .Single(m => m.Name == "GetOutputAudioCodec" && m.ReturnType == typeof(string)
                            && m.GetParameters() is var p && p.Length == 4
                            && p[0].ParameterType == state && p[1].ParameterType == outputStream
                            && p[2].ParameterType == typeof(string[])
                            && p[3].ParameterType.FullName == "MediaBrowser.Controller.MediaEncoding.IFfmpegManager");
                    stage = "video signature";
                    var codecSelector = FindType("Emby.Server.MediaEncoding.Api.BaseStreamingService")
                        .GetMethod("GetOutputVideoCodec", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                            null, new[] { FindType("Emby.Server.MediaEncoding.Api.StreamState") }, null);
                    if (codecSelector?.ReturnType != typeof(string)) throw new InvalidOperationException();
                    stage = "frame rate signature";
                    var frameRateSelector = state.GetMethod("GetTargetFramerate",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new[] { typeof(string), typeof(bool) }, null);
                    if (frameRateSelector?.ReturnType != typeof(float?)) throw new InvalidOperationException();
                    var videoArguments = builder.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Single(m => m.Name == "SetVideoArguments"
                            && m.ReturnType.FullName == "Emby.Server.MediaEncoding.Encoder.Pipelines.FilterBuilder.ClosedCaptionSource"
                            && m.GetParameters() is var p && p.Length == 5 && p[0].ParameterType == state
                            && p[1].ParameterType.FullName == "Emby.Server.MediaEncoding.Codecs.Common.Types.ConfiguredVideoDecoder"
                            && p[2].ParameterType.FullName == "Emby.Server.MediaEncoding.Codecs.Common.Types.ConfiguredVideoEncoder"
                            && p[3].ParameterType == command && p[4].ParameterType == typeof(string));
                    // Some installations add a VOD policy that rewrites3s to1s
                    // after a session's manifest was already built. It must not
                    // change the segment-index timeline for our direct sources.
                    // Stock Emby has no such type and needs no additional hook.
                    var segmentPolicyType = builder.Assembly.GetType(encoder + "FastVodSegmentLengthPolicy", false);
                    MethodInfo? segmentPolicy = null;
                    if (segmentPolicyType != null)
                    {
                        stage = "optional segment policy signature";
                        segmentPolicy = segmentPolicyType.GetMethod("Apply",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                            null, new[] { state, typeof(int) }, null);
                        if (segmentPolicy?.ReturnType != typeof(int)) throw new InvalidOperationException();
                    }
                    stage = "native properties";
                    var options = Property(input, "Options");
                    var baseRequest = Property(state, "BaseRequest");
                    var output0 = Property(command, "Output0");
                    var outputVideo = Property(output0.PropertyType, "Video0");
                    // The video stream hides the base stream's Options with
                    // a more specific type; an inherited lookup is ambiguous.
                    var videoOptions = outputVideo.PropertyType.GetProperty("Options",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (videoOptions == null || !videoOptions.CanRead
                        || videoOptions.PropertyType.FullName != "Emby.Ffmpeg.Lib.Options.OptionsVideoOutput")
                        throw new InvalidOperationException();
                    var binding = new Binding(
                        Property(state, "MediaSource", typeof(MediaSourceInfo)),
                        Property(state, "OutputVideoCodec", typeof(string)),
                        Property(command, "Input0", input), Property(command, "Inputs"),
                        Property(input, "Url", typeof(string)), options,
                        Property(input, "InputProtocol", writable: true), http,
                        new[] { "user_agent", "referer", "headers" }.Select(n => Property(http, n, typeof(string), true)).ToArray(),
                        new[] { "ss", "t", "to" }.Select(n => Property(options.PropertyType, n, typeof(TimeSpan?), true))
                            .Concat(new[] { Property(options.PropertyType, "accurate_seek", typeof(bool?), true),
                                Property(options.PropertyType, "protocol_whitelist", typeof(string), true) }).ToArray(),
                        Property(resolution, "MaxWidth", typeof(int?), true),
                        Property(resolution, "MaxHeight", typeof(int?), true),
                        Property(state, "SupportedVideoCodecs", typeof(string[]), true),
                        Property(outputStream, "OriginalStream", typeof(MediaStream)),
                        baseRequest, Property(baseRequest.PropertyType, "Static", typeof(bool)), frameRateSelector,
                        output0, outputVideo, videoOptions, Property(videoOptions.PropertyType, "r", typeof(string), true),
                        Property(state, "AudioStreams", outputStream.MakeArrayType()));
                    if (http.GetConstructor(Type.EmptyTypes) == null
                        || !binding.Protocol.PropertyType.IsAssignableFrom(http)
                        || !typeof(IEnumerable).IsAssignableFrom(binding.Inputs.PropertyType)
                        || !typeof(IEnumerable).IsAssignableFrom(binding.SelectedAudioStreams.PropertyType))
                        throw new InvalidOperationException();
                    stage = "Harmony load";
                    var assembly = EmbeddedDependencyLoader.LoadHarmonyAssembly();
                    _harmonyType = assembly?.GetType("HarmonyLib.Harmony") ?? throw new InvalidOperationException();
                    var methodType = assembly!.GetType("HarmonyLib.HarmonyMethod") ?? throw new InvalidOperationException();
                    _harmony = Activator.CreateInstance(_harmonyType, HarmonyId) ?? throw new InvalidOperationException();
                    var patch = _harmonyType.GetMethods().Single(m => m.Name == "Patch"
                        && m.GetParameters().Length >= 3 && m.GetParameters()[0].ParameterType == typeof(MethodBase)
                        && m.GetParameters().Any(p => p.Name == "postfix"));
                    void Add(MethodInfo original, string name, bool isPrefix = false)
                    {
                        var callback = typeof(YouTubeDirectSeekCompatibility).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;
                        var patchMethod = Activator.CreateInstance(methodType, callback);
                        var parameters = patch.GetParameters();
                        var arguments = new object?[parameters.Length];
                        arguments[0] = original;
                        for (var i = 1; i < arguments.Length; i++)
                            if (parameters[i].Name == (isPrefix ? "prefix" : "postfix")) arguments[i] = patchMethod;
                        patch.Invoke(_harmony, arguments);
                    }
                    // Publish before patching; no callback can see a half-bound
                    // interface. RequireInstalled remains locked during setup.
                    _binding = binding;
                    stage = "native patching";
                    Add(map, nameof(MapStreamsPostfix));
                    Add(setup, nameof(SetupInputPostfix));
                    Add(resolutionLimits, nameof(CodecResolutionPostfix));
                    Add(codecSelector, nameof(PreferH264Prefix), true);
                    Add(frameRateSelector, nameof(FrameRatePostfix));
                    Add(videoArguments, nameof(VideoArgumentsPostfix));
                    stage = "audio channel patch";
                    Add(audioChannels, nameof(AudioChannelsPostfix));
                    stage = "audio selector patch";
                    Add(audioSelector, nameof(PreferAacPrefix), true);
                    if (segmentPolicy != null) Add(segmentPolicy, nameof(SegmentLengthPostfix));
                    YouTubeChannel.LogPublic("[YT] Native direct-input seek compatibility installed.");
                }
                catch (Exception error)
                {
                    Uninstall();
                    // Only static stage/type names, never messages/paths/URLs.
                    YouTubeChannel.LogPublic("[YT] Native direct-input seek compatibility unavailable at "
                        + stage + " (" + error.GetType().Name + "); separate-input and local HLS playback are disabled.");
                }
            }
        }

        internal static void Uninstall()
        {
            lock (Sync)
            {
                _binding = null;
                try
                {
                    _harmonyType?.GetMethod("UnpatchAll", new[] { typeof(string) })
                        ?.Invoke(_harmony, new object?[] { HarmonyId });
                }
                catch { /* Never expose upstream input values through an exception. */ }
                _harmony = null;
                _harmonyType = null;
            }
        }

        private static void MapStreamsPostfix(object __0, object __1) => Apply(__0, __1, true);
        private static void SetupInputPostfix(object __0, object __1) => Apply(__1, __0, false);
        private static void CodecResolutionPostfix(object __0, object __3) => ApplyOutputLimit(__0, __3);
        private static void FrameRatePostfix(object __instance, string __0, bool __1, ref float? __result)
        {
            // Existing native sessions retain their budget after direct mode is
            // disabled; the validated source, not a mutable setting, owns it.
            var binding = _binding;
            if (binding == null) return;
            var source = binding.Source.GetValue(__instance) as MediaSourceInfo;
            if (!IsCanonicalDirectSource(source)) return;
            if (string.IsNullOrWhiteSpace(__0) || string.Equals(__0, "copy", StringComparison.OrdinalIgnoreCase)) return;
            var request = binding.BaseRequest.GetValue(__instance);
            if (request == null || binding.StaticRequest.GetValue(request) is true) return;
            try
            {
                if (source!.Protocol == MediaProtocol.Http)
                    YouTubeDirectPlayback.RequireOwnedMediaUri(source.Path);
                else if (source.Protocol == MediaProtocol.File && string.Equals(source.Container, "hls", StringComparison.OrdinalIgnoreCase))
                    YouTubeDirectPlayback.RequireOwnedManifestPath(source.Path);
                else return;
                // Bound the final target, not only maxframerate: Emby's explicit
                // framerate option takes precedence over its maximum option.
                // Use Emby's own source/interlace normalization for the optional
                // output-rate call. The false overload cannot recurse here.
                if (!__result.HasValue && __1)
                {
                    var effective = (float?)binding.TargetFrameRate.Invoke(__instance, new object[] { __0, false });
                    if (effective == YouTubeDirectQualityPolicy.MaximumFrameRate)
                        __result = effective;
                }
                if (__result is > YouTubeDirectQualityPolicy.MaximumFrameRate)
                    __result = YouTubeDirectQualityPolicy.MaximumFrameRate;
            }
            catch
            {
                throw new InvalidOperationException("Native YouTube frame rate limit could not be applied.");
            }
        }
        private static void VideoArgumentsPostfix(object __0, object __3)
        {
            var binding = _binding;
            if (binding == null) return;
            var source = binding.Source.GetValue(__0) as MediaSourceInfo;
            if (!IsCanonicalDirectSource(source)) return;
            var codec = binding.OutputCodec.GetValue(__0) as string;
            var request = binding.BaseRequest.GetValue(__0);
            if (string.IsNullOrWhiteSpace(codec) || string.Equals(codec, "copy", StringComparison.OrdinalIgnoreCase)
                || request == null || binding.StaticRequest.GetValue(request) is true) return;
            try
            {
                if (source!.Protocol == MediaProtocol.Http)
                    YouTubeDirectPlayback.RequireOwnedMediaUri(source.Path);
                else if (source.Protocol == MediaProtocol.File && string.Equals(source.Container, "hls", StringComparison.OrdinalIgnoreCase))
                    YouTubeDirectPlayback.RequireOwnedManifestPath(source.Path);
                else return;
                var output = binding.Output0.GetValue(__3);
                var video = output == null ? null : binding.OutputVideo.GetValue(output);
                var options = video == null ? null : binding.VideoOptions.GetValue(video);
                if (options == null) return;
                var oldText = binding.OutputFrameRate.GetValue(options) as string;
                var oldRate = ParseOutputFrameRate(oldText);
                // Preserve an unknown native expression rather than replacing
                // it with a guessed interpretation.
                if (!string.IsNullOrWhiteSpace(oldText) && oldRate == null) return;
                var requested = (float?)binding.TargetFrameRate.Invoke(__0, new object[] { codec, true });
                var validRequested = requested is > 0 && float.IsFinite(requested.Value) ? (double?)requested.Value : null;
                var rate = oldRate.HasValue && validRequested.HasValue ? Math.Min(oldRate.Value, validRequested.Value)
                    : oldRate ?? validRequested;
                if (rate is not > 0 || !double.IsFinite(rate.Value)) return;
                var cappedRate = Math.Min(rate.Value, YouTubeDirectQualityPolicy.MaximumFrameRate);
                // Keep existing exact fractions and very small positive rates
                // unchanged when no reduction is needed.
                if (oldRate == cappedRate) return;
                // VAAPI does not consume the same RequestedFrameRate demand as
                // the software encoder. Apply the scoped final stream option
                // after encoder setup; it serializes as -r:v:0 for either path.
                binding.OutputFrameRate.SetValue(options,
                    cappedRate.ToString("G17", CultureInfo.InvariantCulture));
            }
            catch
            {
                throw new InvalidOperationException("Native YouTube output frame rate could not be applied.");
            }
        }

        private static double? ParseOutputFrameRate(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var parts = text.Split('/');
            if (parts.Length is < 1 or > 2
                || !double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var numerator)
                || !double.IsFinite(numerator) || numerator <= 0) return null;
            double denominator = 1;
            if (parts.Length == 2 && (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out denominator)
                || !double.IsFinite(denominator) || denominator <= 0)) return null;
            var rate = numerator / denominator;
            return double.IsFinite(rate) && rate > 0 ? rate : null;
        }
        private static void PreferH264Prefix(object __0)
        {
            var binding = _binding;
            if (!YouTubeDirectPlayback.Enabled || binding == null) return;
            var source = binding.Source.GetValue(__0) as MediaSourceInfo;
            if (!IsCanonicalDirectSource(source)) return;
            var codecs = binding.SupportedCodecs.GetValue(__0) as string[];
            if (codecs == null) return;
            var index = Array.FindIndex(codecs, c => string.Equals(c, "h264", StringComparison.OrdinalIgnoreCase));
            if (index < 1) return;
            try
            {
                if (source!.Protocol == MediaProtocol.Http)
                    YouTubeDirectPlayback.RequireOwnedMediaUri(source.Path);
                else if (source.Protocol == MediaProtocol.File && string.Equals(source.Container, "hls", StringComparison.OrdinalIgnoreCase))
                    YouTubeDirectPlayback.RequireOwnedManifestPath(source.Path);
                else return;
                // Prefer the compatible H.264 path only when advertised.
                // Native selection, subtitle handling and stricter output
                // limits remain authoritative; shared profiles are untouched.
                var preferred = new string[codecs.Length];
                preferred[0] = codecs[index];
                Array.Copy(codecs, 0, preferred, 1, index);
                Array.Copy(codecs, index + 1, preferred, index + 1, codecs.Length - index - 1);
                binding.SupportedCodecs.SetValue(__0, preferred);
            }
            catch
            {
                throw new InvalidOperationException("Native YouTube codec preference validation failed.");
            }
        }

        private static MediaStream? GetStereoOrUnknownAudio(object state, object outputStream)
        {
            var binding = _binding;
            if (!YouTubeDirectPlayback.Enabled || binding == null || outputStream == null) return null;
            var source = binding.Source.GetValue(state) as MediaSourceInfo;
            if (!IsCanonicalDirectSource(source)) return null;
            var audio = binding.OriginalAudioStream.GetValue(outputStream) as MediaStream;
            if (audio?.Type != MediaStreamType.Audio || audio.Channels > 2) return null;
            try
            {
                if (source!.Protocol == MediaProtocol.Http)
                    YouTubeDirectPlayback.RequireOwnedMediaUri(source.Path);
                else if (source.Protocol == MediaProtocol.File && string.Equals(source.Container, "hls", StringComparison.OrdinalIgnoreCase))
                    YouTubeDirectPlayback.RequireOwnedManifestPath(source.Path);
                else return null;
                return audio;
            }
            catch
            {
                throw new InvalidOperationException("Native YouTube audio output validation failed.");
            }
        }

        private static void AudioChannelsPostfix(object __0, object __1, string __2, ref int? __result)
        {
            if (string.Equals(__2, "copy", StringComparison.OrdinalIgnoreCase)) return;
            var audio = GetStereoOrUnknownAudio(__0, __1);
            if (audio == null) return;
            // HLS often omits CHANNELS. Emby's unknown-channel fallback is six,
            // which also favours AC3. Bound this OUTPUT, without pretending to
            // know the source layout, or raising a stricter native/client cap.
            var maximum = audio.Channels > 0 ? audio.Channels.Value : 2;
            __result = __result > 0 ? Math.Min(__result.Value, maximum) : maximum;
        }

        private static void PreferAacPrefix(object __0, object __1, ref string[] __2)
        {
            if (__2 == null || GetStereoOrUnknownAudio(__0, __1) == null) return;
            var index = Array.FindIndex(__2, c => string.Equals(c, "aac", StringComparison.OrdinalIgnoreCase));
            if (index < 1) return;
            // Native copy eligibility and codec restrictions still apply. Never
            // add an unsupported codec or modify the caller's capability array.
            var preferred = new string[__2.Length];
            preferred[0] = __2[index];
            Array.Copy(__2, 0, preferred, 1, index);
            Array.Copy(__2, index + 1, preferred, index + 1, __2.Length - index - 1);
            __2 = preferred;
        }
        private static void SegmentLengthPostfix(object __0, int __1, ref int __result)
        {
            var binding = _binding;
            if (binding == null) return;
            var source = binding.Source.GetValue(__0) as MediaSourceInfo;
            if (!IsCanonicalDirectSource(source)) return;
            try
            {
                if (source!.Protocol == MediaProtocol.Http)
                    YouTubeDirectPlayback.RequireOwnedMediaUri(source.Path);
                else if (source.Protocol == MediaProtocol.File && string.Equals(source.Container, "hls", StringComparison.OrdinalIgnoreCase))
                    YouTubeDirectPlayback.RequireOwnedManifestPath(source.Path);
                else return;
                // Source identity remains authoritative for an active session
                // even if the user disables direct mode before its next seek.
                __result = __1;
            }
            catch
            {
                throw new InvalidOperationException("Native YouTube segment timeline validation failed.");
            }
        }

        private static bool IsCanonicalDirectSource(MediaSourceInfo? source) =>
            source?.Id != null && source.Id.StartsWith(SourcePrefix, StringComparison.Ordinal)
            && source.Id.Length == SourcePrefix.Length + 11
            && source.Id.Substring(SourcePrefix.Length).All(YouTubeDirectPlayback.IsVideoIdCharacter);

        private static void ApplyOutputLimit(object state, object resolution)
        {
            var binding = _binding;
            if (!YouTubeDirectPlayback.Enabled || binding == null) return;
            var source = binding.Source.GetValue(state) as MediaSourceInfo;
            if (!IsCanonicalDirectSource(source)) return;
            try
            {
                // Validate ownership independently of the advertised source ID.
                if (source!.Protocol == MediaProtocol.Http)
                    YouTubeDirectPlayback.RequireOwnedMediaUri(source.Path);
                else if (source.Protocol == MediaProtocol.File && string.Equals(source.Container, "hls", StringComparison.OrdinalIgnoreCase))
                    YouTubeDirectPlayback.RequireOwnedManifestPath(source.Path);
                else return;
                var maximumHeight = YouTubeDirectQualityPolicy.ClampHeight(Plugin.Instance?.Options.DirectPlaybackMaxHeight);
                if (string.Equals(binding.OutputCodec.GetValue(state) as string, "h264", StringComparison.OrdinalIgnoreCase))
                    maximumHeight = Math.Min(maximumHeight,
                        YouTubeDirectQualityPolicy.ClampHeight(Plugin.Instance?.Options.DirectPlaybackH264MaxHeight));
                var maximumWidth = (int)Math.Ceiling(maximumHeight * (16d / 9d));
                var previousWidth = (int?)binding.ResolutionWidth.GetValue(resolution);
                var previousHeight = (int?)binding.ResolutionHeight.GetValue(resolution);
                // These are bounding dimensions, not forced output dimensions.
                // Native output-size calculation still preserves aspect ratio.
                // Native Resize treats zero as no bound, not a zero-size cap.
                binding.ResolutionWidth.SetValue(resolution,
                    previousWidth > 0 ? Math.Min(previousWidth.Value, maximumWidth) : maximumWidth);
                binding.ResolutionHeight.SetValue(resolution,
                    previousHeight > 0 ? Math.Min(previousHeight.Value, maximumHeight) : maximumHeight);
            }
            catch
            {
                throw new InvalidOperationException("Native YouTube output limit could not be applied.");
            }
        }

        private static void Apply(object state, object command, bool requireMappedInput)
        {
            var binding = _binding;
            // An already-running direct source still needs synchronized inputs
            // if the configuration is disabled before its next native seek.
            // Identity below, not the current global toggle, scopes the hook.
            if (binding == null) return;
            var source = binding.Source.GetValue(state) as MediaSourceInfo;
            if (!IsCanonicalDirectSource(source)) return;
            if (source == null) return;
            try
            {
                bool hls = source.Protocol == MediaProtocol.File && string.Equals(source.Container, "hls", StringComparison.OrdinalIgnoreCase);
                if (!hls && source.Protocol != MediaProtocol.Http) return;
                string OwnedPath(string path) => hls ? YouTubeDirectPlayback.RequireOwnedManifestPath(path)
                    : YouTubeDirectPlayback.RequireOwnedMediaUri(path);
                var videoUrl = OwnedPath(source.Path);
                var primary = binding.Input0.GetValue(command) ?? throw new InvalidOperationException();
                if (!string.Equals(binding.Url.GetValue(primary) as string, videoUrl, StringComparison.Ordinal))
                    throw new InvalidOperationException();
                var primaryOptions = binding.Options.GetValue(primary) ?? throw new InvalidOperationException();
                if (hls) binding.InputOptions.Single(p => p.Name == "protocol_whitelist")
                    .SetValue(primaryOptions, "file,https,tls,tcp,crypto");
                var advertised = source.MediaStreams?.Where(s => s.Type == MediaStreamType.Audio).ToArray()
                    ?? Array.Empty<MediaStream>();
                if (!advertised.Any(track => track.IsExternal)) return;
                var selected = binding.SelectedAudioStreams.GetValue(state) as IEnumerable;
                // Selection is native state, not the complete advertised list.
                // Setup may precede selection; MapStreams must prove it exists.
                if (selected == null)
                {
                    if (requireMappedInput) throw new InvalidOperationException();
                    return;
                }
                var tracks = selected.Cast<object>().Select(stream => binding.OriginalAudioStream.GetValue(stream) as MediaStream)
                    .ToArray();
                if (tracks.Length > YouTubeAudioTracks.MaximumTracks || tracks.Any(track => track == null)
                    || tracks.GroupBy(track => track!.Index).Any(group => group.Count() != 1)) throw new InvalidOperationException();
                foreach (var track in tracks.Where(track => track!.IsExternal))
                {
                    if (track!.Type != MediaStreamType.Audio || track.Protocol != source.Protocol
                        || advertised.Count(candidate => candidate.Index == track.Index && candidate.IsExternal
                            && candidate.Protocol == track.Protocol && string.Equals(candidate.Path, track.Path, StringComparison.Ordinal)) != 1)
                        throw new InvalidOperationException();
                    var audioUrl = OwnedPath(track.Path);
                    var inputs = ((IEnumerable)(binding.Inputs.GetValue(command) ?? throw new InvalidOperationException()))
                        .Cast<object>().Where(input => !ReferenceEquals(input, primary)
                            && string.Equals(binding.Url.GetValue(input) as string, audioUrl, StringComparison.Ordinal)).ToArray();
                    if (inputs.Length == 0 && !requireMappedInput) continue;
                    var outputCodec = binding.OutputCodec.GetValue(state) as string;
                    if (inputs.Length != 1 || string.IsNullOrWhiteSpace(outputCodec)
                        || string.Equals(outputCodec, "copy", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException();
                    var audioOptions = binding.Options.GetValue(inputs[0]) ?? throw new InvalidOperationException();
                    foreach (var property in binding.InputOptions) property.SetValue(audioOptions, property.GetValue(primaryOptions));
                    if (!hls)
                    {
                        var protocol = binding.Protocol.GetValue(primary);
                        if (protocol == null || protocol.GetType() != binding.HttpType) throw new InvalidOperationException();
                        var copiedProtocol = Activator.CreateInstance(binding.HttpType) ?? throw new InvalidOperationException();
                        foreach (var property in binding.HttpStrings) property.SetValue(copiedProtocol, property.GetValue(protocol));
                        binding.Protocol.SetValue(inputs[0], copiedProtocol);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Native YouTube audio/video input synchronization could not be verified.");
            }
        }

        private static Type FindType(string name) => AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType(name, false)).FirstOrDefault(t => t != null) ?? throw new InvalidOperationException();
        private static PropertyInfo Property(Type type, string name, Type? expected = null, bool writable = false)
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();
            if (!property.CanRead || writable && !property.CanWrite || expected != null && property.PropertyType != expected)
                throw new InvalidOperationException();
            return property;
        }
        private static MethodInfo ExactStaticMethod(Type type, string name, params Type[] parameters)
            => ExactStaticReturning(typeof(void), type, name, parameters);

        private static MethodInfo ExactStaticReturning(Type returns, Type type, string name, params Type[] parameters)
        {
            var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, parameters, null) ?? throw new InvalidOperationException();
            if (method.ReturnType != returns) throw new InvalidOperationException();
            return method;
        }
    }
}
