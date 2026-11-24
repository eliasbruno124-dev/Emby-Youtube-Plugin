using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Genre folders inside the Music category (forum request: "how do I get
    // the Music category to show by genre"). YouTube's Music category has no
    // sub-genres, but search.list can filter by YouTube's fixed music topic
    // ids. Each genre folder lists the most viewed music videos of the last
    // 30 days in that topic for the configured region.
    //
    // Quota: one search.list call per genre, cached for a day like every
    // other search. Emby's channel refresh opens all folders, so the 14 genres
    // use 14 of the 100 daily search calls; the option is off by default.
    internal static class YouTubeMusicGenres
    {
        internal const string MusicCategoryId = "10";
        internal const string FolderType = "musicgenre";
        private static string IconCdn => FolderIcons.Cdn;

        // YouTube topic ids (YouTube Data API, "Music topics").
        // Folder ids use the short key; topic ids contain '/' and stay internal.
        internal static readonly (string Key, string Id, string Name, string Icon)[] Genres =
        {
            ("pop", "/m/064t9", "Pop", "1F3A4"),           // 🎤
            ("hiphop", "/m/0glt670", "Hip Hop", "1F3A7"),     // 🎧
            ("rock", "/m/06by7", "Rock", "1F3B8"),          // 🎸
            ("electronic", "/m/02lkt", "Electronic", "1F39B"),    // 🎛
            ("rnb", "/m/06j6l", "R&B", "1F3B7"),           // 🎷
            ("soul", "/m/0gywn", "Soul", "1F3B6"),          // 🎶
            ("country", "/m/01lyv", "Country", "1F920"),       // 🤠
            ("jazz", "/m/03_d0", "Jazz", "1F3BA"),          // 🎺
            ("classical", "/m/0ggq0m", "Classical", "1F3BB"),    // 🎻
            ("reggae", "/m/06cqb", "Reggae", "1F334"),        // 🌴
            ("latin", "/m/0g293", "Latin", "1F483"),         // 💃
            ("asian", "/m/028sqc", "Asian", "1F3EE"),        // 🏮
            ("independent", "/m/05rwpb", "Independent", "1F3BC"),  // 🎼
            ("christian", "/m/02mscn", "Christian", "1F54A"),    // 🕊
        };

        // Own top-level "Music" folder, used when Show Categories is off: the
        // genres must not depend on a second switch to become visible.
        internal const string RootFolderId = "musicgenres_x_root";

        internal static List<ChannelItemInfo> GenreFolders() => Genres.Select(g => new ChannelItemInfo
        {
            Name = "🎵 " + g.Name,
            Id = FolderType + YouTubeChannel.FolderSeparatorValue + g.Key,
            Type = ChannelItemType.Folder,
            FolderType = ChannelFolderType.Container,
            MediaType = MediaBrowser.Model.Channels.ChannelMediaType.Video,
            ImageUrl = IconCdn + g.Icon + ".png"
        }).ToList();

        internal static bool ShowsOwnRootFolder(PluginConfiguration config) => config.ShowMusicGenres && !config.ShowCategories;

        internal static ChannelItemResult RootResult()
        {
            var folders = GenreFolders();
            return new ChannelItemResult { Items = folders, TotalRecordCount = folders.Count };
        }

        internal static string? TopicFor(string? key) =>
            Genres.Where(g => string.Equals(g.Key, key, StringComparison.Ordinal)).Select(g => g.Id).FirstOrDefault();

        // Genre folders go first in the Music category, the category's trending
        // videos follow unchanged. Personal (per-user key) catalogs do not get
        // them: their searches are not disk-cached and would cost 14 extra
        // search calls per profile and day.
        internal static ChannelItemResult AddGenreFolders(string? categoryId, ChannelItemResult result, PluginConfiguration config)
        {
            if (categoryId != MusicCategoryId || !config.ShowMusicGenres || result?.Items == null)
                return result!;
            var folders = GenreFolders();
            // A bare message entry ("No results.") is dropped; the genres still work.
            var videos = result.Items.Where(i => !string.Equals(i.Id, "msg", StringComparison.Ordinal)).ToList();
            result.Items = folders.Concat(videos).ToList();
            result.TotalRecordCount = result.Items.Count;
            return result;
        }

        internal static async Task<JsonDocument?> SearchAsync(string apiKey, string key, PluginConfiguration config, CancellationToken ct)
        {
            var topicId = TopicFor(key);
            if (topicId == null) return null;
            var configured = string.IsNullOrWhiteSpace(config.TrendingRegion) ? "US" : config.TrendingRegion.Trim();
            var region = await YouTubeApi.ResolveContentRegionAsync(apiKey, configured, "US", ct).ConfigureAwait(false) ?? "US";
            // Rounded to the day so the URL (and with it the one-day cache key)
            // stays the same for a whole day.
            var today = DateTime.UtcNow.Date;
            string Since(DateTime day) => day.AddDays(-30).ToString("yyyy-MM-dd'T'00:00:00'Z'", System.Globalization.CultureInfo.InvariantCulture);
            var fresh = await YouTubeApi.SearchMusicTopicAsync(apiKey, topicId, region, Since(today), ct).ConfigureAwait(false);
            if (fresh != null) return fresh;
            // The date in the URL changes at midnight UTC. If today's search
            // fails (search quota used up until about 08:00 UTC, outage), keep
            // the last answer of the previous days instead of an empty folder,
            // which would make Emby delete the genre's videos.
            for (var back = 1; back <= 7; back++)
            {
                var stale = YouTubeApi.TryReadStaleCopy(YouTubeApi.MusicTopicUrl(apiKey, topicId, region, Since(today.AddDays(-back))));
                if (stale != null) return stale;
            }
            return null;
        }
    }
}
