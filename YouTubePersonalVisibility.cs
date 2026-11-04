using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Data;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Library;
using MediaBrowser.Model.Querying;
using System.Reflection;
using System.Reflection.Emit;

namespace Emby.YouTubePlugin
{
    // Presentation isolation for this provider's opaque personal folder tree.
    // This is not DRM for public YouTube videos, nor a restriction on Emby's
    // administrator impersonation. User is the native, server-authorized User.
    internal static class YouTubePersonalVisibility
    {
        private const string Prefix = "personal_x_";
        private static readonly object Sync = new();
        private static volatile State? _state;
        private static volatile bool _ready;
        private static bool _installed;
        private static readonly HashSet<MethodInfo> InstalledTargets = new();

        internal static bool IsReady => _ready;

        // Must be called before the provider returns any personal root. Only a
        // negative initial scan is cached; positive lineage and authorization
        // are never cached, so added folders and permission changes cannot leak.
        internal static void MarkPersonalContent()
        {
            var state = _state;
            if (state != null) Interlocked.Exchange(ref state.Presence, 2);
        }

        // Caller first removes this adapter's Harmony ID. Never leave callbacks
        // or a successful readiness flag from a previous plugin lifetime behind.
        internal static void Shutdown()
        {
            lock (Sync)
            {
                _ready = false;
                _installed = false;
                _state = null;
                InstalledTargets.Clear();
            }
        }

        internal static bool Initialize(ILibraryManager library, IUserViewManager views,
            IDtoService dto, Func<long> channelId, Func<string, long, bool> authorize)
        {
            ArgumentNullException.ThrowIfNull(library);
            ArgumentNullException.ThrowIfNull(views);
            ArgumentNullException.ThrowIfNull(dto);
            Configure(library.GetType(), views.GetType(), dto.GetType(),
                (query, context, cancellation) => library.GetItemList(query, context!, cancellation),
                id => library.GetItemById(id), channelId, authorize);
            return IsReady;
        }

        // Delegates keep native reads explicit and let isolated tests exercise
        // the production adapter against the installed server's real types.
        internal static void Configure(Type libraryType, Type viewsType, Type dtoType,
            Func<InternalItemsQuery, IDataContext?, CancellationToken, BaseItem[]> read,
            Func<long, BaseItem?> lookup, Func<long> channelId, Func<string, long, bool> authorize)
        {
            ArgumentNullException.ThrowIfNull(read);
            ArgumentNullException.ThrowIfNull(lookup);
            ArgumentNullException.ThrowIfNull(channelId);
            ArgumentNullException.ThrowIfNull(authorize);
            lock (Sync)
            {
                _state = new State(libraryType, viewsType, dtoType, read, lookup, channelId, authorize);
                _ready = _installed && FindTargets(libraryType, viewsType, dtoType).Length == 4;
            }
        }

        internal static int Install(object harmony, Type harmonyMethodType)
        {
            lock (Sync)
            {
                if (_installed) return _ready ? 4 : 0;
                _ready = false;
                var state = _state;
                if (state == null) return 0;
                try
                {
                    var targets = CandidateTargets(state.LibraryType, state.ViewsType, state.DtoType);
                    bool complete = FindTargets(state.LibraryType, state.ViewsType, state.DtoType).Length == 4;
                    var patch = harmony.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .SingleOrDefault(method => method.Name == "Patch"
                            && method.GetParameters().Length >= 3
                            && method.GetParameters()[0].ParameterType == typeof(MethodBase)
                            && method.GetParameters().Any(parameter => parameter.Name == "prefix")
                            && method.GetParameters().Any(parameter => parameter.Name == "postfix"));
                    if (patch == null) return 0;
                    string[] handlers = { nameof(QueryPostfix), nameof(ViewsPostfix), nameof(OneDtoPrefix), nameof(ManyDtoPrefix) };
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (targets[i] == null || i == 0 && !complete) continue;
                        if (InstalledTargets.Contains(targets[i]!)) continue;
                        var handler = typeof(YouTubePersonalVisibility).GetMethod(handlers[i], BindingFlags.NonPublic | BindingFlags.Static)!;
                        var descriptor = Activator.CreateInstance(harmonyMethodType, handler)
                            ?? throw new InvalidOperationException("Native visibility patch unavailable.");
                        var parameters = patch.GetParameters();
                        var arguments = new object?[parameters.Length];
                        arguments[0] = targets[i];
                        for (int j = 1; j < parameters.Length; j++)
                            if (parameters[j].Name == (i < 2 ? "postfix" : "prefix")) arguments[j] = descriptor;
                        patch.Invoke(harmony, arguments);
                        InstalledTargets.Add(targets[i]!);
                    }
                    _installed = complete;
                    _ready = complete;
                    return InstalledTargets.Count;
                }
                catch (Exception error) when (error is not OutOfMemoryException)
                {
                    // Successful partial guards remain deny-only. The provider
                    // must not emit personal roots until all bindings are ready.
                    _ready = false;
                    return 0;
                }
            }
        }

