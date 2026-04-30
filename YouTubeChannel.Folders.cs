using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
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
    public partial class YouTubeChannel
    {
        private static async Task<ChannelItemResult> BuildRootItemsAsync(
            string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            var items = new List<ChannelItemInfo>();

            await AddWatchLaterFoldersAsync(items, apiKey, config, ct).ConfigureAwait(false);

            if (config.ShowTrending)
                items.Add(Folder("🔥 Trending", "trending_x_all", FolderIcons.Trending));

            if (config.ShowCategories)
                items.Add(Folder("📂 Categories", "categories_x_root", FolderIcons.Categories));

            if (config.ShowRecentlyAdded && !string.IsNullOrWhiteSpace(config.SavedItems))
                items.Add(Folder("🆕 Recently Added", "recent_x_all", FolderIcons.RecentlyAdded));

            await AddSavedContentFoldersAsync(items, apiKey, config, ct).ConfigureAwait(false);

            Log($"[YT] Root level: returning {items.Count} items");
            return ToResult(items);
        }

        private static async Task AddWatchLaterFoldersAsync(
            List<ChannelItemInfo> items, string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            var playlistIds = SplitConfiguredItems(config.WatchLaterPlaylist)
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            for (var index = 0; index < playlistIds.Count; index++)
            {
                ct.ThrowIfCancellationRequested();
                var playlistId = playlistIds[index];
                var details = await YouTubeApi.GetPlaylistDetailsAsync(apiKey, playlistId, ct)
                    .ConfigureAwait(false);
                if (details.videoCount <= 0)
                {
                    Log($"[YT] Skipping empty Watch Later playlist folder: {playlistId}");
                    continue;
                }

                var displayName = playlistIds.Count == 1
                    ? "\u2B50 " + (details.name ?? "Watch Later")
                    : "\u2B50 " + (details.name ?? $"Watch Later {index + 1}");

                items.Add(Folder(
                    displayName,
                    $"playlist{FolderSeparator}{playlistId}",
                    details.thumb ?? FolderIcons.WatchLater));
            }
        }

        private static async Task AddSavedContentFoldersAsync(
            List<ChannelItemInfo> items, string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            foreach (var term in SplitConfiguredItems(config.SavedItems))
            {
                ct.ThrowIfCancellationRequested();

                if (term.StartsWith(HandlePrefix))
                {
                    Log($"[YT] Loading handle: {term}");
                    var details = await YouTubeApi.GetChannelDetailsAsync(apiKey, term, true, ct)
                        .ConfigureAwait(false);
                    Log($"[YT] Handle {term} resolved to id={details.id}, name={details.name}");
                    items.Add(Folder(
                        details.name ?? term,
                        $"channel{FolderSeparator}{details.id ?? term}",
                        details.thumb));
                }
                else if (term.StartsWith(ChannelIdPrefix) && term.Length > MinChannelIdLength)
                {
                    var details = await YouTubeApi.GetChannelDetailsAsync(apiKey, term, false, ct)
                        .ConfigureAwait(false);
                    items.Add(Folder(
                        details.name ?? "Channel",
                        $"channel{FolderSeparator}{term}",
                        details.thumb));
                }
                else if (term.StartsWith(PlaylistPrefix))
                {
                    var details = await YouTubeApi.GetPlaylistDetailsAsync(apiKey, term, ct)
                        .ConfigureAwait(false);
                    if (details.videoCount <= 0)
                    {
                        Log($"[YT] Skipping empty saved playlist folder: {term}");
                        continue;
                    }

                    items.Add(Folder(
                        details.name ?? "Playlist",
                        $"playlist{FolderSeparator}{term}",
                        details.thumb));
                }
                else
                {
                    items.Add(Folder(
                        $"Search: {term}",
                        $"search{FolderSeparator}{term}",
                        FolderIcons.Search));
                }
            }
        }

        private static IEnumerable<string> SplitConfiguredItems(string? value) =>
            (value ?? string.Empty)
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0);

        private static ChannelItemInfo Folder(string name, string id, string? imageUrl) =>
            new()
            {
                Name = name,
                Id = id,
                Type = ChannelItemType.Folder,
                ImageUrl = imageUrl
            };

        private static async Task<ChannelItemResult> BuildChannelSubfoldersAsync(
            string apiKey, PluginConfiguration config, string term, CancellationToken ct)
        {
            var items = new List<ChannelItemInfo>();
            var channelThumb = (string?)null;
            var resolvedChannelId = term;

            try
            {
                var details = await YouTubeApi.GetChannelDetailsAsync(
                    apiKey, term, term.StartsWith(HandlePrefix), ct)
                    .ConfigureAwait(false);
                channelThumb = details.thumb;
                if (!string.IsNullOrEmpty(details.id))
                    resolvedChannelId = details.id!;
            }
            catch (Exception ex)
            {
                Log($"[YT] Channel details lookup failed for {term}: {ex.Message}");
            }

            var includeShorts = config.ShortsEnabled
                && await ChannelHasShortsAsync(apiKey, resolvedChannelId, ct).ConfigureAwait(false);
            var includeLive = config.ShowLiveFolders
                && await ChannelHasLiveAsync(apiKey, resolvedChannelId, ct).ConfigureAwait(false);

            // If the channel would only have a single "Videos" subfolder, skip the
            // wrapper level and return the videos directly so users don't have to
            // click through an empty parent folder containing one child.
            if (!includeShorts && !includeLive)
            {
                return await LoadMediaFolderAsync(apiKey, config, "channelvideos", resolvedChannelId, ct)
                    .ConfigureAwait(false);
            }

            items.Add(Folder("📺 Videos", $"channelvideos{FolderSeparator}{resolvedChannelId}", channelThumb ?? FolderIcons.Videos));

            if (includeShorts)
                items.Add(Folder("⚡ Shorts", $"channelshorts{FolderSeparator}{resolvedChannelId}", channelThumb ?? FolderIcons.Shorts));

            if (includeLive)
            {
                items.Add(Folder(
                    "🔴 Live & Upcoming",
                    $"channellive{FolderSeparator}{resolvedChannelId}",
                    channelThumb ?? FolderIcons.Live));
            }

            return ToResult(items);
        }

        private static async Task<ChannelItemResult> LoadCategoryRootAsync(
            string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            var region = string.IsNullOrWhiteSpace(config.TrendingRegion)
                ? "US"
                : config.TrendingRegion.Trim();

            var candidates = await GetAssignableCategoriesAsync(apiKey, region, ct)
                .ConfigureAwait(false);
            var nonEmpty = await ProbeNonEmptyCategoriesAsync(apiKey, region, candidates, config.ShortsEnabled, ct)
                .ConfigureAwait(false);

            var items = candidates
                .Where(c => nonEmpty.ContainsKey(c.id))
                .Select(c => Folder(c.name, $"category{FolderSeparator}{c.id}", FolderIcons.ForCategory(c.id)))
                .ToList();

            return items.Count == 0
                ? Msg(items, "No categories available.")
                : ToResult(items);
        }

        private static async Task<List<(string id, string name)>> GetAssignableCategoriesAsync(
            string apiKey, string region, CancellationToken ct)
        {
            using var catDoc = await YouTubeApi.GetVideoCategoriesAsync(apiKey, region, ct)
                .ConfigureAwait(false);
            var candidates = new List<(string id, string name)>();
            if (catDoc == null
                || !catDoc.RootElement.TryGetProperty("items", out var catItems)
                || catItems.ValueKind != JsonValueKind.Array)
                return candidates;

            foreach (var category in catItems.EnumerateArray())
            {
                var id = YouTubeApi.GetString(category, "id");
                var name = YouTubeApi.GetNestedString(category, "snippet", "title");
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name)) continue;
                if (category.TryGetProperty("snippet", out var snippet)
                    && snippet.TryGetProperty("assignable", out var assignable)
                    && assignable.ValueKind == JsonValueKind.False) continue;
                candidates.Add((id, NormalizeCategoryName(name)));
            }

            return candidates;
        }

        private static string NormalizeCategoryName(string name)
        {
            return string.Equals(name, "Autos & Vehicles", StringComparison.OrdinalIgnoreCase)
                ? "Cars & Vehicles"
                : name;
        }

        private static async Task<ConcurrentDictionary<string, byte>> ProbeNonEmptyCategoriesAsync(
            string apiKey,
            string region,
            List<(string id, string name)> candidates,
            bool showShorts,
            CancellationToken ct)
        {
            var nonEmpty = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
            using var probeSem = new SemaphoreSlim(6, 6);
            var probes = candidates.Select(async category =>
            {
                await probeSem.WaitAsync(ct).ConfigureAwait(false);
                try
                {
                    using var doc = await YouTubeApi.GetTrendingAsync(apiKey, region, category.id, ct)
                        .ConfigureAwait(false);
                    if (doc != null && AnyTrendingVideoSurvives(doc, showShorts))
                        nonEmpty.TryAdd(category.id, 1);
                }
                catch (Exception ex)
                {
                    Log($"[YT] Category probe failed for {category.id}: {ex.Message}");
                }
                finally
                {
                    probeSem.Release();
                }
            });

            try { await Task.WhenAll(probes).ConfigureAwait(false); }
            catch (Exception ex) { Log($"[YT] Category probes failed: {ex.Message}"); }

            return nonEmpty;
        }

        private static bool AnyTrendingVideoSurvives(JsonDocument doc, bool showShorts)
        {
            foreach (var video in ExtractTrendingVideos(doc))
            {
                if (!showShorts && video.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)) continue;
                return true;
            }

            return false;
        }
    }
}
