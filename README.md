# Emby YouTube Plugin

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
- Optionally show YouTube root folders at the top level and, on Emby 4.10+, add one latest-videos home row per saved channel.
- Track estimated daily YouTube API quota usage.


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

1. Download the latest `Emby.YouTubePlugin.dll` release.
2. Copy the DLL into Emby's  `programdata/plugins` folder.
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

## Browsing Behavior

Saved channels open in the cleanest layout the plugin can provide:

- If a channel only has normal videos, Emby opens the video list directly.
- If Shorts exist, the channel shows subfolders.
- Empty Shorts folders are hidden.
- Empty playlists and empty category folders are hidden.

By default, Emby shows one top-level YouTube entry and keeps saved channels inside it. Enable **Show saved YouTube channels on Emby's home screen** to expose the YouTube root folders at Emby's top level. On Emby 4.10 and newer, the same option also creates one latest-videos home row per saved channel for every user who can access YouTube. Playlists, searches, Trending, and Categories are not turned into home rows. Emby 4.9 keeps the top-level-folder behavior but has no writable HomeSections API, so it cannot create those rows.

Shorts detection uses YouTube metadata such as tags, `#shorts`, and vertical player dimensions. It does not rely on video length alone, because many normal videos are short without being YouTube Shorts.

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

- This plugin uses the official YouTube Data API v3.
- Playback stays on official YouTube watch/embed players. The plugin clears stale
  Emby audio-track selections so the YouTube player can choose the video's own
  source track, but YouTube's official IFrame API does not expose a command for
  selecting the "original" audio track. For signed-in YouTube players, add the
  languages you normally watch under YouTube's **Preferred languages** setting;
  YouTube then keeps original audio when it matches one of those languages.
- YouTube may serve ads in its official embedded player; the plugin does not
  suppress them. In Emby's server-loaded Windows WebView path, the YouTube
  controls remain available and the embedded player is removed deterministically
  when playback stops or ends, so it cannot remain over the next library item.
  If Chromium blocks the first unmuted autoplay attempt, that WebView path
  retries once muted; use the visible YouTube controls to restore sound. Bundled
  Theater/Xbox/Windows player code may not use this server-side compatibility path.
- The plugin does not download, expose, or pass through YouTube caption tracks.
  During plugin playback, the patched server web players keep the official
  YouTube IFrame caption module disabled, and Emby subtitle-selection commands
  cannot enable it. The native Emby Android app uses its own bundled player,
  which a server plugin cannot patch; use Emby Web or its PWA on Android when
  server-side caption suppression is required. YouTube's own **Audio track**
  menu is available only where the client exposes YouTube's controls.
- Private YouTube account data is not supported. Use playlists that your API key can read.
- If the settings page looks stale after an update, restart Emby and clear the browser cache for the Emby web app.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Support the Developer

This plugin is free and open-source, maintained with a lot of love in my spare time. If it brings value to your Emby setup, a small donation would mean the world to me.

<p align="center">
  <a href="https://paypal.me/eliasbruno123">
    <img src="https://img.shields.io/badge/Donate%20with-PayPal-0070BA?style=for-the-badge&logo=paypal&logoColor=white" alt="Donate with PayPal">
  </a>
</p>
