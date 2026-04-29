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
        private static List<MediaSourceInfo> MakeMediaSources(string videoId, bool isLive = false, long? runTimeTicks = null, string? originalLang = null)
        {
            // Keep runTimeTicks and originalLang in the signature for callers,
            // but do not push them into MediaSourceInfo. Setting RunTimeTicks
            // makes Emby Web treat the watch page like a raw stream and it can
            // hang. Leaving it unset lets the client use YouTube's embed player.
            string url = $"https://www.youtube.com/watch?v={videoId}";
            return new List<MediaSourceInfo>
            {
                new MediaSourceInfo
                {
                    Id = videoId,
                    Path = url,
                    Protocol = MediaProtocol.Http,
                    IsRemote = false,
                    SupportsTranscoding = false,
                    SupportsDirectStream = false,
                    SupportsDirectPlay = true,
                    IsInfiniteStream = isLive,
                    RequiresOpening = false,
                    RequiresClosing = false,
                    RequiresLooping = false,
                }
            };
        }

        private static List<ChannelItemInfo> ExtractTrendingVideos(JsonDocument doc)
        {
            var list = new List<ChannelItemInfo>();
            if (!doc.RootElement.TryGetProperty("items", out var items)
                || items.ValueKind != JsonValueKind.Array)
                return list;

            // Use the API's region metadata instead of probing the watch page.
            // Only apply this when the user chose a region; otherwise we cannot
            // safely guess where the server is.
            var serverRegion = (Plugin.Instance?.Options.TrendingRegion ?? "").Trim();

            foreach (var el in items.EnumerateArray())
            {
                var videoId = YouTubeApi.GetString(el, "id");
                if (string.IsNullOrWhiteSpace(videoId)) continue;

                // Drop videos blocked in the selected region before building items.
                if (!string.IsNullOrEmpty(serverRegion)
                    && el.TryGetProperty("contentDetails", out var cdRR)
                    && cdRR.ValueKind == JsonValueKind.Object
                    && cdRR.TryGetProperty("regionRestriction", out var rrEl)
                    && rrEl.ValueKind == JsonValueKind.Object)
                {
                    bool blocked = false;
                    if (rrEl.TryGetProperty("blocked", out var bl)
                        && bl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var b in bl.EnumerateArray())
                            if (string.Equals(b.GetString(), serverRegion, StringComparison.OrdinalIgnoreCase))
                            { blocked = true; break; }
                    }
                    if (!blocked
                        && rrEl.TryGetProperty("allowed", out var al)
                        && al.ValueKind == JsonValueKind.Array)
                    {
                        bool inAllowed = false;
                        foreach (var a in al.EnumerateArray())
                            if (string.Equals(a.GetString(), serverRegion, StringComparison.OrdinalIgnoreCase))
                            { inAllowed = true; break; }
                        if (!inAllowed) blocked = true;
                    }
                    if (blocked) continue;
                }

                var title = YouTubeApi.GetNestedString(el, "snippet", "title") ?? "Untitled";
                var author = YouTubeApi.GetNestedString(el, "snippet", "channelTitle") ?? "Unknown";
                var desc = YouTubeApi.GetNestedString(el, "snippet", "description");
                var pubStr = YouTubeApi.GetNestedString(el, "snippet", "publishedAt");
                var premiere = YouTubeApi.ParsePublishedAt(pubStr);
                var thumb = YouTubeApi.GetStableVideoThumbnailUrl(
                    videoId,
                    YouTubeApi.GetBestThumbnail(el));

                // Used as a harmless language hint for the YouTube watch URL.
                var origLang = YouTubeApi.GetNestedString(el, "snippet", "defaultAudioLanguage")
                            ?? YouTubeApi.GetNestedString(el, "snippet", "defaultLanguage");

                // Runtime.
                var duration = YouTubeApi.GetNestedString(el, "contentDetails", "duration");
                var ts = YouTubeApi.ParseDuration(duration);

                // Optional engagement stats for the item overview.
                long? viewCount = null;
                long? likeCount = null;
                long? commentCount = null;
                if (el.TryGetProperty("statistics", out var stats))
                {
                    if (long.TryParse(YouTubeApi.GetString(stats, "viewCount"), out var v)) viewCount = v;
                    if (long.TryParse(YouTubeApi.GetString(stats, "likeCount"), out var l)) likeCount = l;
                    if (long.TryParse(YouTubeApi.GetString(stats, "commentCount"), out var c)) commentCount = c;
                }

                var statsParts = new List<string>();
                if (viewCount.HasValue) statsParts.Add($"👁 {viewCount:N0}");
                if (likeCount.HasValue && Plugin.Instance?.Options.ShowLikeCount == true)
                    statsParts.Add($"👍 {likeCount:N0}");
                if (commentCount.HasValue && Plugin.Instance?.Options.ShowCommentCount == true)
                    statsParts.Add($"💬 {commentCount:N0}");
                string statsLine = statsParts.Count > 0 ? string.Join("  ·  ", statsParts) : "";

                string? overview = null;
                if (!string.IsNullOrWhiteSpace(desc))
                    overview = (statsLine.Length > 0 ? statsLine + "\n\n" : "") + desc;
                else if (statsLine.Length > 0)
                    overview = statsLine;

                // Mark currently live videos early so folder filters can see them.
                bool isLive = false;
                if (el.TryGetProperty("liveStreamingDetails", out var lsd))
                {
                    var concurrentViewers = YouTubeApi.GetString(lsd, "concurrentViewers");
                    if (!string.IsNullOrEmpty(concurrentViewers))
                        isLive = true;
                }

                // Detect Shorts with the same explicit signals used during enrichment.
                // Duration alone is unreliable — many normal videos are under three
                // minutes, so we only mark a video as a Short when YouTube tags it
                // or the creator added a #shorts hashtag.
                bool isReel = false;
                if (!isLive && el.TryGetProperty("snippet", out var snipForReel))
                {
                    if (snipForReel.TryGetProperty("tags", out var tagsEl)
                        && tagsEl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var tag in tagsEl.EnumerateArray())
                        {
                            var t = tag.GetString();
                            if (t != null && string.Equals(t.Trim(), "shorts", StringComparison.OrdinalIgnoreCase))
                            {
                                isReel = true;
                                break;
                            }
                        }
                    }
                    if (!isReel)
                    {
                        var sTitle = YouTubeApi.GetString(snipForReel, "title") ?? "";
                        var sDesc = YouTubeApi.GetString(snipForReel, "description") ?? "";
                        if (sTitle.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0
                         || sDesc.IndexOf("#shorts", StringComparison.OrdinalIgnoreCase) >= 0)
                            isReel = true;
                    }
                }

                string itemId = isLive ? LivePrefix + videoId
                    : isReel ? ReelPrefix + videoId
                    : videoId;
                string displayTitle = isLive ? $"🔴 LIVE: {title}"
                    : isReel ? $"▶ Short: {title}"
                    : title;

                var info = new ChannelItemInfo
                {
                    Name = displayTitle,
                    SeriesName = author,
                    Studios = new List<string> { author },
                    Overview = overview,
                    ProductionYear = premiere?.Year,
                    DateCreated = premiere,
                    PremiereDate = premiere,
                    RunTimeTicks = isLive ? null : ts?.Ticks,
                    ContentType = MediaBrowser.Model.Channels.ChannelMediaContentType.Episode,
                    Id = itemId,
                    Type = ChannelItemType.Media,
                    MediaType = MediaBrowser.Model.Channels.ChannelMediaType.Video,
                    ImageUrl = thumb,
                    // Use the YouTube video ID as the external ID so Emby can
                    // recognize the same video across multiple folders.
                    ProviderIds = new MediaBrowser.Model.Entities.ProviderIdDictionary { ["YouTube"] = videoId },
                    MediaSources = MakeMediaSources(videoId, isLive, isLive ? null : ts?.Ticks, origLang)
                };

                MetaCache[itemId] = new VideoMeta(overview, premiere, premiere?.Year, ts?.Ticks, thumb, DateTime.UtcNow, origLang);
                list.Add(info);
            }
            return list;
        }

        // Converts search, channel, and playlist API responses into Emby items.
        private static List<ChannelItemInfo> ExtractVideos(JsonDocument doc, bool isPlaylist = false)
        {
            var list = new List<ChannelItemInfo>();
            if (!doc.RootElement.TryGetProperty("items", out var items)
                || items.ValueKind != JsonValueKind.Array)
                return list;

            foreach (var el in items.EnumerateArray())
            {
                string? videoId = null;

                if (isPlaylist)
                {
                    videoId = YouTubeApi.GetNestedString(el, "contentDetails", "videoId")
                              ?? YouTubeApi.GetNestedString(el, "snippet", "resourceId.videoId");

                    // playlistItems keeps the video ID under snippet.resourceId.
                    if (string.IsNullOrEmpty(videoId)
                        && el.TryGetProperty("snippet", out var snip)
                        && snip.TryGetProperty("resourceId", out var rid))
                    {
                        videoId = YouTubeApi.GetString(rid, "videoId");
                    }
                }
                else
                {
                    // search.list keeps the video ID under id.videoId.
                    if (el.TryGetProperty("id", out var idProp))
                    {
                        if (idProp.ValueKind == JsonValueKind.Object)
                            videoId = YouTubeApi.GetString(idProp, "videoId");
                        else if (idProp.ValueKind == JsonValueKind.String)
                            videoId = idProp.GetString();
                    }
                }

                if (string.IsNullOrWhiteSpace(videoId)) continue;

                var title = YouTubeApi.GetNestedString(el, "snippet", "title") ?? "Untitled";
                var author = YouTubeApi.GetNestedString(el, "snippet", "channelTitle") ?? "Unknown";
                var desc = YouTubeApi.GetNestedString(el, "snippet", "description");
                var pubStr = YouTubeApi.GetNestedString(el, "snippet", "publishedAt");
                var premiere = YouTubeApi.ParsePublishedAt(pubStr);
                var thumb = YouTubeApi.GetStableVideoThumbnailUrl(
                    videoId,
                    YouTubeApi.GetBestThumbnail(el));

                // Add a live badge when the lightweight snippet already knows it.
                var liveBroadcastContent = YouTubeApi.GetNestedString(el, "snippet", "liveBroadcastContent");
                bool isLive = liveBroadcastContent == "live" || liveBroadcastContent == "upcoming";

                string? overview = !string.IsNullOrWhiteSpace(desc) ? desc : null;

                string itemId = isLive ? LivePrefix + videoId : videoId;
                string displayTitle = isLive ? $"🔴 LIVE: {title}" : title;

                var info = new ChannelItemInfo
                {
                    Name = displayTitle,
                    SeriesName = author,
                    Studios = new List<string> { author },
                    Overview = overview,
                    ProductionYear = premiere?.Year,
                    DateCreated = premiere,
                    PremiereDate = premiere,
                    ContentType = MediaBrowser.Model.Channels.ChannelMediaContentType.Episode,
                    Id = itemId,
                    Type = ChannelItemType.Media,
                    MediaType = MediaBrowser.Model.Channels.ChannelMediaType.Video,
                    ImageUrl = thumb,
                    ProviderIds = new MediaBrowser.Model.Entities.ProviderIdDictionary { ["YouTube"] = videoId },
                    MediaSources = MakeMediaSources(videoId, isLive,
                        MetaCache.TryGetValue(itemId, out var __m) ? __m.RuntimeTicks : null,
                        __m?.OriginalLang)
                };

                list.Add(info);
            }
            return list;
        }

        private static int ClampVideos(int val) => Math.Clamp(val, 1, 150);

        private static int ClampSearchVideos(int val) => Math.Clamp(val, 1, 50);
    }
}
