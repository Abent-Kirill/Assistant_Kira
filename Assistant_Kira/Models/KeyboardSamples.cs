using Telegram.Bot.Types.ReplyMarkups;

namespace Assistant_Kira.Models;

internal static class KeyboardSamples
{
    public static ReplyKeyboardMarkup Menu => new(new[]
            {
                new[]
                {
                    new KeyboardButton("Погода"),
                    new KeyboardButton("Новости")
                },
                new[]
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
                    InlineKeyboardButton.WithCallbackData("Назад", "Назад"),
                    InlineKeyboardButton.WithCallbackData("Вперёд", "Вперёд")
                }
            });
    public static InlineKeyboardMarkup VacanciesKeyboard => new(new[]
    {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "Назад v"),
                    InlineKeyboardButton.WithCallbackData("Вперёд", "Вперёд v")
                }
            });

    public static InlineKeyboardMarkup TestInlineKeyboard => new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Кнопка 1", "погода"),
                    InlineKeyboardButton.WithCallbackData("Кнопка 2", "data2")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Кнопка 3", "data3"),
                    InlineKeyboardButton.WithCallbackData("Кнопка 4", "data4")
                }
            });
}
