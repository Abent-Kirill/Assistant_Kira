using System.Collections.ObjectModel;

using Assistant_Kira.Options;
using Assistant_Kira.Requests;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

using MediatR;

using Microsoft.Extensions.Options;

namespace Assistant_Kira.Handlers;

internal sealed class FindEventHandler(IOptions<CalendarOptions> configuration) : IRequestHandler<FindEventRequest, IReadOnlyCollection<string>>
{
    public async Task<IReadOnlyCollection<string>> Handle(FindEventRequest request, CancellationToken cancellationToken)
    {
        var googleCredetional = await GoogleCredential.FromFileAsync(configuration.Value.Aunth.LocalPath, cancellationToken);
        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = googleCredetional.CreateScoped(CalendarService.Scope.CalendarEvents),
            ApplicationName = "Assistant Kira"
        });

        var listRequest = service.Events.List(configuration.Value.Name);
        listRequest.Q = request.Text;
        var events = await listRequest.ExecuteAsync(cancellationToken);
        var findedCalendarEvents = new List<string>();
        foreach (var calendarEvent in events.Items)
        {
            var startDate = calendarEvent.Start.DateTimeDateTimeOffset.ToString() ?? "?";
            var endDate = calendarEvent.End.DateTimeDateTimeOffset.ToString() ?? "?";
            findedCalendarEvents.Add($"{calendarEvent.Id}:{calendarEvent.Summary} {startDate} - {endDate}\n{calendarEvent.Description}");
        }

        return new ReadOnlyCollection<string>(findedCalendarEvents);
    }
}
