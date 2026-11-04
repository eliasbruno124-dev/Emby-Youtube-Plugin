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
            YouTubeChannel.LogPublic, lifetime);
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
