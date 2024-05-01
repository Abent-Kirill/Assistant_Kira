using Assistant_Kira.DTO.OpenWeatherMap;

namespace Assistant_Kira.Services.WeatherServices;

internal interface IWeatherService
{
    public Task<Weather> GetWeatherAsync(string city);
}
