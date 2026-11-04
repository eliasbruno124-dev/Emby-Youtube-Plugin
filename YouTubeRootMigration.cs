using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Channels;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace Emby.YouTubePlugin;

// Runtime bridge: the coordinator owns scheduling; native library rows are the
// convergence evidence. Never relax visibility while old roots are retained.
internal static class YouTubeRootMigration
{
    internal const string FormatRevision = "youtube-root-layout-20261104-v1";
    private static YouTubeRootMaterialization? _current;

    internal static void Start(ILibraryManager library, CancellationToken lifetime, bool force = false)
    {
        var engine = new YouTubeRootMaterialization(Capture,
            token => Task.FromResult(ReadRoots(library, token)),
            ChannelRefreshInvoker.RefreshMaterializationAsync,
            () => string.IsNullOrWhiteSpace(Plugin.DataPath) ? null
                : Path.Combine(Plugin.DataPath, "youtube-root-materialization-v1.json"),
            YouTubeChannel.LogPublic, lifetime, AcceptExisting);
        Interlocked.Exchange(ref _current, engine)?.Dispose();
        TryRequest("startup", force);
    }

    internal static bool TryRequest(string reason, bool force = false)
    {
        var engine = Volatile.Read(ref _current);
        if (engine == null) return false;
        try { _ = engine.RequestAsync(reason, force); }
        catch (Exception error) when (error is not OutOfMemoryException)
        { YouTubeChannel.LogPublic("[YT] Root migration request unavailable (" + error.GetType().Name + ")."); }
        return true;
    }

    internal static async Task<bool> WaitForCurrentAsync()
    {
        var engine = Volatile.Read(ref _current);
        return engine != null && await engine.EnsureCurrentAsync("settings response").ConfigureAwait(false)
            && ReferenceEquals(engine, Volatile.Read(ref _current));
    }

    internal static YouTubeRootMaterialization.RootAttempt? BeginRoot()
    {
        if (YouTubePersonalRequestContext.Current != null) return null;
        return Volatile.Read(ref _current)?.BeginRoot();
    }

    internal static void ObserveRoot(YouTubeRootMaterialization.RootAttempt? attempt, ChannelItemResult result)
    {
        if (attempt == null || YouTubePersonalRequestContext.Current != null || result.Items == null
            || result.Items.Any(item => item.Type != ChannelItemType.Folder || string.IsNullOrEmpty(item.Id) || item.Id == "msg" || !ResolvedRootId(item.Id))) return;
        Volatile.Read(ref _current)?.ObserveRoot(attempt, result.Items.Select(item => item.Id));
    }

    private static bool ResolvedRootId(string id)
    {
        if (YouTubePersonalCatalog.TryParse(id, out _, out var inner))
        {
            if (inner == "root") return Plugin.Instance?.Options.PersonalChannelVisibility == "Shared";
            if (Plugin.Instance?.Options.PersonalChannelVisibility != "OwnUser"
                || !YouTubePersonalCatalog.TryParseOwn(inner, out _, out id)) return false;
        }
        else if (YouTubeChannelProvenance.TryParse(id, out _, out var shared))
        {
            if (Plugin.Instance?.Options.PersonalChannelVisibility != "Shared") return false;
            id = shared;
        }
        return !id.StartsWith("channel_x_", StringComparison.Ordinal)
            || YouTubeChannel.IsCanonicalChannelId(id.Substring("channel_x_".Length));
    }

