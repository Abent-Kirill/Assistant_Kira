using System.Text.Json;
using System.Text.Json.Serialization;

using Telegram.Bot.Types.ReplyMarkups;

namespace Assistant_Kira.JsonConverts;

internal sealed class InlineKeyboardMarkupConverter : JsonConverter<InlineKeyboardMarkup>
{
    public override InlineKeyboardMarkup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var keyboard = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
        var inlineKeyboard = new List<List<InlineKeyboardButton>>();

        if (keyboard.TryGetProperty("inline_keyboard", out var inlineKeyboardArray) && inlineKeyboardArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var rowElement in inlineKeyboardArray.EnumerateArray())
            {
                var row = new List<InlineKeyboardButton>();

                foreach (var buttonElement in rowElement.EnumerateArray())
                {
                    var button = new InlineKeyboardButton(buttonElement.GetProperty("text").GetString()!)
                    {
                        CallbackData = buttonElement.GetProperty("callback_data").GetString()
                    };
                    row.Add(button);
                }
                inlineKeyboard.Add(row);
            }
        }

        return new InlineKeyboardMarkup(inlineKeyboard);
    }

    public override void Write(Utf8JsonWriter writer, InlineKeyboardMarkup value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteStartArray("inline_keyboard");

        foreach (var row in value.InlineKeyboard)
        {
            writer.WriteStartArray();

            foreach (var button in row)
            {
                writer.WriteStartObject();
                writer.WriteString("text", button.Text);
                writer.WriteString("callback_data", button.CallbackData);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
