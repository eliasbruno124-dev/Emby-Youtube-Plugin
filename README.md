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

<img width="741" height="884" alt="setup 1" src="https://github.com/user-attachments/assets/4389d805-1a44-4823-8017-9900bdc8564c" />

<img width="743" height="498" alt="setup 2" src="https://github.com/user-attachments/assets/c6754257-73fb-483e-bd2f-67116190d2be" />

<img width="741" height="524" alt="setup 3" src="https://github.com/user-attachments/assets/9caecbe5-e7c0-4415-afe3-c7694bb4caa1" />

<img width="743" height="604" alt="setup 4" src="https://github.com/user-attachments/assets/d2efff79-1dd1-4b85-ad36-091b4a3adf6a" />

<img width="743" height="631" alt="setup 5" src="https://github.com/user-attachments/assets/87b95b3f-4498-4e44-b633-bae4def5668b" />

### Emby Root Folder

<img width="1702" height="383" alt="Root Folder" src="https://github.com/user-attachments/assets/f21d1775-3b38-4477-aeac-ef0e3e701b6d" />

### Channel Folders

<img width="1708" height="322" alt="channel folder" src="https://github.com/user-attachments/assets/40b91757-f8b5-4ef1-b41d-513c97e75ee6" />

### Video Listing

<img width="1713" height="842" alt="trending videos" src="https://github.com/user-attachments/assets/32defc56-dc0a-4a7a-9cf3-315cbe1eb4bc" />


## Requirements

- A selfhostet Emby Server.
- .NET 8 SDK for building from source
- A YouTube Data API v3 key from Google Cloud (it's free)

The API key only needs access to public YouTube data. After installation, open the plugin settings and use the built-in **Setup Guide** to create and restrict the key.

## Installation

1. Download the latest `Emby.YouTubePlugin.dll` release
2. Copy the DLL into Emby's Programdata/plugin folder
3. Restart Emby
4. Open the YouTube plugin settings
5. Use the **Setup Guide** tab to create a YouTube Data API v3 key
6. Paste the API key into the **Settings** tab
7. Add at least one channel, playlist, or search entry
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

- If a channel only has normal videos, Emby opens the video list directly
- If Shorts or live content exist, the channel shows subfolders
- Empty Shorts and Live & Upcoming folders are hidden
- Empty playlists and empty category folders are hidden

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


## Notes

- This plugin uses the official YouTube Data API v3
- Private YouTube account data is not supported. Use playlists that your API key can read
- If the settings page looks stale after an update, restart Emby and clear the browser cache for the Emby web app

## Support the Developer

This plugin is free and open-source, maintained with a lot of love in my spare time. If it brings value to your Emby setup, a small donation would mean the world to me.

<p align="center">
  <a href="https://paypal.me/eliasbruno123">
    <img src="https://img.shields.io/badge/Donate%20with-PayPal-0070BA?style=for-the-badge&logo=paypal&logoColor=white" alt="Donate with PayPal">
  </a>
</p>
