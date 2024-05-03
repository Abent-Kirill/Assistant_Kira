using Assistant_Kira.DTO.OpenWeatherMap;

using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record WeatherRequest(string City) : IRequest<Weather>{}