using System.Text.Json;
using JsonExtensions.Reading;

namespace YoutubeExplode.Bridge;

internal class ThumbnailData(JsonElement content)
{

    public string? Url => content.GetPropertyOrNull("url")?.GetStringOrNull();

    public int? Width => content.GetPropertyOrNull("width")?.GetInt32OrNull();

    public int? Height => content.GetPropertyOrNull("height")?.GetInt32OrNull();
}
