using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Emby can wipe channel thumbnails on a "Replace existing metadata" refresh.
    // This provider quietly puts them back.
    //
    // ChannelItemInfo.ImageUrl only helps when the item is first created. After
    // that, an image provider has to claim the item again. We rebuild the
    // thumbnail URL from the video ID in the item's path, so this doesn't cost
    // any YouTube Data API quota.
    public class YouTubeImageProvider : IDynamicImageProvider, IRemoteImageProvider, IHasItemChangeMonitor, IHasItemChangeWithItemResultMonitor
    {
        public string Name => "YouTube";

        // Remember failed thumbnail fetches briefly. Without this cooldown,
        // Emby can get stuck hammering the same missing image over and over
        // after a metadata refresh.
        // Key: item.InternalId. Value: UTC time of the failed attempt.
        // A successful fetch clears the entry so a later metadata wipe can try
        // again normally.
        private static readonly ConcurrentDictionary<long, DateTime> _failedFetches = new();
        private static readonly TimeSpan FailedFetchCooldown = TimeSpan.FromHours(2);

        private static readonly Regex VideoIdRegex = new(
            @"(?:youtube\.com/watch\?v=|youtu\.be/|/vi/)([A-Za-z0-9_-]{6,})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly HttpClient Http = CreateImageHttp();

        private static HttpClient CreateImageHttp()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            // i.ytimg.com sometimes hands back a tiny placeholder when the
            // request doesn't look like a browser. A real User-Agent fixes that.
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent",
                "Mozilla/5.0 (compatible; EmbyYouTubePlugin/1.0)");
            return client;
        }

        private const int MaxFailedFetches = 5000;

        internal static string? TryGetVideoId(BaseItem? item)
        {
            if (item == null) return null;
            try
            {
                // Identify our channel videos by the tuple Emby persists for
                // channel items: watch URL, matching YouTube provider id, and
                // an ExternalId derived from the raw video id. Trailer/preview
                // rows can also have YouTube watch paths and channel media
                // source data, but their ExternalId is usually the URL or
                // another library id.
                var path = item.Path;
                if (!string.IsNullOrEmpty(path))
                {
                    var m = VideoIdRegex.Match(path);
                    if (m.Success)
                    {
                        var videoId = m.Groups[1].Value;
                        var providerId = item.GetProviderId("YouTube");
                        if (string.Equals(providerId, videoId, StringComparison.Ordinal)
                            && IsPluginExternalId(item.ExternalId, videoId))
                        {
                            return videoId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeImageProvider] Could not inspect item: {ex.Message}");
            }
            return null;
        }

        private static bool IsPluginExternalId(string? externalId, string videoId)
        {
            return string.Equals(externalId, videoId, StringComparison.Ordinal)
                || string.Equals(externalId, "REEL_" + videoId, StringComparison.Ordinal)
                || string.Equals(externalId, "LIVE_" + videoId, StringComparison.Ordinal);
        }

        public bool Supports(BaseItem item) => TryGetVideoId(item) != null;

        public ImageType[] GetSupportedImages(BaseItem item) =>
            new[] { ImageType.Primary, ImageType.Thumb };

        IEnumerable<ImageType> IRemoteImageProvider.GetSupportedImages(BaseItem item) =>
            GetSupportedImages(item);

        public async Task<DynamicImageResponse> GetImage(
            BaseMetadataResult result, ImageType type, CancellationToken cancellationToken)
        {
            var response = new DynamicImageResponse();
            var videoId = TryGetVideoId(result?.BaseItem);
            if (string.IsNullOrEmpty(videoId)) return response;

            foreach (var url in GetThumbnailCandidates(videoId))
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    using var resp = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                        .ConfigureAwait(false);
                    if (!resp.IsSuccessStatusCode) continue;
                    var bytes = await resp.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                    if (bytes == null || bytes.Length < 1024) continue; // Ignore tiny placeholder images.
                    // We've got a usable thumbnail again. If the image ever
                    // gets wiped later, future refreshes can retry normally.
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

            // Nothing worked. Video might be gone, unavailable, or still
            // waiting for YouTube to generate its thumbnails.
            if (result?.BaseItem != null)
            {
                _failedFetches[result.BaseItem.InternalId] = DateTime.UtcNow;
                EvictFailedFetches();
            }

            return response;
        }

        private static void EvictFailedFetches()
        {
            if (_failedFetches.Count <= MaxFailedFetches) return;
            var cutoff = DateTime.UtcNow - FailedFetchCooldown;
            foreach (var kvp in _failedFetches)
            {
                if (kvp.Value < cutoff)
                    _failedFetches.TryRemove(kvp.Key, out _);
            }
            if (_failedFetches.Count > MaxFailedFetches)
            {
                var overflow = _failedFetches.Count - MaxFailedFetches;
                var oldest = _failedFetches
                    .OrderBy(kvp => kvp.Value)
                    .Take(overflow)
                    .Select(kvp => kvp.Key)
                    .ToList();
                foreach (var key in oldest)
                    _failedFetches.TryRemove(key, out _);
            }
        }

        public Task<IEnumerable<RemoteImageInfo>> GetImages(
            BaseItem item, LibraryOptions libraryOptions, CancellationToken cancellationToken)
        {
            var videoId = TryGetVideoId(item);
            if (string.IsNullOrEmpty(videoId))
                return Task.FromResult(Enumerable.Empty<RemoteImageInfo>());

            var url = GetStableThumbnailUrl(videoId);
            IEnumerable<RemoteImageInfo> images = new[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Url = url,
                    ThumbnailUrl = url,
                    Type = ImageType.Primary,
                    Width = 320,
                    Height = 180
                },
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Url = url,
                    ThumbnailUrl = url,
                    Type = ImageType.Thumb,
                    Width = 320,
                    Height = 180
                }
            };

            return Task.FromResult(images);
        }

        public async Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var response = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return new HttpResponseInfo(new IDisposable[] { response })
            {
                Content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false),
                ContentLength = response.Content.Headers.ContentLength,
                ContentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg",
                ResponseUrl = url,
                StatusCode = response.StatusCode,
                Headers = response.Headers.ToDictionary(
                    h => h.Key,
                    h => string.Join(",", h.Value),
                    StringComparer.OrdinalIgnoreCase)
            };
        }

        public bool HasChanged(BaseItem item, LibraryOptions libraryOptions, IDirectoryService directoryService)
        {
            // YouTube thumbnails are stable per video ID, so we only step in
            // when Emby has no stored image left.
            try
            {
                if (item.HasImage(ImageType.Primary) || item.HasImage(ImageType.Thumb))
                    return false;

                // Give missing thumbnails some breathing room before trying
                // again. Covers both brand-new uploads and permanently
                // unavailable videos without getting too chatty.
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

        public bool HasChanged(
            BaseMetadataResult itemResult,
            LibraryOptions libraryOptions,
            MetadataRefreshOptions options,
            IDirectoryService directoryService)
        {
            try
            {
                var item = itemResult?.BaseItem;
                if (string.IsNullOrEmpty(TryGetVideoId(item)))
                    return false;

                if (options.ReplaceAllImages || options.ReplaceThumbnailImages)
                {
                    YouTubeChannel.LogPublic($"[YTIMG] Forcing image refresh for item {item!.InternalId} after replace-images request.");
                    return true;
                }

                return item != null && HasChanged(item, libraryOptions, directoryService);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YouTubeImageProvider] Result change check failed: {ex.Message}");
            }

            return false;
        }

        internal static string GetStableThumbnailUrl(string videoId) =>
            $"https://i.ytimg.com/vi/{videoId}/mqdefault.jpg";

        internal static bool EnsurePrimaryImage(BaseItem? item, string reason)
        {
            var videoId = TryGetVideoId(item);
            if (item == null || string.IsNullOrEmpty(videoId))
                return false;

            if (item.HasImage(ImageType.Primary))
                return false;

            var url = GetStableThumbnailUrl(videoId);
            item.SetImage(new ItemImageInfo
            {
                Path = url,
                Type = ImageType.Primary,
                DateModified = DateTimeOffset.UtcNow,
                Width = 320,
                Height = 180
            }, 0, true);

            YouTubeChannel.LogPublic($"[YTIMG] Restored primary image for YouTube item {item.InternalId} after {reason}: {url}");
            return true;
        }

        private static IEnumerable<string> GetThumbnailCandidates(string videoId)
        {
            // mqdefault.jpg is the same stable URL we expose as a remote image.
            // Bigger variants are useful as a dynamic fallback, but lots of
            // videos never get maxres/sd thumbnails.
            yield return GetStableThumbnailUrl(videoId);
            yield return $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";
            yield return $"https://i.ytimg.com/vi/{videoId}/sddefault.jpg";
            yield return $"https://i.ytimg.com/vi/{videoId}/maxresdefault.jpg";
            yield return $"https://i.ytimg.com/vi/{videoId}/default.jpg";
        }
    }
}
