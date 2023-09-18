using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.OpenWeatherMap;

internal struct Wind
{
	[JsonPropertyName(name: "speed")]
	public float Speed { get; set; }

	public override readonly string ToString() => $"Скорость ветра - {Speed} м/с";
}