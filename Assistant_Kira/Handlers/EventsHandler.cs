using Assistant_Kira.Options;
using Assistant_Kira.Requests;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

using MediatR;

using Microsoft.Extensions.Options;

namespace Assistant_Kira.Handlers;

internal sealed class EventsHandler(IOptions<CalendarOptions> configuration) : IRequestHandler<EventsRequest, IReadOnlyCollection<string>>
{
    public async Task<IReadOnlyCollection<string>> Handle(EventsRequest request, CancellationToken cancellationToken)
    {
        var json = File.ReadAllTextAsync(configuration.Value.Aunth.LocalPath, cancellationToken);
        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await json)
            .CreateScoped(CalendarService.Scope.CalendarEventsReadonly),
            ApplicationName = "Assistant Kira"
        });

        var eventsRequest = service.Events.List(configuration.Value.Name);
        eventsRequest.TimeMinDateTimeOffset = request.DateTime;
        eventsRequest.TimeMaxDateTimeOffset = request.DateTime.AddDays(1);

        var events = await eventsRequest.ExecuteAsync(cancellationToken);
        return events.Items.Select(x => $"{x.Summary}\n{x.Description}").ToList();
    }
}
