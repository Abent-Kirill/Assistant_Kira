using System.Text.Json.Serialization;

namespace Assistant_Kira.Options;

public sealed class PathOptions
{
    public Uri Photo { get; init; }
    public Uri Files { get; init; }

    [JsonConstructor]
    public PathOptions(string photo, string files)
    {
        Photo = new Uri(photo);
        Files = new Uri(files);
    }
}
