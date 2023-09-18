using System.Text.Json;
using Assistant_Kira.Models.OpenWeatherMap;

namespace Assistant_Kira.Services;

internal sealed class WeatherService
{
	private readonly IConfiguration _configuration;
	private readonly IHttpClientFactory _httpClientFactory;

	public WeatherService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
	{
		_configuration = configuration;
		_httpClientFactory = httpClientFactory;
	}

	public async Task<Weather> GetWeatherAsync(string city = "Astana")
	{
		var response = await _httpClientFactory.CreateClient("OpenWeather")
			.GetAsync(new Uri(@$"weather?q={city}&units=metric&lang=ru&appid={_configuration["WeatherToken"]}", UriKind.Relative));

		if (!response.IsSuccessStatusCode)
		{
			throw new HttpRequestException();
		}
		try
		{
			var weather = JsonSerializer.Deserialize<Weather>(await response.Content.ReadAsStringAsync());
			return weather;
		}
		catch (ArgumentNullException ex)
		{
			throw;
		}
		catch (NotSupportedException ex)
		{
			throw;
		}
	}
}
