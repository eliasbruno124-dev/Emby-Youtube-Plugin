using Emby.Web.GenericEdit;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Emby.YouTubePlugin
{
    public class PluginConfiguration : EditableOptionsBase
    {
        public override string EditorTitle => "YouTube Plugin Settings";

        // ─────────────────────────────────────────────────────────────────────
        // CONNECTION
        // ─────────────────────────────────────────────────────────────────────

        [DisplayName("YouTube Data API Key")]
        [Description(
            "Your YouTube Data API v3 key.\n" +
            "Get one at: https://console.cloud.google.com/apis/credentials\n" +
            "Enable 'YouTube Data API v3' in the Google Cloud Console.")]
        public string ApiKey { get; set; } = "";

        // ─────────────────────────────────────────────────────────────────────
        // CONTENT SOURCES
        // ─────────────────────────────────────────────────────────────────────

        [DisplayName("My YouTube Content")]
        [Description(
            "Comma-separated list of content sources. Three types are supported:\n" +
            "  • @Handle  — a YouTube channel handle (e.g. @GitHub)\n" +
            "  • UCxxxxxx — a YouTube channel ID (e.g. UCVHFbw7woebKtYXvKgG-Z6Q)\n" +
            "  • PLxxxxxx — a playlist ID (e.g. PL0lo9MOBetEFcp4SCWinBdpml9B2U25-f)\n" +
            "  • any text — a search query (e.g. Linux Tutorials)\n\n" +
            "Example: @GitHub, PLxxxxxx, Linux Tutorials")]
        public string SavedItems { get; set; } = "";

        [DisplayName("Watch Later Playlist")]
        [Description(
            "Playlist ID for a '⭐ Watch Later' folder with live refresh (~10 seconds).\n" +
            "Add videos to this playlist on YouTube and they appear in Emby within ~15 seconds.\n" +
            "Only the YouTube channel is refreshed — other channels stay untouched.\n" +
            "Example: PLxxxxxx")]
        public string WatchLaterPlaylist { get; set; } = "";

        // ─────────────────────────────────────────────────────────────────────
        // DISCOVER
        // ─────────────────────────────────────────────────────────────────────

        [DisplayName("Show Trending")]
        [Description("Show a 'Trending' folder with trending and popular videos from YouTube.")]
        public bool ShowTrending { get; set; } = true;

        [DisplayName("Trending Region")]
        [Description(
            "ISO 3166-1 country code for YouTube trending results.\n" +
            "Examples: DE (Germany), US (USA), AT (Austria), CH (Switzerland), GB (UK), FR (France).\n" +
            "Leave empty for default.")]
        public string TrendingRegion { get; set; } = "";

        [DisplayName("Trending Video Category")]
        [Description(
            "YouTube video category ID for trending results.\n" +
            "Common IDs: 0 (All), 10 (Music), 20 (Gaming), 24 (Entertainment), 1 (Film).\n" +
            "Leave empty or 0 for all categories.")]
        public string TrendingCategory { get; set; } = "";

        // ─────────────────────────────────────────────────────────────────────
        // SORTING
        // ─────────────────────────────────────────────────────────────────────

        [DisplayName("Sort Channel Videos By")]
        [Description(
            "How to sort videos when browsing a channel.\n" +
            "Options: date, viewCount, rating, relevance")]
        public string ChannelSortBy { get; set; } = "date";

        // ─────────────────────────────────────────────────────────────────────
        // LIMITS
        // ─────────────────────────────────────────────────────────────────────

        [DisplayName("Max Videos per Channel or Playlist")]
        [Description("Maximum number of videos loaded per channel or playlist. Range: 1–150.")]
        [Range(1, 150)]
        public int MaxChannelVideos { get; set; } = 50;

        [DisplayName("Max Videos per Search Query")]
        [Description("Maximum number of videos loaded per search query. Range: 1–150.")]
        [Range(1, 150)]
        public int MaxSearchVideos { get; set; } = 50;
    }
}
