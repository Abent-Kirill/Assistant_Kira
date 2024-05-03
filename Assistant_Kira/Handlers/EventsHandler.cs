using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

using MediatR;

namespace Assistant_Kira;

internal sealed class EventsHandler(IConfiguration configuration) : IRequestHandler<EventsRequest, IReadOnlyCollection<string>>
{
    public async Task<IReadOnlyCollection<string>> Handle(EventsRequest request, CancellationToken cancellationToken)
    {
        var json = File.ReadAllTextAsync(configuration["ServicesApiKeys:GoogleCalendarAunth"], cancellationToken);
        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await json)
            .CreateScoped(CalendarService.Scope.CalendarEventsReadonly),
            ApplicationName = "Assistant Kira"
        });

        var eventsRequest = service.Events.List("blayner0027@gmail.com");
        eventsRequest.TimeMinDateTimeOffset = request.DateTime;
        eventsRequest.TimeMaxDateTimeOffset = request.DateTime.AddDays(1);

        var events = await eventsRequest.ExecuteAsync(cancellationToken);
        return events.Items.Select(x => $"{x.Summary}\n{x.Description}").ToList();
    }
}
