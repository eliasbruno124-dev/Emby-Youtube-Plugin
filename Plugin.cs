using MediaBrowser.Common;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    public class Plugin : BasePluginSimpleUI<PluginConfiguration>, IHasThumbImage
    {
        private static readonly string PluginVersion =
            typeof(Plugin).Assembly.GetName().Version?.ToString(3) ?? "0.0.0";

        public override string Name => $"YouTube v{PluginVersion}";
        public override string Description => "Official YouTube integration for Emby via YouTube Data API v3.";
        public override Guid Id => Guid.Parse("B2C3D4E5-F6A7-4B5C-9D0E-1F2A3B4C5D6E");

        public static Plugin? Instance { get; private set; }
        public static string? LibraryDbPath { get; private set; }
        public static IApplicationHost? AppHost { get; private set; }

        public Plugin(IApplicationHost applicationHost) : base(applicationHost)
        {
            Instance = this;
            AppHost = applicationHost;
            try
            {
                var paths = applicationHost.Resolve<IServerApplicationPaths>();
                if (paths != null)
                {
                    var candidate = Path.Combine(paths.DataPath, "library.db");
                    if (File.Exists(candidate))
                        LibraryDbPath = candidate;
                }
            }
            catch { }
        }

        public PluginConfiguration Options => GetOptions();

        public ImageFormat ThumbImageFormat => ImageFormat.Png;

        public override PluginInfo GetPluginInfo()
        {
            var info = base.GetPluginInfo();
            var assemblyDir = Path.GetDirectoryName(AssemblyFilePath) ?? string.Empty;
            var thumbPath = Path.Combine(assemblyDir, "thumb.png");

            if (File.Exists(thumbPath))
            {
                var imageTag = File.GetLastWriteTimeUtc(thumbPath)
                    .Ticks
                    .ToString(CultureInfo.InvariantCulture);

                SetStringPropertyIfExists(info, "ImageTag", imageTag);

                var pngBytes = File.ReadAllBytes(thumbPath);
                var imageDataUrl = "data:image/png;base64," + Convert.ToBase64String(pngBytes);
                SetStringPropertyIfExists(info, "ImageUrl", imageDataUrl);
            }

            return info;
        }

        private static void SetStringPropertyIfExists(object target, string propertyName, string value)
        {
            var property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            if (property?.CanWrite == true && property.PropertyType == typeof(string))
            {
                property.SetValue(target, value);
            }
        }

        public Stream GetThumbImage()
        {
            var type = GetType();
            var stream = type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.png");
            return stream ?? new MemoryStream();
        }
    }

    public class PluginEntryPoint : IServerEntryPoint
    {
        private Timer? _pollTimer;
        private string _lastVideoIds = "";
        private static object? _channelMgr;
        private static object? _registeredChannel;
        private static MethodInfo? _refreshContentMethod;
        private static readonly HttpClient PollHttp = new() { Timeout = TimeSpan.FromSeconds(10) };

        public void Run()
        {
            YouTubeChannel.ScheduleSortNameFix();
            _pollTimer = new Timer(PollTick, null,
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(10));
        }

        private void PollTick(object? state)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var config = Plugin.Instance?.Options;
                    if (config == null) return;
                    var playlist = (config.WatchLaterPlaylist ?? "").Trim();
                    if (playlist.Length <= 2) return;
                    var apiKey = (config.ApiKey ?? "").Trim();
                    if (string.IsNullOrEmpty(apiKey)) return;

                    var url = $"https://www.googleapis.com/youtube/v3/playlistItems" +
                              $"?part=contentDetails&playlistId={Uri.EscapeDataString(playlist)}" +
                              $"&maxResults=50&key={Uri.EscapeDataString(apiKey)}";

                    var json = await PollHttp.GetStringAsync(url).ConfigureAwait(false);

                    var ids = new List<string>();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("items", out var items)
                        && items.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            if (item.TryGetProperty("contentDetails", out var cd)
                                && cd.TryGetProperty("videoId", out var vid))
                            {
                                var videoId = vid.GetString();
                                if (!string.IsNullOrEmpty(videoId))
                                    ids.Add(videoId);
                            }
                        }
                    }

                    var current = string.Join(",", ids);
                    if (_lastVideoIds.Length > 0 && current != _lastVideoIds)
                        TriggerRefresh();

                    _lastVideoIds = current;
                }
                catch { }
            });
        }

        private static void TriggerRefresh()
        {
            try
            {
                if (!EnsureChannelManager()) return;

                var pars = _refreshContentMethod!.GetParameters();
                var args = new object?[pars.Length];
                for (int i = 0; i < pars.Length; i++)
                {
                    var pt = pars[i].ParameterType;
                    var name = pars[i].Name ?? "";
                    if (pt.Name == "IChannel" || name == "channel")
                        args[i] = _registeredChannel;
                    else if (pt == typeof(CancellationToken))
                        args[i] = CancellationToken.None;
                    else if (name.Contains("maxRefresh", StringComparison.OrdinalIgnoreCase))
                        args[i] = 2;
                    else if (pt == typeof(string))
                        args[i] = null;
                    else if (pars[i].HasDefaultValue)
                        args[i] = pars[i].DefaultValue;
                    else
                        args[i] = pt.IsValueType ? Activator.CreateInstance(pt) : null;
                }

                var result = _refreshContentMethod.Invoke(_channelMgr, args);
                if (result is Task task)
                    task.Wait(TimeSpan.FromSeconds(60));
            }
            catch { }
        }

        private static bool EnsureChannelManager()
        {
            if (_channelMgr != null && _registeredChannel != null && _refreshContentMethod != null)
                return true;

            var appHost = Plugin.AppHost;
            if (appHost == null) return false;

            Type? iface = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try { iface = asm.GetTypes().FirstOrDefault(t => t.IsInterface && t.Name == "IChannelManager"); }
                catch { }
                if (iface != null) break;
            }
            if (iface == null) return false;

            var resolve = appHost.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Resolve" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
            if (resolve == null) return false;

            _channelMgr = resolve.MakeGenericMethod(iface).Invoke(appHost, null);
            if (_channelMgr == null) return false;

            _refreshContentMethod =
                _channelMgr.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == "RefreshChannelContent");
            if (_refreshContentMethod == null) return false;

            var channelsProp = _channelMgr.GetType().GetProperty("Channels",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (channelsProp != null)
            {
                var channels = channelsProp.GetValue(_channelMgr);
                if (channels is System.Collections.IEnumerable enumerable)
                {
                    foreach (var ch in enumerable)
                    {
                        if (ch?.GetType().Name == "YouTubeChannel")
                        {
                            _registeredChannel = ch;
                            break;
                        }
                    }
                }
            }

            return _registeredChannel != null;
        }

        public void Dispose()
        {
            _pollTimer?.Dispose();
        }
    }
}
