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

Save screenshots in `docs/screenshots/` with the exact filenames below. The README already references these paths, so the images will appear on GitHub as soon as the files are added.

Use a clean browser window, keep the Emby theme consistent, and blur or cover the API key before committing screenshots.

### Settings

![Settings page](docs/screenshots/settings.png)

Capture the **Settings** tab with the API key field blurred, saved content chips, Watch Later playlists, quota panel, and the main toggles visible.

### Setup Guide

![Setup guide](docs/screenshots/setup-guide.png)

Capture the **Setup Guide** tab with the step cards and guide images visible. This is the best screenshot to show that new users do not need to guess how to create the API key.

### Emby Root Folder

![Emby root folder](docs/screenshots/root-folders.png)

Capture the YouTube channel root in Emby with folders such as Watch Later, Trending, Categories, Recently Added, and saved channels/playlists.

### Channel Folders

![Channel folders](docs/screenshots/channel-folders.png)

Open a saved channel that has enough content to show **Videos**, **Shorts**, and **Live & Upcoming**. This makes the folder behavior obvious.

### Video Listing

![Video listing](docs/screenshots/videos.png)

Open a Videos folder and capture the normal Emby video grid/list with thumbnails loaded.

Optional extra screenshots:

| File | What to capture |
| --- | --- |
| `docs/screenshots/quota.png` | A close crop of the quota panel after a few API calls. |
| `docs/screenshots/shorts-hidden.png` | A channel view with Shorts disabled, showing the cleaner long-form layout. |
| `docs/screenshots/watch-later.png` | A Watch Later playlist folder with videos loaded. |

## Requirements

- Emby Server.
- .NET 8 SDK for building from source.
- A YouTube Data API v3 key from Google Cloud.

The API key only needs access to public YouTube data. After installation, open the plugin settings and use the built-in **Setup Guide** to create and restrict the key.

## Installation

1. Build or download `Emby.YouTubePlugin.dll`.
2. Copy the DLL into Emby's plugin folder.
3. Restart Emby.
4. Open the YouTube plugin settings.
5. Use the **Setup Guide** tab to create a YouTube Data API v3 key.
6. Paste the API key into the **Settings** tab.
7. Add at least one channel, playlist, or search entry.
8. Save the settings.

On many Windows installations, the plugin folder is:

```text
C:\ProgramData\Emby-Server\programdata\plugins
```

If your Emby data path is different, use the `plugins` folder inside that Emby program data directory.

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

## Sponsor

If this plugin makes your Emby setup better and you want to support development, you can donate through PayPal.

<p align="center">
  <a href="https://paypal.me/eliasbruno123">
    <img src="https://img.shields.io/badge/Donate%20with-PayPal-0070BA?style=for-the-badge&logo=paypal&logoColor=white" alt="Donate with PayPal">
  </a>
</p>
