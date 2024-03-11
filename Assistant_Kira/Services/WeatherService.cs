using System.Text.Json;

using Assistant_Kira.Models.OpenWeatherMap;

namespace Assistant_Kira.Services;

internal sealed class WeatherService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
{
    public async Task<Weather> GetWeatherAsync(string city = "Astana")
    {
        var response = await httpClientFactory.CreateClient("OpenWeather")
            .GetAsync(new Uri(@$"weather?q={city}&units=metric&lang=ru&appid={configuration["ServicesApiKeys:Weather"]}", UriKind.Relative));

        var result = response.EnsureSuccessStatusCode();
        var weather = JsonSerializer.Deserialize<Weather>(await result.Content.ReadAsStringAsync());
        return weather;
    }
}
