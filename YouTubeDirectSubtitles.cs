using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.MediaInfo;
using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin
{
    // Captions are ordinary external Emby streams. No subtitle selection or user
    // preference is changed here, and no upstream URL is returned to the client.
    internal static class YouTubeDirectSubtitles
    {
        private const int MaximumTracks = 32;
        private const int MaximumCaptionBytes = YouTubeCaptionNormalizer.MaximumPayloadBytes;
        private const int MaximumCacheFiles = 512;
        private const long MaximumCacheBytes = 128L * 1024 * 1024;
        private static readonly TimeSpan Retention = TimeSpan.FromHours(12);
        private static readonly TimeSpan TotalBudget = TimeSpan.FromSeconds(20);
        private static readonly TimeSpan TrackBudget = TimeSpan.FromSeconds(8);
        private static readonly SemaphoreSlim InvocationGate = new(4, 4);
        private static readonly SemaphoreSlim FetchGate = new(3, 3);
        private static readonly SemaphoreSlim CacheGate = new(1, 1);
        private static readonly Dictionary<string, CacheEntry> Cache = new(StringComparer.Ordinal);
        private static readonly UTF8Encoding StrictUtf8 = new(false, true);

        internal sealed record CaptionCandidate(
            string Key, string Language, string Title, bool Automatic,
            Uri? Url, string? Data, Dictionary<string, string> Headers);
        private sealed record CacheEntry(string Path, DateTime CachedUtc, DateTime LastUseUtc);

        public static async Task<List<MediaStream>> GetStreamsAsync(
            YouTubeResolvedVideo resolved, int firstIndex, CancellationToken cancellationToken)
        {
            if (firstIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(firstIndex));
            if (!IsVideoId(resolved.VideoId) || string.IsNullOrEmpty(Plugin.CachePath))
                return new List<MediaStream>();
            cancellationToken.ThrowIfCancellationRequested();
            if (!await InvocationGate.WaitAsync(0, cancellationToken).ConfigureAwait(false))
            {
                YouTubeChannel.LogPublic("[YT] Direct subtitles omitted: caption request capacity is in use.");
                return new List<MediaStream>();
            }

            try
            {
                var candidates = SelectCandidates(resolved.Metadata, out var translated, out var truncated);
                if (translated > 0 || truncated > 0)
                    YouTubeChannel.LogPublic($"[YT] Direct subtitles: {translated} translated track(s) excluded; {truncated} original track(s) omitted by the {MaximumTracks}-track limit.");
                if (candidates.Count == 0)
                    return new List<MediaStream>();
                var indexedCandidates = AssignIndices(candidates, firstIndex, out var collisions);
                if (collisions > 0)
                    YouTubeChannel.LogPublic($"[YT] Direct subtitles: {collisions} track(s) omitted due to an index identity collision; no track was renumbered.");

                using var budget = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                budget.CancelAfter(TotalBudget);
                // The index depends only on the stable track key, never its
                // position in a changing language inventory. Conflicts are
                // removed before fetch, not resolved by renumbering a track.
                var pending = indexedCandidates.Select(entry =>
                    GetStreamAsync(resolved.VideoId, entry.Candidate, entry.Index, budget.Token)).ToArray();
                var streams = await Task.WhenAll(pending).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var result = streams.Where(stream => stream != null).Cast<MediaStream>().ToList();
                if (result.Count != candidates.Count)
                    YouTubeChannel.LogPublic($"[YT] Direct subtitles: {result.Count}/{candidates.Count} original track(s) verified; unavailable tracks omitted without blocking video.");
                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Exception messages from upstream/file APIs can contain private
                // signed URLs or filesystem paths. Report only a bounded category.
                YouTubeChannel.LogPublic($"[YT] Direct subtitles unavailable ({ex.GetType().Name}); video playback continues.");
                return new List<MediaStream>();
            }
            finally
            {
                InvocationGate.Release();
            }
        }

        internal static int TrackIndex(string key, int firstIndex)
        {
            if (firstIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(firstIndex));
            var minimum = Math.Max(firstIndex, 1); // Zero and Off(-1) are never subtitle identities.
            var digest = SHA256.HashData(StrictUtf8.GetBytes(key));
            var value = BinaryPrimitives.ReadUInt32BigEndian(digest.AsSpan(0, 4));
            var range = (ulong)int.MaxValue - (uint)minimum + 1;
            return (int)((ulong)minimum + value % range);
        }

        internal static List<(CaptionCandidate Candidate, int Index)> AssignIndices(
            IReadOnlyList<CaptionCandidate> candidates, int firstIndex, out int collisionCount)
        {
            var indexed = candidates.Select(candidate =>
                (Candidate: candidate, Index: TrackIndex(candidate.Key, firstIndex))).ToArray();
            var collisions = indexed.GroupBy(entry => entry.Index).Where(group => group.Count() > 1)
                .Select(group => group.Key).ToHashSet();
            collisionCount = indexed.Count(entry => collisions.Contains(entry.Index));
            return indexed.Where(entry => !collisions.Contains(entry.Index)).ToList();
        }

        internal static List<CaptionCandidate> SelectCandidates(
            JsonElement metadata, out int translated, out int truncated)
        {
            translated = 0;
            truncated = 0;
            var selected = new Dictionary<string, CaptionCandidate>(StringComparer.Ordinal);
            if (metadata.ValueKind != JsonValueKind.Object)
                return new List<CaptionCandidate>();
            var commonHeaders = YouTubeDirectHttp.ReadHeaders(Property(metadata, "http_headers"));
            foreach (var (property, automatic) in new[] { ("subtitles", false), ("automatic_captions", true) })
            {
                var dictionary = Property(metadata, property);
                if (dictionary.ValueKind != JsonValueKind.Object)
                    continue;
                // Extraction JSON is already byte-bounded by the resolver. Limit
                // work as well, without making dictionary insertion order a choice.
                var languages = dictionary.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal).Take(2048);
                foreach (var language in languages)
                {
                    if (language.Name.Length > 100 || language.Value.ValueKind != JsonValueKind.Array)
                        continue;
                    var isOriginalAlias = automatic && language.Name.EndsWith("-orig", StringComparison.Ordinal);
                    var keyLanguage = isOriginalAlias ? language.Name[..^5] : language.Name;
                    var key = (automatic ? "auto:" : "manual:") + keyLanguage;
                    CaptionCandidate? candidate = null;
                    var isTranslation = false;
                    foreach (var format in language.Value.EnumerateArray().Take(128))
                    {
                        if (!string.Equals(Text(format, "ext"), "vtt", StringComparison.OrdinalIgnoreCase))
                            continue;
                        try
                        {
                            Uri? uri = null;
                            var url = Text(format, "url");
                            var data = Text(format, "data");
                            var query = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                uri = YouTubeDirectHttp.ValidateCaptionUri(url);
                                query = ReadQuery(uri);
                            }
                            else if (data == null || (automatic && !isOriginalAlias))
                                continue; // Inline automatic data needs explicit original provenance.
                            if (query.TryGetValue("tlang", out var target) && target.Length != 0)
                            {
                                isTranslation = true;
                                continue;
                            }
                            if (automatic && !isOriginalAlias && uri != null
                                && (!query.TryGetValue("kind", out var kind)
                                    || !string.Equals(kind, "asr", StringComparison.OrdinalIgnoreCase)))
                                continue; // Do not relabel an ambiguous translated/manual resource as ASR.
                            var sourceLanguage = query.TryGetValue("lang", out var lang) && lang.Length != 0
                                ? lang : keyLanguage;
                            var headers = new Dictionary<string, string>(commonHeaders, StringComparer.OrdinalIgnoreCase);
                            foreach (var header in YouTubeDirectHttp.ReadHeaders(Property(format, "http_headers")))
                                headers[header.Key] = header.Value;
                            var display = CleanTitle(Text(format, "name") ?? sourceLanguage);
                            // yt-dlp's Original alias is preferred without changing
                            // its identity or merging manual and ASR of one language.
                            candidate = new CaptionCandidate(key, CanonicalLanguage(sourceLanguage),
                                display + (automatic ? " (automatisch)" : " (manuell)"), automatic, uri, data, headers);
                            break;
                        }
                        catch (Exception ex) when (ex is ArgumentException or InvalidDataException or FormatException or InvalidOperationException)
                        {
                            // Invalid metadata cannot produce a selectable phantom.
                        }
                    }
                    if (candidate != null && (!selected.ContainsKey(key) || isOriginalAlias))
                        selected[key] = candidate;
                    else if (isTranslation)
                        translated++;
                }
            }
            var ordered = selected.Values.OrderBy(c => c.Language, StringComparer.Ordinal)
                .ThenBy(c => c.Automatic).ThenBy(c => c.Key, StringComparer.Ordinal).ToArray();
            truncated = Math.Max(0, ordered.Length - MaximumTracks);
            return ordered.Take(MaximumTracks).ToList();
        }

        private static async Task<MediaStream?> GetStreamAsync(
            string videoId, CaptionCandidate candidate, int index, CancellationToken cancellationToken)
        {
            try
            {
                // Format revisions invalidate payload cache entries, never the
                // stable manual/ASR key used for native subtitle track identity.
                var cacheKey = videoId + "|" + candidate.Key + "|" + YouTubeCaptionNormalizer.CacheRevision;
                var path = await FindCachedAsync(cacheKey, cancellationToken).ConfigureAwait(false);
                if (path == null)
                {
                    await FetchGate.WaitAsync(cancellationToken).ConfigureAwait(false);
                    try
                    {
                        // Another request for this video may have completed while
                        // this request was waiting for the global fetch permit.
                        path = await FindCachedAsync(cacheKey, cancellationToken).ConfigureAwait(false);
                        if (path == null)
                        {
                            using var track = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                            track.CancelAfter(TrackBudget);
                            byte[] payload;
                            if (candidate.Data != null)
                            {
                                if (StrictUtf8.GetByteCount(candidate.Data) > MaximumCaptionBytes)
                                    return null;
                                payload = StrictUtf8.GetBytes(candidate.Data);
                            }
                            else if (candidate.Url != null)
                                payload = await YouTubeDirectHttp.GetBytesAsync(candidate.Url, candidate.Headers,
                                    MaximumCaptionBytes, track.Token).ConfigureAwait(false);
                            else
                                return null;
                            if (!YouTubeCaptionNormalizer.TryNormalize(payload, out var normalized))
                                return null;
                            path = await StoreAsync(videoId, cacheKey, normalized, track.Token).ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        FetchGate.Release();
                    }
                }
                return path == null ? null : new MediaStream
                {
                    Type = MediaStreamType.Subtitle,
                    Index = index,
                    Codec = "vtt",
                    Language = candidate.Language,
                    Title = candidate.Title,
                    DisplayTitle = candidate.Title,
                    IsExternal = true,
                    IsDefault = false,
                    IsForced = false,
                    SupportsExternalStream = true,
                    Protocol = MediaProtocol.File,
                    Path = path
                };
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Includes per-track timeout and caller cancellation; the caller
                // rethrows its own cancellation only after all bounded work ends.
                return null;
            }
        }

        private static async Task<string?> FindCachedAsync(string key, CancellationToken cancellationToken)
        {
            await CacheGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var now = DateTime.UtcNow;
                if (!Cache.TryGetValue(key, out var entry) || now - entry.CachedUtc >= Retention)
                    return null;
                var info = new FileInfo(entry.Path);
                if (!info.Exists || IsLink(info) || info.Length > MaximumCaptionBytes
                    || !YouTubeCaptionNormalizer.IsCanonical(await File.ReadAllBytesAsync(entry.Path, cancellationToken).ConfigureAwait(false)))
                {
                    Cache.Remove(key);
                    return null;
                }
                File.SetLastWriteTimeUtc(entry.Path, now);
                Cache[key] = entry with { LastUseUtc = now };
                return entry.Path;
            }
            finally
            {
                CacheGate.Release();
            }
        }

        private static async Task<string?> StoreAsync(
            string videoId, string key, byte[] payload, CancellationToken cancellationToken)
        {
            await CacheGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            string? temporaryPath = null;
            try
            {
                if (string.IsNullOrEmpty(Plugin.CachePath))
                    return null;
                var directory = Path.Combine(Path.GetFullPath(Plugin.CachePath), "direct-captions-v1");
                Directory.CreateDirectory(directory);
                if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0)
                    return null;
                var now = DateTime.UtcNow;
                var keyHash = Hash(StrictUtf8.GetBytes(key));
                var contentHash = Hash(payload);
                var path = Path.Combine(directory, $"ytcap-{videoId}-{keyHash}-{contentHash}.vtt");
                if (File.Exists(path))
                {
                    var existing = new FileInfo(path);
                    if (IsLink(existing) || existing.Length != payload.Length
                        || Hash(await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false)) != contentHash)
                        return null; // Never overwrite a path potentially used by playback.
                }
                else
                {
                    if (!HasCapacity(directory, payload.Length, now))
                    {
                        YouTubeChannel.LogPublic("[YT] Direct subtitle cache full or active-state unknown; no active caption file was evicted.");
                        return null;
                    }
                    temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".part";
                    await using (var output = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write,
                        FileShare.None, 64 * 1024, FileOptions.Asynchronous))
                    {
                        await output.WriteAsync(payload.AsMemory(), cancellationToken).ConfigureAwait(false);
                        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
                    }
                    File.Move(temporaryPath, path, overwrite: false);
                    temporaryPath = null;
                }
                File.SetLastWriteTimeUtc(path, now);
                if (Cache.Count >= MaximumCacheFiles && !Cache.ContainsKey(key))
                {
                    var oldest = Cache.OrderBy(pair => pair.Value.LastUseUtc).First().Key;
                    Cache.Remove(oldest); // Memory eviction does not delete a file.
                }
                Cache[key] = new CacheEntry(path, now, now);
                return path;
            }
            finally
            {
                if (temporaryPath != null)
                {
                    try { File.Delete(temporaryPath); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
                CacheGate.Release();
            }
        }

        private static bool HasCapacity(string directory, int incomingBytes, DateTime now)
        {
            // Exact dedicated directory, no recursion and no removal of unknown
            // files. Inability to account for it fails closed for new captions.
            var files = new DirectoryInfo(directory).EnumerateFiles().Take(MaximumCacheFiles + 1).ToArray();
            if (files.Length > MaximumCacheFiles)
                return false;
            var activeVideos = TryGetActiveVideos();
            long bytes = 0;
            var count = 0;
            foreach (var file in files)
            {
                if (IsLink(file))
                    return false;
                var videoId = OwnedFileVideoId(file.Name);
                if (videoId != null && activeVideos != null && !activeVideos.Contains(videoId)
                    && now - file.LastWriteTimeUtc >= Retention)
                {
                    try
                    {
                        file.Delete();
                        continue;
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
                bytes = checked(bytes + file.Length);
                count++;
            }
            return count < MaximumCacheFiles && bytes + incomingBytes <= MaximumCacheBytes;
        }

        private static HashSet<string>? TryGetActiveVideos()
        {
            try
            {
                var manager = Plugin.ResolveService<ISessionManager>();
                if (manager == null)
                    return null; // Unknown means no deletion, not no playback.
                var result = new HashSet<string>(StringComparer.Ordinal);
                foreach (var session in manager.Sessions)
                {
                    var item = session.FullNowPlayingItem;
                    if (item == null)
                    {
                        if (session.NowPlayingItem != null)
                            return null;
                        continue;
                    }
                    var id = YouTubeImageProvider.TryGetVideoId(item);
                    if (!string.IsNullOrEmpty(id))
                        result.Add(id);
                }
                return result;
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                return null;
            }
        }

        internal static bool IsValidVtt(byte[] payload) => YouTubeCaptionNormalizer.TryNormalize(payload, out _);

        private static string CanonicalLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language) || language.Length > 100)
                return "und";
            try
            {
                var known = Plugin.ResolveService<ILocalizationManager>()?.FindLanguageInfo(language);
                if (!string.IsNullOrEmpty(known?.ThreeLetterISOLanguageName))
                    return known.ThreeLetterISOLanguageName.ToLowerInvariant();
            }
            catch (Exception ex) when (ex is not OutOfMemoryException) { }
            try
            {
                var culture = CultureInfo.GetCultureInfo(language);
                var iso = culture.ThreeLetterISOLanguageName;
                return iso.Length == 3 && iso != "ivl" ? iso.ToLowerInvariant() : "und";
            }
            catch (CultureNotFoundException) { return "und"; }
        }

        private static Dictionary<string, string> ReadQuery(Uri uri)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var part in uri.Query.TrimStart('?').Split('&').Take(128))
            {
                var separator = part.IndexOf('=');
                if (separator <= 0) continue;
                result[Uri.UnescapeDataString(part[..separator])] = Uri.UnescapeDataString(part[(separator + 1)..]);
            }
            return result;
        }

        private static JsonElement Property(JsonElement value, string name) =>
            value.ValueKind == JsonValueKind.Object && value.TryGetProperty(name, out var property) ? property : default;
        private static string? Text(JsonElement value, string name) =>
            Property(value, name) is var property && property.ValueKind == JsonValueKind.String ? property.GetString() : null;
        private static string CleanTitle(string value) =>
            new(value.Where(character => !char.IsControl(character)).Take(120).ToArray());
        private static string Hash(byte[] value) => Convert.ToHexString(SHA256.HashData(value)).ToLowerInvariant();
        private static bool IsLink(FileSystemInfo info) => (info.Attributes & FileAttributes.ReparsePoint) != 0;
        private static bool IsVideoId(string value) => value.Length == 11
            && value.All(character => character is >= 'a' and <= 'z' or >= 'A' and <= 'Z'
                or >= '0' and <= '9' or '-' or '_');
        private static string? OwnedFileVideoId(string name)
        {
            const int expectedLength = 6 + 11 + 1 + 64 + 1 + 64 + 4;
            if (name.Length != expectedLength || !name.StartsWith("ytcap-", StringComparison.Ordinal)
                || !name.EndsWith(".vtt", StringComparison.Ordinal) || name[17] != '-' || name[82] != '-')
                return null;
            var videoId = name.Substring(6, 11);
            return IsVideoId(videoId) && name.Substring(18, 64).All(IsHex) && name.Substring(83, 64).All(IsHex)
                ? videoId : null;
        }
        private static bool IsHex(char value) => value is >= '0' and <= '9' or >= 'a' and <= 'f';
    }
}
