using Assistant_Kira.Options;
using Assistant_Kira.Requests;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

using MediatR;

using Microsoft.Extensions.Options;

namespace Assistant_Kira.Handlers;

internal sealed class DeleteEventHandler(IOptions<CalendarOptions> configuration) : IRequestHandler<DeleteEventRequest, string>
{
    public async Task<string> Handle(DeleteEventRequest request, CancellationToken cancellationToken)
    {
        var key = File.ReadAllTextAsync(configuration.Value.Aunth.LocalPath, cancellationToken);

        using var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(await key)
            .CreateScoped(CalendarService.Scope.CalendarEvents),
            ApplicationName = "Assistant Kira"
        });

        var deleteRequest = service.Events.Delete(configuration.Value.Name, request.EventId);
        return await deleteRequest.ExecuteAsync(cancellationToken);
    }
}
