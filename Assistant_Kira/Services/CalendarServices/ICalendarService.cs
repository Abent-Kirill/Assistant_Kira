namespace Assistant_Kira.Services.CalendarServices;

internal interface ICalendarService
{
    public Task<bool> CreateEventAsync(string[] args);
}
