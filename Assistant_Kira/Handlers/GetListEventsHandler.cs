using System.Collections.ObjectModel;

using Assistant_Kira.Options;
using Assistant_Kira.Requests;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

using MediatR;

using Microsoft.Extensions.Options;

namespace Assistant_Kira.Handlers;

internal sealed class GetListEventsHandler(IOptions<CalendarOptions> configuration) : IRequestHandler<GetListEventsRequest, IReadOnlyCollection<string>>
{
    public async Task<IReadOnlyCollection<string>> Handle(GetListEventsRequest request, CancellationToken cancellationToken)
    {
        var key = File.ReadAllTextAsync(configuration.Value.Aunth.LocalPath, cancellationToken);

        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await key)
            .CreateScoped(CalendarService.Scope.CalendarEvents),
            ApplicationName = "Assistant Kira"
        });

        var listRequest = service.Events.List(configuration.Value.Name);
        listRequest.TimeMinDateTimeOffset = DateTimeOffset.UtcNow;
        listRequest.TimeMaxDateTimeOffset = DateTimeOffset.UtcNow.AddDays(request.Days);
        var events = await listRequest.ExecuteAsync(cancellationToken);
        var tmp = new List<string>();
        foreach (var calendarEvent in events.Items)
        {
            tmp.Add($"{calendarEvent.Id}:{calendarEvent.Summary}");
        }

        return new ReadOnlyCollection<string>(tmp);
    }
}
