using System.Text.Json;
using System.Text.Json.Serialization;

namespace Assistant_Kira;

internal sealed class UnixTimestampConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Number)
		{
			long unixTime = reader.GetInt64();
			return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
		}
		throw new JsonException("Invalid timestamp format");
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		long unixTime = new DateTimeOffset(value).ToUnixTimeSeconds();
		writer.WriteNumberValue(unixTime);
	}
}
