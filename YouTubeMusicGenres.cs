using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // Music folders shaped like YouTube Music's "Moods & genres" page (forum
    // request: the Music category by genre). Tree:
    //   Music -> Moods & moments | Genres -> category (Chill, Hip-Hop, ...)
    //     -> section as in the app (Featured playlists, Community playlists,
    //        Songs, Music videos, Albums, ...) -> playlist/album -> videos.
    // Song and video sections hold their videos directly.
    //
    // Emby fills every folder during its channel scan (the scheduled task
    // walks eight levels), and YouTube Music lists about 3,000 playlists and
    // 1,250 albums. Filling them all would put 150,000+ videos into the
    // library and use the daily API quota. So every playlist is shown, but
    // only playlists someone opened (or whose section someone opened) get
    // their videos; see YouTubeMusicBrowse. The folder tree itself costs no
    // quota (YouTubeMusicCatalog).
    internal static class YouTubeMusicGenres
    {
        internal const string MusicCategoryId = "10";
        internal const string RootFolderId = "musicgenres_x_root";
        internal const string GroupType = "ytmgroup";
        internal const string CategoryType = "ytmcat";
        internal const string SectionType = "ytmsec";
        internal const string PlaylistType = "ytmpl";
        private static string Sep => YouTubeChannel.FolderSeparatorValue;
        private static readonly string[] GroupIcons = { "1F3A7", "1F3BC" }; // 🎧 moods, 🎼 genres
        private const string MusicIcon = "1F3B5";                          // 🎵

        // Section folders are "<category params>.<section key>"; params are
        // base64url and never contain a dot.
        private const char SectionSeparator = '.';

        // Raised instead of returning an empty list when YouTube Music cannot
        // be read: an empty answer would make Emby delete the folder's items.
        internal sealed class UnavailableException : Exception
        {
            internal UnavailableException() : base("YouTube Music is temporarily unavailable.") { }
        }

        internal static bool ShowsOwnRootFolder(PluginConfiguration config) => config.ShowMusicGenres && !config.ShowCategories;

        internal static bool IsMusicFolder(string type, string term) =>
            type is GroupType or CategoryType or SectionType or PlaylistType
            || type + Sep + term == RootFolderId;

        internal static bool HoldsVideos(string type) => type is SectionType or PlaylistType;

        private static ChannelItemInfo Folder(string name, string id, string? image, string? overview = null) => new()
        {
            Name = name,
            Id = id,
            Overview = overview,
            Type = ChannelItemType.Folder,
            FolderType = ChannelFolderType.Container,
            MediaType = MediaBrowser.Model.Channels.ChannelMediaType.Video,
            ImageUrl = image
        };

        private static ChannelItemResult Result(List<ChannelItemInfo> items) =>
            new() { Items = items, TotalRecordCount = items.Count };

        private static async Task<List<ChannelItemInfo>> GroupFoldersAsync(PluginConfiguration config, CancellationToken ct)
        {
            var groups = await YouTubeMusicCatalog.GetGroupsAsync(config, ct).ConfigureAwait(false)
                ?? throw new UnavailableException();
            return groups.Select((group, index) => Folder(group.Name, GroupType + Sep + index,
                FolderIcons.Cdn + GroupIcons[Math.Min(index, GroupIcons.Length - 1)] + ".png")).ToList();
        }

        // Music folders go first in the Music category of the categories
        // browser, the category's trending videos follow unchanged.
        internal static async Task<ChannelItemResult> AddGenreFoldersAsync(string? categoryId, ChannelItemResult result,
            PluginConfiguration config, CancellationToken ct)
        {
            if (categoryId != MusicCategoryId || !config.ShowMusicGenres || result?.Items == null)
                return result!;
            var folders = await GroupFoldersAsync(config, ct).ConfigureAwait(false);
            // A bare message entry ("No results.") is dropped; the music folders still work.
            var videos = result.Items.Where(i => !string.Equals(i.Id, "msg", StringComparison.Ordinal)).ToList();
            result.Items = folders.Concat(videos).ToList();
            result.TotalRecordCount = result.Items.Count;
            return result;
        }

        // Folder list of a music folder, or null when the folder holds videos
        // (the caller loads them like any playlist).
        internal static async Task<ChannelItemResult?> GetFoldersAsync(string type, string term, PluginConfiguration config, CancellationToken ct)
        {
            if (type + Sep + term == RootFolderId)
                return Result(await GroupFoldersAsync(config, ct).ConfigureAwait(false));

            switch (type)
            {
                case GroupType:
                {
                    // Without the list of opened playlists every playlist
                    // would answer empty and lose its videos. Stop here, so
                    // Emby keeps everything below.
                    if (!YouTubeMusicBrowse.StateKnown) throw new UnavailableException();
                    var groups = await YouTubeMusicCatalog.GetGroupsAsync(config, ct).ConfigureAwait(false)
                        ?? throw new UnavailableException();
                    if (!int.TryParse(term, out var index) || index < 0 || index >= groups.Count)
                        return Result(new List<ChannelItemInfo>());
                    var categories = groups[index].Categories;
                    // Each category shows the cover of its first playlist. The
                    // pages are needed one level down anyway; load them now,
                    // a few at a time.
                    using var gate = new SemaphoreSlim(4);
                    var covers = await Task.WhenAll(categories.Select(async category =>
                    {
                        await gate.WaitAsync(ct).ConfigureAwait(false);
                        try
                        {
                            var page = await YouTubeMusicCatalog.GetPageAsync(category.Params, config, ct).ConfigureAwait(false);
                            return page?.Sections.FirstOrDefault(s => !s.Videos)?.Entries.FirstOrDefault()?.Thumb
                                ?? page?.Sections.FirstOrDefault()?.Entries.FirstOrDefault()?.Thumb;
                        }
                        finally { gate.Release(); }
                    })).ConfigureAwait(false);
                    return Result(categories.Select((category, i) => Folder(category.Name, CategoryType + Sep + category.Params,
                        covers[i] ?? FolderIcons.Cdn + MusicIcon + ".png")).ToList());
                }
                case CategoryType:
                {
                    var page = await YouTubeMusicCatalog.GetPageAsync(term, config, ct).ConfigureAwait(false)
                        ?? throw new UnavailableException();
                    return Result(page.Sections.Select(section => Folder(section.Title,
                        SectionType + Sep + term + SectionSeparator + section.Key,
                        section.Entries.FirstOrDefault()?.Thumb ?? FolderIcons.Cdn + MusicIcon + ".png")).ToList());
                }
                case SectionType:
                {
                    var section = await GetSectionAsync(term, config, ct).ConfigureAwait(false);
                    if (section == null) return Result(new List<ChannelItemInfo>());
                    if (section.Videos) return null;
                    return Result(section.Entries.Select(entry => Folder(entry.Title, PlaylistType + Sep + entry.Id,
                        entry.Thumb, entry.Subtitle)).ToList());
                }
                case PlaylistType:
                    // Unopened playlists stay empty; see YouTubeMusicBrowse.
                    return YouTubeMusicBrowse.IsWanted(term) switch
                    {
                        true => null,
                        false => Result(new List<ChannelItemInfo>()),
                        null => throw new UnavailableException()
                    };
            }
            return Result(new List<ChannelItemInfo>());
        }

        internal static (string Params, string Key)? SplitSection(string term)
        {
            var dot = term.LastIndexOf(SectionSeparator);
            return dot <= 0 || dot == term.Length - 1 ? null : (term.Substring(0, dot), term.Substring(dot + 1));
        }

        // Null when the section is gone from the page (Emby then removes its
        // folder); throws when the page cannot be read at all.
        private static async Task<YouTubeMusicCatalog.Section?> GetSectionAsync(string term, PluginConfiguration config, CancellationToken ct)
        {
            if (SplitSection(term) is not { } parts) return null;
            var page = await YouTubeMusicCatalog.GetPageAsync(parts.Params, config, ct).ConfigureAwait(false)
                ?? throw new UnavailableException();
            return page.Sections.FirstOrDefault(s => string.Equals(s.Key, parts.Key, StringComparison.Ordinal));
        }

        internal static YouTubeMusicCatalog.Section? PeekSection(string term, PluginConfiguration? config)
        {
            if (SplitSection(term) is not { } parts) return null;
            return YouTubeMusicCatalog.PeekPage(parts.Params, config)?.Sections
                .FirstOrDefault(s => string.Equals(s.Key, parts.Key, StringComparison.Ordinal));
        }

        // The videos of a song or music-video section, shaped like a
        // playlistItems answer so the normal playlist path (details,
        // availability, age restriction) handles them.
        internal static async Task<JsonDocument?> SectionVideosAsync(string term, PluginConfiguration config, CancellationToken ct)
        {
            var section = await GetSectionAsync(term, config, ct).ConfigureAwait(false);
            if (section == null || !section.Videos) return null;
            using var buffer = new MemoryStream();
            using (var writer = new Utf8JsonWriter(buffer))
            {
                writer.WriteStartObject();
                writer.WriteStartArray("items");
                foreach (var entry in section.Entries)
                {
                    writer.WriteStartObject();
                    writer.WriteStartObject("contentDetails");
                    writer.WriteString("videoId", entry.Id);
                    writer.WriteEndObject();
                    writer.WriteStartObject("snippet");
                    writer.WriteString("title", entry.Title);
                    if (entry.Subtitle != null) writer.WriteString("channelTitle", entry.Subtitle);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            return JsonDocument.Parse(buffer.ToArray());
        }
    }
}
