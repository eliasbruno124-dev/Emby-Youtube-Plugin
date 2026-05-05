using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Emby.YouTubePlugin
{
    // Lightweight file logger used as a guaranteed sink so plugin diagnostics
    // never depend on Emby's ILoggerFactory wiring. Writes are best-effort:
    // any IO error is swallowed because losing a log line must never break
    // playback hooks. The file rotates by size so it cannot grow without bound.
    internal static class PluginFileLog
    {
        private const long MaxBytes = 2 * 1024 * 1024;
        private const int MaxBackups = 3;
        private static readonly object Sync = new();
        private static string? _resolvedPath;

        internal static string? Path => _resolvedPath ?? ResolvePath();

        internal static void Write(string message)
        {
            try
            {
                var path = ResolvePath();
                if (path == null)
                    return;

                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [T{Thread.CurrentThread.ManagedThreadId:000}] {message}{Environment.NewLine}";

                lock (Sync)
                {
                    RotateIfNeeded(path);
                    File.AppendAllText(path, line, Encoding.UTF8);
                }
            }
            catch
            {
                // Logging must never throw; swallow failures.
            }
        }

        private static string? ResolvePath()
        {
            if (_resolvedPath != null)
                return _resolvedPath;

            var dataPath = Plugin.DataPath;
            if (string.IsNullOrWhiteSpace(dataPath))
                return null;

            try
            {
                Directory.CreateDirectory(dataPath);
                _resolvedPath = System.IO.Path.Combine(dataPath, "youtube-plugin.log");
                return _resolvedPath;
            }
            catch
            {
                return null;
            }
        }

        private static void RotateIfNeeded(string path)
        {
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists || info.Length < MaxBytes)
                    return;

                for (var i = MaxBackups; i >= 1; i--)
                {
                    var src = i == 1 ? path : path + "." + (i - 1);
                    var dst = path + "." + i;
                    if (!File.Exists(src))
                        continue;

                    if (File.Exists(dst))
                        File.Delete(dst);
                    File.Move(src, dst);
                }
            }
            catch
            {
                // Rotation is opportunistic. If it fails, the next call will
                // simply continue appending to the existing file.
            }
        }
    }
}
