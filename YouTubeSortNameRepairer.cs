using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    internal sealed class YouTubeSortNameRepairer : IDisposable
    {
        private static readonly TimeSpan UpdateSpacing = TimeSpan.FromMilliseconds(750);
        private static readonly TimeSpan RetryBaseDelay = TimeSpan.FromSeconds(2);
        private const int MaxAttempts = 5;
        private const long SortDescendingFrom = 9_999_999_999L;

        private readonly ConcurrentDictionary<long, BaseItem> _pending = new();
        private readonly SemaphoreSlim _signal = new(0);
        private readonly CancellationTokenSource _cts = new();
        private Task? _worker;

        public void Start()
        {
            _worker ??= Task.Run(ProcessLoop);
        }

        public void Enqueue(BaseItem? item)
        {
            if (item == null)
                return;

            if (string.IsNullOrEmpty(YouTubeImageProvider.TryGetVideoId(item)))
                return;

            _pending[item.InternalId] = item;
            try { _signal.Release(); } catch { }
        }

        private async Task ProcessLoop()
        {
            var ct = _cts.Token;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(ct).ConfigureAwait(false);

                    while (!ct.IsCancellationRequested && TryTakeNext(out var item))
                    {
                        await RepairWithRetry(item, ct).ConfigureAwait(false);
                        await Task.Delay(UpdateSpacing, ct).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] SortName repair loop failed: {ex.Message}");
                }
            }
        }

        private bool TryTakeNext(out BaseItem item)
        {
            foreach (var key in _pending.Keys)
            {
                if (_pending.TryRemove(key, out item!))
                    return true;
            }

            item = null!;
            return false;
        }

        private static async Task RepairWithRetry(BaseItem item, CancellationToken ct)
        {
            for (var attempt = 1; attempt <= MaxAttempts && !ct.IsCancellationRequested; attempt++)
            {
                try
                {
                    if (!TryApplySortName(item, out var desiredSortName))
                        return;

                    item.UpdateToRepository(ItemUpdateType.MetadataEdit);
                    YouTubeChannel.LogPublic($"[YT] SortName repair updated item {item.InternalId} to '{desiredSortName}'.");
                    return;
                }
                catch (Exception ex)
                {
                    if (attempt >= MaxAttempts || !LooksLikeTransientDatabaseLock(ex))
                    {
                        YouTubeChannel.LogPublic($"[YT] SortName repair failed for item {item.InternalId}: {ex.Message}");
                        return;
                    }

                    var delay = TimeSpan.FromTicks(RetryBaseDelay.Ticks * attempt);
                    YouTubeChannel.LogPublic($"[YT] SortName repair retry {attempt} for item {item.InternalId} after database lock.");
                    await Task.Delay(delay, ct).ConfigureAwait(false);
                }
            }
        }

        private static bool TryApplySortName(BaseItem item, out string desiredSortName)
        {
            desiredSortName = string.Empty;

            var videoId = YouTubeImageProvider.TryGetVideoId(item);
            if (string.IsNullOrEmpty(videoId))
                return false;

            var date = item.PremiereDate ?? item.DateCreated;
            if (date <= DateTimeOffset.MinValue)
                return false;

            var currentSortName = item.SortName ?? string.Empty;
            var baseSortName = StripDatePrefix(currentSortName);
            if (string.IsNullOrWhiteSpace(baseSortName))
                baseSortName = item.Name ?? string.Empty;

            baseSortName = baseSortName.Trim();
            if (baseSortName.Length == 0)
                return false;

            var dayNumber = date.UtcDateTime.Ticks / TimeSpan.TicksPerDay;
            var prefix = Math.Max(0, SortDescendingFrom - dayNumber)
                .ToString("D10", CultureInfo.InvariantCulture);
            desiredSortName = prefix + " " + baseSortName;

            if (string.Equals(currentSortName, desiredSortName, StringComparison.Ordinal))
                return false;

            item.SetSortNameDirect(desiredSortName);
            return true;
        }

        private static string StripDatePrefix(string sortName)
        {
            if (sortName.Length < 11 || sortName[10] != ' ')
                return sortName;

            for (var i = 0; i < 10; i++)
            {
                if (!char.IsDigit(sortName[i]))
                    return sortName;
            }

            return sortName.Substring(11);
        }

        private static bool LooksLikeTransientDatabaseLock(Exception ex)
        {
            for (var current = ex; current != null; current = current.InnerException)
            {
                var message = current.Message ?? string.Empty;
                if (message.IndexOf("database is locked", StringComparison.OrdinalIgnoreCase) >= 0
                    || message.IndexOf("database is busy", StringComparison.OrdinalIgnoreCase) >= 0
                    || message.IndexOf("Busy", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            try { _cts.Cancel(); } catch { }
            try { _signal.Release(); } catch { }
            try { _worker?.Wait(TimeSpan.FromSeconds(2)); } catch { }
            _signal.Dispose();
            _cts.Dispose();
        }
    }
}
