# Emby YouTube Plugin

An Emby channel plugin for YouTube, built around the official YouTube Data API v3.

[![Donate with PayPal](https://img.shields.io/badge/Donate-PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white)](https://paypal.me/eliasbruno123)

This plugin is free and open-source. If it makes your Emby setup better, a small donation helps keep development moving.

## Features

- Add YouTube channels by handle or channel ID.
- Add YouTube playlists, including Watch Later-style playlists.
- Create search folders from plain search terms.
- Toggle Shorts globally when you want either the full channel view or a cleaner long-form view.
- Show dedicated Videos, Shorts, and Live folders per channel.
- Show Trending, Categories, and Recently Added folders.
- Skip empty playlist folders automatically.
- Cache YouTube API responses to reduce quota usage.
- Keep search results cached for 24 hours because `search.list` costs 100 quota units.
- Restore thumbnails after Emby metadata refreshes without spending extra API quota.

## Screenshots

Add screenshots under `docs/screenshots/`. These filenames are already planned for the README:

| File | What to capture |
| --- | --- |
| `docs/screenshots/settings-page.png` | The plugin settings page with API key, folder toggles, quota status, and the donate button. |
| `docs/screenshots/setup-api-key-01-console.png` | Google Cloud Console after signing in. |
| `docs/screenshots/setup-api-key-02-project.png` | Google Cloud project picker or new project screen. |
| `docs/screenshots/setup-api-key-03-enable-api.png` | YouTube Data API v3 page with the Enable button. |
| `docs/screenshots/setup-api-key-04-credentials.png` | APIs & Services Credentials page. |
| `docs/screenshots/setup-api-key-05-create-key.png` | API key creation dialog with the generated key. |
| `docs/screenshots/setup-api-key-06-emby-settings.png` | The Emby YouTube settings page with the API key pasted in. |
| `docs/screenshots/root-folders.png` | The YouTube channel root in Emby, showing Watch Later, Trending, Categories, Recently Added, and saved content folders. |
| `docs/screenshots/channel-folders.png` | A saved channel opened in Emby, showing Videos, Shorts, and Live folders. |
| `docs/screenshots/watch-later.png` | A Watch Later playlist folder with videos loaded. |
| `docs/screenshots/shorts-filter.png` | The same channel with Shorts visible or hidden, depending on the setting you want to demonstrate. |
| `docs/screenshots/quota-status.png` | The quota panel after the plugin has made a few API calls. |

When the images exist, add them to this section like this:

```md
![Settings page](docs/screenshots/settings-page.png)
![Root folders](docs/screenshots/root-folders.png)
```

## Setup

1. Open Google Cloud Console and sign in with the account that should own the API key.
2. Use the project selector in the top bar to create a new project or select an existing one.
3. Open **APIs & Services** > **Library**.
4. Search for **YouTube Data API v3**, open it, and click **Enable**.
5. Open **APIs & Services** > **Credentials**.
6. Click **Create credentials** > **API key**.
7. Copy the generated API key. Optionally restrict it to YouTube Data API v3.
8. Open the YouTube plugin settings in Emby.
9. Paste the API key and add your channels, playlists, or search terms.
10. Save the settings. The channel refresh starts shortly after saving.

The settings page has a top navigation bar with **Settings** and **Setup Guide** tabs. The guide links directly to:

- YouTube Data API v3: https://console.cloud.google.com/apis/library/youtube.googleapis.com
- Google Cloud Credentials: https://console.cloud.google.com/apis/credentials

Add the API-key screenshots listed above, then replace the placeholder boxes with those images if you want the guide to be visual inside Emby too.

## Saved Items

The settings page lets you add entries one at a time or paste a full comma, semicolon, or line-separated list. After clicking **Add**, every entry appears as its own chip and can be removed with the `x` button.

The `My YouTube Content` list accepts:

- `@handle`
- YouTube channel IDs, such as `UC...`
- YouTube playlist IDs, such as `PL...`
- Plain search text

The `Watch Later Playlists` list accepts playlist IDs and stores each playlist as a separate folder.

Search folders are intentionally limited and cached because they use the expensive YouTube `search.list` endpoint.

## Quota Behavior

Most plugin browsing uses low-cost endpoints:

- Channel uploads: `playlistItems.list`, 1 quota unit.
- Playlist videos: `playlistItems.list`, 1 quota unit.
- Video details and enrichment: `videos.list`, 1 quota unit per batch.
- Trending and categories: low-cost `videos.list` / `videoCategories.list` calls.
- Search folders: `search.list`, 100 quota units, cached for 24 hours.

The cache is stored on disk so normal API responses survive an Emby restart. Quota tracking is stored separately from the response cache.

## Build

```powershell
dotnet restore
dotnet build Emby-Youtube-Plugin.sln
```

The compiled plugin DLL is written to:

```text
bin/Debug/net8.0/Emby.YouTubePlugin.dll
```

## Support

If this plugin saves you time or makes your media library nicer to use, you can support development here:

**PayPal:** https://paypal.me/eliasbruno123
