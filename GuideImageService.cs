using MediaBrowser.Model.Services;
using System;
using System.IO;

namespace Emby.YouTubePlugin
{
    [Route("/YouTubePlugin/GuideImage/{Name}", "GET", Summary = "Gets a setup guide image.")]
    public class GetYouTubeGuideImage : IReturn<Stream>
    {
        public string? Name { get; set; }
    }

    public sealed class GuideImageService : IService, IRequiresRequest
    {
        public IRequest Request { get; set; } = null!;

        public object Get(GetYouTubeGuideImage request)
        {
            var name = Path.GetFileName(request.Name ?? string.Empty);
            if (string.IsNullOrWhiteSpace(name)
                || name.IndexOfAny(new[] { '/', '\\' }) >= 0
                || !name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            var resourceName = $"{typeof(Plugin).Namespace}.Configuration.guide.{name}";
            var stream = typeof(Plugin).Assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return NotFound();
            }

            SetResponse("image/png", 200);
            Request.Response.AddHeader("Cache-Control", "public, max-age=86400");
            return stream;
        }

        private object NotFound()
        {
            SetResponse("text/plain", 404);
            return "Guide image not found.";
        }

        private void SetResponse(string contentType, int statusCode)
        {
            Request.ResponseContentType = contentType;
            Request.Response.ContentType = contentType;
            Request.Response.StatusCode = statusCode;
        }
    }
}
