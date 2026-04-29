using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    /// <summary>
    /// Keeps YouTube items sorted newest-first by updating Emby's stored
    /// SortName values. It runs twice because Emby may still be writing new
    /// items during the first pass.
    /// </summary>
    internal static class SortNameFixer
    {
        [DllImport("sqlite3")] private static extern int sqlite3_open(string filename, out IntPtr db);
        [DllImport("sqlite3")] private static extern int sqlite3_exec(IntPtr db, string sql, IntPtr cb, IntPtr arg, out IntPtr errmsg);
        [DllImport("sqlite3")] private static extern int sqlite3_close(IntPtr db);
        [DllImport("sqlite3")] private static extern void sqlite3_free(IntPtr ptr);

        private static int _scheduled;

        public static void Schedule()
        {
            if (Interlocked.CompareExchange(ref _scheduled, 1, 0) != 0) return;
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(5_000).ConfigureAwait(false);
                    Apply();
                    await Task.Delay(30_000).ConfigureAwait(false);
                    Apply();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SortNameFixer] Scheduled sort-name update failed: {ex.Message}");
                }
                finally { Interlocked.Exchange(ref _scheduled, 0); }
            });
        }

        private static void Apply()
        {
            var dbPath = Plugin.LibraryDbPath;
            if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath)) return;

            if (sqlite3_open(dbPath, out var db) != 0) return;
            try
            {
                sqlite3_exec(db, "PRAGMA busy_timeout = 5000;", IntPtr.Zero, IntPtr.Zero, out _);

                const string sql = @"
                    UPDATE MediaItems
                    SET SortName = printf('%010d', 9999999999 - COALESCE(PremiereDate, DateCreated, 0))
                                   || ' ' || SortName
                    WHERE type = 8
                      AND PremiereDate IS NOT NULL
                      AND ExternalId IS NOT NULL
                      AND (length(ExternalId) = 11 OR ExternalId LIKE 'LIVE_%' OR ExternalId LIKE 'REEL_%')
                      AND SortName NOT GLOB '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9] *'";

                sqlite3_exec(db, sql, IntPtr.Zero, IntPtr.Zero, out var errmsg);
                if (errmsg != IntPtr.Zero) sqlite3_free(errmsg);
            }
            finally { sqlite3_close(db); }
        }
    }
}
