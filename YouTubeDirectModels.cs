using System;
using System.Text.Json;

namespace Emby.YouTubePlugin
{
    // Extraction data stays server-side. Never serialize this into an API response.
    internal sealed record YouTubeResolvedVideo(
        string VideoId,
        JsonElement Metadata,
        DateTime ExpiresUtc);
}
