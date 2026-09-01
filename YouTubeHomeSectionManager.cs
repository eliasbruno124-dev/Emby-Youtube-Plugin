using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Library;
using MediaBrowser.Model.Querying;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    /// <summary>
    /// Keeps one opt-in Emby home row per saved YouTube channel. Emby 4.9 can
    /// read home sections but cannot write them; the write surface was added in
    /// 4.10, so every mutation is discovered and invoked at runtime.
    /// </summary>
    internal sealed class YouTubeHomeSectionManager
    {
        // AddHomeSection preserves a non-empty caller-provided id. The plugin id
        // in this prefix makes ownership unambiguous, so we never have to infer
        // ownership from a user-editable row name or ParentId.
        private const string ManagedIdPrefix =
            "emby-youtube-b2c3d4e5f6a74b5c9d0e1f2a3b4c5d6e-latest-";
        private const string AggregateBackupDirectoryName = "youtube-home-aggregate-backups";
        private const string LatestMediaBlockFilterDirectoryName =
            "youtube-home-latest-media-block-filters";
        private const string LegacyAggregateName = "Neueste YouTube-Videos";
        private const string LatestMediaBlockSectionType = "latestmediablock";
        private static readonly JsonSerializerOptions BackupJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        private readonly IUserManager _userManager;
        private readonly IUserViewManager _userViewManager;
        private readonly MethodInfo? _addHomeSection;
        private readonly MethodInfo? _updateHomeSection;
        private readonly MethodInfo? _deleteHomeSections;
        private readonly MethodInfo? _moveHomeSections;
        private readonly SemaphoreSlim _syncGate = new(1, 1);
        private int _unsupportedLogged;

        private sealed class AggregateBackup
        {
            public string SectionId { get; set; } = string.Empty;
            public string SectionJson { get; set; } = string.Empty;
            public int OriginalIndex { get; set; } = -1;
            public DateTime SavedAtUtc { get; set; }
        }

        private sealed class LatestMediaBlockFilterState
        {
            public int Version { get; set; } = 1;
            public List<LatestMediaBlockFilterEntry> Entries { get; set; } = new();
        }

        private sealed class LatestMediaBlockFilterEntry
        {
            public string SectionId { get; set; } = string.Empty;
            public List<string> AddedExcludedFolderIds { get; set; } = new();
            public DateTime SavedAtUtc { get; set; }
        }

        private sealed record AggregateFolderContext(
            HashSet<string> AllViewIds,
            HashSet<string> YouTubeRootIds,
            string YouTubeViewId);

        public YouTubeHomeSectionManager(
            IUserManager userManager,
            IUserViewManager userViewManager)
        {
            _userManager = userManager;
            _userViewManager = userViewManager;
            _addHomeSection = FindSectionMutation("AddHomeSection", typeof(ContentSection));
            _updateHomeSection = FindSectionMutation("UpdateHomeSection", typeof(ContentSection));
            _deleteHomeSections = FindSectionMutation("DeleteHomeSections", typeof(string[]));
            _moveHomeSections = FindMoveHomeSections();
        }

        private bool SupportsHomeSectionWrites =>
            _addHomeSection != null
            && _updateHomeSection != null
            && _deleteHomeSections != null;

        public async Task SyncAsync(CancellationToken cancellationToken)
        {
            if (!SupportsHomeSectionWrites)
            {
                if (Interlocked.Exchange(ref _unsupportedLogged, 1) == 0)
                {
                    YouTubeChannel.LogPublic(
                        "[YT] Per-channel Home rows are unavailable because this Emby runtime "
                        + "does not expose the 4.10 HomeSections write API. Top-level root-folder "
                        + "display remains available.");
                }
                return;
            }

            await _syncGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var config = Plugin.Instance?.Options;
                if (config == null)
                    return;

                var enabled = config.ShowRootFoldersAtTopLevel;
                var hasConfiguredChannels = YouTubeChannel.HasConfiguredSavedChannels(config.SavedItems);
                var users = _userManager.GetUserList(new UserQuery()) ?? Array.Empty<User>();

                foreach (var user in users)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        var userEnabled = !_userManager.GetUserPolicy(user).IsDisabled;
                        await SyncUserAsync(
                                user,
                                enabled,
                                enabled && userEnabled,
                                hasConfiguredChannels,
                                cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        YouTubeChannel.LogPublic(
                            $"[YT] Home row sync failed for user {user.Name}: {ex.Message}");
                    }
                }
            }
            finally
            {
                _syncGate.Release();
            }
        }

        private async Task SyncUserAsync(
            User user,
            bool globallyEnabled,
            bool enabled,
            bool hasConfiguredChannels,
            CancellationToken cancellationToken)
        {
            var homeSections = _userManager.GetHomeSections(user.InternalId, cancellationToken);
            var current = homeSections?.Sections;

            // On 4.10 the REST save path first materializes Emby's implicit
            // default sections. Calling AddHomeSection directly while this list
            // is empty would instead turn the first plugin row into the user's
            // complete saved layout and make the legacy defaults disappear.
            if (current == null || current.Length == 0)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Home row sync skipped for user {user.Name}: HomeSections are still implicit/empty.");
                return;
            }

            IReadOnlyList<Folder>? roots = Array.Empty<Folder>();
            AggregateFolderContext? aggregateContext = null;
            if (enabled)
            {
                roots = GetSavedChannelRoots(
                    user,
                    hasConfiguredChannels,
                    out aggregateContext);
                if (roots == null)
                {
                    // A configured channel can temporarily be absent while Emby
                    // is refreshing the dynamic channel tree. Preserve existing
                    // managed rows until the post-refresh sync has authoritative
                    // roots instead of deleting them during that gap.
                    YouTubeChannel.LogPublic(
                        $"[YT] Home row sync deferred for user {user.Name}: saved channel roots are not materialized yet.");
                    return;
                }
            }

            var desired = roots
                .Select(BuildSection)
                .GroupBy(section => section.Id, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToDictionary(section => section.Id, StringComparer.OrdinalIgnoreCase);
            var managedGroups = current
                .Where(IsManaged)
                .GroupBy(section => section.Id, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var duplicateDesiredGroups = managedGroups
                .Where(group => group.Count() > 1 && desired.ContainsKey(group.Key))
                .ToArray();

            // Emby treats section ids case-insensitively. If a damaged layout
            // already contains duplicate plugin-owned ids, Update cannot target
            // one copy. Remove those owned duplicates and recreate exactly one
            // desired row rather than silently hiding them in a dictionary.
            foreach (var group in duplicateDesiredGroups)
            {
                await InvokeMutationAsync(
                        _deleteHomeSections!,
                        user.InternalId,
                        new[] { group.Key },
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            var duplicateDesiredIds = duplicateDesiredGroups
                .Select(group => group.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var managed = managedGroups
                .Where(group => !duplicateDesiredIds.Contains(group.Key))
                .ToDictionary(
                    group => group.Key,
                    group => group.First(),
                    StringComparer.OrdinalIgnoreCase);

            var added = 0;
            var updated = 0;
            var removed = duplicateDesiredGroups.Sum(group => group.Count());
            foreach (var pair in desired.OrderBy(pair => pair.Value.Name, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!managed.TryGetValue(pair.Key, out var existing))
                {
                    await InvokeMutationAsync(
                            _addHomeSection!,
                            user.InternalId,
                            pair.Value,
                            cancellationToken)
                        .ConfigureAwait(false);
                    added++;
                }
                else if (NeedsUpdate(existing, pair.Value))
                {
                    var update = ApplyManagedValues(existing, pair.Value);
                    await InvokeMutationAsync(
                            _updateHomeSection!,
                            user.InternalId,
                            update,
                            cancellationToken)
                        .ConfigureAwait(false);
                    updated++;
                }
            }

            // Delete only ids carrying the plugin ownership prefix, and only
            // after every desired create/update succeeded. Manual rows remain
            // untouched even when they use the same ParentId or display name.
            var staleIds = managed.Keys
                .Where(id => !desired.ContainsKey(id))
                .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (staleIds.Length > 0)
            {
                await InvokeMutationAsync(
                        _deleteHomeSections!,
                        user.InternalId,
                        staleIds,
                        cancellationToken)
                    .ConfigureAwait(false);

                removed += managedGroups
                    .Where(group => staleIds.Contains(group.Key, StringComparer.OrdinalIgnoreCase))
                    .Sum(group => group.Count());
            }

            var aggregateSuppressed = 0;
            var aggregateRestored = 0;
            var latestMediaBlockFiltered = 0;
            var latestMediaBlockRestored = 0;
            if (enabled && desired.Count > 0 && aggregateContext != null)
            {
                aggregateSuppressed = await SuppressLegacyAggregateAsync(
                        user,
                        aggregateContext,
                        desired,
                        cancellationToken)
                    .ConfigureAwait(false);
                latestMediaBlockFiltered = await FilterLatestMediaBlocksAsync(
                        user,
                        aggregateContext,
                        desired,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            else if (!globallyEnabled)
            {
                aggregateRestored = await RestoreLegacyAggregateAsync(
                        user,
                        cancellationToken)
                    .ConfigureAwait(false);
                latestMediaBlockRestored = await RestoreLatestMediaBlockAsync(
                        user,
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            if (added > 0
                || updated > 0
                || removed > 0
                || aggregateSuppressed > 0
                || aggregateRestored > 0
                || latestMediaBlockFiltered > 0
                || latestMediaBlockRestored > 0)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Home rows synchronized for user {user.Name}: "
                    + $"added={added}, updated={updated}, removed={removed}, "
                    + $"aggregateSuppressed={aggregateSuppressed}, "
                    + $"aggregateRestored={aggregateRestored}, "
                    + $"latestMediaBlockFiltered={latestMediaBlockFiltered}, "
                    + $"latestMediaBlockRestored={latestMediaBlockRestored}.");
            }
        }

        /// <summary>
        /// Returns null when saved channel inputs exist but their dynamic root
        /// items have not been materialized yet. An empty list is authoritative
        /// when the user has no YouTube view/access or no channels are configured.
        /// </summary>
        private IReadOnlyList<Folder>? GetSavedChannelRoots(
            User user,
            bool hasConfiguredChannels,
            out AggregateFolderContext? aggregateContext)
        {
            aggregateContext = null;
            // With dynamic children disabled Emby returns the YouTube Channel
            // object itself. With them enabled, it replaces that Channel with
            // its root folders. We need both calls: the first gives us the
            // authoritative provider id, the second gives us its children.
            var topLevelViews = GetUserViews(user.InternalId, allowDynamicChildren: false);
            var youtubeCandidates = topLevelViews
                .OfType<Channel>()
                .Where(view => string.Equals(
                    view.Name,
                    Plugin.Instance?.Name ?? "YouTube",
                    StringComparison.OrdinalIgnoreCase))
                .ToArray();

            // No provider view means this user does not currently have access;
            // stale managed rows should therefore be removed.
            if (youtubeCandidates.Length == 0)
                return Array.Empty<Folder>();

            var dynamicViews = GetUserViews(user.InternalId, allowDynamicChildren: true);
            var youtubeView = youtubeCandidates.FirstOrDefault(candidate =>
                    dynamicViews.Any(view =>
                        view.ParentId == candidate.InternalId
                        && YouTubeChannel.IsSavedChannelRootExternalId(view.ExternalId)))
                ?? youtubeCandidates[0];

            // The legacy aggregate was created from the REST /Views surface,
            // whose UserViewQuery includes Live TV by default. Build its exact
            // comparison domain separately; excluding Live TV here makes an
            // otherwise exact ExcludedFolders set fail the strict matcher.
            var aggregateViews = GetUserViews(
                user.InternalId,
                allowDynamicChildren: true,
                includeLiveTvView: true);
            var allViewIds = aggregateViews
                .Select(view => view.GetClientId())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var youtubeRootIds = aggregateViews
                .Where(view => view.ParentId == youtubeView.InternalId)
                .Select(view => view.GetClientId())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            aggregateContext = new AggregateFolderContext(
                allViewIds,
                youtubeRootIds,
                youtubeView.GetClientId());

            var roots = dynamicViews
                .Where(view =>
                    view.ParentId == youtubeView.InternalId
                    && YouTubeChannel.IsSavedChannelRootExternalId(view.ExternalId))
                .GroupBy(view => view.ExternalId, StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(view => view.ExternalId, StringComparer.Ordinal)
                .ToArray();

            if (roots.Length == 0 && hasConfiguredChannels)
                return null;

            return roots;
        }

        private Folder[] GetUserViews(
            long userId,
            bool allowDynamicChildren,
            bool includeLiveTvView = false) =>
            _userViewManager.GetUserViews(new UserViewQuery
            {
                UserId = userId,
                IncludeExternalContent = true,
                AllowDynamicChildren = allowDynamicChildren,
                IncludeLiveTVView = includeLiveTvView,
                IncludeHidden = false
            }) ?? Array.Empty<Folder>();

        private async Task<int> SuppressLegacyAggregateAsync(
            User user,
            AggregateFolderContext context,
            IReadOnlyDictionary<string, ContentSection> desired,
            CancellationToken cancellationToken)
        {
            // Re-read after all per-channel creates/updates. A section that was
            // renamed or otherwise changed concurrently must not be deleted
            // based on the stale snapshot taken at the start of this sync.
            var current = GetCurrentSections(user, cancellationToken);
            if (!AreDesiredRowsReady(current, desired))
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Legacy aggregate row left unchanged for user {user.Name}: "
                    + "not all per-channel rows are present in the HomeSections readback.");
                return 0;
            }

            var matches = current
                .Select((section, index) => (Section: section, Index: index))
                .Where(match => IsStrictLegacyAggregate(match.Section, context))
                .ToArray();
            if (matches.Length == 0)
            {
                LogLegacyAggregateNoMatch(user, current, context);
                return 0;
            }
            if (matches.Length > 1)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Legacy aggregate row left unchanged for user {user.Name}: "
                    + $"{matches.Length} strict matches are ambiguous.");
                return 0;
            }

            var candidate = matches[0];
            var duplicateIdCount = current.Count(section => string.Equals(
                section.Id,
                candidate.Section.Id,
                StringComparison.OrdinalIgnoreCase));
            if (duplicateIdCount != 1)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Legacy aggregate row left unchanged for user {user.Name}: "
                    + $"id {candidate.Section.Id} occurs {duplicateIdCount} times.");
                return 0;
            }

            if (!TryLoadAggregateBackup(user, out var existingBackup, out _))
                return 0;
            if (existingBackup != null)
            {
                if (!string.Equals(
                        existingBackup.SectionId,
                        candidate.Section.Id,
                        StringComparison.OrdinalIgnoreCase))
                {
                    YouTubeChannel.LogPublic(
                        $"[YT] Legacy aggregate row left unchanged for user {user.Name}: "
                        + "a different aggregate id is already backed up.");
                    return 0;
                }
                if (!MatchesAggregateSnapshot(candidate.Section, existingBackup))
                {
                    YouTubeChannel.LogPublic(
                        $"[YT] Legacy aggregate row left unchanged for user {user.Name}: "
                        + "the live section differs from its existing backup.");
                    return 0;
                }
            }
            else if (!TryStoreAggregateBackup(user, candidate.Section, candidate.Index))
            {
                // Never delete a row unless its complete runtime ContentSection
                // was durably saved first.
                return 0;
            }

            await InvokeMutationAsync(
                    _deleteHomeSections!,
                    user.InternalId,
                    new[] { candidate.Section.Id },
                    cancellationToken)
                .ConfigureAwait(false);

            if (GetCurrentSections(user, cancellationToken).Any(section => string.Equals(
                    section.Id,
                    candidate.Section.Id,
                    StringComparison.OrdinalIgnoreCase)))
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Legacy aggregate row remained present for user {user.Name}; "
                    + "the durable backup was retained and no suppression was reported.");
                return 0;
            }

            YouTubeChannel.LogPublic(
                $"[YT] Suppressed legacy aggregate latest row for user {user.Name}: "
                + $"{candidate.Section.Id} (original index {candidate.Index}).");
            return 1;
        }

        private static bool AreDesiredRowsReady(
            IEnumerable<ContentSection> current,
            IReadOnlyDictionary<string, ContentSection> desired)
        {
            var sections = current.ToArray();
            return desired.All(pair =>
            {
                var live = sections
                    .Where(section => string.Equals(
                        section.Id,
                        pair.Key,
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                return live.Length == 1 && !NeedsUpdate(live[0], pair.Value);
            });
        }

        private async Task<int> RestoreLegacyAggregateAsync(
            User user,
            CancellationToken cancellationToken)
        {
            if (!TryLoadAggregateBackup(user, out var backup, out var section)
                || backup == null
                || section == null)
                return 0;

            var current = GetCurrentSections(user, cancellationToken);
            var sameId = current.Where(section => string.Equals(
                    section.Id,
                    backup.SectionId,
                    StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (sameId.Length == 1)
            {
                // The delete may have failed after the durable backup, or the
                // user may have reused/edited that id. In either case the live
                // row wins and must never be overwritten.
                TryRemoveAggregateBackup(user);
                return 0;
            }
            if (sameId.Length > 1)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Aggregate restore deferred for user {user.Name}: "
                    + $"original id {backup.SectionId} occurs {sameId.Length} times.");
                return 0;
            }

            if (current.Any(HasLegacyAggregateShape))
            {
                // Another tool or the user already restored an equivalent
                // aggregate under a new id. Avoid creating a duplicate, but
                // retain our original snapshot instead of discarding it based
                // on a row we do not own.
                YouTubeChannel.LogPublic(
                    $"[YT] Aggregate restore deferred for user {user.Name}: "
                    + "a similar row with another id already exists.");
                return 0;
            }

            await InvokeMutationAsync(
                    _addHomeSection!,
                    user.InternalId,
                    section,
                    cancellationToken)
                .ConfigureAwait(false);

            var restoredSections = GetCurrentSections(user, cancellationToken);
            var restored = restoredSections
                .Select((candidate, index) => (Section: candidate, Index: index))
                .Where(candidate => string.Equals(
                    candidate.Section.Id,
                    backup.SectionId,
                    StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (restored.Length != 1 || !MatchesAggregateSnapshot(restored[0].Section, backup))
            {
                throw new InvalidOperationException(
                    "Emby did not retain exactly one unchanged aggregate HomeSection after restore.");
            }

            var restoredIndex = restored[0].Index;
            var targetIndex = Math.Min(backup.OriginalIndex, restoredSections.Length - 1);
            if (_moveHomeSections != null && restoredIndex != targetIndex)
            {
                await InvokeMoveAsync(
                        user.InternalId,
                        backup.SectionId,
                        targetIndex,
                        cancellationToken)
                    .ConfigureAwait(false);
                var moved = GetCurrentSections(user, cancellationToken)
                    .Select((candidate, index) => (Section: candidate, Index: index))
                    .Where(candidate => string.Equals(
                        candidate.Section.Id,
                        backup.SectionId,
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if (moved.Length != 1
                    || moved[0].Index != targetIndex
                    || !MatchesAggregateSnapshot(moved[0].Section, backup))
                {
                    throw new InvalidOperationException(
                        "Emby did not retain the aggregate section at its restored position.");
                }
                restoredIndex = moved[0].Index;
            }

            TryRemoveAggregateBackup(user);
            YouTubeChannel.LogPublic(
                $"[YT] Restored legacy aggregate latest row for user {user.Name}: "
                + $"{backup.SectionId} (original index {backup.OriginalIndex}, restored index {restoredIndex}).");
            return 1;
        }

        private async Task<int> FilterLatestMediaBlocksAsync(
            User user,
            AggregateFolderContext context,
            IReadOnlyDictionary<string, ContentSection> desired,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(context.YouTubeViewId)
                || !TryLoadLatestMediaBlockFilterState(user, out var state))
            {
                return 0;
            }

            // latestmediablock expands one row per top-level user view. Adding
            // only the provider id to ExcludedFolders removes Emby's mixed
            // "Latest YouTube" expansion while preserving its movie/TV rows.
            var current = GetCurrentSections(user, cancellationToken);
            if (!AreDesiredRowsReady(current, desired))
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Automatic mixed YouTube latest row left unchanged for user {user.Name}: "
                    + "not all per-channel rows are present in the HomeSections readback.");
                return 0;
            }

            var withoutId = current.Count(section =>
                IsLatestMediaBlock(section) && string.IsNullOrWhiteSpace(section.Id));
            if (withoutId > 0)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Automatic latest-media filtering skipped {withoutId} block(s) "
                    + $"without an id for user {user.Name}.");
            }

            var filtered = 0;
            var groups = current
                .Where(section =>
                    IsLatestMediaBlock(section) && !string.IsNullOrWhiteSpace(section.Id))
                .GroupBy(section => section.Id, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            foreach (var group in groups)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (group.Count() != 1)
                {
                    YouTubeChannel.LogPublic(
                        $"[YT] Automatic latest-media block left unchanged for user {user.Name}: "
                        + $"id {group.Key} occurs {group.Count()} times.");
                    continue;
                }

                var section = group.First();
                var entry = state.Entries.FirstOrDefault(candidate => string.Equals(
                    candidate.SectionId,
                    section.Id,
                    StringComparison.OrdinalIgnoreCase));
                var excluded = GetRuntimeStringArray(section, "ExcludedFolders")
                    ?? Array.Empty<string>();
                var providerAlreadyExcluded = ContainsFolderId(
                    excluded,
                    context.YouTubeViewId);
                var providerOwnedByPlugin = entry != null
                    && ContainsFolderId(
                        entry.AddedExcludedFolderIds,
                        context.YouTubeViewId);

                // A pre-existing exclusion belongs to the user or another
                // layout tool. Do not journal it and therefore never undo it.
                if (providerAlreadyExcluded)
                    continue;

                var stateChanged = false;
                if (entry == null)
                {
                    entry = new LatestMediaBlockFilterEntry
                    {
                        SectionId = section.Id,
                        SavedAtUtc = DateTime.UtcNow
                    };
                    state.Entries.Add(entry);
                    stateChanged = true;
                }
                if (!providerOwnedByPlugin)
                {
                    entry.AddedExcludedFolderIds.Add(context.YouTubeViewId);
                    stateChanged = true;
                }

                // Journal ownership before UpdateHomeSection. A crash between
                // these operations is convergent: the next sync re-applies the
                // missing id, while disabling can remove only journalled ids.
                if (stateChanged && !TrySaveLatestMediaBlockFilterState(user, state))
                    return filtered;

                var updatedExcluded = AppendFolderId(excluded, context.YouTubeViewId);
                SetRequiredRuntimeProperty(section, "ExcludedFolders", updatedExcluded);
                await InvokeMutationAsync(
                        _updateHomeSection!,
                        user.InternalId,
                        section,
                        cancellationToken)
                    .ConfigureAwait(false);

                var readback = GetCurrentSections(user, cancellationToken)
                    .Where(candidate => string.Equals(
                        candidate.Id,
                        section.Id,
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if (readback.Length != 1
                    || !IsLatestMediaBlock(readback[0])
                    || !ContainsFolderId(
                        GetRuntimeStringArray(readback[0], "ExcludedFolders")
                            ?? Array.Empty<string>(),
                        context.YouTubeViewId))
                {
                    throw new InvalidOperationException(
                        "Emby did not retain the YouTube exclusion on exactly one latest-media block.");
                }

                YouTubeChannel.LogPublic(
                    $"[YT] Filtered automatic mixed YouTube latest row for user {user.Name}: "
                    + $"block={section.Id}, provider={context.YouTubeViewId}.");
                filtered++;
            }

            return filtered;
        }

        private async Task<int> RestoreLatestMediaBlockAsync(
            User user,
            CancellationToken cancellationToken)
        {
            if (!TryLoadLatestMediaBlockFilterState(user, out var state)
                || state.Entries.Count == 0)
            {
                return 0;
            }

            var restored = 0;
            foreach (var entry in state.Entries.ToArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var sameId = GetCurrentSections(user, cancellationToken)
                    .Where(section => string.Equals(
                        section.Id,
                        entry.SectionId,
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if (sameId.Length == 0)
                {
                    // The user removed the mutation target. Dropping only our
                    // obsolete journal entry must not recreate that section.
                    state.Entries.Remove(entry);
                    if (!TrySaveLatestMediaBlockFilterState(user, state))
                        return restored;
                    continue;
                }
                if (sameId.Length != 1 || !IsLatestMediaBlock(sameId[0]))
                {
                    YouTubeChannel.LogPublic(
                        $"[YT] Automatic latest-media restore deferred for user {user.Name}: "
                        + $"id {entry.SectionId} is duplicated or has changed type.");
                    continue;
                }

                var section = sameId[0];
                var currentExcluded = GetRuntimeStringArray(section, "ExcludedFolders")
                    ?? Array.Empty<string>();
                var ownedIds = entry.AddedExcludedFolderIds
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                var restoredExcluded = currentExcluded
                    .Where(id => !ownedIds.Contains(id))
                    .ToArray();

                // A crash after Update but before journal cleanup lands here.
                // Nothing remains to mutate, so only clear the stale marker.
                if (restoredExcluded.Length == currentExcluded.Length)
                {
                    state.Entries.Remove(entry);
                    if (!TrySaveLatestMediaBlockFilterState(user, state))
                        return restored;
                    continue;
                }

                SetRequiredRuntimeProperty(section, "ExcludedFolders", restoredExcluded);
                await InvokeMutationAsync(
                        _updateHomeSection!,
                        user.InternalId,
                        section,
                        cancellationToken)
                    .ConfigureAwait(false);

                var readback = GetCurrentSections(user, cancellationToken)
                    .Where(candidate => string.Equals(
                        candidate.Id,
                        entry.SectionId,
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                var readbackExcluded = readback.Length == 1
                    ? GetRuntimeStringArray(readback[0], "ExcludedFolders")
                        ?? Array.Empty<string>()
                    : Array.Empty<string>();
                if (readback.Length != 1
                    || !IsLatestMediaBlock(readback[0])
                    || readbackExcluded.Any(ownedIds.Contains)
                    || !readbackExcluded.SequenceEqual(
                        restoredExcluded,
                        StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Emby did not remove only the plugin-owned latest-media exclusions.");
                }

                state.Entries.Remove(entry);
                if (!TrySaveLatestMediaBlockFilterState(user, state))
                    return restored + 1;

                YouTubeChannel.LogPublic(
                    $"[YT] Restored automatic mixed YouTube latest row for user {user.Name}: "
                    + $"block={entry.SectionId}, removedExclusions={ownedIds.Count}.");
                restored++;
            }

            return restored;
        }

        private static bool IsLatestMediaBlock(ContentSection section) =>
            string.Equals(
                section.SectionType,
                LatestMediaBlockSectionType,
                StringComparison.OrdinalIgnoreCase);

        private static bool ContainsFolderId(
            IEnumerable<string> folderIds,
            string folderId) =>
            folderIds.Contains(folderId, StringComparer.OrdinalIgnoreCase);

        private static string[] AppendFolderId(
            IEnumerable<string> folderIds,
            string folderId)
        {
            var values = folderIds.ToArray();
            return ContainsFolderId(values, folderId)
                ? values
                : values.Append(folderId).ToArray();
        }

        private ContentSection[] GetCurrentSections(
            User user,
            CancellationToken cancellationToken) =>
            _userManager.GetHomeSections(user.InternalId, cancellationToken)?.Sections
            ?? Array.Empty<ContentSection>();

        private static bool IsStrictLegacyAggregate(
            ContentSection section,
            AggregateFolderContext context)
        {
            if (!HasLegacyAggregateShape(section)
                || context.AllViewIds.Count == 0
                || context.YouTubeRootIds.Count == 0)
            {
                return false;
            }

            var excluded = GetRuntimeStringArray(section, "ExcludedFolders");
            if (excluded == null || excluded.Length == 0)
                return false;

            var excludedSet = excluded.ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (excludedSet.Overlaps(context.YouTubeRootIds))
                return false;

            var nonYouTubeViewIds = context.AllViewIds
                .Where(id => !context.YouTubeRootIds.Contains(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            // Extra ids can be stale, hidden, or no longer materialized. They
            // only narrow the row and are safe to tolerate. Every current
            // non-YouTube view must still be excluded, and the overlap guard
            // above still rejects every current YouTube channel root.
            return nonYouTubeViewIds.Count > 0
                   && nonYouTubeViewIds.IsSubsetOf(excludedSet);
        }

        private static void LogLegacyAggregateNoMatch(
            User user,
            IEnumerable<ContentSection> sections,
            AggregateFolderContext context)
        {
            var candidates = sections
                .Where(section =>
                    string.Equals(section.Name, LegacyAggregateName, StringComparison.Ordinal)
                    || string.Equals(
                        GetRuntimeString(section, "CustomName"),
                        LegacyAggregateName,
                        StringComparison.Ordinal))
                .ToArray();
            if (candidates.Length == 0)
                return;

            var expectedExcluded = context.AllViewIds
                .Where(id => !context.YouTubeRootIds.Contains(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var candidate in candidates)
            {
                var excluded = (GetRuntimeStringArray(candidate, "ExcludedFolders")
                        ?? Array.Empty<string>())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                var customName = GetRuntimeString(candidate, "CustomName");
                var customNameState = string.IsNullOrWhiteSpace(customName)
                    ? "missing"
                    : string.Equals(customName, LegacyAggregateName, StringComparison.Ordinal)
                        ? "match"
                        : "different";

                YouTubeChannel.LogPublic(
                    $"[YT] Legacy aggregate candidate did not match for user {user.Name}: "
                    + $"id={candidate.Id}, shapeMatch={HasLegacyAggregateShape(candidate)}, "
                    + $"customName={customNameState}, excluded={excluded.Count}, "
                    + $"expectedExcluded={expectedExcluded.Count}, "
                    + $"missingExpected={expectedExcluded.Except(excluded).Count()}, "
                    + $"unexpectedExcluded={excluded.Except(expectedExcluded).Count()}, "
                    + $"excludedYouTubeRoots={excluded.Intersect(context.YouTubeRootIds).Count()}.");
            }
        }

        private static bool HasLegacyAggregateShape(ContentSection section)
        {
            if (string.IsNullOrWhiteSpace(section.Id)
                || IsManaged(section)
                || !string.Equals(
                    section.Name,
                    LegacyAggregateName,
                    StringComparison.Ordinal)
                || !string.Equals(section.SectionType, "items", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var itemTypes = GetRuntimeStringArray(section, "ItemTypes");
            var customName = GetRuntimeString(section, "CustomName");
            return itemTypes?.Length == 1
                   && string.Equals(itemTypes[0], "Episode", StringComparison.OrdinalIgnoreCase)
                   && (string.IsNullOrWhiteSpace(customName)
                       || string.Equals(
                           customName,
                           LegacyAggregateName,
                           StringComparison.Ordinal))
                   && string.IsNullOrWhiteSpace(GetRuntimeString(section, "ParentId"))
                   && string.Equals(GetRuntimeString(section, "SortBy"), "DateCreated", StringComparison.OrdinalIgnoreCase)
                   && string.Equals(GetRuntimeString(section, "SortOrder"), "Descending", StringComparison.OrdinalIgnoreCase);
        }

        private static object? GetRuntimeProperty(ContentSection section, string propertyName) =>
            section.GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
                ?.GetValue(section);

        private static string? GetRuntimeString(ContentSection section, string propertyName) =>
            GetRuntimeProperty(section, propertyName) as string;

        private static string[]? GetRuntimeStringArray(ContentSection section, string propertyName) =>
            GetRuntimeProperty(section, propertyName) is IEnumerable<string> values
                ? values.ToArray()
                : null;

        private static bool MatchesAggregateSnapshot(
            ContentSection section,
            AggregateBackup backup)
        {
            try
            {
                return string.Equals(
                    JsonSerializer.Serialize(section, section.GetType(), BackupJsonOptions),
                    backup.SectionJson,
                    StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        private bool TryLoadLatestMediaBlockFilterState(
            User user,
            out LatestMediaBlockFilterState state)
        {
            state = new LatestMediaBlockFilterState();
            var path = GetLatestMediaBlockFilterPath(user);
            if (path == null)
                return false;
            if (!File.Exists(path))
                return true;

            try
            {
                state = JsonSerializer.Deserialize<LatestMediaBlockFilterState>(
                        File.ReadAllText(path),
                        BackupJsonOptions)
                    ?? throw new InvalidDataException("Filter state is empty.");
                if (state.Version != 1 || state.Entries == null)
                    throw new InvalidDataException("Filter state has an unsupported shape.");

                var sectionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var entry in state.Entries)
                {
                    if (entry == null
                        || string.IsNullOrWhiteSpace(entry.SectionId)
                        || entry.AddedExcludedFolderIds == null
                        || entry.AddedExcludedFolderIds.Count == 0
                        || entry.AddedExcludedFolderIds.Any(string.IsNullOrWhiteSpace)
                        || !sectionIds.Add(entry.SectionId)
                        || entry.AddedExcludedFolderIds
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .Count() != entry.AddedExcludedFolderIds.Count)
                    {
                        throw new InvalidDataException(
                            "Filter state contains an incomplete or duplicate entry.");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                state = new LatestMediaBlockFilterState();
                YouTubeChannel.LogPublic(
                    $"[YT] Automatic latest-media filter state for user {user.Name} "
                    + $"could not be read; no block will be changed ({ex.Message}).");
                return false;
            }
        }

        private bool TrySaveLatestMediaBlockFilterState(
            User user,
            LatestMediaBlockFilterState state)
        {
            var path = GetLatestMediaBlockFilterPath(user);
            if (path == null)
                return false;

            string? tempPath = null;
            try
            {
                if (state.Entries.Count == 0)
                {
                    File.Delete(path);
                    return true;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(
                    tempPath,
                    JsonSerializer.Serialize(state, BackupJsonOptions));
                File.Move(tempPath, path, overwrite: true);
                return true;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Automatic latest-media filter state for user {user.Name} "
                    + $"could not be saved; the operation was stopped ({ex.Message}).");
                return false;
            }
            finally
            {
                try { if (tempPath != null && File.Exists(tempPath)) File.Delete(tempPath); }
                catch { }
            }
        }

        private bool TryLoadAggregateBackup(
            User user,
            out AggregateBackup? backup,
            out ContentSection? section)
        {
            backup = null;
            section = null;
            var path = GetAggregateBackupPath(user);
            if (path == null)
                return false;
            if (!File.Exists(path))
                return true;

            try
            {
                backup = JsonSerializer.Deserialize<AggregateBackup>(
                    File.ReadAllText(path),
                    BackupJsonOptions);
                if (backup == null
                    || string.IsNullOrWhiteSpace(backup.SectionId)
                    || string.IsNullOrWhiteSpace(backup.SectionJson)
                    || backup.OriginalIndex < 0)
                {
                    throw new InvalidDataException("Backup file is incomplete.");
                }

                section = JsonSerializer.Deserialize(
                    backup.SectionJson,
                    typeof(ContentSection),
                    BackupJsonOptions) as ContentSection;
                if (section == null
                    || !string.Equals(section.Id, backup.SectionId, StringComparison.OrdinalIgnoreCase)
                    || !HasLegacyAggregateShape(section))
                {
                    throw new InvalidDataException("Backup section failed its identity/shape guard.");
                }
                return true;
            }
            catch (Exception ex)
            {
                backup = null;
                section = null;
                YouTubeChannel.LogPublic(
                    $"[YT] Aggregate backup for user {user.Name} could not be read; "
                    + $"no row will be suppressed or restored ({ex.Message}).");
                return false;
            }
        }

        private bool TryStoreAggregateBackup(User user, ContentSection section, int originalIndex)
        {
            var path = GetAggregateBackupPath(user);
            if (path == null)
                return false;

            string? tempPath = null;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                if (File.Exists(path))
                    throw new IOException("A backup already exists and will not be overwritten.");

                var backup = new AggregateBackup
                {
                    SectionId = section.Id,
                    SectionJson = JsonSerializer.Serialize(
                        section,
                        section.GetType(),
                        BackupJsonOptions),
                    OriginalIndex = originalIndex,
                    SavedAtUtc = DateTime.UtcNow
                };
                tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(tempPath, JsonSerializer.Serialize(backup, BackupJsonOptions));
                File.Move(tempPath, path);
                return true;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Legacy aggregate row was not suppressed: backup failed ({ex.Message}).");
                return false;
            }
            finally
            {
                try { if (tempPath != null && File.Exists(tempPath)) File.Delete(tempPath); }
                catch { }
            }
        }

        private static void TryRemoveAggregateBackup(User user)
        {
            var path = GetAggregateBackupPath(user);
            if (path == null)
                return;
            try { File.Delete(path); }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Restored aggregate backup marker could not be removed for user "
                    + $"{user.Name} ({ex.Message}).");
            }
        }

        private static string? GetAggregateBackupPath(User user) =>
            string.IsNullOrWhiteSpace(Plugin.DataPath)
                ? null
                : Path.Combine(
                    Plugin.DataPath,
                    AggregateBackupDirectoryName,
                    user.InternalId.ToString(CultureInfo.InvariantCulture) + ".json");

        private static string? GetLatestMediaBlockFilterPath(User user) =>
            string.IsNullOrWhiteSpace(Plugin.DataPath)
                ? null
                : Path.Combine(
                    Plugin.DataPath,
                    LatestMediaBlockFilterDirectoryName,
                    user.InternalId.ToString(CultureInfo.InvariantCulture) + ".json");

        private static ContentSection BuildSection(Folder root)
        {
            var name = string.IsNullOrWhiteSpace(root.Name)
                ? "YouTube channel"
                : root.Name.Trim();
            var section = new ContentSection
            {
                Id = BuildManagedId(root.ExternalId),
                Name = name,
                Subtitle = string.Empty,
                SectionType = "latestmedia",
                Monitor = new[] { "markplayed", "videoplayback" },
                CardSizeOffset = 0,
                ScrollDirection = ScrollDirection.Horizontal
            };

            SetRequiredRuntimeProperty(section, "CustomName", name);
            var parentId = root.GetClientId();
            if (string.IsNullOrWhiteSpace(parentId))
                throw new InvalidOperationException($"YouTube root {root.ExternalId} has no client id.");
            SetRequiredRuntimeProperty(section, "ParentId", parentId);
            SetRuntimeProperty(section, "ImageType", "Primary");
            // YouTube channel videos are persisted as Episode items. An empty
            // filter lets Emby's latestmedia implementation include them;
            // forcing ItemTypes=Video would make a valid channel row empty.
            SetRuntimeProperty(section, "ItemTypes", Array.Empty<string>());
            return section;
        }

        private static bool NeedsUpdate(ContentSection current, ContentSection desired) =>
            !string.Equals(current.Name, desired.Name, StringComparison.Ordinal)
            || !string.Equals(current.SectionType, desired.SectionType, StringComparison.Ordinal)
            || current.ScrollDirection != desired.ScrollDirection
            || !(current.Monitor ?? Array.Empty<string>())
                .SequenceEqual(desired.Monitor ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase)
            || !RuntimePropertyEquals(current, desired, "CustomName")
            || !RuntimePropertyEquals(current, desired, "ParentId")
            || !RuntimePropertyEquals(current, desired, "ImageType")
            || !RuntimePropertyEquals(current, desired, "ItemTypes");

        private static ContentSection ApplyManagedValues(
            ContentSection current,
            ContentSection desired)
        {
            // UpdateHomeSection replaces the submitted section. Mutating the
            // current instance preserves Query, DisplayMode and any fields a
            // newer Emby version may add instead of resetting the full row to
            // the 4.9 compile-time shape.
            current.Name = desired.Name;
            current.SectionType = desired.SectionType;
            current.Monitor = desired.Monitor?.ToArray() ?? Array.Empty<string>();
            current.ScrollDirection = desired.ScrollDirection;
            CopyRuntimeProperty(desired, current, "CustomName");
            CopyRuntimeProperty(desired, current, "ParentId");
            CopyRuntimeProperty(desired, current, "ImageType");
            CopyRuntimeProperty(desired, current, "ItemTypes");
            return current;
        }

        private static bool IsManaged(ContentSection section) =>
            !string.IsNullOrEmpty(section.Id)
            && section.Id.StartsWith(ManagedIdPrefix, StringComparison.OrdinalIgnoreCase);

        private static string BuildManagedId(string externalId)
        {
            using var sha = SHA256.Create();
            var digest = Convert.ToHexString(
                    sha.ComputeHash(Encoding.UTF8.GetBytes(externalId ?? string.Empty)))
                .ToLowerInvariant();
            return ManagedIdPrefix + digest.Substring(0, 24);
        }

        private static MethodInfo? FindSectionMutation(string name, Type payloadType)
        {
            return typeof(IUserManager)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(method =>
                {
                    if (!string.Equals(method.Name, name, StringComparison.Ordinal))
                        return false;

                    var parameters = method.GetParameters();
                    return parameters.Length == 3
                           && parameters[0].ParameterType == typeof(long)
                           && parameters[1].ParameterType == payloadType
                           && parameters[2].ParameterType == typeof(CancellationToken);
                });
        }

        private static MethodInfo? FindMoveHomeSections() =>
            typeof(IUserManager).GetMethod(
                "MoveHomeSections",
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: new[]
                {
                    typeof(long),
                    typeof(string[]),
                    typeof(int),
                    typeof(CancellationToken)
                },
                modifiers: null);

        private async Task InvokeMoveAsync(
            long userId,
            string sectionId,
            int newIndex,
            CancellationToken cancellationToken)
        {
            object? result;
            try
            {
                result = _moveHomeSections!.Invoke(
                    _userManager,
                    new object[] { userId, new[] { sectionId }, newIndex, cancellationToken });
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw new InvalidOperationException(
                    $"Emby MoveHomeSections failed: {ex.InnerException.Message}",
                    ex.InnerException);
            }

            if (result is Task task)
                await task.ConfigureAwait(false);
        }

        private async Task InvokeMutationAsync(
            MethodInfo method,
            long userId,
            object payload,
            CancellationToken cancellationToken)
        {
            object? result;
            try
            {
                result = method.Invoke(
                    _userManager,
                    new[] { (object)userId, payload, cancellationToken });
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw new InvalidOperationException(
                    $"Emby {method.Name} failed: {ex.InnerException.Message}",
                    ex.InnerException);
            }

            if (result is Task task)
                await task.ConfigureAwait(false);
        }

        private static void SetRequiredRuntimeProperty(
            ContentSection section,
            string propertyName,
            object value)
        {
            if (!SetRuntimeProperty(section, propertyName, value))
            {
                throw new MissingMemberException(
                    section.GetType().FullName,
                    propertyName);
            }
        }

        private static bool SetRuntimeProperty(
            ContentSection section,
            string propertyName,
            object value)
        {
            var property = section.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);
            if (property?.CanWrite != true || !property.PropertyType.IsInstanceOfType(value))
                return false;

            property.SetValue(section, value);
            return true;
        }

        private static void CopyRuntimeProperty(
            ContentSection source,
            ContentSection target,
            string propertyName)
        {
            var property = source.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);
            if (property?.CanRead != true || property.CanWrite != true)
                throw new MissingMemberException(source.GetType().FullName, propertyName);

            property.SetValue(target, property.GetValue(source));
        }

        private static bool RuntimePropertyEquals(
            ContentSection current,
            ContentSection desired,
            string propertyName)
        {
            var property = current.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);
            if (property?.CanRead != true)
                return false;

            var currentValue = property.GetValue(current);
            var desiredValue = property.GetValue(desired);
            if (currentValue == null && desiredValue is Array emptyDesired && emptyDesired.Length == 0)
                return true;
            if (desiredValue == null && currentValue is Array emptyCurrent && emptyCurrent.Length == 0)
                return true;
            if (currentValue is Array currentArray && desiredValue is Array desiredArray)
            {
                var currentStrings = currentArray.Cast<object?>().OfType<string>().ToArray();
                var desiredStrings = desiredArray.Cast<object?>().OfType<string>().ToArray();
                if (currentStrings.Length == currentArray.Length
                    && desiredStrings.Length == desiredArray.Length)
                {
                    return currentStrings.SequenceEqual(
                        desiredStrings,
                        StringComparer.OrdinalIgnoreCase);
                }

                return currentArray.Cast<object?>().SequenceEqual(desiredArray.Cast<object?>());
            }

            return Equals(currentValue, desiredValue);
        }
    }
}
