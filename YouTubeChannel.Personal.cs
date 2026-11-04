using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Querying;
using System.Reflection;

namespace Emby.YouTubePlugin;

public partial class YouTubeChannel
{
    private static void AddPersonalRoots(List<ChannelItemInfo> items, PluginConfiguration config)
    {
        if (YouTubePersonalRequestContext.Current != null || !YouTubePersonalVisibility.IsReady) return;
        foreach (var profile in YouTubePersonalCatalog.ActiveProfiles(config))
        {
            YouTubePersonalVisibility.MarkPersonalContent();
            items.Add(Folder("My YouTube", YouTubePersonalCatalog.Wrap(profile.ScopeId, "root"), FolderIcons.WatchLater));
        }
    }

    private async Task<ChannelItemResult> GetPersonalItemsAsync(string id, PluginConfiguration config, CancellationToken ct)
    {
        if (!YouTubePersonalCatalog.TryParse(id, out var scope, out var inner)
            || !YouTubePersonalVisibility.IsReady)
            return new ChannelItemResult { Items = new List<ChannelItemInfo>() };
        var profile = YouTubePersonalCatalog.ActiveProfiles(config).SingleOrDefault(p => p.ScopeId == scope);
        if (profile == null) return new ChannelItemResult { Items = new List<ChannelItemInfo>() };
        var usePersonalKey = config.EnablePersonalApiKeys && !string.IsNullOrWhiteSpace(profile.ApiKey);
        var key = usePersonalKey ? profile.ApiKey! : config.ApiKey;
        if (string.IsNullOrWhiteSpace(key))
            return Msg(new List<ChannelItemInfo>(), "Add an API key in My YouTube or ask your administrator.");
        var configurationHash = PluginEntryPoint.ComputeConfigHash(config);
        var catalogRevision = Interlocked.Read(ref YouTubePersonalCatalog.Revision);
        using var context = new YouTubePersonalRequestContext(profile.UserId, profile.Revision, key, usePersonalKey,
            () => YouTubePersonalCatalog.IsCurrent(profile, configurationHash, catalogRevision));
        var saved = SplitConfiguredItems(config.SavedItems).ToList();
        if (config.EnablePersonalSubscriptions)
            saved.AddRange(config.PersonalChannelVisibility == "Shared"
                ? YouTubePersonalCatalog.SharedChannels(config) : profile.Channels);
        var local = new PluginConfiguration
        {
            ApiKey = key, SavedItems = string.Join("\n", saved.Distinct(StringComparer.Ordinal)),
            WatchLaterPlaylist = config.WatchLaterPlaylist, ShowTrending = config.ShowTrending,
            ShowCategories = config.ShowCategories, ShowShorts = config.ShowShorts, HideShorts = config.HideShorts,
            TrendingRegion = config.TrendingRegion, TrendingCategory = config.TrendingCategory,
            ShowLikeCount = config.ShowLikeCount, ShowCommentCount = config.ShowCommentCount,
            ChannelSortBy = config.ChannelSortBy, MaxChannelVideos = config.MaxChannelVideos,
            MaxSearchVideos = config.MaxSearchVideos
        };
        ChannelItemResult result;
        try
        {
            result = inner == "root"
                ? await BuildRootItemsAsync(key, local, ct).ConfigureAwait(false)
                : await GetStandardFolderAsync(inner, key, local, ct).ConfigureAwait(false);
            context.EnsureCurrent();
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            LogPublic("[YT] Personal channel refresh failed (" + error.GetType().Name + ").");
            return Msg(new List<ChannelItemInfo>(), "YouTube is temporarily unavailable. Check My YouTube settings.");
        }
        foreach (var item in result.Items)
            if (item.Type == ChannelItemType.Folder)
                item.Id = YouTubePersonalCatalog.Wrap(scope, item.Id);
        return result;
    }

    private static async Task<ChannelItemResult> GetStandardFolderAsync(string folderId,
        string apiKey, PluginConfiguration config, CancellationToken ct)
    {
        var separator = folderId.IndexOf(FolderSeparator, StringComparison.Ordinal);
        if (separator < 1) return new ChannelItemResult { Items = new List<ChannelItemInfo>() };
        var type = folderId.Substring(0, separator);
        var term = folderId.Substring(separator + FolderSeparator.Length);
        if (type == "channel") return await BuildChannelSubfoldersAsync(apiKey, config, term, ct).ConfigureAwait(false);
        if (type == "trending") return await LoadTrending(apiKey, ct,
            (config.TrendingRegion ?? "").Trim(), (config.TrendingCategory ?? "").Trim()).ConfigureAwait(false);
        if (type == "categories" && term == "root") return await LoadCategoryRootAsync(apiKey, config, ct).ConfigureAwait(false);
        if (type == "category") return await LoadTrending(apiKey, ct,
            string.IsNullOrWhiteSpace(config.TrendingRegion) ? "US" : config.TrendingRegion.Trim(), term).ConfigureAwait(false);
        return await LoadMediaFolderAsync(apiKey, config, type, term, ct).ConfigureAwait(false);
    }
}

