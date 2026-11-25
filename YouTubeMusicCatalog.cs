using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // The "Moods & genres" page of YouTube Music (forum request: the Music
    // category by genre; Elias 2026-11-25: "all playlists, like when I tap a
    // genre in the app"). The Data API has no such list, so the pages come
    // from YouTube Music's own web API (InnerTube "browse", client WEB_REMIX),
    // which needs no key and costs no quota. Only the page structure is read
    // here: categories, their sections and the playlists, albums and videos
    // listed in them. Playlist contents still come from the Data API.
    //
    // Pages are cached on disk as small extracted models (a category page is
    // 2-3 MB of JSON, the model a few KB): the category list for a week, a
    // category page for a day. A failed fetch keeps the last model up to 30
    // days, so a YouTube outage or a layout change never empties the folders
    // (an empty answer would make Emby delete their contents).
    internal static class YouTubeMusicCatalog
    {
        internal sealed class Category
        {
            public string Params { get; set; } = "";
            public string Name { get; set; } = "";
        }

        internal sealed class Group
        {
            public string Name { get; set; } = "";
            public List<Category> Categories { get; set; } = new();
        }

        internal sealed class Entry
        {
            // A playlist id (RDCLAK5uy_, PL, OLAK5uy_ for albums) or a video id.
            public string Id { get; set; } = "";
            public string Title { get; set; } = "";
            public string? Subtitle { get; set; }
            public string? Thumb { get; set; }
        }

        internal sealed class Section
        {
            public string Key { get; set; } = "";
            public string Title { get; set; } = "";
            public bool Videos { get; set; }
            public List<Entry> Entries { get; set; } = new();
        }

        internal sealed class Page
        {
            public List<Section> Sections { get; set; } = new();
        }

        private const string BrowseUrl = "https://music.youtube.com/youtubei/v1/browse?prettyPrint=false";
        private const string RootBrowseId = "FEmusic_moods_and_genres";
        private const string CategoryBrowseId = "FEmusic_moods_and_genres_category";
        private static readonly TimeSpan RootTtl = TimeSpan.FromDays(7);
        private static readonly TimeSpan PageTtl = TimeSpan.FromDays(1);
        // YouTube's API terms cap stored data at 30 days; the same limit is
        // used for the stale fallback here.
        private static readonly TimeSpan StaleLimit = TimeSpan.FromDays(30);
        // After a failed fetch the same page is not asked again for a while:
        // a channel scan opens every section of a category one after another.
        private static readonly TimeSpan FailureBackoff = TimeSpan.FromMinutes(10);
        private const int MaxResponseBytes = 16 * 1024 * 1024;

        private static readonly HttpClient Http = CreateHttp();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new(StringComparer.Ordinal);
        private static readonly ConcurrentDictionary<string, (DateTime Loaded, object Model)> Memory = new(StringComparer.Ordinal);
        private static readonly ConcurrentDictionary<string, DateTime> Failures = new(StringComparer.Ordinal);
        private static long _lastCleanupTicks;

        private static HttpClient CreateHttp()
        {
            var client = new HttpClient(YouTubeHttpClientFactory.CreateHandler(
                allowAutoRedirect: false,
                automaticDecompression: DecompressionMethods.All))
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://music.youtube.com");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://music.youtube.com/");
            return client;
        }

        // Language and country of the pages. The country is the plugin's
        // content region; the language follows it, so a German region gets
        // the German names YouTube Music shows there.
        internal static (string Hl, string Gl) Locale(PluginConfiguration? config)
        {
            var gl = (config?.TrendingRegion ?? "").Trim().ToUpperInvariant();
            if (gl.Length != 2 || gl.Any(c => c < 'A' || c > 'Z')) gl = "US";
            return (LanguageFor(gl), gl);
        }

        private static string LanguageFor(string gl) => gl switch
        {
            "DE" or "AT" or "CH" or "LI" => "de",
            "FR" or "LU" or "MC" => "fr",
            "ES" or "MX" or "AR" or "CO" or "CL" or "PE" or "VE" or "EC" or "UY" or "PY" or "BO"
                or "CR" or "GT" or "HN" or "NI" or "PA" or "SV" or "DO" or "PR" => "es",
            "IT" or "SM" => "it",
            "NL" => "nl",
            "PT" => "pt-PT",
            "BR" => "pt",
            "PL" => "pl",
            "CZ" => "cs",
            "SK" => "sk",
            "HU" => "hu",
            "RO" => "ro",
            "BG" => "bg",
            "HR" => "hr",
            "RS" => "sr",
            "SI" => "sl",
            "GR" or "CY" => "el",
            "TR" => "tr",
            "RU" or "BY" or "KZ" => "ru",
            "UA" => "uk",
            "SE" => "sv",
            "NO" => "no",
            "DK" => "da",
            "FI" => "fi",
            "EE" => "et",
            "LV" => "lv",
            "LT" => "lt",
            "JP" => "ja",
            "KR" => "ko",
            "TW" or "HK" => "zh-TW",
            "TH" => "th",
            "VN" => "vi",
            "ID" => "id",
            "MY" => "ms",
            "IL" => "iw",
            "SA" or "AE" or "EG" or "MA" or "DZ" or "TN" or "JO" or "KW" or "QA" or "BH" or "OM" or "LB" or "IQ" => "ar",
            _ => "en"
        };

        internal static Task<List<Group>?> GetGroupsAsync(PluginConfiguration? config, CancellationToken ct)
        {
            var (hl, gl) = Locale(config);
            return LoadAsync($"root-{hl}-{gl}", RootTtl,
                new Dictionary<string, object> { ["browseId"] = RootBrowseId }, hl, gl, ParseRoot, ct);
        }

        internal static Task<Page?> GetPageAsync(string categoryParams, PluginConfiguration? config, CancellationToken ct)
        {
            var (hl, gl) = Locale(config);
            return LoadAsync($"page-{hl}-{gl}-{ShortHash(categoryParams)}", PageTtl,
                new Dictionary<string, object> { ["browseId"] = CategoryBrowseId, ["params"] = categoryParams },
                hl, gl, ParsePage, ct);
        }

        // The cached page only, never a network request. For the folder
        // icons of the category list, which must not wait for 41 pages.
        internal static Page? PeekPage(string categoryParams, PluginConfiguration? config)
        {
            var (hl, gl) = Locale(config);
            var key = $"page-{hl}-{gl}-{ShortHash(categoryParams)}";
            if (Memory.TryGetValue(key, out var hit) && hit.Model is Page page) return page;
            return ReadDisk<Page>(key, StaleLimit)?.Model;
        }

        internal static string ShortHash(string value) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).Substring(0, 16).ToLowerInvariant();

        private static async Task<T?> LoadAsync<T>(string key, TimeSpan ttl, Dictionary<string, object> request,
            string hl, string gl, Func<JsonElement, T?> parse, CancellationToken ct) where T : class
        {
            if (Memory.TryGetValue(key, out var mem) && mem.Model is T fresh && DateTime.UtcNow - mem.Loaded < ttl)
                return fresh;

            var gate = Locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (Memory.TryGetValue(key, out mem) && mem.Model is T again && DateTime.UtcNow - mem.Loaded < ttl)
                    return again;

                var disk = ReadDisk<T>(key, StaleLimit);
                if (disk != null && disk.Value.Age < ttl)
                {
                    Memory[key] = (DateTime.UtcNow - disk.Value.Age, disk.Value.Model);
                    return disk.Value.Model;
                }

                T? model = null;
                if (!Failures.TryGetValue(key, out var failedAt) || DateTime.UtcNow - failedAt > FailureBackoff)
                {
                    try
                    {
                        model = await FetchAsync(request, hl, gl, parse, ct).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
                    catch (Exception error) when (error is not OutOfMemoryException)
                    {
                        YouTubeChannel.LogPublic($"[YT] YouTube Music page {key} unavailable ({error.GetType().Name}: {error.Message}).");
                    }
                    if (model == null) Failures[key] = DateTime.UtcNow;
                }

                if (model != null)
                {
                    Failures.TryRemove(key, out _);
                    WriteDisk(key, model);
                    Memory[key] = (DateTime.UtcNow, model);
                    return model;
                }

                if (disk != null)
                {
                    // Keep serving the last good copy; retried after the backoff.
                    Memory[key] = (DateTime.UtcNow - ttl + FailureBackoff, disk.Value.Model);
                    return disk.Value.Model;
                }
                return null;
            }
            finally
            {
                gate.Release();
            }
        }

        private static async Task<T?> FetchAsync<T>(Dictionary<string, object> request, string hl, string gl,
            Func<JsonElement, T?> parse, CancellationToken ct) where T : class
        {
            var body = new Dictionary<string, object>(request)
            {
                ["context"] = new Dictionary<string, object>
                {
                    ["client"] = new Dictionary<string, object>
                    {
                        ["clientName"] = "WEB_REMIX",
                        // Same scheme as the web app (1.<date>.01.00).
                        ["clientVersion"] = "1." + DateTime.UtcNow.AddDays(-2).ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) + ".01.00",
                        ["hl"] = hl,
                        ["gl"] = gl
                    }
                }
            };

            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
            deadline.CancelAfter(TimeSpan.FromSeconds(45));
            using var message = new HttpRequestMessage(HttpMethod.Post, BrowseUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            using var response = await Http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, deadline.Token)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("HTTP " + (int)response.StatusCode);
            if (response.Content.Headers.ContentLength > MaxResponseBytes)
                throw new InvalidDataException("response too large");

            await using var stream = await response.Content.ReadAsStreamAsync(deadline.Token).ConfigureAwait(false);
            using var buffer = new MemoryStream();
            var chunk = new byte[32 * 1024];
            int read;
            while ((read = await stream.ReadAsync(chunk.AsMemory(), deadline.Token).ConfigureAwait(false)) > 0)
            {
                if (buffer.Length + read > MaxResponseBytes) throw new InvalidDataException("response too large");
                buffer.Write(chunk, 0, read);
            }
            using var doc = JsonDocument.Parse(buffer.ToArray());
            return parse(doc.RootElement);
        }

        // ---- parsing ----

        private static JsonElement? Path(JsonElement element, params string[] names)
        {
            var current = element;
            foreach (var name in names)
            {
                if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(name, out var next))
                    return null;
                current = next;
            }
            return current;
        }

        private static string? Str(JsonElement? element) =>
            element is { ValueKind: JsonValueKind.String } value ? value.GetString() : null;

        private static string Text(JsonElement? element)
        {
            if (element is not { ValueKind: JsonValueKind.Object } value) return "";
            if (value.TryGetProperty("simpleText", out var simple) && simple.ValueKind == JsonValueKind.String)
                return simple.GetString() ?? "";
            if (!value.TryGetProperty("runs", out var runs) || runs.ValueKind != JsonValueKind.Array) return "";
            var text = new StringBuilder();
            foreach (var run in runs.EnumerateArray())
                if (run.ValueKind == JsonValueKind.Object && run.TryGetProperty("text", out var t) && t.ValueKind == JsonValueKind.String)
                    text.Append(t.GetString());
            // YouTube Music uses non-breaking spaces and soft hyphens in names
            // ("Mandopop & Cantopop", "At­mo­sphä­risch").
            return text.ToString().Replace(' ', ' ').Replace("­", "").Trim();
        }

        private static IEnumerable<JsonElement> Array(JsonElement? element)
        {
            if (element is { ValueKind: JsonValueKind.Array } array)
                foreach (var item in array.EnumerateArray())
                    yield return item;
        }

        private static IEnumerable<JsonElement> SectionList(JsonElement root)
        {
            foreach (var tab in Array(Path(root, "contents", "singleColumnBrowseResultsRenderer", "tabs")))
                foreach (var section in Array(Path(tab, "tabRenderer", "content", "sectionListRenderer", "contents")))
                    yield return section;
        }

        private static string? BestThumb(JsonElement? thumbnails)
        {
            string? best = null;
            long bestWidth = -1;
            foreach (var thumb in Array(thumbnails))
            {
                if (thumb.ValueKind != JsonValueKind.Object) continue;
                var url = YouTubeApi.GetString(thumb, "url");
                if (string.IsNullOrEmpty(url)) continue;
                long width = thumb.TryGetProperty("width", out var w) && w.ValueKind == JsonValueKind.Number && w.TryGetInt64(out var px) ? px : 0;
                if (width >= bestWidth) { best = url; bestWidth = width; }
            }
            if (best != null && best.StartsWith("//", StringComparison.Ordinal)) best = "https:" + best;
            return best != null && best.StartsWith("https://", StringComparison.Ordinal) ? best : null;
        }

        internal static List<Group>? ParseRoot(JsonElement root)
        {
            var groups = new List<Group>();
            foreach (var section in SectionList(root))
            {
                if (Path(section, "gridRenderer") is not { } grid) continue;
                var group = new Group { Name = Text(Path(grid, "header", "gridHeaderRenderer", "title")) };
                foreach (var item in Array(Path(grid, "items")))
                {
                    if (Path(item, "musicNavigationButtonRenderer") is not { } button) continue;
                    var name = Text(Path(button, "buttonText"));
                    var parameters = Str(Path(button, "clickCommand", "browseEndpoint", "params"));
                    var browseId = Str(Path(button, "clickCommand", "browseEndpoint", "browseId"));
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(parameters)
                        || !string.Equals(browseId, CategoryBrowseId, StringComparison.Ordinal))
                        continue;
                    group.Categories.Add(new Category { Name = name, Params = parameters! });
                }
                if (group.Categories.Count > 0)
                {
                    if (group.Name.Length == 0) group.Name = groups.Count == 0 ? "Moods & moments" : "Genres";
                    groups.Add(group);
                }
            }
            return groups.Count > 0 ? groups : null;
        }

        internal static Page? ParsePage(JsonElement root)
        {
            var page = new Page();
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var section in SectionList(root))
            {
                string title;
                IEnumerable<JsonElement> items;
                if (Path(section, "musicCarouselShelfRenderer") is { } shelf)
                {
                    title = Text(Path(shelf, "header", "musicCarouselShelfBasicHeaderRenderer", "title"));
                    items = Array(Path(shelf, "contents"));
                }
                else if (Path(section, "gridRenderer") is { } grid)
                {
                    title = Text(Path(grid, "header", "gridHeaderRenderer", "title"));
                    items = Array(Path(grid, "items"));
                }
                else continue;

                var playlists = new List<Entry>();
                var videos = new List<Entry>();
                foreach (var item in items)
                {
                    if (Path(item, "musicTwoRowItemRenderer") is { } card)
                        AddCard(card, playlists, videos);
                    else if (Path(item, "musicResponsiveListItemRenderer") is { } row)
                        AddRow(row, videos);
                }

                var isVideos = videos.Count > playlists.Count;
                var entries = isVideos ? videos : playlists;
                // Within a section each playlist or video appears once.
                entries = entries.GroupBy(e => e.Id, StringComparer.Ordinal).Select(g => g.First()).ToList();
                if (entries.Count == 0 || title.Length == 0) continue;

                var key = ShortHash(title).Substring(0, 10);
                for (var n = 2; !keys.Add(key); n++) key = ShortHash(title + "#" + n).Substring(0, 10);
                page.Sections.Add(new Section { Key = key, Title = title, Videos = isVideos, Entries = entries });
            }
            return page.Sections.Count > 0 ? page : null;
        }

        private static void AddCard(JsonElement card, List<Entry> playlists, List<Entry> videos)
        {
            var title = Text(Path(card, "title"));
            if (title.Length == 0) return;
            var subtitle = Text(Path(card, "subtitle"));
            var thumb = BestThumb(Path(card, "thumbnailRenderer", "musicThumbnailRenderer", "thumbnail", "thumbnails"));
            var videoId = Str(Path(card, "navigationEndpoint", "watchEndpoint", "videoId"));
            if (!string.IsNullOrEmpty(videoId))
            {
                videos.Add(new Entry { Id = videoId!, Title = title, Subtitle = Artist(subtitle), Thumb = thumb });
                return;
            }

            var browseId = Str(Path(card, "navigationEndpoint", "browseEndpoint", "browseId")) ?? "";
            string? playlistId = null;
            if (browseId.StartsWith("VL", StringComparison.Ordinal) && browseId.Length > 2)
                playlistId = browseId.Substring(2);
            else if (browseId.StartsWith("MPRE", StringComparison.Ordinal))
                // An album opens as its audio playlist (OLAK5uy_...), which
                // the Data API can read like any other playlist.
                playlistId = Str(Path(card, "thumbnailOverlay", "musicItemThumbnailOverlayRenderer", "content",
                    "musicPlayButtonRenderer", "playNavigationEndpoint", "watchPlaylistEndpoint", "playlistId"));
            if (string.IsNullOrEmpty(playlistId) || !IsPlaylistId(playlistId!)) return;
            playlists.Add(new Entry { Id = playlistId!, Title = title, Subtitle = subtitle.Length > 0 ? subtitle : null, Thumb = thumb });
        }

        private static void AddRow(JsonElement row, List<Entry> videos)
        {
            var videoId = Str(Path(row, "playlistItemData", "videoId"))
                ?? Str(Path(row, "overlay", "musicItemThumbnailOverlayRenderer", "content", "musicPlayButtonRenderer",
                    "playNavigationEndpoint", "watchEndpoint", "videoId"));
            if (string.IsNullOrEmpty(videoId)) return;
            var columns = Array(Path(row, "flexColumns")).ToList();
            var title = columns.Count > 0 ? Text(Path(columns[0], "musicResponsiveListItemFlexColumnRenderer", "text")) : "";
            if (title.Length == 0) return;
            var subtitle = columns.Count > 1 ? Text(Path(columns[1], "musicResponsiveListItemFlexColumnRenderer", "text")) : "";
            var thumb = BestThumb(Path(row, "thumbnail", "musicThumbnailRenderer", "thumbnail", "thumbnails"));
            videos.Add(new Entry { Id = videoId!, Title = title, Subtitle = Artist(subtitle), Thumb = thumb });
        }

        // "MERO und Poizi • 11 Mio. Aufrufe" -> "MERO und Poizi".
        private static string? Artist(string subtitle)
        {
            var cut = subtitle.IndexOf(" • ", StringComparison.Ordinal);
            var artist = (cut >= 0 ? subtitle.Substring(0, cut) : subtitle).Trim();
            return artist.Length > 0 ? artist : null;
        }

        internal static bool IsPlaylistId(string id) =>
            id.Length is >= 10 and <= 64 && id.All(c => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '-');

        // ---- disk cache ----

        private static string? CacheDir()
        {
            var root = Plugin.CachePath;
            return string.IsNullOrEmpty(root) ? null : System.IO.Path.Combine(root, "youtube-music");
        }

        private static (T Model, TimeSpan Age)? ReadDisk<T>(string key, TimeSpan maxAge) where T : class
        {
            try
            {
                var dir = CacheDir();
                if (dir == null) return null;
                var file = System.IO.Path.Combine(dir, key + ".json");
                if (!File.Exists(file)) return null;
                var age = DateTime.UtcNow - File.GetLastWriteTimeUtc(file);
                if (age > maxAge) return null;
                var model = JsonSerializer.Deserialize<T>(File.ReadAllText(file, Encoding.UTF8));
                return model == null ? null : (model, age < TimeSpan.Zero ? TimeSpan.Zero : age);
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException or NotSupportedException)
            {
                return null;
            }
        }

        private static void WriteDisk<T>(string key, T model)
        {
            string? temp = null;
            try
            {
                var dir = CacheDir();
                if (dir == null) return;
                Directory.CreateDirectory(dir);
                Cleanup(dir);
                var file = System.IO.Path.Combine(dir, key + ".json");
                temp = file + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(temp, JsonSerializer.Serialize(model), new UTF8Encoding(false));
                File.Move(temp, file, overwrite: true);
                temp = null;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                YouTubeChannel.LogPublic($"[YT] YouTube Music cache write failed ({error.GetType().Name}).");
            }
            finally
            {
                if (temp != null) try { File.Delete(temp); } catch { }
            }
        }

        // Pages of other regions and old temp files go after 30 days.
        private static void Cleanup(string dir)
        {
            var now = DateTime.UtcNow.Ticks;
            var last = Interlocked.Read(ref _lastCleanupTicks);
            if (now - last < TimeSpan.TicksPerDay || Interlocked.CompareExchange(ref _lastCleanupTicks, now, last) != last)
                return;
            try
            {
                foreach (var file in Directory.EnumerateFiles(dir))
                    if (DateTime.UtcNow - File.GetLastWriteTimeUtc(file) > StaleLimit
                        || file.EndsWith(".tmp", StringComparison.Ordinal) && DateTime.UtcNow - File.GetLastWriteTimeUtc(file) > TimeSpan.FromHours(1))
                        try { File.Delete(file); } catch { }
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException) { }
        }
    }
}
