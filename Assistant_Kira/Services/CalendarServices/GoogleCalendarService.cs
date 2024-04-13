using System.Collections.Immutable;
using System.Text;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

using NodaTime.Text;

using static Google.Apis.Calendar.v3.EventsResource;

namespace Assistant_Kira.Services.CalendarServices;

internal sealed class GoogleCalendarService(IConfiguration configuration) : ICalendarService
{
    public async Task<bool> CreateEventAsync(string[] args)
    {
        var newEvent = CreateEvent(args);
        if(newEvent is null)
        {
            return false; //Ошибку?
        }

        var json = File.ReadAllTextAsync(configuration["ServicesApiKeys:GoogleCalendarAunth"]);

        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await json)
            .CreateScoped(CalendarService.Scope.CalendarEvents),
            ApplicationName = "Assistant Kira"
        });
        InsertRequest request = service.Events.Insert(newEvent, "blayner0027@gmail.com");
        Event createdEvent = await request.ExecuteAsync();
        return !string.IsNullOrEmpty(createdEvent.HtmlLink);
    }

    private TimeOnly? ParseTime(string[] args)
    {
        foreach (var arg in args)
        {
            var timeParser = LocalTimePattern.CreateWithInvariantCulture("HH':'mm").Parse(arg);
            if (timeParser.Success)
            {
                return timeParser.Value.ToTimeOnly();
            }
        }

        return null;
    }

    private DateOnly? ParseDate(string[] args)
    {
        if (args.Any(x => x.Equals("завтра", StringComparison.CurrentCultureIgnoreCase)))
        {
            return DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        }

        ImmutableArray<LocalDatePattern> patterns =
        [
            LocalDatePattern.CreateWithInvariantCulture("dd'.'MM"),
                LocalDatePattern.CreateWithInvariantCulture("dd'.'MM'.'uuuu"),
                LocalDatePattern.CreateWithInvariantCulture("dd' 'MMMM")
        ];

        foreach (var pattern in patterns)
        {
            foreach (var arg in args)
            {
                var parser = pattern.Parse(arg);
                if (parser.Success)
                {
                    return parser.Value.ToDateOnly();
                }
            }
        }
        return null;
    }

    private DateTime ParseDateTime(string[] args)
    {
        var dateOnly = ParseDate(args);
        var timeOnly = ParseTime(args);

        ArgumentNullException.ThrowIfNull(dateOnly);
        ArgumentNullException.ThrowIfNull(timeOnly);

        return new DateTime(dateOnly!.Value.Year, dateOnly.Value.Month, dateOnly.Value.Day, timeOnly!.Value.Hour, timeOnly.Value.Minute, timeOnly.Value.Second);
    }

    private string GetDescription(string[] args)
    {
        var strBuilder = new StringBuilder();
        foreach (var arg in args)
        {
            strBuilder.Append($"{arg} ");
        }
        return strBuilder.ToString();
    }

    private Event? CreateEvent(string[] args)
    {
        var dateTime = new DateTime();
        try
        {
            dateTime = ParseDateTime(args);
        }
        catch (ArgumentNullException)
        {
            return null;
        }

        return new Event()
        {
            Summary = GetSummary(args),
            Description = GetDescription(args),
            Start = new EventDateTime() { DateTimeDateTimeOffset = new DateTimeOffset(dateTime) },
            End = new EventDateTime() { DateTimeDateTimeOffset = new DateTimeOffset(dateTime).AddHours(1) }
        };
    }

    private string GetSummary(string[] args)
    {
        return $"{args[0]} {args[1]} {args[2]} {args[3]}";
    }

    public async Task<IReadOnlyCollection<string>> GetEvents(DateTimeOffset date)
    {
        var json = File.ReadAllTextAsync(configuration["ServicesApiKeys:GoogleCalendarAunth"]);
        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await json)
            .CreateScoped(CalendarService.Scope.CalendarEventsReadonly),
            ApplicationName = "Assistant Kira"
        });
        var eventsRequest = service.Events.List("blayner0027@gmail.com");
        eventsRequest.TimeMinDateTimeOffset = date;
        eventsRequest.TimeMaxDateTimeOffset = date.AddDays(1);

        var events = await eventsRequest.ExecuteAsync();
        return events.Items.Select(x => $"{x.Summary}\n{x.Description}").ToList();
    }
}