    private static YouTubeRootLayout? Capture()
    {
        if (!YouTubePersonalVisibility.IsReady || Plugin.Instance?.Options is not { } config) return null;
        var hash = PluginEntryPoint.ComputeConfigHash(config);
        var revision = Interlocked.Read(ref YouTubePersonalCatalog.Revision);
        var snapshot = YouTubeChannelProvenance.Copy(config);
        var adminIdentity = YouTubeAdminProvenance.IdentityRevision(snapshot);
        var profiles = YouTubePersonalCatalog.ActiveProfiles(snapshot).OrderBy(profile => profile.ScopeId, StringComparer.Ordinal)
            .Select(profile => new
            {
                profile.ScopeId, profile.Revision,
                HasEnabledKey = snapshot.EnablePersonalApiKeys && !string.IsNullOrWhiteSpace(profile.ApiKey),
                Channels = profile.Channels.Select(YouTubeChannelProvenance.Token).Distinct(StringComparer.Ordinal)
                    .OrderBy(value => value, StringComparer.Ordinal).ToArray()
            }).ToArray();
        if (hash != PluginEntryPoint.ComputeConfigHash(snapshot) || hash != PluginEntryPoint.ComputeConfigHash(config)
            || revision != Interlocked.Read(ref YouTubePersonalCatalog.Revision)
            || adminIdentity != YouTubeAdminProvenance.IdentityRevision(snapshot))
            throw new OperationCanceledException("Root policy changed while capturing its revision.");
        // Hashes only in the stamp: no key, profile or channel list is persisted.
        // Catalog.Revision is a process epoch and deliberately not in stable.
        var stable = Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(new
        { FormatRevision, ChannelFormat = YouTubeChannel.ChannelDataRevision, ContentHash = hash, AdminIdentity = adminIdentity, Profiles = profiles }))).ToLowerInvariant();
        return new YouTubeRootLayout(stable, stable + ":" + revision);
    }

    // On first installation the user may already have run Emby's task. Only
    // adopt an exact offline-provable OwnUser layout; ambiguity keeps the scan.
    private static bool AcceptExisting(YouTubeRootLayout layout, string[] actual)
    {
        if (Capture()?.Epoch != layout.Epoch || Plugin.Instance?.Options is not { } config) return false;
        var snapshot = YouTubeChannelProvenance.Copy(config);
        if (snapshot.PersonalChannelVisibility != "OwnUser" || string.IsNullOrWhiteSpace(snapshot.ApiKey)) return false;
        var expected = new HashSet<string>(StringComparer.Ordinal);
        if (snapshot.ShowTrending) expected.Add("trending_x_all");
        if (snapshot.ShowCategories) expected.Add("categories_x_root");
        foreach (var term in YouTubeChannel.ConfiguredTerms(snapshot.WatchLaterPlaylist))
        {
            if (!YouTubeChannel.IsSupportedPublicPlaylistId(term)) return false;
            expected.Add("playlist_x_" + term);
        }
        string? Canonical(string term) => YouTubeChannel.IsCanonicalChannelId(term) ? term
            : YouTubeChannel.IsCanonicalHandle(term) ? YouTubeAdminProvenance.KnownChannel(snapshot, term) : null;
        foreach (var term in YouTubeChannel.ConfiguredTerms(snapshot.SavedItems))
        {
            var id = Canonical(term);
            if (id != null) expected.Add("channel_x_" + id);
            else if (YouTubeChannel.IsSupportedPublicPlaylistId(term)) expected.Add("playlist_x_" + term);
            else return false;
        }
        var common = expected.ToHashSet(StringComparer.Ordinal);
        if (snapshot.EnablePersonalSubscriptions)
            foreach (var profile in YouTubePersonalCatalog.ActiveProfiles(snapshot))
            {
                var seen = common.ToHashSet(StringComparer.Ordinal);
                foreach (var term in profile.Channels.Distinct(StringComparer.Ordinal))
                {
                    var id = Canonical(term);
                    if (id == null && YouTubeChannel.IsCanonicalHandle(term))
                    {
                        // A pre-existing scoped row binds this current owner and
                        // subscription token to one canonical channel. It proves
                        // shape only; no global identity or permission is added.
                        var prefix = YouTubePersonalCatalog.WrapOwn(profile.ScopeId,
                            YouTubeChannelProvenance.Token(term), "channel_x_");
                        var matches = actual.Where(value => value.StartsWith(prefix, StringComparison.Ordinal))
                            .Select(value => value.Substring(prefix.Length))
                            .Where(YouTubeChannel.IsCanonicalChannelId).Distinct(StringComparer.Ordinal).ToArray();
                        if (matches.Length == 1) id = matches[0];
                    }
                    if (id == null) return false;
                    if (seen.Add("channel_x_" + id))
                        expected.Add(YouTubePersonalCatalog.WrapOwn(profile.ScopeId,
                            YouTubeChannelProvenance.Token(term), "channel_x_" + id));
                }
            }
        return actual.Length == expected.Count && expected.SetEquals(actual)
            && actual.All(ResolvedRootId) && Capture()?.Epoch == layout.Epoch;
    }

    private static string[] ReadRoots(ILibraryManager library, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var manager = Plugin.ResolveService<IChannelManager>() ?? throw new InvalidOperationException("Channel manager unavailable.");
        var provider = manager.GetType().GetMethod("GetChannelProvider", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, new[] { typeof(Channel) }, null) ?? throw new InvalidOperationException("Channel identity binding unavailable.");
        var channels = library.GetItemList(new InternalItemsQuery
        { IncludeItemTypes = new[] { "Channel" }, Recursive = true, EnableTotalRecordCount = false }, token).OfType<Channel>();
        var matches = channels.Where(channel => provider.Invoke(manager, new object[] { channel }) is YouTubeChannel).ToArray();
        if (matches.Length == 0) return new[] { "__youtube_channel_not_yet_materialized__" };
        if (matches.Length != 1) throw new InvalidOperationException("Ambiguous YouTube materialized channel.");
        return library.GetItemList(new InternalItemsQuery
        {
            ParentIds = new[] { matches[0].InternalId }, Recursive = false,
            DtoOptions = new MediaBrowser.Controller.Dto.DtoOptions(new[]
                { MediaBrowser.Model.Querying.ItemFields.Id, MediaBrowser.Model.Querying.ItemFields.ExternalId })
            { EnableImages = false, EnableUserData = false, AutoExcludeExternalId = false },
            GroupByPresentationUniqueKey = false, EnableTotalRecordCount = false, ForceOriginalFolders = true
        }, token).Where(item => !string.IsNullOrEmpty(item.ExternalId)).Select(item => item.ExternalId).ToArray();
    }

    internal static void Stop() => Interlocked.Exchange(ref _current, null)?.Dispose();
}
