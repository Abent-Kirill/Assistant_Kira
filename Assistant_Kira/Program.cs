using System.Text.Json;
using System.Text.Json.Serialization;

using Assistant_Kira;
using Assistant_Kira.Commands;
using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Microsoft.AspNetCore.Server.Kestrel.Core;

using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps();
    });
});

builder.Services.AddLogging(x => x.ClearProviders().AddSerilog());
builder.Services.AddControllers().AddJsonOptions(option =>
{
    option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    option.JsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
    option.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient
(
    "OpenWeather", client =>
    {
        client.BaseAddress = new(@"https://api.openweathermap.org/data/2.5/", UriKind.Absolute);
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
    }
);
builder.Services.AddHttpClient
(
    "Apilayer", client =>
    {
        client.BaseAddress = new(@"https://api.apilayer.com/fixer/", UriKind.Absolute);
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("apikey", builder.Configuration["ApikeyCurrency"]);
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
    }
);

builder.Services.AddSingleton<KiraBot>();
builder.Services.AddTransient<WeatherService>();
builder.Services.AddTransient<CurrencyService>();
builder.Services.AddTransient<ICommand, HelloCommand>();
builder.Services.AddTransient<ICommand, WeatherCommand>();
builder.Services.AddTransient<ICommand, CurrencyCommand>();
builder.Services.AddTransient<ICommand, ConvertCurrencyCommand>();
builder.Services.AddTransient<ICommandExecutor, CommandExecutor>();
builder.Services.AddHostedService<WeatherTimerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
try
{
    app.Services.GetRequiredService<KiraBot>();
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal("Произошла необработанная ошибка {Exception}", ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}
