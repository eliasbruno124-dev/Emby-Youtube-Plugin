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


### Settings


<img width="600" height="821" alt="setting 1" src="https://github.com/user-attachments/assets/33b71a79-1ac1-4993-8d0f-e17abef024a4" />
<br>
<img width="600" height="923" alt="setting 2" src="https://github.com/user-attachments/assets/a6ee08c9-debb-4a10-9f78-b7bbffad0238" />
<br>
<img width="600" height="929" alt="setting 3" src="https://github.com/user-attachments/assets/fdf3384f-4c8e-4176-b295-a3b4e3ed4dae" />



### Setup Guide


<img width="740" height="882" alt="setup 1" src="https://github.com/user-attachments/assets/f02db634-08b5-42d1-ad17-48b2c74faefc" />
<br>
<img width="740" height="495" alt="setup 2" src="https://github.com/user-attachments/assets/fc3f3c08-b1b6-41c8-9daf-bf1818f2b501" />
<br>
<img width="740" height="523" alt="setup 3" src="https://github.com/user-attachments/assets/9b802063-d7e1-4e1e-b469-0041e84d293a" />
<br>
<img width="740" height="601" alt="setup 4" src="https://github.com/user-attachments/assets/8adf7113-84a4-42e3-9d07-7a4f819de200" />
<br>
<img width="740" height="628" alt="setup 5" src="https://github.com/user-attachments/assets/dc174935-95b7-4901-b5d6-ba919432afd7" />


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
