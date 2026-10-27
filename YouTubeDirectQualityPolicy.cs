using System;

namespace Emby.YouTubePlugin
{
    // Shared by persisted settings and the runtime source/output selectors.
    // The hard upper bound also applies to legacy configurations above 1080p.
    internal static class YouTubeDirectQualityPolicy
    {
        internal const int MinimumHeight = 144;
        internal const int MaximumHeight = 1080;
        // Total native output budget (video plus audio), not an upstream
        // measurement. Auto clients can otherwise request a 200 Mbit/s transcode
        // from a much smaller 1080p source. Lower positive client limits win.
        internal const int MaximumStreamingBitrate = 6_000_000;
        internal const int MaximumFrameRate = 30;

        internal static int ClampHeight(int? height) =>
            Math.Clamp(height ?? MaximumHeight, MinimumHeight, MaximumHeight);

        internal static long ClampStreamingBitrate(long? requested, long? query = null)
        {
            long maximum = MaximumStreamingBitrate;
            if (requested is > 0) maximum = Math.Min(maximum, requested.Value);
            if (query is > 0) maximum = Math.Min(maximum, query.Value);
            return maximum;
        }
    }
}
