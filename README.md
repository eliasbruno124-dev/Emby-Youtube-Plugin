<p align="center">
  <img src="docs/social-preview.jpg" alt="Emby YouTube Plugin – YouTube channels, playlists, Shorts and Trending inside Emby" width="100%">
</p>

<p align="center">
  <a href="https://github.com/eliasbruno124-dev/Emby-Youtube-Plugin/releases/latest"><img src="https://img.shields.io/github/v/release/eliasbruno124-dev/Emby-Youtube-Plugin?label=release&color=52B54B" alt="Latest release"></a>
  <a href="LICENSE"><img src="https://img.shields.io/github/license/eliasbruno124-dev/Emby-Youtube-Plugin?color=555" alt="MIT license"></a>
  <a href="https://emby.media"><img src="https://img.shields.io/badge/plugin%20for-Emby-52B54B" alt="Plugin for Emby"></a>
</p>

# Emby YouTube Plugin

**YouTube, right inside Emby.** Add your favourite YouTube channels and
playlists once, and they show up in Emby as folders – next to your movies and
shows. Trending, categories and Shorts are there too.

An Emby channel plugin for browsing YouTube content with the official YouTube Data API v3.

The plugin adds a YouTube entry to Emby and lets you browse saved channels, playlists, Watch Later-style playlists, trending videos, categories, and Shorts from one place.

## Highlights

- Add YouTube channels by `@handle` or channel ID.
- Add playlists as their own Emby folders.
- Add search folders for occasional YouTube searches.
- Create Watch Later-style folders from playlist IDs.
- Show Trending and category folders for a selected region.
- Split channels into Videos and Shorts when those folders have content.
- Hide Shorts globally if you prefer a long-form-only view.
- Hide empty playlists, categories, and Shorts folders.
- Optionally show YouTube root folders at the top level and, on Emby 4.10+, add one latest-videos home row per saved channel while reversibly hiding both forms of combined YouTube-latest row.
- Track estimated daily YouTube API quota usage.
- Ad-free native playback in Emby (2.0.12+): videos are streamed directly into Emby's own player, so YouTube ads are not played. The optional YouTube embed player is YouTube's own player and can show ads.


## Screenshots

### Setup Guide

<img width="740" alt="Setup guide 1" src="docs/screenshots/setup%201.png" />
<br>
<img width="740" alt="Setup guide 2" src="docs/screenshots/setup%202.png" />
<br>
<img width="740" alt="Setup guide 3" src="docs/screenshots/setup%203.png" />
<br>
<img width="740" alt="Setup guide 4" src="docs/screenshots/setup%204.png" />
<br>
<img width="740" alt="Setup guide 5" src="docs/screenshots/setup%205.png" />

### Emby Root Folder

<img width="900" alt="Emby root folder" src="docs/screenshots/Root%20Folder.png" />

### Channel Folders

<img width="900" alt="Channel folder layout" src="docs/screenshots/channel%20folder.png" />

### Video Listing

<img width="900" alt="Trending video listing" src="docs/screenshots/trending%20videos.png" />

## Requirements

- An Emby Server.
- A YouTube Data API v3 key from Google Cloud. The built-in Setup Guide walks you through creating one.
- .NET 8 SDK, only if you want to build the plugin from source.

The API key only needs access to public YouTube data. After installation, open the plugin settings and use the built-in **Setup Guide** to create and restrict the key.

## Installation

1. Download [`Emby.YouTubePlugin.dll`](https://github.com/eliasbruno124-dev/Emby-Youtube-Plugin/releases/latest/download/Emby.YouTubePlugin.dll)
   from the [latest release](https://github.com/eliasbruno124-dev/Emby-Youtube-Plugin/releases/latest).
2. Copy the DLL into Emby's `programdata/plugins` folder.
3. Restart Emby.
4. Open the YouTube plugin settings.
5. Use the **Setup Guide** tab to create a YouTube Data API v3 key.
6. Paste the API key into the **Settings** tab.
7. Add at least one channel, playlist, or search entry.
8. Save the settings.

Useful Google Cloud pages:

- YouTube Data API v3: https://console.cloud.google.com/apis/library/youtube.googleapis.com
- Credentials: https://console.cloud.google.com/apis/credentials

## Saved Content

The **My YouTube Content** field accepts:

- `@handle`, for example `@MrBeast`
- Channel IDs starting with `UC...`
- Playlist IDs starting with `PL...`
- Plain search text

Handles, channel IDs, and playlist IDs are the best options because they avoid the separate daily `search.list` limit. Plain search text works too, but each result page uses one Search Queries call.

The **Watch Later Playlists** field accepts playlist IDs. Each playlist becomes its own top-level folder and can be checked regularly for new videos.


## Quota

Most browsing uses low-cost YouTube API endpoints:

| Action | Endpoint | Typical cost |
| --- | --- | --- |
| Channel uploads | `playlistItems.list` | 1 unit |
| Playlist videos | `playlistItems.list` | 1 unit |
| Video details | `videos.list` | 1 unit per batch |
| Categories | `videoCategories.list` | 1 unit |
| Search folders | `search.list` | 1 Search Queries call |

The plugin caches responses on disk. Search results are cached longer because
`search.list` has a separate default limit of 100 calls per day, while most
other endpoints share the regular 10,000-unit daily bucket.

## Notes

- Since 2.0.12, videos play natively in Emby (up to 1080p at 30 fps, with audio tracks, subtitles and chapters); the YouTube embed player remains available as an option. Native playback can use Emby's FFmpeg, like any other video.
- Video information comes from the official YouTube Data API v3.
- The plugin requests captions off, but some TV and Android players may ignore this.
- Resume and player cleanup depend on the Emby client. Bundled players may not use the server-side fixes.
- Private account data is not supported. Use channels and playlists accessible with your API key.
- After updating, restart Emby. If the web app still looks outdated, clear its browser cache.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Support the Developer

This plugin is free and open-source, maintained with a lot of love in my spare time. If it brings value to your Emby setup, a small donation would mean the world to me.

<p align="center">
  <a href="https://paypal.me/eliasbruno123">
    <img src="https://img.shields.io/badge/Donate%20with-PayPal-0070BA?style=for-the-badge&logo=paypal&logoColor=white" alt="Donate with PayPal">
  </a>
</p>
