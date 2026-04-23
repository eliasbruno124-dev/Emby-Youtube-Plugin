using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Re-supplies thumbnails for YouTube channel items after Emby's
    // "Refresh metadata → Replace existing metadata" wipes them.
    //
    // ChannelItemInfo.ImageUrl is only consumed when Emby first creates a
    // channel item; once metadata refresh clears the image rows, nothing
    // re-populates them unless an image provider claims the item. This
    // provider derives the YouTube thumbnail URL straight from the video ID
    // encoded in the item's path, so no extra YouTube Data API quota is
    // consumed.
    public class YouTubeImageProvider : IDynamicImageProvider, IHasItemChangeMonitor
    {
        public string Name => "YouTube";

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
            catch { }
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

            // Try maxresdefault first (often missing for older / less popular
            // videos), then fall back through sd → hq → mq → default.
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
                    if (bytes == null || bytes.Length < 1024) continue; // skip 1x1 placeholders
                    response.Format = ImageFormat.Jpg;
                    response.Stream = new MemoryStream(bytes, writable: false);
                    return response;
                }
                catch (OperationCanceledException) { throw; }
                catch { }
            }

            return response;
        }

        public bool HasChanged(BaseItem item, LibraryOptions libraryOptions, IDirectoryService directoryService)
        {
            // YouTube thumbnails for a given video ID are immutable; only
            // (re-)fetch when the item is missing both Primary and Thumb
            // (e.g. right after "Replace existing metadata" wiped them).
            try
            {
                if (!item.HasImage(ImageType.Primary) && !item.HasImage(ImageType.Thumb))
                    return true;
            }
            catch { }
            return false;
        }
    }
}
