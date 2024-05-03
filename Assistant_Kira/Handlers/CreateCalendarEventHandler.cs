using System.Collections.Immutable;
using System.Text;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

using NodaTime.Text;

using static Google.Apis.Calendar.v3.EventsResource;

using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class CreateCalendarEventHandler(IConfiguration  configuration) : IRequestHandler<CreateCalendarEventRequest, bool>
{
    public async Task<bool> Handle(CreateCalendarEventRequest request, CancellationToken cancellationToken)
    {
        var newEvent = CreateEvent(request.Text.Split(' '));
        if (newEvent is null)
        {
            return false; //Ошибку?
        }

        var json = File.ReadAllTextAsync(configuration["ServicesApiKeys:GoogleCalendarAunth"], cancellationToken);

        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await json)
            .CreateScoped(CalendarService.Scope.CalendarEvents),
            ApplicationName = "Assistant Kira"
        });
        InsertRequest request1 = service.Events.Insert(newEvent, "blayner0027@gmail.com");
        Event createdEvent = await request1.ExecuteAsync(cancellationToken);
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
        else if (args.Any(x => x.Equals("сегодня", StringComparison.CurrentCultureIgnoreCase)))
        {
            return DateOnly.FromDateTime(DateTime.Today);
        }
        else if (args.Any(x => x.Equals("послезавтра", StringComparison.CurrentCultureIgnoreCase)))
        {
            return DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        }
        else if (args.Any(x => x.Equals("вчера", StringComparison.CurrentCultureIgnoreCase)))
        {
            return DateOnly.FromDateTime(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day - 1));
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
}
