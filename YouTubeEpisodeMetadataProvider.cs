using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public class YouTubeEpisodeMetadataProvider : ICustomMetadataProvider<Episode>, IForcedProvider
    {
        public string Name => "YouTube";

        public Task<ItemUpdateType> FetchAsync(
            MetadataResult<Episode> itemResult,
            MetadataRefreshOptions options,
            LibraryOptions libraryOptions,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseItem = itemResult?.BaseItem;
            var resultItem = itemResult?.Item;
            var videoId = YouTubeImageProvider.TryGetVideoId(baseItem)
                ?? YouTubeImageProvider.TryGetVideoId(resultItem);

            if (string.IsNullOrEmpty(videoId))
                return Task.FromResult(ItemUpdateType.None);

            if (!ShouldRestoreImage(baseItem, resultItem, options))
                return Task.FromResult(ItemUpdateType.None);

            var url = YouTubeImageProvider.GetStableThumbnailUrl(videoId);
            YouTubeImageProvider.EnsurePrimaryImage(baseItem, "metadata refresh");
            YouTubeImageProvider.EnsurePrimaryImage(resultItem, "metadata refresh result");

            if (itemResult != null)
            {
                itemResult.SearchImageUrl = url;
                itemResult.HasMetadata = true;
                itemResult.Provider = Name;
            }

            YouTubeChannel.LogPublic($"[YTIMG] Restored primary image for YouTube item {baseItem?.InternalId ?? 0} from {url}.");
            return Task.FromResult(ItemUpdateType.ImageUpdate);
        }

        private static bool ShouldRestoreImage(BaseItem? baseItem, BaseItem? resultItem, MetadataRefreshOptions options)
        {
            if (options.ReplaceAllImages || options.ReplaceThumbnailImages)
                return true;

            return baseItem?.HasImage(ImageType.Primary) != true
                && resultItem?.HasImage(ImageType.Primary) != true;
        }

    }
}
