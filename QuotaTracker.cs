using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Emby.YouTubePlugin
{
    /// <summary>
    /// Tracks the plugin's estimated YouTube Data API v3 quota usage.
    /// Quota costs (per call):
    ///   - search.list:        100
    ///   - videos.list:        1
    ///   - playlistItems.list: 1
    ///   - playlists.list:     1
    ///   - channels.list:      1
    ///   - captions.list:      50
    ///   - videoCategories:    1
    /// The default daily budget is 10,000 units.
    /// </summary>
    internal static class QuotaTracker
    {
        private const int DailyQuota = 10_000;
        private const string StateFile = "youtube-quota.json";

        private static readonly object _lock = new();
        private static long _usedToday;
        private static DateTime _quotaDate;
        private static long _totalUsed;
        private static int _loaded;

        // YouTube resets quota at midnight Pacific Time.
        private static readonly (TimeZoneInfo Zone, bool Fallback) PacificTimeInfo = ResolvePacific();
        private static readonly TimeZoneInfo PacificTime = PacificTimeInfo.Zone;
        private static readonly bool _pacificFallback = PacificTimeInfo.Fallback;

        private static (TimeZoneInfo Zone, bool Fallback) ResolvePacific()
        {
            var zone = TryFindTimeZone("America/Los_Angeles")
                ?? TryFindTimeZone("Pacific Standard Time");

            if (zone != null)
                return (zone, false);

            // If tzdata is missing, fall back to fixed UTC-8. It is not perfect
            // during daylight saving time, but it is much closer to YouTube's
            // reset window than using UTC.
            return (
                TimeZoneInfo.CreateCustomTimeZone(
                    "Pacific-Approx", TimeSpan.FromHours(-8), "Pacific (approx)", "Pacific (approx)"),
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

        private static DateTime CurrentQuotaDay()
        {
            var ptNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PacificTime);
            return ptNow.Date;
        }

        public static void RecordCall(string url)
        {
            int cost = EstimateCost(url);
            Record(cost);
        }

        public static int EstimateCost(string url)
        {
            if (string.IsNullOrEmpty(url)) return 0;
            // Only a few endpoints have special costs; the rest are 1 unit.
            if (url.Contains("/search?", StringComparison.Ordinal)) return 100;
            if (url.Contains("/captions?", StringComparison.Ordinal)) return 50;
            return 1;
        }

        public static void Record(int cost)
        {
            if (cost <= 0) return;
            EnsureLoaded();
            lock (_lock)
            {
                var today = CurrentQuotaDay();
                if (today != _quotaDate)
                {
                    _quotaDate = today;
                    _usedToday = 0;
                }
                _usedToday += cost;
                _totalUsed += cost;
            }
            SaveAsync();
        }

        public static (long usedToday, long dailyQuota, long totalUsed, DateTime quotaDate, TimeSpan untilReset) GetStats()
        {
            EnsureLoaded();
            lock (_lock)
            {
                var today = CurrentQuotaDay();
                if (today != _quotaDate)
                {
                    _quotaDate = today;
                    _usedToday = 0;
                }
                var nextReset = TimeZoneInfo.ConvertTimeToUtc(today.AddDays(1), PacificTime);
                var until = nextReset - DateTime.UtcNow;
                if (until < TimeSpan.Zero) until = TimeSpan.Zero;
                return (_usedToday, DailyQuota, _totalUsed, _quotaDate, until);
            }
        }

        public static string FormatStatus()
        {
            var s = GetStats();
            var pct = s.dailyQuota > 0 ? (s.usedToday * 100.0 / s.dailyQuota) : 0;
            string bar = BuildBar(pct);
            string resetIn = s.untilReset.TotalHours >= 1
                ? $"{(int)s.untilReset.TotalHours}h {s.untilReset.Minutes}m"
                : $"{s.untilReset.Minutes}m";
            string tzNote = _pacificFallback
                ? " ⚠ tzdata missing — using UTC-8 approximation"
                : " (resets midnight Pacific Time)";

            return
                $"Today: {s.usedToday:N0} / {s.dailyQuota:N0} units ({pct:F1}%)  {bar}\n" +
                $"Reset in: {resetIn}{tzNote}\n" +
                $"Lifetime tracked: {s.totalUsed:N0} units";
        }

        private static string BuildBar(double pct)
        {
            const int width = 20;
            var filled = (int)Math.Clamp(Math.Round(pct / 100.0 * width), 0, width);
            return "[" + new string('█', filled) + new string('░', width - filled) + "]";
        }

        public static bool IsExhausted()
        {
            var s = GetStats();
            return s.usedToday >= s.dailyQuota;
        }

        // Persistence.
        private static string? FilePath()
        {
            var dir = Plugin.CachePath;
            if (string.IsNullOrEmpty(dir)) return null;
            // Keep quota state outside the cache folder so clearing cached API
            // responses does not erase the usage history.
            var parent = Path.GetDirectoryName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrEmpty(parent)) parent = dir;
            return Path.Combine(parent, StateFile);
        }

        private static void EnsureLoaded()
        {
            if (Interlocked.CompareExchange(ref _loaded, 1, 0) != 0) return;
            try
            {
                var path = FilePath();
                if (path == null)
                {
                    _quotaDate = CurrentQuotaDay();
                    return;
                }
                // Older builds stored this inside the cache folder. Move it out
                // once so future cache cleanup leaves it alone.
                if (!File.Exists(path))
                {
                    var legacy = Path.Combine(Plugin.CachePath ?? "", StateFile);
                    if (!string.IsNullOrEmpty(legacy) && File.Exists(legacy))
                    {
                        try { File.Move(legacy, path); }
                        catch (Exception ex) { Debug.WriteLine($"[QuotaTracker] Legacy quota file move failed: {ex.Message}"); }
                    }
                }
                if (!File.Exists(path))
                {
                    _quotaDate = CurrentQuotaDay();
                    return;
                }
                var json = File.ReadAllText(path);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("date", out var d) && DateTime.TryParse(d.GetString(), out var dt))
                    _quotaDate = dt.Date;
                else
                    _quotaDate = CurrentQuotaDay();
                if (root.TryGetProperty("used", out var u) && u.TryGetInt64(out var uv))
                    _usedToday = uv;
                if (root.TryGetProperty("total", out var t) && t.TryGetInt64(out var tv))
                    _totalUsed = tv;
                if (_quotaDate != CurrentQuotaDay())
                {
                    _quotaDate = CurrentQuotaDay();
                    _usedToday = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QuotaTracker] Load failed: {ex.Message}");
                _quotaDate = CurrentQuotaDay();
            }
        }

        private static long _lastSaveTicks;
        private static void SaveAsync()
        {
            var now = Environment.TickCount64;
            // Save at most once every five seconds to avoid unnecessary writes.
            if (now - Interlocked.Read(ref _lastSaveTicks) < 5000) return;
            Interlocked.Exchange(ref _lastSaveTicks, now);

            try
            {
                var path = FilePath();
                if (path == null) return;
                long used, total;
                DateTime date;
                lock (_lock)
                {
                    used = _usedToday;
                    total = _totalUsed;
                    date = _quotaDate;
                }
                var json = JsonSerializer.Serialize(new
                {
                    date = date.ToString("yyyy-MM-dd"),
                    used,
                    total
                });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QuotaTracker] Save failed: {ex.Message}");
            }
        }
    }
}
