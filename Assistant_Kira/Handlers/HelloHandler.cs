using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class HelloHandler : IRequestHandler<HelloRequest, string>
{
    public async Task<string> Handle(HelloRequest request, CancellationToken cancellationToken) => await Task.Run(() => $"{GetGreeting()}, {request.Name}\nЧто хотите сделать?");

    private string GetGreeting()
    {
        var currentTime = DateTime.Now;
        var currentHour = currentTime.Hour;

        if (currentHour >= 6 && currentHour < 12)
        {
            return "Доброе утро";
        }
        else if (currentHour >= 12 && currentHour < 18)
        {
            return "Добрый день";
        }
        else
        {
            return "Добрый вечер";
        }
    }
}