        internal static MethodInfo[] FindTargets(Type library, Type views, Type dto)
        {
            var candidates = CandidateTargets(library, views, dto);
            if (candidates.Any(method => method == null) || library.Assembly != views.Assembly || library.Assembly != dto.Assembly)
                return Array.Empty<MethodInfo>();
            try
            {
                var parameters = new[] { typeof(InternalItemsQuery), typeof(IDataContext), typeof(CancellationToken) };
                var list = Exact(library, "GetItemList", typeof(BaseItem[]), parameters);
                var result = Exact(library, "GetItemsResult", typeof(QueryResult<BaseItem>), parameters);
                var repository = library.Assembly.GetType("Emby.Server.Implementations.Data.SqliteItemRepository");
                var where = repository == null ? null : Exact(repository, "GetWhereClauses", typeof(List<string>),
                    typeof(InternalItemsQuery), typeof(List<KeyValuePair<string, string>>), typeof(string), typeof(bool));
                // Structural checks work across operating systems and compatible
                // Emby builds. A matching name alone is not a supported binding.
                if (list == null || result == null || where == null
                    || !NormalizesBeforeRead(list, candidates[0]!) || !NormalizesBeforeRead(result, candidates[0]!))
                    return Array.Empty<MethodInfo>();
                var calls = Calls(where).ToArray();
                if (!calls.Any(method => method.DeclaringType == typeof(InternalItemsQuery) && method.Name == "get_ExcludeItemIds")
                    || !calls.Any(method => method.DeclaringType == typeof(InternalItemsQuery) && method.Name == "get_ExcludeParentIds")
                    || !calls.Any(method => method.Name == "GetParameterizedNotInOrEquals" && method.DeclaringType?.FullName == "Emby.Sqlite.BaseSqliteRepository"))
                    return Array.Empty<MethodInfo>();
                return candidates.Select(method => method!).ToArray();
            }
            catch (Exception error) when (error is not OutOfMemoryException) { return Array.Empty<MethodInfo>(); }
        }

        private static MethodInfo?[] CandidateTargets(Type library, Type views, Type dto)
        {
            bool Native(Type type, string name, Type contract) => type.FullName == name
                && type.Assembly.GetName().Name == "Emby.Server.Implementations" && contract.IsAssignableFrom(type);
            bool lib = Native(library, "Emby.Server.Implementations.Library.LibraryManager", typeof(ILibraryManager));
            bool view = Native(views, "Emby.Server.Implementations.Library.UserViewManager", typeof(IUserViewManager));
            bool dtos = Native(dto, "Emby.Server.Implementations.Dto.DtoService", typeof(IDtoService));
            return new[]
            {
                lib ? Exact(library, "SetUserAndParents", typeof(void), typeof(InternalItemsQuery), typeof(IDataContext), typeof(CancellationToken)) : null,
                view ? Exact(views, "GetUserViews", typeof(Folder[]), typeof(UserViewQuery), typeof(User), typeof(Folder[]), typeof(IDataContext), typeof(CancellationToken)) : null,
                dtos ? Exact(dto, "GetBaseItemDto", typeof(BaseItemDto), typeof(BaseItem), typeof(DtoOptions), typeof(User), typeof(CancellationToken)) : null,
                dtos ? Exact(dto, "GetBaseItemDtos", typeof(BaseItemDto[]), typeof(BaseItem[]), typeof(DtoOptions), typeof(User), typeof(CancellationToken)) : null
            };
        }

