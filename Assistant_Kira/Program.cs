using System.Text.Json;
using System.Text.Json.Serialization;
using Assistant_Kira;
using Assistant_Kira.Commands;
using Assistant_Kira.Models;
using Assistant_Kira.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(option =>
{
	option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
	option.JsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
	option.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient
(
	"OpenWeather", client =>
	{
		client.BaseAddress = new(@"https://api.openweathermap.org/data/2.5/");
		client.Timeout = TimeSpan.FromSeconds(10);
	}
);
builder.Services.AddSingleton<KiraBot>();
builder.Services.AddTransient<WeatherService>();
builder.Services.AddTransient<CurrencyService>();
builder.Services.AddTransient<ICommand, HelloCommand>();
builder.Services.AddTransient<ICommand, WeatherCommand>();
builder.Services.AddTransient<ICommand, CurrencyCommand>();
builder.Services.AddTransient<ICommandExecutor, CommandExecutor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Services.GetRequiredService<KiraBot>();
app.UseRouting();
app.Run();