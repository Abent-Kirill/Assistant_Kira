using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.OpenWeatherMap;

internal struct Wind
{
	[JsonPropertyName("speed")]
	public float Speed { get; init; }

    [JsonConstructor]
    public Wind(float speed)
    {
        Speed = speed;
    }

    public override readonly string ToString() => $"Скорость ветра - {Speed} м/с";
}