        private static bool NormalizesBeforeRead(MethodInfo method, MethodInfo normalize)
        {
            bool normalized = false, read = false;
            foreach (var call in Calls(method))
            {
                if (call == normalize) normalized = true;
                if (call.DeclaringType?.Name == "IItemRepository" && call.Name is "GetItemList" or "GetItems")
                {
                    if (!normalized) return false;
                    read = true;
                }
            }
            return normalized && read;
        }

        private static readonly Dictionary<short, OpCode> OpCodesByValue = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(field => field.FieldType == typeof(OpCode)).Select(field => (OpCode)field.GetValue(null)!)
            .ToDictionary(code => code.Value);

        private static IEnumerable<MethodBase> Calls(MethodInfo method)
        {
            var il = method.GetMethodBody()?.GetILAsByteArray() ?? throw new InvalidOperationException();
            for (int offset = 0; offset < il.Length;)
            {
                short value = il[offset++];
                if (value == 0xfe) value = (short)(0xfe00 | il[offset++]);
                var code = OpCodesByValue[value];
                int size = code.OperandType switch
                {
                    OperandType.InlineNone => 0,
                    OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
                    OperandType.InlineVar => 2,
                    OperandType.InlineI8 or OperandType.InlineR => 8,
                    OperandType.InlineSwitch => 4 + 4 * BitConverter.ToInt32(il, offset),
                    _ => 4
                };
                if (code == OpCodes.Call || code == OpCodes.Callvirt)
                    yield return method.Module.ResolveMethod(BitConverter.ToInt32(il, offset))!;
                offset += size;
            }
        }