internal static class YouTubePersonalCatalog
{
    internal const string Prefix = "personal_x_";
    private const string HarmonyId = "emby.youtubeplugin.personal-catalog";
    private static IUserManager? _users;
    private static YouTubePersonalProfiles.Snapshot[] _profiles = Array.Empty<YouTubePersonalProfiles.Snapshot>();
    private static object? _harmony;
    private static long _channelId;
    private static int _refreshQueued;
    private static CancellationTokenSource? _lifetime;
    internal static long Revision;

    internal static string Wrap(string scope, string inner) => Prefix + scope + "_" + inner;

    internal static bool TryParse(string? value, out string scope, out string inner)
    {
        scope = inner = string.Empty;
        if (value == null || !value.StartsWith(Prefix, StringComparison.Ordinal)
            || value.Length <= Prefix.Length + 33 || value[Prefix.Length + 32] != '_') return false;
        scope = value.Substring(Prefix.Length, 32);
        if (!Guid.TryParseExact(scope, "N", out _) || scope != scope.ToLowerInvariant()) return false;
        inner = value.Substring(Prefix.Length + 33);
        return inner.Length > 0 && !inner.StartsWith(Prefix, StringComparison.Ordinal);
    }

    internal static void Start(ILibraryManager library, IUserManager users, IUserViewManager views)
    {
        _lifetime = new CancellationTokenSource();
        _channelId = 0;
        _refreshQueued = 0;
        _users = users;
        ReloadProfiles();
        YouTubePersonalProfiles.Changed += ProfilesChanged;
        try
        {
            var dto = Plugin.ResolveService<IDtoService>();
            var assembly = EmbeddedDependencyLoader.LoadHarmonyAssembly();
            var harmonyType = assembly?.GetType("HarmonyLib.Harmony");
            var methodType = assembly?.GetType("HarmonyLib.HarmonyMethod");
            if (dto == null || harmonyType == null || methodType == null) return;
            _harmony = Activator.CreateInstance(harmonyType, HarmonyId);
            if (_harmony == null) return;
            YouTubePersonalVisibility.Initialize(library, views, dto, () =>
            {
                if (_channelId > 0) return _channelId;
                var channels = library.GetItemList(new InternalItemsQuery
                    { IncludeItemTypes = new[] { "Channel" }, Recursive = true }, CancellationToken.None).OfType<Channel>();
                var manager = Plugin.ResolveService<IChannelManager>();
                var providerMethod = manager?.GetType().GetMethod("GetChannelProvider",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, new[] { typeof(Channel) }, null);
                foreach (var channel in channels)
                    if (providerMethod?.Invoke(manager, new object[] { channel }) is YouTubeChannel)
                        return _channelId = channel.InternalId;
                return 0;
            }, MayView);
            var count = YouTubePersonalVisibility.Install(_harmony, methodType);
            YouTubeChannel.LogPublic("[YT] Personal catalog visibility " + (YouTubePersonalVisibility.IsReady
                ? "ready (" + count + " bindings)." : "unavailable; new personal folders are disabled. Check server compatibility."));
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            YouTubeChannel.LogPublic("[YT] Personal catalog unavailable (" + error.GetType().Name + ").");
        }
    }

    internal static YouTubePersonalProfiles.Snapshot[] ActiveProfiles(PluginConfiguration config)
    {
        if (!config.EnablePersonalApiKeys && !config.EnablePersonalSubscriptions) return Array.Empty<YouTubePersonalProfiles.Snapshot>();
        var users = _users?.GetUserList(new UserQuery()) ?? Array.Empty<User>();
        var enabled = users.Where(user => !_users!.GetUserPolicy(user).IsDisabled).Select(user => user.Id).ToHashSet();
        return Volatile.Read(ref _profiles).Where(profile => enabled.Contains(profile.UserId)
            && (config.EnablePersonalApiKeys && !string.IsNullOrWhiteSpace(profile.ApiKey)
                || config.EnablePersonalSubscriptions && profile.Channels.Length > 0)).ToArray();
    }

