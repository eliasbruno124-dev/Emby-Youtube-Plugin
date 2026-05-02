using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    /// <summary>
    /// Keeps YouTube items sorted newest-first by updating Emby's stored
    /// SortName values. Runs twice because Emby may still be writing new items
    /// during the first pass.
    ///
    /// Works on Linux, Docker (Debian, Alpine, LSIO), Synology DSM, and
    /// Windows. The native sqlite library ships under different names per
    /// platform (sqlite3.dll, libsqlite3.so, libsqlite3.so.0, e_sqlite3.*),
    /// so we use a DllImportResolver that tries every candidate. When no
    /// sqlite library can be loaded, the sort-name fix is silently skipped
    /// and Emby's default sort order takes over.
    /// </summary>
    internal static class SortNameFixer
    {
        private const string LibName = "yt_sqlite_native";

        // SQLite expects UTF-8 strings. Marshal explicitly so non-ASCII paths
        // and SQL literals behave the same on every platform.
        [DllImport(LibName, EntryPoint = "sqlite3_open", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int sqlite3_open(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename, out IntPtr db);

        [DllImport(LibName, EntryPoint = "sqlite3_exec", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int sqlite3_exec(
            IntPtr db,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string sql,
            IntPtr cb, IntPtr arg, out IntPtr errmsg);

        [DllImport(LibName, EntryPoint = "sqlite3_close", ExactSpelling = true)]
        private static extern int sqlite3_close(IntPtr db);

        [DllImport(LibName, EntryPoint = "sqlite3_free", ExactSpelling = true)]
        private static extern void sqlite3_free(IntPtr ptr);

        [DllImport(LibName, EntryPoint = "sqlite3_changes", ExactSpelling = true)]
        private static extern int sqlite3_changes(IntPtr db);

        private static int _scheduled;
        private static int _resolverInstalled;
        private static int _nativeAvailable = -1; // -1 = unknown, 0 = no, 1 = yes

        static SortNameFixer()
        {
            EnsureResolver();
        }

        private static void EnsureResolver()
        {
            if (Interlocked.CompareExchange(ref _resolverInstalled, 1, 0) != 0) return;
            try
            {
                NativeLibrary.SetDllImportResolver(typeof(SortNameFixer).Assembly, ResolveSqlite);
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] SortNameFixer: could not install DllImportResolver: {ex.Message}");
            }
        }

        private static IntPtr ResolveSqlite(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName != LibName)
                return IntPtr.Zero;

            // Order matters: try the system sqlite first, then Emby's bundled
            // SQLitePCL raw lib (e_sqlite3), then a handful of platform names.
            string[] candidates;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                candidates = new[]
                {
                    "sqlite3", "sqlite3.dll",
                    "e_sqlite3", "e_sqlite3.dll",
                    "winsqlite3", "winsqlite3.dll",
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                candidates = new[]
                {
                    "libsqlite3.dylib", "libsqlite3.0.dylib",
                    "libe_sqlite3.dylib",
                    "/usr/lib/libsqlite3.dylib",
                };
            }
            else
            {
                // Linux / Synology / Docker (Debian, Alpine, LSIO).
                candidates = new[]
                {
                    "libsqlite3.so.0", "libsqlite3.so", "libsqlite3.so.3",
                    "libe_sqlite3.so",
                    "/usr/lib/libsqlite3.so.0",
                    "/usr/lib/x86_64-linux-gnu/libsqlite3.so.0",
                    "/usr/local/lib/libsqlite3.so.0",
                    "/lib/libsqlite3.so.0",
                    "/lib/libsqlite3.so.3",
                    "/lib/x86_64-linux-gnu/libsqlite3.so.0",
                    "/volume1/@appstore/sqlite/lib/libsqlite3.so.0",
                };
            }

            foreach (var candidate in candidates)
            {
                if (NativeLibrary.TryLoad(candidate, out var handle))
                {
                    Interlocked.Exchange(ref _nativeAvailable, 1);
                    return handle;
                }
            }

            // Last resort: scan a few common lib directories for any sqlite3
            // shared object. Catches odd builds like libsqlite3.so.3.49.2 that
            // some Emby docker images ship.
            string[] dirs =
            {
                "/lib", "/lib64",
                "/usr/lib", "/usr/lib64",
                "/usr/lib/x86_64-linux-gnu",
                "/usr/lib/aarch64-linux-gnu",
                "/usr/local/lib",
            };
            foreach (var dir in dirs)
            {
                try
                {
                    if (!System.IO.Directory.Exists(dir)) continue;
                    foreach (var file in System.IO.Directory.EnumerateFiles(dir, "libsqlite3.so*"))
                    {
                        if (NativeLibrary.TryLoad(file, out var handle))
                        {
                            Interlocked.Exchange(ref _nativeAvailable, 1);
                            return handle;
                        }
                    }
                }
                catch { }
            }

            Interlocked.Exchange(ref _nativeAvailable, 0);
            return IntPtr.Zero;
        }

        public static void Schedule()
        {
            if (Interlocked.CompareExchange(ref _scheduled, 1, 0) != 0) return;
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(2_000).ConfigureAwait(false);
                    Apply("pass1");
                    await Task.Delay(20_000).ConfigureAwait(false);
                    Apply("pass2");
                }
                catch (Exception ex)
                {
                    YouTubeChannel.LogPublic($"[YT] SortNameFixer: scheduled run failed: {ex.Message}");
                }
                finally { Interlocked.Exchange(ref _scheduled, 0); }
            });
        }

        private static void Apply(string passLabel)
        {
            // If we already know there's no sqlite available, don't keep
            // retrying. The plugin still works — Emby just falls back to its
            // default sort order until a future restart can find sqlite.
            if (Volatile.Read(ref _nativeAvailable) == 0)
            {
                YouTubeChannel.LogPublic($"[YT] SortNameFixer({passLabel}): skipped, no sqlite native lib found earlier");
                return;
            }

            var dbPath = Plugin.LibraryDbPath;
            if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
            {
                YouTubeChannel.LogPublic($"[YT] SortNameFixer({passLabel}): library.db not found (path={dbPath ?? "<null>"})");
                return;
            }

            IntPtr db;
            try
            {
                if (sqlite3_open(dbPath, out db) != 0)
                {
                    if (db != IntPtr.Zero) sqlite3_close(db);
                    YouTubeChannel.LogPublic($"[YT] SortNameFixer({passLabel}): sqlite3_open returned non-zero for {dbPath}");
                    return;
                }
            }
            catch (DllNotFoundException ex)
            {
                Interlocked.Exchange(ref _nativeAvailable, 0);
                YouTubeChannel.LogPublic($"[YT] SortNameFixer({passLabel}): sqlite library not found: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                YouTubeChannel.LogPublic($"[YT] SortNameFixer({passLabel}): sqlite3_open failed: {ex.Message}");
                return;
            }

            try
            {
                Run(db, "PRAGMA busy_timeout = 5000;");

                // PremiereDate/DateCreated can be stored as .NET ticks
                // (~6e17 for "now") or as Unix epoch seconds (~1.7e9), depending
                // on the Emby build. We pick via threshold 1e12: below = seconds,
                // above = ticks. Convert to days-since-epoch and subtract from
                // a big constant so newer videos sort first.
                //
                // We strip any existing 10-digit prefix from SortName before
                // re-applying so old wrong prefixes get overwritten. The WHERE
                // makes the whole update self-healing: every row whose current
                // prefix doesn't match what we'd compute now gets re-processed.
                //
                // Runs across every YouTube item regardless of the per-channel
                // ChannelSortBy setting, so the user doesn't have to pick a
                // sort order per folder.
                const string sql = @"
                    UPDATE MediaItems
                    SET SortName = printf('%010d',
                                          9999999999 -
                                          CASE
                                            WHEN COALESCE(PremiereDate, DateCreated, 0) > 1000000000000
                                              THEN COALESCE(PremiereDate, DateCreated, 0) / 864000000000
                                            ELSE COALESCE(PremiereDate, DateCreated, 0) / 86400
                                          END)
                                   || ' ' ||
                                   COALESCE(
                                     NULLIF(
                                       CASE
                                         WHEN SortName GLOB '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9] *'
                                           THEN substr(SortName, 12)
                                         ELSE SortName
                                       END, ''),
                                     Name, '')
                    WHERE type = 8
                      AND COALESCE(PremiereDate, DateCreated, 0) > 0
                      AND ExternalId IS NOT NULL
                      AND (length(ExternalId) = 11 OR ExternalId LIKE 'LIVE_%' OR ExternalId LIKE 'REEL_%')
                      AND (
                        SortName IS NULL
                        OR SortName NOT GLOB '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9] *'
                        OR substr(SortName, 1, 10) <> printf('%010d',
                            9999999999 -
                            CASE
                              WHEN COALESCE(PremiereDate, DateCreated, 0) > 1000000000000
                                THEN COALESCE(PremiereDate, DateCreated, 0) / 864000000000
                              ELSE COALESCE(PremiereDate, DateCreated, 0) / 86400
                            END)
                      )";

                Run(db, sql);
                int changed;
                try { changed = sqlite3_changes(db); } catch { changed = -1; }
                YouTubeChannel.LogPublic($"[YT] SortNameFixer({passLabel}): updated {changed} rows");
            }
            finally { sqlite3_close(db); }
        }

        private static void Run(IntPtr db, string sql)
        {
            var rc = sqlite3_exec(db, sql, IntPtr.Zero, IntPtr.Zero, out var errmsg);
            if (errmsg != IntPtr.Zero)
            {
                try
                {
                    var msg = Marshal.PtrToStringUTF8(errmsg);
                    if (rc != 0 && !string.IsNullOrEmpty(msg))
                        YouTubeChannel.LogPublic($"[YT] SortNameFixer: sqlite_exec rc={rc}: {msg}");
                }
                finally { sqlite3_free(errmsg); }
            }
        }
    }
}