        private static MethodInfo? Exact(Type type, string name, Type result, params Type[] parameters)
        {
            var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameters, null);
            return method != null && method.DeclaringType == type && method.ReturnType == result
                && !method.IsAbstract && !method.IsGenericMethod && method.GetMethodBody() != null ? method : null;
        }

        private static void QueryPostfix(InternalItemsQuery __0, IDataContext? __1, CancellationToken __2)
            => GuardQuery(__0, __1, __2);

        internal static void GuardQuery(InternalItemsQuery query, IDataContext? context = null, CancellationToken cancellation = default)
        {
            var state = _state;
            // The native channel refresh intentionally materializes every user's
            // tree with no user. Filtering that refresh would delete other trees.
            long userId = query.User?.InternalId ?? 0;
            if (state == null || userId <= 0) return;
            long channel = state.ChannelId();
            if (channel <= 0) return;
            var folders = state.Read(new InternalItemsQuery
            {
                AncestorIds = new[] { channel }, IsFolder = true, Recursive = true,
                GroupByPresentationUniqueKey = false, EnableTotalRecordCount = false,
                ForceOriginalFolders = true
            }, context, cancellation);
            var byId = folders.Where(item => item.InternalId > 0)
                .GroupBy(item => item.InternalId).ToDictionary(group => group.Key, group => group.First());
            var denied = new HashSet<long>();
            bool containsPersonal = false;
            foreach (var folder in folders)
            {
                cancellation.ThrowIfCancellationRequested();
                if (!IsGuarded(folder, channel)) continue;
                containsPersonal = true;
                if (!Allowed(state, folder.ExternalId, userId)) denied.Add(folder.InternalId);
            }
            if (containsPersonal) Interlocked.Exchange(ref state.Presence, 2);
            else Interlocked.CompareExchange(ref state.Presence, 1, 0);
            // Also cover a legacy/unwrapped intermediate folder. Every video is
            // excluded by its direct parent; video IDs and source data stay intact.
            foreach (var folder in folders)
            {
                var cursor = folder;
                var seen = new HashSet<long>();
                while (cursor.ParentId > 0 && seen.Add(cursor.InternalId))
                {
                    if (denied.Contains(cursor.ParentId)) { denied.Add(folder.InternalId); break; }
                    if (!byId.TryGetValue(cursor.ParentId, out cursor!)) break;
                }
            }
            if (denied.Count == 0) return;
            // These are native WHERE predicates, not page-size-changing result
            // filters. Existing caller exclusions, sort and paging are retained.
            query.ExcludeItemIds = (query.ExcludeItemIds ?? Array.Empty<long>()).Concat(denied).Distinct().ToArray();
            query.ExcludeParentIds = (query.ExcludeParentIds ?? Array.Empty<long>()).Concat(denied).Distinct().ToArray();
        }

        private static void ViewsPostfix(User __1, ref Folder[] __result)
        {
            if (__1?.InternalId > 0 && __result != null)
                __result = __result.Where(item => CanSee(item, __1.InternalId)).ToArray();
        }

        private static void OneDtoPrefix(ref BaseItem __0, User __2)
        {
            if (__2?.InternalId > 0)
            {
                if (!CanSee(__0, __2.InternalId))
                    throw new ResourceNotFoundException(ResourceExceptionType.LibraryItem, string.Empty);
                __0 = RequestItem(__0);
            }
        }

        private static void ManyDtoPrefix(ref BaseItem[] __0, User __2)
        {
            if (__2?.InternalId > 0 && __0 != null)
                __0 = __0.Where(item => CanSee(item, __2.InternalId)).Select(RequestItem).ToArray();
        }

        internal static bool CanSee(BaseItem? item, long userId)
        {
            var state = _state;
            if (item == null || userId <= 0) return true;
            if (state == null) return !LooksGuarded(item.ExternalId);
            long channel = state.ChannelId();
            var guarded = new List<string>();
            bool providerHint = false;
            var seen = new HashSet<long>();
            var cursor = item;
            // Ownership is meaningful only inside this provider's verified tree.
            while (cursor != null && (channel <= 0 || cursor.InternalId != channel) && seen.Add(cursor.InternalId))
            {
                providerHint |= LooksGuarded(cursor.ExternalId);
                if (IsGuarded(cursor, channel)) guarded.Add(cursor.ExternalId);
                // Parentless persons/genres/studios and incomplete unrelated
                // trees are native Emby content, not evidence of private data.
                // A provider-prefixed orphan remains denied. A complete known
                // foreign folder tree wins over coincidentally matching IDs.
                if (cursor.ParentId <= 0)
                    return !providerHint || cursor is Folder && !LooksGuarded(cursor.ExternalId);
                cursor = state.Lookup(cursor.ParentId);
            }
            if (channel > 0 && cursor?.InternalId == channel)
                return guarded.All(externalId => Allowed(state, externalId, userId));
            // Missing or cyclic ancestry only fails closed with an actual
            // provider prefix on this item or one of its observed ancestors.
            return !providerHint;
        }

        private static bool LooksGuarded(string? id) => id?.StartsWith(Prefix, StringComparison.Ordinal) == true
            || id?.StartsWith("personalshared_x_", StringComparison.Ordinal) == true
            || id?.StartsWith("channel_x_", StringComparison.Ordinal) == true;

        private static bool IsGuarded(BaseItem item, long channel) =>
            item.ExternalId?.StartsWith(Prefix, StringComparison.Ordinal) == true
            || item.ExternalId?.StartsWith("personalshared_x_", StringComparison.Ordinal) == true
            || item.ParentId == channel && item.ExternalId?.StartsWith("channel_x_", StringComparison.Ordinal) == true;

        private static readonly MethodInfo ShallowCopy = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private static BaseItem RequestItem(BaseItem item)
        {
            var state = _state;
            if (state == null || item == null) return item!;
            var channel = state.ChannelId();
            var cursor = item;
            var seen = new HashSet<long>();
            while (cursor != null && cursor.InternalId != channel && seen.Add(cursor.InternalId))
                cursor = cursor.ParentId > 0 ? state.Lookup(cursor.ParentId) : null;
            if (channel <= 0 || cursor?.InternalId != channel) return item;
            // Counts on cached BaseItem instances carry no UserId. The native
            // DTO service must recompute them on a request-local instance.
            var copy = (BaseItem)ShallowCopy.Invoke(item, null)!;
            copy.UserDataSet = false;
            copy.ChildCount = null;
            copy.RecursiveItemCount = null;
            return copy;
        }

        private static bool Allowed(State state, string externalId, long userId)
        {
            if (!IsReady) return false;
            try { return state.Authorize(externalId, userId); }
            catch (Exception error) when (error is not OutOfMemoryException) { return false; }
        }

        private sealed record State(Type LibraryType, Type ViewsType, Type DtoType,
            Func<InternalItemsQuery, IDataContext?, CancellationToken, BaseItem[]> Read,
            Func<long, BaseItem?> Lookup, Func<long> ChannelId, Func<string, long, bool> Authorize)
        {
            // 0 unknown, 1 initially absent, 2 observed/emission scheduled (sticky).
            internal int Presence;
        }
    }
}
