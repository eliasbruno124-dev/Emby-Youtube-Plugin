# Emby YouTube Plugin

An Emby channel plugin for browsing YouTube content with the official YouTube Data API v3.

The plugin adds a YouTube entry to Emby and lets you browse saved channels, playlists, Watch Later-style playlists, trending videos, categories, Shorts, live streams, and recently added uploads from one place.

## Highlights

- Add YouTube channels by `@handle` or channel ID.
- Add playlists as their own Emby folders.
- Add search folders for occasional YouTube searches.
- Create Watch Later-style folders from playlist IDs.
- Show Trending and category folders for a selected region.
- Show Recently Added across all saved channels.
- Split channels into Videos, Shorts, and Live & Upcoming when those folders have content.
- Hide Shorts globally if you prefer a long-form-only view.
- Hide empty playlists, categories, Shorts folders, and live folders.
- Track estimated daily YouTube API quota usage.
- Cache API responses to reduce quota usage and speed up browsing.
- Restore missing thumbnails after Emby metadata refreshes.

## Screenshots

Screenshots are not required for the plugin to work, but these are the best ones to add later:

| Screenshot | Recommended file | What it should show |
| --- | --- | --- |
| Settings page | `docs/screenshots/settings.png` | API key field blurred, saved content chips, Watch Later entries, toggles, and quota panel. |
| Setup guide | `docs/screenshots/setup-guide.png` | The built-in setup guide with the Google Cloud steps and images. |
| Emby root folder | `docs/screenshots/root-folders.png` | Watch Later, Trending, Categories, Recently Added, and saved YouTube entries. |
| Channel folders | `docs/screenshots/channel-folders.png` | A saved channel with Videos, Shorts, and Live & Upcoming. |
| Video listing | `docs/screenshots/videos.png` | A normal Emby grid/list view with YouTube videos and thumbnails. |

When screenshots exist, place them in the relevant section with normal Markdown image tags:

```md
![Settings page](docs/screenshots/settings.png)
```

Do not commit screenshots that show a real API key.

## Requirements

- Emby Server with plugin support.
- .NET 8 SDK for building from source.
- A YouTube Data API v3 key from Google Cloud.

The API key only needs access to public YouTube data. Restricting the key to **YouTube Data API v3** is recommended.

## Setup

1. Build the plugin and copy `Emby.YouTubePlugin.dll` into your Emby plugins folder.
2. Restart Emby.
3. Open the YouTube plugin settings.
4. Use the **Setup Guide** tab to create a YouTube Data API v3 key.
5. Paste the API key into the **Settings** tab.
6. Add at least one channel, playlist, or search entry.
7. Save the settings.

Useful Google Cloud pages:

- YouTube Data API v3: https://console.cloud.google.com/apis/library/youtube.googleapis.com
- Credentials: https://console.cloud.google.com/apis/credentials

## Saved Content

The **My YouTube Content** field accepts:

- `@handle`, for example `@MrBeast`
- Channel IDs starting with `UC...`
- Playlist IDs starting with `PL...`
- Plain search text

Handles, channel IDs, and playlist IDs are the best options because they use cheap API calls. Plain search text works too, but it uses YouTube's expensive `search.list` endpoint.

The **Watch Later Playlists** field accepts playlist IDs. Each playlist becomes its own top-level folder and can be checked regularly for new videos.

## Browsing Behavior

Saved channels open in the cleanest layout the plugin can provide:

- If a channel only has normal videos, Emby opens the video list directly.
- If Shorts or live content exist, the channel shows subfolders.
- Empty Shorts and Live & Upcoming folders are hidden.
- Empty playlists and empty category folders are hidden.

Shorts detection uses YouTube metadata such as tags, `#shorts`, and vertical player dimensions. It does not rely on video length alone, because many normal videos are short without being YouTube Shorts.

## Quota

Most browsing uses low-cost YouTube API endpoints:

| Action | Endpoint | Typical cost |
| --- | --- | --- |
| Channel uploads | `playlistItems.list` | 1 unit |
| Playlist videos | `playlistItems.list` | 1 unit |
| Video details | `videos.list` | 1 unit per batch |
| Categories | `videoCategories.list` | 1 unit |
| Search folders | `search.list` | 100 units |

The plugin caches responses on disk. Search results are cached longer because search is the most expensive operation.

## Build

```powershell
dotnet restore
dotnet build Emby-Youtube-Plugin.sln
```

The compiled DLL is created here:

```text
bin/Debug/net8.0/Emby.YouTubePlugin.dll
```

After copying the DLL into Emby's plugin folder, restart Emby so the new version loads.

## Notes

- This plugin uses the official YouTube Data API v3, but it is not an official Google or YouTube product.
- Private YouTube account data is not supported. Use playlists that your API key can read.
- If the settings page looks stale after an update, restart Emby and clear the browser cache for the Emby web app.
