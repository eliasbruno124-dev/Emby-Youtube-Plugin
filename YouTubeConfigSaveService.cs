using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Services;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Emby Server 4.10.0.x (beta) regressed POST /Plugins/{guid}/Configuration:
    // its handler throws System.FormatException "Unrecognized Guid format" in
    // Emby.Api.PluginService.Post(UpdatePluginConfiguration) and returns HTTP 500
    // for EVERY plugin (confirmed against YouTube and TheIntroDB, both with valid
    // GUIDs). Reads still work; only saving is broken. This custom endpoint lets
    // the configuration page persist settings without touching Emby's broken
    // route, so settings remain saveable on the beta. The page auto-saves to it.
    [Route("/YouTubePlugin/SaveConfiguration", "POST", Summary = "Saves YouTube plugin configuration directly, bypassing Emby 4.10's broken PluginService endpoint.")]
    [Authenticated(Roles = "Admin")]
    public class SaveYouTubeConfiguration : IRequiresRequestStream, IReturn<SaveYouTubeConfigurationResult>
    {
        public Stream RequestStream { get; set; } = null!;
    }

    public class SaveYouTubeConfigurationResult
    {
        public bool Saved { get; set; }
    }

    public sealed class YouTubeConfigSaveService : IService, IRequiresRequest
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // Auto-save can fire saves in quick succession (and from multiple
        // dashboard tabs). Serialize the mutate-then-persist so two requests
        // can't interleave field writes on the single live Configuration.
        private static readonly object SaveLock = new();

        public IRequest Request { get; set; } = null!;

        public async Task<object> Post(SaveYouTubeConfiguration request)
        {
            var plugin = Plugin.Instance;
            if (plugin == null)
                throw new InvalidOperationException("YouTube plugin is not initialized.");

            // Read the body asynchronously: Emby 4.10's Kestrel host runs with
            // AllowSynchronousIO=false, so a synchronous stream read throws
            // "Synchronous operations are disallowed". The read is awaited
            // BEFORE the lock so no await sits inside the lock body.
            var dto = await ReadDtoAsync(request).ConfigureAwait(false);
            if (dto == null)
                throw new ArgumentException("Empty or invalid YouTube configuration payload.");

            lock (SaveLock)
            {
                var config = plugin.Options;

                // Only overwrite fields the page actually sent. Strings/numbers/bools
                // are nullable on the DTO so a partial payload can't wipe other values.
                if (dto.ApiKey != null) config.ApiKey = dto.ApiKey;
                if (dto.SavedItems != null) config.SavedItems = dto.SavedItems;
                if (dto.WatchLaterPlaylist != null) config.WatchLaterPlaylist = dto.WatchLaterPlaylist;
                if (dto.ShowTrending.HasValue) config.ShowTrending = dto.ShowTrending.Value;
                if (dto.ShowCategories.HasValue) config.ShowCategories = dto.ShowCategories.Value;
                if (dto.ShowRootFoldersAtTopLevel.HasValue) config.ShowRootFoldersAtTopLevel = dto.ShowRootFoldersAtTopLevel.Value;
                if (dto.ShowShorts.HasValue) config.ShowShorts = dto.ShowShorts.Value;
                if (dto.HideShorts.HasValue) config.HideShorts = dto.HideShorts.Value;
                if (dto.TrendingRegion != null) config.TrendingRegion = dto.TrendingRegion;
                if (dto.TrendingCategory != null) config.TrendingCategory = dto.TrendingCategory;
                if (dto.ShowLikeCount.HasValue) config.ShowLikeCount = dto.ShowLikeCount.Value;
                if (dto.ShowCommentCount.HasValue) config.ShowCommentCount = dto.ShowCommentCount.Value;
                if (dto.ChannelSortBy != null) config.ChannelSortBy = dto.ChannelSortBy;
                if (dto.MaxChannelVideos.HasValue) config.MaxChannelVideos = dto.MaxChannelVideos.Value;
                if (dto.MaxSearchVideos.HasValue) config.MaxSearchVideos = dto.MaxSearchVideos.Value;
                if (dto.WatchLaterPollMinutes.HasValue) config.WatchLaterPollMinutes = dto.WatchLaterPollMinutes.Value;
                if (dto.Donate != null) config.Donate = dto.Donate;

                // Reuse the plugin's own override: it normalizes/clamps the values,
                // persists to disk, and — only when a content field actually
                // changed — invalidates caches and kicks a channel refresh.
                plugin.SaveConfiguration();
            }

            YouTubeChannel.LogPublic("[YT] Configuration saved via plugin endpoint (Emby 4.10 PluginService 500 bypass).");
            return new SaveYouTubeConfigurationResult { Saved = true };
        }

        private static async Task<ConfigDto?> ReadDtoAsync(SaveYouTubeConfiguration request)
        {
            if (request.RequestStream == null)
                return null;

            // leaveOpen: the request body stream is owned by Emby's pipeline;
            // disposing it here could double-dispose on some server builds.
            using var reader = new StreamReader(
                request.RequestStream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);
            var json = await reader.ReadToEndAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<ConfigDto>(json, JsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"Malformed YouTube configuration payload: {ex.Message}", ex);
            }
        }

        private sealed class ConfigDto
        {
            public string? ApiKey { get; set; }
            public string? SavedItems { get; set; }
            public string? WatchLaterPlaylist { get; set; }
            public bool? ShowTrending { get; set; }
            public bool? ShowCategories { get; set; }
            public bool? ShowRootFoldersAtTopLevel { get; set; }
            public bool? ShowShorts { get; set; }
            public bool? HideShorts { get; set; }
            public string? TrendingRegion { get; set; }
            public string? TrendingCategory { get; set; }
            public bool? ShowLikeCount { get; set; }
            public bool? ShowCommentCount { get; set; }
            public string? ChannelSortBy { get; set; }
            public int? MaxChannelVideos { get; set; }
            public int? MaxSearchVideos { get; set; }
            public int? WatchLaterPollMinutes { get; set; }
            public string? Donate { get; set; }
        }
    }
}
