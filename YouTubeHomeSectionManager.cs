using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Library;
using MediaBrowser.Model.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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

        private readonly IUserManager _userManager;
        private readonly IUserViewManager _userViewManager;
        private readonly MethodInfo? _addHomeSection;
        private readonly MethodInfo? _updateHomeSection;
        private readonly MethodInfo? _deleteHomeSections;
        private readonly SemaphoreSlim _syncGate = new(1, 1);
        private int _unsupportedLogged;

        public YouTubeHomeSectionManager(
            IUserManager userManager,
            IUserViewManager userViewManager)
        {
            _userManager = userManager;
            _userViewManager = userViewManager;
            _addHomeSection = FindSectionMutation("AddHomeSection", typeof(ContentSection));
            _updateHomeSection = FindSectionMutation("UpdateHomeSection", typeof(ContentSection));
            _deleteHomeSections = FindSectionMutation("DeleteHomeSections", typeof(string[]));
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
            if (enabled)
            {
                roots = GetSavedChannelRoots(user, hasConfiguredChannels);
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

            if (added > 0 || updated > 0 || removed > 0)
            {
                YouTubeChannel.LogPublic(
                    $"[YT] Home rows synchronized for user {user.Name}: "
                    + $"added={added}, updated={updated}, removed={removed}.");
            }
        }

        /// <summary>
        /// Returns null when saved channel inputs exist but their dynamic root
        /// items have not been materialized yet. An empty list is authoritative
        /// when the user has no YouTube view/access or no channels are configured.
        /// </summary>
        private IReadOnlyList<Folder>? GetSavedChannelRoots(
            User user,
            bool hasConfiguredChannels)
        {
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

        private Folder[] GetUserViews(long userId, bool allowDynamicChildren) =>
            _userViewManager.GetUserViews(new UserViewQuery
            {
                UserId = userId,
                IncludeExternalContent = true,
                AllowDynamicChildren = allowDynamicChildren,
                IncludeLiveTVView = false,
                IncludeHidden = false
            }) ?? Array.Empty<Folder>();

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
