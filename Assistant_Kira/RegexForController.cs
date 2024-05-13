using System.Text.RegularExpressions;

namespace Assistant_Kira;

internal static partial class RegexForController
{
    //TODO: Написать unit-tests
    [GeneratedRegex(@"^\d+\s\w{3}\s\w{3}$")]
    public static partial Regex ConvertCurrencyRegex();

    [GeneratedRegex(@"(?<hour>\d{1,2}):(?<minute>\d{1,2})", RegexOptions.IgnoreCase, "ru-KZ")]
    public static partial Regex CalendarEventRegex();

    public static bool IsMatchConvertCurrencyRegex(this string text)
    {
        return ConvertCurrencyRegex().Match(text).Success;
    }

    public static bool IsMatchCalendarEventRegex(this string text)
    {
        return CalendarEventRegex().Match(text).Success;
    }
}
