using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace Emby.YouTubePlugin
{
    public class YouTubeExternalId : IExternalId
    {
        public string Name => "YouTube";

        public string Key => "YouTube";

        public string UrlFormatString => "http://www.youtube.com/watch?v={0}";

        public bool Supports(IHasProviderIds item)
        {
            return item?.GetProviderId(Key) is { Length: > 0 };
        }
    }
}
