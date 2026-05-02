using MediaBrowser.Model.Plugins;
using System.Xml.Serialization;

namespace Emby.YouTubePlugin
{
    // Settings persisted by the custom configuration page.
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string ApiKey { get; set; } = "";

        public string SavedItems { get; set; } = "";

        public string WatchLaterPlaylist { get; set; } = "";

        public bool ShowTrending { get; set; } = true;

        public bool ShowCategories { get; set; } = true;

        public bool ShowRecentlyAdded { get; set; } = false;

        public bool ShowLiveFolders { get; set; } = false;

        public bool ShowShorts { get; set; } = true;

        // Kept around so older saved configs migrate cleanly.
        public bool HideShorts { get; set; } = false;

        [XmlIgnore]
        public bool ShortsEnabled => ShowShorts && !HideShorts;

        public string TrendingRegion { get; set; } = "";

        public string TrendingCategory { get; set; } = "";

        public bool ShowLikeCount { get; set; } = true;

        public bool ShowCommentCount { get; set; } = false;

        public string ChannelSortBy { get; set; } = "date";

        public int MaxChannelVideos { get; set; } = 50;

        public int MaxSearchVideos { get; set; } = 50;

        public int RecentlyAddedPerChannel { get; set; } = 10;

        public int WatchLaterPollMinutes { get; set; } = 3;

        public string Donate { get; set; } = "https://paypal.me/eliasbruno123";

        [XmlIgnore]
        public string QuotaStatus => QuotaTracker.FormatStatus();

        // Structured quota data so the settings UI can draw a real progress
        // bar instead of the ASCII version embedded in QuotaStatus.
        [XmlIgnore]
        public long QuotaUsedToday => QuotaTracker.GetStats().usedToday;

        [XmlIgnore]
        public long QuotaDailyLimit => QuotaTracker.GetStats().dailyQuota;

        [XmlIgnore]
        public long QuotaResetSeconds
        {
            get
            {
                var sec = (long)QuotaTracker.GetStats().untilReset.TotalSeconds;
                return sec < 0 ? 0 : sec;
            }
        }

        [XmlIgnore]
        public long QuotaLifetime => QuotaTracker.GetStats().totalUsed;
    }
}