    internal static string[] SharedChannels(PluginConfiguration config) =>
        config.EnablePersonalSubscriptions && config.PersonalChannelVisibility == "Shared"
            ? ActiveProfiles(config).SelectMany(profile => profile.Channels).Distinct(StringComparer.Ordinal).ToArray()
            : Array.Empty<string>();

    internal static bool HasContentFor(PluginConfiguration config, Guid userId) =>
        ActiveProfiles(config).Any(profile => profile.UserId == userId
            || config.EnablePersonalSubscriptions && config.PersonalChannelVisibility == "Shared"
                && profile.Channels.Length > 0);

    private static bool MayView(string externalId, long userId)
    {
        if (userId <= 0 || !TryParse(externalId, out var scope, out _)) return false;
        var config = Plugin.Instance?.Options;
        var user = _users?.GetUserById(userId);
        if (config == null || user == null || _users!.GetUserPolicy(user).IsDisabled) return false;
        return Volatile.Read(ref _profiles).Any(profile => profile.ScopeId == scope && profile.UserId == user.Id
            && IsEnabled(profile, config));
    }

    internal static bool IsCurrent(YouTubePersonalProfiles.Snapshot original, string configurationHash, long catalogRevision)
    {
        if (_lifetime == null || _lifetime.IsCancellationRequested) return false;
        if (catalogRevision != Interlocked.Read(ref Revision)) return false;
        var config = Plugin.Instance?.Options;
        if (config == null || PluginEntryPoint.ComputeConfigHash(config) != configurationHash) return false;
        var user = _users?.GetUserList(new UserQuery()).FirstOrDefault(candidate => candidate.Id == original.UserId);
        if (user == null || _users!.GetUserPolicy(user).IsDisabled) return false;
        return Volatile.Read(ref _profiles).Any(profile => profile.UserId == original.UserId
            && profile.ScopeId == original.ScopeId && profile.Revision == original.Revision && IsEnabled(profile, config));
    }

    private static bool IsEnabled(YouTubePersonalProfiles.Snapshot profile, PluginConfiguration config) =>
        config.EnablePersonalApiKeys && !string.IsNullOrWhiteSpace(profile.ApiKey)
        || config.EnablePersonalSubscriptions && profile.Channels.Length > 0;

    private static void ReloadProfiles()
    {
        try { Volatile.Write(ref _profiles, YouTubePersonalProfiles.Current?.List() ?? Array.Empty<YouTubePersonalProfiles.Snapshot>()); }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            Volatile.Write(ref _profiles, Array.Empty<YouTubePersonalProfiles.Snapshot>());
            YouTubeChannel.LogPublic("[YT] Personal profiles unavailable (" + error.GetType().Name + ").");
        }
        Interlocked.Increment(ref Revision);
    }

    private static void ProfilesChanged()
    {
        ReloadProfiles();
        var lifetime = _lifetime;
        if (lifetime == null || lifetime.IsCancellationRequested) return;
        CancellationToken token;
        try { token = lifetime.Token; }
        catch (ObjectDisposedException) { return; }
        if (Interlocked.Exchange(ref _refreshQueued, 1) != 0) return;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), token).ConfigureAwait(false);
                Interlocked.Exchange(ref _refreshQueued, 0);
                token.ThrowIfCancellationRequested();
                await ChannelRefreshInvoker.TriggerRefreshAsync(4).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();
                PluginEntryPoint.RequestHomeSectionSync("personal settings changed");
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                Interlocked.Exchange(ref _refreshQueued, 0);
                YouTubeChannel.LogPublic("[YT] Personal refresh unavailable (" + error.GetType().Name + ").");
            }
        });
    }

    internal static void Stop()
    {
        YouTubePersonalProfiles.Changed -= ProfilesChanged;
        var lifetime = Interlocked.Exchange(ref _lifetime, null);
        lifetime?.Cancel();
        lifetime?.Dispose();
        try { _harmony?.GetType().GetMethod("UnpatchAll", new[] { typeof(string) })?.Invoke(_harmony, new object[] { HarmonyId }); }
        catch (Exception error) when (error is not OutOfMemoryException)
        { YouTubeChannel.LogPublic("[YT] Personal catalog shutdown incomplete (" + error.GetType().Name + ")."); }
        finally { YouTubePersonalVisibility.Shutdown(); }
        _harmony = null;
        _users = null;
        Volatile.Write(ref _profiles, Array.Empty<YouTubePersonalProfiles.Snapshot>());
    }
}
