using System.Text.Json.Serialization;

namespace Assistant_Kira.Options;

public sealed class PathOptions
{
    public Uri Photo { get; set; }
    public Uri Files { get; set; }
}
