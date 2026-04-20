using Emby.Web.GenericEdit;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Emby.YouTubePlugin
{
    public class PluginConfiguration : EditableOptionsBase
    {
        public override string EditorTitle => "YouTube Plugin Settings";
        public override string EditorDescription => QuotaTracker.FormatStatus();

        [DisplayName("YouTube Data API Key")]
        [Description("Your YouTube Data API v3 key. Get one at: https://console.cloud.google.com/apis/credentials. Daily quota: 10,000 units.")]
        public string ApiKey { get; set; } = "";

        [DisplayName("My YouTube Content")]
        [Description("Comma-separated list. Supports @Handle, UCxxxx (channel ID), PLxxxx (playlist), or any text (search).")]
        public string SavedItems { get; set; } = "";

        [DisplayName("Watch Later Playlist")]
        [Description("Playlist ID for the Watch Later folder. Polled regularly for new items.")]
        public string WatchLaterPlaylist { get; set; } = "";

        [DisplayName("Show Trending")]
        [Description("Show a Trending folder.")]
        public bool ShowTrending { get; set; } = true;

        [DisplayName("Show Categories Browser")]
        [Description("Show a Categories folder for browsing trending videos by YouTube category.")]
        public bool ShowCategories { get; set; } = true;

        [DisplayName("Show Recently Added")]
        [Description("Show a Recently Added folder mixing newest videos from all channels. NOTE: Videos in this folder also appear in their channel folder, which causes duplicates in Emby's 'Latest' view across the YouTube channel.")]
        public bool ShowRecentlyAdded { get; set; } = false;

        [DisplayName("Show Live Folders")]
        [Description("Show Live & Upcoming subfolder per channel. Costs 100 quota units when first opened (cached 12h afterwards).")]
        public bool ShowLiveFolders { get; set; } = false;

        [DisplayName("Hide Shorts")]
        [Description("Completely hide YouTube Shorts: no Shorts sub-folder per channel and Shorts are filtered out of Videos, Search, Trending and Recently Added.")]
        public bool HideShorts { get; set; } = false;

        [DisplayName("Trending Region")]
        [Description("ISO 3166-1 country code (DE, US, GB, etc.). Empty = default.")]
        public string TrendingRegion { get; set; } = "";

        [DisplayName("Trending Video Category")]
        [Description("YouTube category ID. 0=All, 10=Music, 20=Gaming, 24=Entertainment, 1=Film.")]
        public string TrendingCategory { get; set; } = "";

        [DisplayName("Show Like Count")]
        [Description("Include like counts in video descriptions.")]
        public bool ShowLikeCount { get; set; } = true;

        [DisplayName("Show Comment Count")]
        [Description("Include comment counts in video descriptions.")]
        public bool ShowCommentCount { get; set; } = false;

        [DisplayName("Sort Channel Videos By")]
        [Description("Sort order: date, viewCount, rating, relevance.")]
        public string ChannelSortBy { get; set; } = "date";

        [DisplayName("Max Videos per Channel or Playlist")]
        [Description("Maximum videos loaded per channel or playlist (1-150).")]
        [Range(1, 150)]
        public int MaxChannelVideos { get; set; } = 50;

        [DisplayName("Max Videos per Search Query")]
        [Description("Maximum videos loaded per search query (1-150).")]
        [Range(1, 150)]
        public int MaxSearchVideos { get; set; } = 50;

        [DisplayName("Recently Added: videos per channel")]
        [Description("How many newest videos to pull from each channel into Recently Added (1-25).")]
        [Range(1, 25)]
        public int RecentlyAddedPerChannel { get; set; } = 10;

        [DisplayName("Watch Later Poll Interval (minutes)")]
        [Description("How often to poll Watch Later for new videos (1-60). Each poll = 1 quota unit.")]
        [Range(1, 60)]
        public int WatchLaterPollMinutes { get; set; } = 3;
    }
}