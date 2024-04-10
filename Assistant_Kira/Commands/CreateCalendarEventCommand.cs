using Assistant_Kira.Services.CalendarServices;

namespace Assistant_Kira.Commands;

internal sealed class CreateCalendarEventCommand(ICalendarService calendarService) : Command
{
    public override string Name => "Новое событие";

    public override async Task<string> ExecuteAsync(params string[] args)
    {
        var result = await calendarService.CreateEventAsync(args);
        return result ? $"Событие создано" : "Что-то пошло не так";
    }
}
