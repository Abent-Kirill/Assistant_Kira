using Telegram.Bot.Types.ReplyMarkups;

namespace Assistant_Kira;

internal static class KeyboardSamples
{
    public static ReplyKeyboardMarkup Menu => new(new[]
            {
                [
                    new KeyboardButton("Погода"),
                    new KeyboardButton("Новости")
                ],
                new []
                {
                    new KeyboardButton("Курс"),
                    new KeyboardButton("Вакансии")
                }
            })
    {
        ResizeKeyboard = true
    };

    public static InlineKeyboardMarkup NewsKeyboard => new(new[]
    {
        new []
        {
            InlineKeyboardButton.WithCallbackData("Назад", "back_news"),
            InlineKeyboardButton.WithCallbackData("Вперёд", "next_news")
        }
    });

    public static InlineKeyboardMarkup VacanciesKeyboard => new(new[]
    {
        new []
        {
            InlineKeyboardButton.WithCallbackData("Назад", "back_vacancy"),
            InlineKeyboardButton.WithCallbackData("Вперёд", "next_vacancy")
        }
    });
}
