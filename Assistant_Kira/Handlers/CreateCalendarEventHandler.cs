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
using Microsoft.Extensions.Options;
using Assistant_Kira.Options;

namespace Assistant_Kira.Handlers;

internal sealed class CreateCalendarEventHandler(IOptions<CalendarOptions> configuration) : IRequestHandler<CreateCalendarEventRequest, bool>
{
    public async Task<bool> Handle(CreateCalendarEventRequest request, CancellationToken cancellationToken)
    {
        var newEvent = CreateEvent(request.Text.Split(' '));
        if (newEvent is null)
        {
            return false; //Ошибку?
        }

        var key = File.ReadAllTextAsync(configuration.Value.Aunth.LocalPath, cancellationToken);

        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await key)
            .CreateScoped(CalendarService.Scope.CalendarEvents),
            ApplicationName = "Assistant Kira"
        });
        var insertRequest = service.Events.Insert(newEvent, configuration.Value.Name);
        var createdEvent = await insertRequest.ExecuteAsync(cancellationToken);
        return !string.IsNullOrEmpty(createdEvent.HtmlLink);
    }

    private static TimeOnly? ParseTime(string[] args)
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

    private static DateOnly? ParseDate(string[] args)
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

    private static DateTime ParseDateTime(string[] args)
    {
        var dateOnly = ParseDate(args);
        var timeOnly = ParseTime(args);

        ArgumentNullException.ThrowIfNull(dateOnly);
        ArgumentNullException.ThrowIfNull(timeOnly);

        return new DateTime(dateOnly!.Value.Year, dateOnly.Value.Month, dateOnly.Value.Day, timeOnly!.Value.Hour, timeOnly.Value.Minute, timeOnly.Value.Second);
    }

    private static string GetDescription(string[] args)
    {
        var strBuilder = new StringBuilder();
        foreach (var arg in args)
        {
            strBuilder.Append($"{arg} ");
        }
        return strBuilder.ToString();
    }

    private static Event? CreateEvent(string[] args)
    {
        var dateTime = ParseDateTime(args);

        return new Event()
        {
            Summary = GetSummary(args),
            Description = GetDescription(args),
            Start = new EventDateTime() { DateTimeDateTimeOffset = new DateTimeOffset(dateTime) },
            End = new EventDateTime() { DateTimeDateTimeOffset = new DateTimeOffset(dateTime).AddHours(1) }
        };
    }

    private static string GetSummary(string[] args)
    {
        return $"{args[0]} {args[1]} {args[2]} {args[3]}";
    }
}
