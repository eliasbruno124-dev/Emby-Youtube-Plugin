using System;

namespace Emby.YouTubePlugin
{
    // Shared by persisted settings and compatible source/output dimensions.
    // The hard upper bound also applies to legacy configurations above 1080p.
    internal static class YouTubeDirectQualityPolicy
    {
        internal const int MinimumHeight = 144;
        internal const int MaximumHeight = 1080;
        internal const int MaximumFrameRate = 30;

        internal static int ClampHeight(int? height) =>
            Math.Clamp(height ?? MaximumHeight, MinimumHeight, MaximumHeight);
    }
}
