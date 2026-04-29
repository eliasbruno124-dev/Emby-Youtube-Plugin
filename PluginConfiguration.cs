using Emby.Web.GenericEdit;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Emby.YouTubePlugin
{
    // This class holds the configuration for the YouTube plugin.
    public class PluginConfiguration : EditableOptionsBase
    {
        public override string EditorTitle => "YouTube Plugin Settings";
        public override string EditorDescription => QuotaTracker.FormatStatus();

        [DisplayName("YouTube Data API Key")]
        [Description("How to get a free key (≈2 min): "
            + "1) Open https://console.cloud.google.com/ and sign in with any Google account. "
            + "2) Top bar → 'Select a project' → 'New Project' → give it any name → Create. "
            + "3) Left menu → 'APIs & Services' → 'Library' → search 'YouTube Data API v3' → Enable. "
            + "4) Left menu → 'APIs & Services' → 'Credentials' → '+ Create Credentials' → 'API key'. "
            + "5) Copy the key shown and paste it here. "
            + "Free daily quota: 10,000 units (more than enough for normal use).")]
        public string ApiKey { get; set; } = "";

        [DisplayName("My YouTube Content")]
        [Description("Comma-separated. Accepts @Handle, channel ID (UCxxxx), playlist ID (PLxxxx) or any text (search).")]
        public string SavedItems { get; set; } = "";

        [DisplayName("Watch Later Playlists")]
        [Description("Comma-separated playlist IDs (PLxxxx). Each becomes its own folder and is polled regularly for new items (1 quota unit per playlist per poll).")]
        public string WatchLaterPlaylist { get; set; } = "";

        [DisplayName("Show Trending")]
        [Description("Show a Trending folder.")]
        public bool ShowTrending { get; set; } = true;

        [DisplayName("Show Categories Browser")]
        [Description("Show a Categories folder for browsing trending videos by YouTube category.")]
        public bool ShowCategories { get; set; } = true;

        [DisplayName("Show Recently Added")]
        [Description("Show a Recently Added folder mixing newest videos from all channels.")]
        public bool ShowRecentlyAdded { get; set; } = false;

        [DisplayName("Show Live Folders")]
        [Description("Show a Live & Upcoming subfolder per channel.")]
        public bool ShowLiveFolders { get; set; } = false;

        [DisplayName("Hide Shorts")]
        [Description("Filter Shorts out of all folders and hide the per-channel Shorts subfolder.")]
        public bool HideShorts { get; set; } = false;

        [DisplayName("Trending Region")]
        [Description("ISO 3166-1 country code (DE, US, GB, ...). Empty = YouTube default.")]
        public string TrendingRegion { get; set; } = "";

        [DisplayName("Trending Video Category")]
        [Description("YouTube category ID (0=All, 10=Music, 20=Gaming, 24=Entertainment, 1=Film).")]
        public string TrendingCategory { get; set; } = "";

        [DisplayName("Show Like Count")]
        [Description("Include like counts in video descriptions.")]
        public bool ShowLikeCount { get; set; } = true;

        [DisplayName("Show Comment Count")]
        [Description("Include comment counts in video descriptions.")]
        public bool ShowCommentCount { get; set; } = false;

        [DisplayName("Sort Channel Videos By")]
        [Description("date, viewCount, rating or relevance.")]
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
        [Description("How many newest videos to pull from each channel (1-25).")]
        [Range(1, 25)]
        public int RecentlyAddedPerChannel { get; set; } = 10;

        [DisplayName("Watch Later Poll Interval (minutes)")]
        [Description("How often to poll Watch Later playlists for new videos (1-60). Cost = 1 quota unit per playlist per poll.")]
        [Range(1, 60)]
        public int WatchLaterPollMinutes { get; set; } = 3;

        [DisplayName("❤️  Support the Developer")]
        [Description("This plugin is free and open-source — maintained with a lot of love in my spare time.\n"
            + "If it brings value to your Emby setup, a small donation would mean the world to me!\n\n"
            + "👉  paypal.me/eliasbruno123")]
        [ReadOnly(true)]
        public string Donate { get; set; } = "https://paypal.me/eliasbruno123";
    }
}