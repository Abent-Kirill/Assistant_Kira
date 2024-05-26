using System.Text.Json.Serialization;

namespace Assistant_Kira.Options;

internal sealed class CalendarOptions
{
    public string Name { get; init; }
    public Uri Aunth { get; init; }

    [JsonConstructor]
    public CalendarOptions(string name, string aunth)
    {
        Name = name;
        Aunth = new Uri(aunth);
    }
}
