using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Thumbnails used to be handed to Emby as plain https URLs. Emby then
    // downloads them with its own HTTP client, which has no IPv4 fallback: on
    // hosts whose advertised IPv6 route is a black hole (common in Docker) every
    // image request times out while titles and lists (fetched by the plugin's
    // IPv4-first client) load fine. Result: a library without a single image.
    //
    // Emby has an official hook for images it should not download itself: an
    // image path of "emby://<key>" is resolved through the ILazyImageProvider
    // with that key. We point our images there and download them with the same
    // IPv4-first client as everything else. The real URL travels with the item
    // as a provider id, because the image path cannot hold it ('|' and '*' are
    // delimiters in Emby's image column).
    public sealed class YouTubeLazyImageProvider : ILazyImageProvider
    {
        internal const string ProviderKey = "youtubeplugin-thumb";
        internal const string LazyPath = "emby://" + ProviderKey;
        internal const string SourceProviderId = "YouTubeThumb";

        public string Key => ProviderKey;

        // A thumbnail that just failed is not retried on every image request:
        // each visible card would otherwise repeat up to six downloads (video
        // deleted) or hold a request open for the full timeout (network down).
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<long, DateTime> RetryAfter = new();
        private static readonly TimeSpan GoneCooldown = TimeSpan.FromMinutes(60);
        private static readonly TimeSpan UnreachableCooldown = TimeSpan.FromMinutes(2);
        private const int MaxRetryEntries = 5000;

        public async Task<DynamicImageResponse> GetImage(BaseItem item, ImageType type, CancellationToken cancellationToken)
        {
            var itemId = item?.InternalId ?? 0;
            if (itemId > 0 && RetryAfter.TryGetValue(itemId, out var retryAt))
            {
                if (DateTime.UtcNow < retryAt)
                    throw new IOException("YouTube thumbnail unavailable; retry later.");
                RetryAfter.TryRemove(itemId, out _);
            }

            var candidates = YouTubeLazyImage.GetCandidates(item).ToList();
            if (candidates.Count == 0)
            {
                YouTubeLazyImage.LogFailure($"item {item?.InternalId}", "no thumbnail source stored for this item");
                // Not ArgumentException/HttpException: Emby treats those as
                // "this image is gone" and deletes it from the item.
                throw new IOException("YouTube thumbnail source unknown.");
            }

            string lastReason = "no attempt";
            var reachable = true;
            foreach (var url in candidates)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var (bytes, reason, hostReachable) = await YouTubeLazyImage.DownloadAsync(url, cancellationToken).ConfigureAwait(false);
                if (bytes == null)
                {
                    lastReason = $"{url}: {reason}";
                    // Host unreachable: the other sizes live on the same host,
                    // trying them only multiplies the timeout.
                    if (!hostReachable) { reachable = false; break; }
                    continue;
                }

                var format = YouTubeLazyImage.DetectFormat(bytes);
                var path = YouTubeLazyImage.WriteTemp(bytes, format);
                YouTubeLazyImage.LogDelivered(itemId, bytes.Length, format);
                return new DynamicImageResponse { Path = path, Format = format };
            }

            if (itemId > 0)
            {
                if (RetryAfter.Count >= MaxRetryEntries) RetryAfter.Clear();
                RetryAfter[itemId] = DateTime.UtcNow + (reachable ? GoneCooldown : UnreachableCooldown);
            }
            YouTubeLazyImage.LogFailure($"item {item?.InternalId}", lastReason);
            throw new IOException("YouTube thumbnail download failed: " + lastReason);
        }
    }

    internal static class YouTubeLazyImage
    {
        private static readonly HttpClient Http = CreateHttp();
        private static int _failures;
        private static long _lastTempSweepTicks;

        private static HttpClient CreateHttp()
        {
            var client = new HttpClient(
                YouTubeHttpClientFactory.CreateHandler(
                    allowAutoRedirect: true,
                    automaticDecompression: System.Net.DecompressionMethods.All))
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
            // i.ytimg.com sometimes hands back a tiny placeholder when the
            // request doesn't look like a browser. A real User-Agent fixes that.
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent",
                "Mozilla/5.0 (compatible; EmbyYouTubePlugin/1.0)");
            return client;
        }

        internal static bool IsLazyPath(string? path) =>
            !string.IsNullOrEmpty(path)
            && path.StartsWith(YouTubeLazyImageProvider.LazyPath, StringComparison.OrdinalIgnoreCase);

        // True for a plain remote image URL (video thumbnails, channel avatars
        // and the folder icons from the OpenMoji CDN all count).
        internal static bool IsRemoteImageUrl(string? url) =>
            !string.IsNullOrEmpty(url)
            && Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

        private static bool IsVideoThumbnailHost(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && uri.Host.EndsWith("ytimg.com", StringComparison.OrdinalIgnoreCase);

        internal static string Encode(string url) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(url)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        internal static string? Decode(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            try
            {
                var s = value.Replace('-', '+').Replace('_', '/');
                s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
                var url = Encoding.UTF8.GetString(Convert.FromBase64String(s));
                return IsRemoteImageUrl(url) ? url : null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        // Called on everything the channel hands to Emby: the thumbnail URL
        // moves into a provider id and the image path becomes the lazy path.
        internal static void Apply(ChannelItemResult? result)
        {
            if (result?.Items == null) return;
            foreach (var info in result.Items)
                Apply(info);
        }

        internal static void Apply(ChannelItemInfo? info)
        {
            if (info == null || !IsRemoteImageUrl(info.ImageUrl)) return;
            info.ProviderIds ??= new ProviderIdDictionary();
            info.ProviderIds[YouTubeLazyImageProvider.SourceProviderId] = Encode(info.ImageUrl);
            info.ImageUrl = YouTubeLazyImageProvider.LazyPath;
        }

        internal static IEnumerable<string> GetCandidates(BaseItem? item)
        {
            if (item == null) yield break;
            var seen = new HashSet<string>(StringComparer.Ordinal);

            string? stored = null;
            try { stored = Decode(item.GetProviderId(YouTubeLazyImageProvider.SourceProviderId)); }
            catch (Exception) { stored = null; }
            if (stored != null && seen.Add(stored))
                yield return stored;

            var videoId = YouTubeImageProvider.TryGetVideoId(item);
            if (string.IsNullOrEmpty(videoId)) yield break;
            foreach (var url in YouTubeImageProvider.GetThumbnailCandidates(videoId))
            {
                if (seen.Add(url))
                    yield return url;
            }
        }

        internal static async Task<(byte[]? Bytes, string Reason, bool HostReachable)> DownloadAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                using var resp = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode)
                    return (null, $"HTTP {(int)resp.StatusCode}", true);
                var bytes = await resp.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                // i.ytimg.com answers with a tiny placeholder for some videos.
                if (bytes == null || bytes.Length < (IsVideoThumbnailHost(url) ? 1024 : 64))
                    return (null, $"placeholder ({bytes?.Length ?? 0} bytes)", true);
                return (bytes, "ok", true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (OperationCanceledException)
            {
                return (null, "timed out", false);
            }
            catch (Exception ex)
            {
                return (null, $"{ex.GetType().Name}: {ex.Message}", false);
            }
        }

        internal static ImageFormat DetectFormat(byte[] bytes)
        {
            if (bytes.Length >= 12)
            {
                if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                    return ImageFormat.Png;
                if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                    return ImageFormat.Gif;
                if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46
                    && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
                    return ImageFormat.Webp;
            }
            return ImageFormat.Jpg;
        }

        // Emby copies the file into the item's metadata folder and leaves ours
        // behind, so sweep old ones now and then.
        internal static string WriteTemp(byte[] bytes, ImageFormat format)
        {
            var dir = Path.Combine(Plugin.CachePath ?? Path.GetTempPath(), "lazy-thumbs");
            Directory.CreateDirectory(dir);
            SweepTemp(dir);
            var path = Path.Combine(dir, Guid.NewGuid().ToString("N") + "." + format.ToString().ToLowerInvariant());
            File.WriteAllBytes(path, bytes);
            return path;
        }

        private static void SweepTemp(string dir)
        {
            var now = DateTime.UtcNow.Ticks;
            var last = Interlocked.Read(ref _lastTempSweepTicks);
            if (now - last < TimeSpan.FromMinutes(5).Ticks
                || Interlocked.CompareExchange(ref _lastTempSweepTicks, now, last) != last)
                return;
            try
            {
                var cutoff = DateTime.UtcNow - TimeSpan.FromMinutes(10);
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    try
                    {
                        if (File.GetLastWriteTimeUtc(file) < cutoff)
                            File.Delete(file);
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static int _delivered;

        // Proof in the log that Emby asked for thumbnails and got them: the
        // first ones individually, then a running total.
        internal static void LogDelivered(long itemId, int bytes, ImageFormat format)
        {
            var n = Interlocked.Increment(ref _delivered);
            if (n <= 10)
                YouTubeChannel.LogPublic($"[YTIMG] Thumbnail delivered to Emby (item {itemId}, {bytes} bytes, {format}).");
            else if (n % 100 == 0)
                YouTubeChannel.LogPublic($"[YTIMG] Thumbnails delivered to Emby since start: {n}.");
        }

        // What the channel handed to Emby: how many entries came with a
        // thumbnail and how many without (those show as empty cards).
        internal static void LogHandover(string? folderId, ChannelItemResult? result)
        {
            if (result?.Items == null || result.Items.Count == 0) return;
            var without = result.Items.Count(info => string.IsNullOrEmpty(info.ImageUrl));
            if (without > 0)
                YouTubeChannel.LogPublic($"[YTIMG] Folder {(string.IsNullOrEmpty(folderId) ? "(root)" : folderId)}: {result.Items.Count} entries, {without} without a thumbnail source.");
        }

        // One line per start with the image state of everything the plugin
        // owns in the library; tells apart "no image stored", "not downloaded
        // yet" and "stored locally" without database access.
        internal static void LogInventory(ILibraryManager library, CancellationToken token)
        {
            int total = 0, local = 0, lazy = 0, plain = 0, none = 0, missingFile = 0;
            foreach (var channel in FindOwnChannels(library, token))
            {
                foreach (var item in library.GetItemList(new InternalItemsQuery
                {
                    AncestorIds = new[] { channel.InternalId },
                    Recursive = true,
                    EnableTotalRecordCount = false
                }, token))
                {
                    total++;
                    var image = item.GetImageInfo(ImageType.Primary, 0);
                    if (image == null || string.IsNullOrEmpty(image.Path)) none++;
                    else if (IsLazyPath(image.Path)) lazy++;
                    else if (!image.IsLocalFile) plain++;
                    else
                    {
                        local++;
                        try { if (!File.Exists(image.Path)) missingFile++; }
                        catch (Exception) { }
                    }
                }
            }
            YouTubeChannel.LogPublic($"[YTIMG] Library image state: {total} YouTube items; {local} stored locally"
                + (missingFile > 0 ? $" ({missingFile} files missing on disk)" : "")
                + $", {lazy} waiting for first display, {plain} with a plain URL, {none} without any image.");
        }

        // Thumbnail failures used to be invisible (Debug output only). Log the
        // first few in full and then every 50th, so a broken network shows up
        // in the plugin log without flooding it.
        internal static void LogFailure(string what, string reason)
        {
            var n = Interlocked.Increment(ref _failures);
            if (n <= 5 || n % 50 == 0)
                YouTubeChannel.LogPublic($"[YTIMG] Thumbnail download failed ({what}): {reason}. Failures since start: {n}.");
        }

        // Points an item's primary image at the lazy provider. Returns true
        // when the item changed and needs saving.
        internal static bool UseLazyPrimary(BaseItem item, string? sourceUrl)
        {
            // Keep a source the channel already stored (e.g. the API's own
            // thumbnail URL); the caller's URL is only a fallback.
            if (IsRemoteImageUrl(sourceUrl) && Decode(item.GetProviderId(YouTubeLazyImageProvider.SourceProviderId)) == null)
                item.SetProviderId(YouTubeLazyImageProvider.SourceProviderId, Encode(sourceUrl!));
            item.SetImage(new ItemImageInfo
            {
                Path = YouTubeLazyImageProvider.LazyPath,
                Type = ImageType.Primary,
                DateModified = DateTimeOffset.UtcNow
            }, 0, true);
            return true;
        }

        // Items created by older versions still carry the plain URL, which
        // Emby would keep downloading itself. Move them over once: a marker
        // file records completion so later starts skip the scan.
        internal static async Task<int> MigrateExistingAsync(ILibraryManager library, Func<bool> busy, CancellationToken token)
        {
            var marker = Plugin.CachePath == null ? null : Path.Combine(Plugin.CachePath, "lazy-thumbs-migrated-v1");
            if (marker != null && File.Exists(marker)) return 0;

            var channels = FindOwnChannels(library, token);
            // No channel yet (fresh install): everything created from now on
            // is lazy already, but a restored database could still appear, so
            // do not write the marker before a channel was actually scanned.
            if (channels.Length == 0) return 0;

            var ids = new List<long>();
            foreach (var channel in channels)
            {
                ids.AddRange(library.GetItemList(new InternalItemsQuery
                {
                    AncestorIds = new[] { channel.InternalId },
                    Recursive = true,
                    EnableTotalRecordCount = false
                }, token).Where(NeedsMigration).Select(item => item.InternalId));
            }

            int migrated = 0, failed = 0;
            foreach (var id in ids)
            {
                token.ThrowIfCancellationRequested();
                // Don't write to library.db while a channel refresh is
                // persisting items (SQLite "database is locked").
                while (busy())
                    await Task.Delay(TimeSpan.FromMilliseconds(500), token).ConfigureAwait(false);
                try
                {
                    // Saving writes the whole row. Load the item right before
                    // the write so a refresh or edit that happened while this
                    // loop was waiting is not overwritten with an old copy.
                    var item = library.GetItemById(id);
                    if (item == null || !NeedsMigration(item)) continue;
                    UseLazyPrimary(item, item.GetImageInfo(ImageType.Primary, 0).Path);
                    item.UpdateToRepository(ItemUpdateType.ImageUpdate);
                    migrated++;
                }
                catch (Exception ex) when (ex is not OutOfMemoryException)
                {
                    if (++failed <= 3)
                        YouTubeChannel.LogPublic($"[YTIMG] Thumbnail migration skipped item {id}: {ex.GetType().Name}.");
                }
                if (migrated % 50 == 49)
                    await Task.Delay(TimeSpan.FromMilliseconds(200), token).ConfigureAwait(false);
            }

            if (failed == 0 && marker != null)
            {
                try { File.WriteAllText(marker, DateTime.UtcNow.ToString("o")); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            else if (failed > 0)
            {
                YouTubeChannel.LogPublic($"[YTIMG] Thumbnail migration left {failed} items for the next start.");
            }
            return migrated;
        }

        private static bool NeedsMigration(BaseItem item)
        {
            var image = item.GetImageInfo(ImageType.Primary, 0);
            return image != null && !image.IsLocalFile && IsRemoteImageUrl(image.Path);
        }

        private static Channel[] FindOwnChannels(ILibraryManager library, CancellationToken token)
        {
            var manager = Plugin.ResolveService<IChannelManager>();
            var provider = manager?.GetType().GetMethod("GetChannelProvider",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, new[] { typeof(Channel) }, null);
            if (manager == null || provider == null)
                return Array.Empty<Channel>();
            return library.GetItemList(new InternalItemsQuery
            {
                IncludeItemTypes = new[] { "Channel" },
                Recursive = true,
                EnableTotalRecordCount = false
            }, token).OfType<Channel>()
                .Where(channel => provider.Invoke(manager, new object[] { channel }) is YouTubeChannel)
                .ToArray();
        }
    }
}
