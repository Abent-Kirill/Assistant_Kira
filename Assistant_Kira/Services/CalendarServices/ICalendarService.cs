namespace Assistant_Kira.Services.CalendarServices;

internal interface ICalendarService
{
    public Task<bool> CreateEventAsync(string[] args);
    public Task<IReadOnlyCollection<string>> GetEvents(DateTimeOffset date);
}
