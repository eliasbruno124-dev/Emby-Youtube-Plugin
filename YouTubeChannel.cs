using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public partial class YouTubeChannel : IChannel
    {
        private static void Log(string msg) => LogPublic(msg);
        public static void LogPublic(string msg)
        {
            var logger = Plugin.PluginLogger;
            if (logger != null)
                logger.Log(LogLevel.Information, 0, msg, null, (s, _) => s);
            else
                System.Diagnostics.Debug.WriteLine(msg);

            // Always tee to a plugin-owned file. Emby's ILoggerFactory does
            // not always surface plugin categories in embyserver.txt, and
            // without local logs we cannot diagnose client-specific issues
            // such as resume seeks misbehaving on the LG TV app.
            PluginFileLog.Write(msg);
        }

        public string Name => "YouTube";
        public string Description => "YouTube integration via official YouTube Data API v3.";
        public string Id => "youtube_channel_10";

        public string DataVersion => "1.0.0";
        public ChannelParentalRating ParentalRating => ChannelParentalRating.GeneralAudience;
        public bool IsEnabledByDefault => true;

        public ChannelFeatures GetChannelFeatures() => new ChannelFeatures();

        public async Task<ChannelItemResult> GetChannelItems(
            InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            var items = new List<ChannelItemInfo>();
            var plugin = Plugin.Instance;
            if (plugin == null)
                return Msg(items, "ERROR: Plugin not initialized.");

            var config = plugin.Options;
            var apiKey = (config.ApiKey ?? "").Trim();

            Log($"[YT] GetChannelItems called. FolderId={query.FolderId ?? "(root)"}, ApiKey={!string.IsNullOrEmpty(apiKey)}, SavedItems={config.SavedItems ?? "(empty)"}");

            // Mark the channel pipeline as active so the sort-name / image repair
            // queues pause their library.db writes. Covers refreshes started by
            // Emby's own scheduled task as well as plugin-triggered ones.
            ChannelRefreshInvoker.NoteChannelScanActivity();

            if (string.IsNullOrWhiteSpace(apiKey))
                return Msg(items, "ERROR: Please configure a YouTube API Key in the plugin settings.");

            try
            {
                // Root: build the visible folders.
                if (string.IsNullOrEmpty(query.FolderId))
                {
                    return await BuildRootItemsAsync(apiKey, config, cancellationToken)
                        .ConfigureAwait(false);
                }

                // Subfolder: load videos or drop one more level.
                if (query.FolderId.Contains(FolderSeparator))
                {
                    var sepIdx = query.FolderId.IndexOf(FolderSeparator, StringComparison.Ordinal);
                    if (sepIdx < 0) return new ChannelItemResult { Items = items };
                    string type = query.FolderId.Substring(0, sepIdx);
                    string term = query.FolderId.Substring(sepIdx + FolderSeparator.Length);

                    // Channel folders open into Videos / Shorts / Live.
                    if (type == "channel")
                    {
                        return await BuildChannelSubfoldersAsync(apiKey, config, term, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    if (type == "trending")
                    {
                        return await LoadTrending(apiKey, cancellationToken,
                            (config.TrendingRegion ?? "").Trim(),
                            (config.TrendingCategory ?? "").Trim())
                            .ConfigureAwait(false);
                    }

                    // Root of the categories browser.
                    if (type == "categories" && term == "root")
                    {
                        return await LoadCategoryRootAsync(apiKey, config, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    // A single category just hits the trending endpoint with a category filter.
                    if (type == "category")
                    {
                        var region = string.IsNullOrWhiteSpace(config.TrendingRegion) ? "US" : config.TrendingRegion.Trim();
                        return await LoadTrending(apiKey, cancellationToken, region, term).ConfigureAwait(false);
                    }

                    // Newest uploads across every saved channel.
                    if (type == "recent" && term == "all")
                    {
                        return await LoadRecentlyAdded(apiKey, config, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    return await LoadMediaFolderAsync(apiKey, config, type, term, cancellationToken)
                        .ConfigureAwait(false);
                }

                return new ChannelItemResult { Items = items };
            }
            catch (Exception ex)
            {
                Log($"[YT] GetChannelItems error: {ex}");
                return Msg(items, $"ERROR: {ex.Message}");
            }
        }

        public IEnumerable<ImageType> GetSupportedChannelImages() =>
            new List<ImageType> { ImageType.Thumb, ImageType.Primary };

        public Task<DynamicImageResponse> GetChannelImage(
            ImageType type, CancellationToken cancellationToken)
        {
            var response = new DynamicImageResponse();
            var t = GetType();
            var stream = t.Assembly.GetManifestResourceStream(t.Namespace + ".thumb.png");
            if (stream != null)
            {
                response.Format = ImageFormat.Png;
                response.Stream = stream;
                return Task.FromResult(response);
            }
            var assemblyDir = Path.GetDirectoryName(t.Assembly.Location) ?? "";
            var filePath = Path.Combine(assemblyDir, "thumb.png");
            if (File.Exists(filePath))
            {
                response.Format = ImageFormat.Png;
                response.Path = filePath;
            }
            return Task.FromResult(response);
        }

        private static ChannelItemResult Msg(List<ChannelItemInfo> items, string msg)
        {
            items.Add(new ChannelItemInfo
            {
                Name = msg,
                Id = "msg",
                Type = ChannelItemType.Folder
            });
            return new ChannelItemResult
            {
                Items = items,
                TotalRecordCount = items.Count
            };
        }

        private static ChannelItemResult ToResult(List<ChannelItemInfo> items) =>
            new()
            {
                Items = items,
                TotalRecordCount = items.Count
            };

    }
}
