using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // This class restores thumbnails for YouTube channel items after Emby's
    // "Refresh metadata → Replace existing metadata" wipes them out.
    //
    // ChannelItemInfo.ImageUrl is only used when Emby first creates a channel item.
    // After a metadata refresh clears the image fields, nothing brings them back unless an image provider claims the item.
    // This provider rebuilds the YouTube thumbnail URL directly from the video ID in the item's path, so it doesn't use any extra YouTube Data API quota.
    public class YouTubeImageProvider : IDynamicImageProvider, IHasItemChangeMonitor
    {
        public string Name => "YouTube";

        // If a thumbnail fetch fails for an item, we wait before trying again.
        // This prevents a loop where Episode-Refresh wipes the image, HasChanged fires, fetch fails again, and the item keeps showing missing thumbnails every hour.
        // Key = item.InternalId, Value = UTC time of the failed attempt.
        // On a successful fetch, we remove the entry so a future metadata wipe (like from Episode Refresh) will trigger a fresh download.
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

            // Try maxresdefault first (it's often missing for older or less popular videos), then fall back through sd, hq, mq, and finally default.
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
                    // Success: clear any previous failure record so a future metadata wipe (like Episode Refresh) triggers a fresh download.
                    if (result?.BaseItem != null)
                        _failedFetches.TryRemove(result.BaseItem.InternalId, out _);
                    response.Format = ImageFormat.Jpg;
                    response.Stream = new MemoryStream(bytes, writable: false);
                    return response;
                }
                catch (OperationCanceledException) { throw; }
                catch { }
            }

            // If all candidates fail (video deleted, unavailable, or thumbnail not generated yet), record the failure so HasChanged won't re-trigger for another 2 hours.
            if (result?.BaseItem != null)
                _failedFetches[result.BaseItem.InternalId] = DateTime.UtcNow;

            return response;
        }

        public bool HasChanged(BaseItem item, LibraryOptions libraryOptions, IDirectoryService directoryService)
        {
            // YouTube thumbnails never change for a given video ID, so only fetch again when both image slots are empty (like right after "Replace existing metadata" wiped them).
            // Once Emby stores a valid image, HasImage returns true and we never re-trigger.
            try
            {
                if (item.HasImage(ImageType.Primary) || item.HasImage(ImageType.Thumb))
                    return false;

                // Don't keep hitting YouTube for items whose last fetch failed recently.
                // A 2-hour cooldown covers both "YouTube still processing the thumbnail for a brand-new upload" and "video permanently unavailable" cases, but still allows recovery after an Episode-Refresh wipe.
                if (_failedFetches.TryGetValue(item.InternalId, out var lastFail)
                    && (DateTime.UtcNow - lastFail) < FailedFetchCooldown)
                    return false;

                return true;
            }
            catch { }
            return false;
        }
    }
}
