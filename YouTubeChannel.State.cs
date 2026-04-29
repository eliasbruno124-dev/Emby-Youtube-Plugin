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
        private const string ChannelIdPrefix = "UC";
        private const int MinChannelIdLength = 20;
        private const string PlaylistPrefix = "PL";
        private const string HandlePrefix = "@";
        private const string FolderSeparator = "_x_";
        private const int MaxMetaCacheEntries = 2000;

        private record VideoMeta(
            string? Overview, DateTime? Premiere, int? Year,
            long? RuntimeTicks, string? ThumbUrl, DateTime CachedAt,
            string? OriginalLang = null);

        private static readonly ConcurrentDictionary<string, VideoMeta> MetaCache = new();
        private static readonly TimeSpan MetaCacheTtl = TimeSpan.FromDays(365);
        private static readonly TimeSpan NegativeCacheTtl = TimeSpan.FromHours(1);

        // Cross-folder deduplication:
        // The same YouTube video can appear in several places. To keep Emby's
        // Latest view readable, channel-style folders always keep their full
        // contents while aggregator folders can check what has already appeared.
        //
        // The seen set is intentionally short-lived. It resets after cache
        // invalidation or after ten minutes, so a manual or scheduled refresh
        // still gets a fresh pass.
        private static readonly ConcurrentDictionary<string, byte> CrossFolderSeen = new(StringComparer.Ordinal);
        private static DateTime _crossFolderSeenStartedAt = DateTime.UtcNow;
        private static readonly TimeSpan CrossFolderSeenMaxAge = TimeSpan.FromMinutes(10);

        public static void ResetCrossFolderSeen()
        {
            CrossFolderSeen.Clear();
            _crossFolderSeenStartedAt = DateTime.UtcNow;
        }

        private static string StripPrefix(string id)
        {
            if (id.StartsWith(LivePrefix, StringComparison.Ordinal)) return id.Substring(LivePrefix.Length);
            if (id.StartsWith(ReelPrefix, StringComparison.Ordinal)) return id.Substring(ReelPrefix.Length);
            return id;
        }

        // Channel folders should never hide their own videos. They only mark
        // the IDs they returned so aggregator folders can stay tidy later.
        private static void MarkAsSeen(IEnumerable<ChannelItemInfo> items)
        {
            if ((DateTime.UtcNow - _crossFolderSeenStartedAt) > CrossFolderSeenMaxAge)
                ResetCrossFolderSeen();
            foreach (var it in items)
                CrossFolderSeen.TryAdd(StripPrefix(it.Id), 1);
        }

        // Warm the seen set from cached uploads before aggregator folders run.
        // Emby does not always fetch folders in a predictable order, especially
        // after a restart. Pre-seeding keeps duplicate videos out of aggregator
        // views without hiding them from their real channel or playlist folder.
        private static async Task PreSeedChannelSeenAsync(
            string apiKey, PluginConfiguration config, CancellationToken ct)
        {
            try
            {
                if ((DateTime.UtcNow - _crossFolderSeenStartedAt) > CrossFolderSeenMaxAge)
                    ResetCrossFolderSeen();

                var savedItems = (config.SavedItems ?? "")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var channelIds = new List<string>();
                foreach (var raw in savedItems)
                {
                    var term = raw.Trim();
                    if (string.IsNullOrEmpty(term)) continue;
                    if (term.StartsWith(HandlePrefix))
                    {
                        var d = await YouTubeApi.GetChannelDetailsAsync(apiKey, term, true, ct).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(d.id)) channelIds.Add(d.id!);
                    }
                    else if (term.StartsWith(ChannelIdPrefix) && term.Length > MinChannelIdLength)
                    {
                        channelIds.Add(term);
                    }
                }

                // Watch Later playlists count as first-class folders too.
                var watchLater = (config.WatchLaterPlaylist ?? "")
                    .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).Where(s => s.Length > 2).ToList();

                async Task SeedFromDocAsync(JsonDocument? doc)
                {
                    await Task.CompletedTask;
                    if (doc == null) return;
                    if (!doc.RootElement.TryGetProperty("items", out var items)
                        || items.ValueKind != JsonValueKind.Array) return;
                    foreach (var el in items.EnumerateArray())
                    {
                        // playlistItems and videos.list expose the ID in different places.
                        string? vid = null;
                        if (el.TryGetProperty("snippet", out var sn)
                            && sn.TryGetProperty("resourceId", out var ri)
                            && ri.TryGetProperty("videoId", out var vidEl))
                            vid = vidEl.GetString();
                        if (string.IsNullOrEmpty(vid))
                            vid = YouTubeApi.GetString(el, "id");
                        if (!string.IsNullOrEmpty(vid))
                            CrossFolderSeen.TryAdd(vid!, 1);
                    }
                }

                foreach (var cid in channelIds)
                {
                    ct.ThrowIfCancellationRequested();
                    using var doc = await YouTubeApi.GetChannelVideosAsync(apiKey, cid, null, ct, "date")
                        .ConfigureAwait(false);
                    await SeedFromDocAsync(doc).ConfigureAwait(false);
                }
                foreach (var pid in watchLater)
                {
                    ct.ThrowIfCancellationRequested();
                    using var doc = await YouTubeApi.GetPlaylistVideosAsync(apiKey, pid, null, ct)
                        .ConfigureAwait(false);
                    await SeedFromDocAsync(doc).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Log($"[YT] PreSeedChannelSeenAsync failed: {ex.Message}");
            }
        }

        private const string LivePrefix = "LIVE_";
        private const string ReelPrefix = "REEL_";
    }
}
