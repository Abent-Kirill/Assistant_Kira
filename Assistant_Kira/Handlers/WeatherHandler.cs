using System.Text.Json;

using Assistant_Kira.DTO.OpenWeatherMap;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class WeatherHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IRequestHandler<WeatherRequest, Weather>
{
    public async Task<Weather> Handle(WeatherRequest request, CancellationToken cancellationToken)
    {
        using var response = await httpClientFactory.CreateClient("OpenWeather")
            .GetAsync(new Uri(@$"weather?q={request.City}&units=metric&lang=ru&appid={configuration["ServicesApiKeys:Weather"]}", UriKind.Relative), cancellationToken);

        var result = response.EnsureSuccessStatusCode();
        var weather = JsonSerializer.Deserialize<Weather>(await result.Content.ReadAsStringAsync());
        return weather;
    }
}
