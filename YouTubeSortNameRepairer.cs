using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    internal sealed class YouTubeSortNameRepairer : IDisposable
    {
        private static readonly TimeSpan UpdateSpacing = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan RetryBaseDelay = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan RefreshPollDelay = TimeSpan.FromMilliseconds(500);
        private const int MaxAttempts = 5;
        private const long SortDescendingFrom = 9_999_999_999L;
        private const int DayPrefixLength = 10;
        private const int TimePrefixLength = 12;

        private readonly ConcurrentDictionary<long, BaseItem> _pending = new();
        private readonly SemaphoreSlim _signal = new(0);
        private readonly CancellationTokenSource _cts = new();
        private Task? _worker;

        public void Start()
        {
            _worker ??= Task.Run(ProcessLoop);
        }

        public bool Enqueue(BaseItem? item)
        {
            if (item == null)
                return false;

            if (string.IsNullOrEmpty(YouTubeImageProvider.TryGetVideoId(item)))
                return false;

            _pending[item.InternalId] = item;
            try { _signal.Release(); } catch { }
            return true;
        }

        public int Enqueue(IEnumerable<BaseItem> items)
        {
            var count = 0;
            foreach (var item in items)
            {
                if (Enqueue(item))
                    count++;
            }

            return count;
        }

        private async Task ProcessLoop()
        {
            var ct = _cts.Token;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(ct).ConfigureAwait(false);

                    while (!ct.IsCancellationRequested)
                    {
                        // Never write to library.db while Emby is persisting a
                        // channel refresh. Two concurrent writers make SQLite
                        // fail with "Busy: database is locked" on macOS Emby, so
                        // the queued items wait here and drain once it finishes.
                        if (ChannelRefreshInvoker.IsRefreshInProgress)
                        {
                            await Task.Delay(RefreshPollDelay, ct).ConfigureAwait(false);
                            continue;
                        }

                        if (!TryTakeNext(out var item))
                            break;

                        if (await RepairWithRetry(item, ct).ConfigureAwait(false))
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

        private static async Task<bool> RepairWithRetry(BaseItem item, CancellationToken ct)
        {
            for (var attempt = 1; attempt <= MaxAttempts && !ct.IsCancellationRequested; attempt++)
            {
                try
                {
                    if (!TryApplySortName(item, out var desiredSortName))
                        return false;

                    item.UpdateToRepository(ItemUpdateType.MetadataEdit);
                    YouTubeChannel.LogPublic($"[YT] SortName repair updated item {item.InternalId} to '{desiredSortName}'.");
                    return true;
                }
                catch (Exception ex)
                {
                    if (attempt >= MaxAttempts || !LooksLikeTransientDatabaseLock(ex))
                    {
                        YouTubeChannel.LogPublic($"[YT] SortName repair failed for item {item.InternalId}: {ex.Message}");
                        return false;
                    }

                    var delay = TimeSpan.FromTicks(RetryBaseDelay.Ticks * attempt);
                    YouTubeChannel.LogPublic($"[YT] SortName repair retry {attempt} for item {item.InternalId} after database lock.");
                    await Task.Delay(delay, ct).ConfigureAwait(false);
                }
            }

            return false;
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

            var utc = date.UtcDateTime;
            var dayNumber = utc.Ticks / TimeSpan.TicksPerDay;
            var dayPrefix = Math.Max(0, SortDescendingFrom - dayNumber)
                .ToString("D10", CultureInfo.InvariantCulture);
            var timePrefix = Math.Max(0, TimeSpan.TicksPerDay - 1 - utc.TimeOfDay.Ticks)
                .ToString("D12", CultureInfo.InvariantCulture);
            desiredSortName = dayPrefix + " " + timePrefix + " " + baseSortName;

            if (string.Equals(currentSortName, desiredSortName, StringComparison.Ordinal))
                return false;

            item.SetSortNameDirect(desiredSortName);
            return true;
        }

        private static string StripDatePrefix(string sortName)
        {
            if (!HasNumericPrefix(sortName, DayPrefixLength) || sortName.Length <= DayPrefixLength || sortName[DayPrefixLength] != ' ')
                return sortName;

            var remainder = sortName.Substring(DayPrefixLength + 1);
            if (HasNumericPrefix(remainder, TimePrefixLength)
                && remainder.Length > TimePrefixLength
                && remainder[TimePrefixLength] == ' ')
            {
                return remainder.Substring(TimePrefixLength + 1);
            }

            return remainder;
        }

        private static bool HasNumericPrefix(string value, int length)
        {
            if (value.Length < length)
                return false;

            for (var i = 0; i < length; i++)
            {
                if (!char.IsDigit(value[i]))
                    return false;
            }

            return true;
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
