using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    /// <summary>
    /// Tracks estimated YouTube Data API v3 usage in the separate quota buckets
    /// introduced on 2026-06-01. search.list calls no longer consume the common
    /// 10,000-unit bucket; they have their own default daily call limit.
    /// </summary>
    internal static class QuotaTracker
    {
        private const long SearchDailyLimit = 100;
        private const long OtherDailyLimit = 10_000;
        private const int StateSchema = 2;
        private const string StateFile = "youtube-quota.json";
        private const int SaveIntervalMs = 5000;

        internal readonly record struct QuotaStats(
            long SearchCallsToday,
            long SearchCallLimit,
            long OtherUnitsToday,
            long OtherUnitLimit,
            long TotalSearchCalls,
            long TotalOtherUnits,
            DateTime QuotaDate,
            TimeSpan UntilReset);

        private readonly record struct QuotaCharge(bool IsSearch, int OtherUnits);

        private readonly record struct QuotaSnapshot(
            DateTime Date,
            long SearchCalls,
            long OtherUnits,
            long TotalSearchCalls,
            long TotalOtherUnits);

        private static readonly object StateLock = new();
        private static long _searchCallsToday;
        private static long _otherUnitsToday;
        private static long _totalSearchCalls;
        private static long _totalOtherUnits;
        private static DateTime _quotaDate;
        private static long _stateVersion;
        private static long _persistedVersion;
        private static int _loaded;
        private static int _saveWorkerRunning;
        private static long _lastSaveTicks;

        // YouTube resets quota at midnight Pacific Time.
        private static readonly (TimeZoneInfo Zone, bool Fallback) PacificTimeInfo = ResolvePacific();
        private static readonly TimeZoneInfo PacificTime = PacificTimeInfo.Zone;
        private static readonly bool PacificFallback = PacificTimeInfo.Fallback;

        public static void RecordCall(string url)
        {
            var charge = Classify(url);
            if (!charge.IsSearch && charge.OtherUnits <= 0)
                return;

            EnsureLoaded();
            lock (StateLock)
            {
                ResetDayIfNeededLocked();
                if (charge.IsSearch)
                {
                    _searchCallsToday++;
                    _totalSearchCalls++;
                }
                else
                {
                    _otherUnitsToday += charge.OtherUnits;
                    _totalOtherUnits += charge.OtherUnits;
                }

                _stateVersion++;
            }

            ScheduleSave();
        }

        private static QuotaCharge Classify(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new QuotaCharge(false, 0);
            if (url.Contains("/search?", StringComparison.Ordinal))
                return new QuotaCharge(true, 0);
            if (url.Contains("/captions?", StringComparison.Ordinal))
                return new QuotaCharge(false, 50);
            return new QuotaCharge(false, 1);
        }

        public static QuotaStats GetStats()
        {
            EnsureLoaded();
            QuotaStats stats;
            bool reset;
            lock (StateLock)
            {
                reset = ResetDayIfNeededLocked();
                var nextReset = TimeZoneInfo.ConvertTimeToUtc(_quotaDate.AddDays(1), PacificTime);
                var until = nextReset - DateTime.UtcNow;
                if (until < TimeSpan.Zero) until = TimeSpan.Zero;
                stats = new QuotaStats(
                    _searchCallsToday,
                    SearchDailyLimit,
                    _otherUnitsToday,
                    OtherDailyLimit,
                    _totalSearchCalls,
                    _totalOtherUnits,
                    _quotaDate,
                    until);
            }

            if (reset)
                ScheduleSave();
            return stats;
        }

        public static string FormatStatus()
        {
            var stats = GetStats();
            var searchPct = Percentage(stats.SearchCallsToday, stats.SearchCallLimit);
            var otherPct = Percentage(stats.OtherUnitsToday, stats.OtherUnitLimit);
            var resetIn = stats.UntilReset.TotalHours >= 1
                ? $"{(int)stats.UntilReset.TotalHours}h {stats.UntilReset.Minutes}m"
                : $"{stats.UntilReset.Minutes}m";
            var tzNote = PacificFallback
                ? " (tzdata missing; UTC-8 approximation)"
                : " (midnight Pacific Time)";

            return
                $"Search: {stats.SearchCallsToday:N0} / {stats.SearchCallLimit:N0} calls ({searchPct:F1}%) {BuildBar(searchPct)}\n" +
                $"Other API: {stats.OtherUnitsToday:N0} / {stats.OtherUnitLimit:N0} units ({otherPct:F1}%) {BuildBar(otherPct)}\n" +
                $"Reset in: {resetIn}{tzNote}";
        }

        private static double Percentage(long used, long limit) =>
            limit > 0 ? used * 100.0 / limit : 0;

        private static string BuildBar(double pct)
        {
            const int width = 20;
            var filled = (int)Math.Clamp(Math.Round(pct / 100.0 * width), 0, width);
            return "[" + new string('█', filled) + new string('░', width - filled) + "]";
        }

        private static bool ResetDayIfNeededLocked()
        {
            var today = CurrentQuotaDay();
            if (today == _quotaDate)
                return false;

            _quotaDate = today;
            _searchCallsToday = 0;
            _otherUnitsToday = 0;
            _stateVersion++;
            return true;
        }

        private static DateTime CurrentQuotaDay()
        {
            var ptNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PacificTime);
            return ptNow.Date;
        }

        private static (TimeZoneInfo Zone, bool Fallback) ResolvePacific()
        {
            var zone = TryFindTimeZone("America/Los_Angeles")
                ?? TryFindTimeZone("Pacific Standard Time");
            if (zone != null)
                return (zone, false);

            return (
                TimeZoneInfo.CreateCustomTimeZone(
                    "Pacific-Approx",
                    TimeSpan.FromHours(-8),
                    "Pacific (approx)",
                    "Pacific (approx)"),
                true);
        }

        private static TimeZoneInfo? TryFindTimeZone(string id)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { return null; }
            catch (InvalidTimeZoneException ex)
            {
                Debug.WriteLine($"[QuotaTracker] Invalid time zone '{id}': {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QuotaTracker] Time zone lookup failed for '{id}': {ex.Message}");
                return null;
            }
        }

        private static string? FilePath()
        {
            var dir = Plugin.CachePath;
            if (string.IsNullOrEmpty(dir)) return null;
            var parent = Path.GetDirectoryName(
                dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrEmpty(parent)) parent = dir;
            return Path.Combine(parent, StateFile);
        }

        private static void EnsureLoaded()
        {
            if (Volatile.Read(ref _loaded) != 0)
                return;

            var needsSave = false;
            lock (StateLock)
            {
                if (_loaded != 0)
                    return;

                _quotaDate = CurrentQuotaDay();
                try
                {
                    var path = FilePath();
                    if (path != null)
                    {
                        MoveLegacyStateIfNeeded(path);
                        if (File.Exists(path))
                        {
                            using var doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
                            var root = doc.RootElement;
                            var schema = ReadInt64(root, "schema");
                            if (schema == StateSchema)
                            {
                                if (root.TryGetProperty("date", out var dateElement)
                                    && DateTime.TryParse(dateElement.GetString(), out var parsedDate))
                                {
                                    _quotaDate = parsedDate.Date;
                                }

                                _searchCallsToday = ReadInt64(root, "searchCalls");
                                _otherUnitsToday = ReadInt64(root, "otherUnits");
                                _totalSearchCalls = ReadInt64(root, "totalSearchCalls");
                                _totalOtherUnits = ReadInt64(root, "totalOtherUnits");
                            }
                            else
                            {
                                // The legacy counter mixed search costs and
                                // common units, so there is no honest conversion.
                                Debug.WriteLine("[QuotaTracker] Migrating legacy mixed quota state to separate buckets.");
                                _searchCallsToday = 0;
                                _otherUnitsToday = 0;
                                _totalSearchCalls = 0;
                                _totalOtherUnits = 0;
                                needsSave = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[QuotaTracker] Load failed: {ex.Message}");
                    _quotaDate = CurrentQuotaDay();
                    _searchCallsToday = 0;
                    _otherUnitsToday = 0;
                    needsSave = true;
                }

                if (_quotaDate != CurrentQuotaDay())
                {
                    _quotaDate = CurrentQuotaDay();
                    _searchCallsToday = 0;
                    _otherUnitsToday = 0;
                    needsSave = true;
                }

                if (needsSave)
                    _stateVersion++;
                _persistedVersion = needsSave ? 0 : _stateVersion;
                Volatile.Write(ref _loaded, 1);
            }

            if (needsSave)
                ScheduleSave();
        }

        private static void MoveLegacyStateIfNeeded(string path)
        {
            if (File.Exists(path)) return;
            var legacy = Path.Combine(Plugin.CachePath ?? string.Empty, StateFile);
            if (string.IsNullOrEmpty(legacy) || !File.Exists(legacy)) return;
            try { File.Move(legacy, path); }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QuotaTracker] Legacy quota file move failed: {ex.Message}");
            }
        }

        private static long ReadInt64(JsonElement root, string name) =>
            root.TryGetProperty(name, out var element) && element.TryGetInt64(out var value)
                ? value
                : 0;

        private static void ScheduleSave()
        {
            if (Interlocked.CompareExchange(ref _saveWorkerRunning, 1, 0) != 0)
                return;
            _ = Task.Run(SaveWorkerAsync);
        }

        private static async Task SaveWorkerAsync()
        {
            try
            {
                while (true)
                {
                    var lastSave = Interlocked.Read(ref _lastSaveTicks);
                    var elapsed = Environment.TickCount64 - lastSave;
                    if (lastSave != 0 && elapsed < SaveIntervalMs)
                        await Task.Delay((int)(SaveIntervalMs - elapsed)).ConfigureAwait(false);

                    QuotaSnapshot snapshot;
                    long version;
                    lock (StateLock)
                    {
                        if (_persistedVersion >= _stateVersion)
                            return;
                        version = _stateVersion;
                        snapshot = new QuotaSnapshot(
                            _quotaDate,
                            _searchCallsToday,
                            _otherUnitsToday,
                            _totalSearchCalls,
                            _totalOtherUnits);
                    }

                    if (WriteSnapshot(snapshot))
                    {
                        Interlocked.Exchange(ref _lastSaveTicks, Environment.TickCount64);
                        lock (StateLock)
                        {
                            if (version > _persistedVersion)
                                _persistedVersion = version;
                        }
                    }
                    else
                    {
                        await Task.Delay(SaveIntervalMs).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QuotaTracker] Save worker failed: {ex.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref _saveWorkerRunning, 0);
                bool dirty;
                lock (StateLock)
                    dirty = _persistedVersion < _stateVersion;
                if (dirty)
                    ScheduleSave();
            }
        }

        private static bool WriteSnapshot(QuotaSnapshot snapshot)
        {
            string? tempPath = null;
            try
            {
                var path = FilePath();
                if (path == null) return true;
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                var json = JsonSerializer.Serialize(new
                {
                    schema = StateSchema,
                    date = snapshot.Date.ToString("yyyy-MM-dd"),
                    searchCalls = snapshot.SearchCalls,
                    otherUnits = snapshot.OtherUnits,
                    totalSearchCalls = snapshot.TotalSearchCalls,
                    totalOtherUnits = snapshot.TotalOtherUnits
                });

                tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(
                    tempPath,
                    json,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                File.Move(tempPath, path, overwrite: true);
                tempPath = null;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QuotaTracker] Save failed: {ex.Message}");
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempPath))
                {
                    try
                    {
                        if (File.Exists(tempPath))
                            File.Delete(tempPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[QuotaTracker] Temp state cleanup failed: {ex.Message}");
                    }
                }
            }
        }
    }
}
