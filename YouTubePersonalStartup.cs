using MediaBrowser.Common;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;

namespace Emby.YouTubePlugin;

// Emby clears its startup HTTP gate before ordinary entrypoints run. Cached
// library rows therefore need guards in the explicit pre-startup phase.
public sealed class YouTubePersonalStartup : IServerEntryPoint, IRunBeforeStartup
{
    private readonly ILibraryManager _library;
    private readonly IUserManager _users;
    private readonly IUserViewManager _views;
    public YouTubePersonalStartup(IApplicationHost host, ILibraryManager library, IUserManager users, IUserViewManager views)
    {
        Plugin.InitializeApplicationHost(host);
        _library = library;
        _users = users;
        _views = views;
    }
    public void Run()
    {
        EmbeddedDependencyLoader.Register();
        YouTubePersonalCatalog.Start(_library, _users, _views);
    }
    public void Dispose() => YouTubePersonalCatalog.Stop();
}
