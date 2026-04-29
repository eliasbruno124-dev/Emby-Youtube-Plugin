using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Emby can wipe channel thumbnails during a "Replace existing metadata"
    // refresh. This provider quietly puts them back.
    //
    // ChannelItemInfo.ImageUrl only helps when the item is first created. After
    // that, an image provider has to claim the item again. We rebuild the
    // thumbnail URL from the video ID in the item path, so this does not spend
    // any YouTube Data API quota.
    public class YouTubeImageProvider : IDynamicImageProvider, IHasItemChangeMonitor
    {
        public string Name => "YouTube";

        // Remember failed thumbnail fetches for a short while. Without this
        // cooldown, Emby can get stuck asking for the same missing image over
        // and over after a metadata refresh.
        // Key: item.InternalId. Value: UTC time of the failed attempt.
        // A successful fetch clears the entry so a later metadata wipe can try
        // normally again.
        private static readonly ConcurrentDictionary<long, DateTime> _failedFetches = new();
        private static readonly TimeSpan FailedFetchCooldown = TimeSpan.FromHours(2);

        private static readonly Regex VideoIdRegex = new(
            @"(?:youtube\.com/watch\?v=|youtu\.be/|/vi/)([A-Za-z0-9_-]{6,})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

        private static string? TryGetVideoId(BaseItem? item)
        {
            if (item == null) return null;
            try
            {
                var path = item.Path;
                if (!string.IsNullOrEmpty(path))
                {
                    var m = VideoIdRegex.Match(path);
                    if (m.Success) return m.Groups[1].Value;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeImageProvider] Could not inspect item path: {ex.Message}");
            }
            return null;
        }

        public bool Supports(BaseItem item) => TryGetVideoId(item) != null;

        public ImageType[] GetSupportedImages(BaseItem item) =>
            new[] { ImageType.Primary, ImageType.Thumb };

        public async Task<DynamicImageResponse> GetImage(
            BaseMetadataResult result, ImageType type, CancellationToken cancellationToken)
        {
            var response = new DynamicImageResponse();
            var videoId = TryGetVideoId(result?.BaseItem);
            if (string.IsNullOrEmpty(videoId)) return response;

            // Start with the best-looking thumbnail and step down until YouTube
            // gives us a real image.
            string[] candidates =
            {
                $"https://i.ytimg.com/vi/{videoId}/maxresdefault.jpg",
                $"https://i.ytimg.com/vi/{videoId}/sddefault.jpg",
                $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg",
                $"https://i.ytimg.com/vi/{videoId}/mqdefault.jpg",
                $"https://i.ytimg.com/vi/{videoId}/default.jpg",
            };

            foreach (var url in candidates)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    using var resp = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                        .ConfigureAwait(false);
                    if (!resp.IsSuccessStatusCode) continue;
                    var bytes = await resp.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                    if (bytes == null || bytes.Length < 1024) continue; // Ignore tiny placeholder images.
                    // The item has a usable thumbnail again. Future refreshes
                    // can try normally if the image ever gets wiped.
                    if (result?.BaseItem != null)
                        _failedFetches.TryRemove(result.BaseItem.InternalId, out _);
                    response.Format = ImageFormat.Jpg;
                    response.Stream = new MemoryStream(bytes, writable: false);
                    return response;
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[YouTubeImageProvider] Thumbnail fetch failed for {url}: {ex.Message}");
                }
            }

            // Nothing worked. The video may be gone, unavailable, or still
            // waiting for YouTube to generate its thumbnails.
            if (result?.BaseItem != null)
                _failedFetches[result.BaseItem.InternalId] = DateTime.UtcNow;

            return response;
        }

        public bool HasChanged(BaseItem item, LibraryOptions libraryOptions, IDirectoryService directoryService)
        {
            // YouTube thumbnails are stable for a video ID, so only step in
            // when Emby has no stored image left.
            try
            {
                if (item.HasImage(ImageType.Primary) || item.HasImage(ImageType.Thumb))
                    return false;

                // Give missing thumbnails some breathing room before trying
                // again. This covers both brand-new uploads and permanently
                // unavailable videos without getting noisy.
                if (_failedFetches.TryGetValue(item.InternalId, out var lastFail)
                    && (DateTime.UtcNow - lastFail) < FailedFetchCooldown)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeImageProvider] Change check failed: {ex.Message}");
            }
            return false;
        }
    }
}
