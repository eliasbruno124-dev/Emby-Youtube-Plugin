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
        // Popular videos, optionally narrowed down by region or category.
        private static async Task<ChannelItemResult> LoadTrending(
            string apiKey, CancellationToken ct, string region = "", string category = "")
        {
            var allVideos = new List<ChannelItemInfo>();
            var seenIds = new HashSet<string>(StringComparer.Ordinal);

            var cfg = Plugin.Instance?.Options;
            const int target = 50;
            bool showShorts = cfg?.ShortsEnabled != false;
            bool isCategoryChild = !string.IsNullOrEmpty(category);

            void TryAdd(ChannelItemInfo v)
            {
                var rawId = StripPrefix(v.Id);
                if (!seenIds.Add(rawId)) return;
                if (!showShorts && v.Id.StartsWith(ReelPrefix, StringComparison.Ordinal)) return;
                allVideos.Add(v);
            }

            try
            {
                if (!isCategoryChild)
                {
                    // Each bucket is one cheap videos.list call (not search.list).
                    string?[] buckets = new string?[] { null, "10", "20", "24", "17", "22", "23", "28", "1" };
                    foreach (var cat in buckets)
                    {
                        if (allVideos.Count >= target) break;
                        ct.ThrowIfCancellationRequested();
                        string? reg = string.IsNullOrEmpty(region) ? null : region;
                        using var doc = await YouTubeApi.GetTrendingAsync(apiKey, reg, cat, ct)
                            .ConfigureAwait(false);
                        if (doc == null) continue;
                        foreach (var v in ExtractTrendingVideos(doc))
                        {
                            TryAdd(v);
                            if (allVideos.Count >= target) break;
                        }
                    }
                }
                else
                {
                    ct.ThrowIfCancellationRequested();
                    string? reg = string.IsNullOrEmpty(region) ? null : region;
                    using var doc = await YouTubeApi.GetTrendingAsync(apiKey, reg, category, ct)
                        .ConfigureAwait(false);
                    if (doc != null)
                    {
                        foreach (var v in ExtractTrendingVideos(doc))
                        {
                            TryAdd(v);
                            if (allVideos.Count >= target) break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Msg(new List<ChannelItemInfo>(), $"ERROR: {ex.Message}");
            }

            if (allVideos.Count == 0)
                return Msg(new List<ChannelItemInfo>(), "No results.");

            // Local Shorts upgrade via URL probe (no extra API quota).
            // ExtractTrendingVideos only catches Shorts via tag/hashtag, so this
            // flips the rest so the Shorts folder filter (and the user's
            // ShortsEnabled toggle) actually work in here too.
            try
            {
                await ApplyShortsProbeUpgradeAsync(allVideos, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log($"[YT] Trending Shorts upgrade failed: {ex.Message}");
            }

            // Re-apply the ShortsEnabled filter in case the probe just promoted
            // something to a Short while the user has Shorts off.
            if (!showShorts)
                allVideos.RemoveAll(v => v.Id.StartsWith(ReelPrefix, StringComparison.Ordinal));

            if (allVideos.Count == 0)
                return Msg(new List<ChannelItemInfo>(), "No results.");

            return new ChannelItemResult
            {
                Items = allVideos,
                TotalRecordCount = allVideos.Count
            };
        }

    }
}
