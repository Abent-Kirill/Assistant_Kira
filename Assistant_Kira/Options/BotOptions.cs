using System.Text.Json.Serialization;

namespace Assistant_Kira.Options;

public sealed class BotOptions
{
    public string Token { get; init; }
    public Uri WebHook { get; init; }
    public long ChatId { get; init; }

    [JsonConstructor]
    public BotOptions(string token, string webHook, string chatId)
    {
        Token = token;
        WebHook = new Uri(webHook, UriKind.Absolute);

        if (long.TryParse(chatId, out var result))
        {
            ChatId = result;
        }
        else
        {
            throw new ArgumentException("ChatId не валидный");
        }

    }
}
