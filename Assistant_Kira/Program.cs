using Assistant_Kira.Commands;
using Assistant_Kira.Models;
using Assistant_Kira.Services;
using Assistant_Kira.Services.CurrencyServices;
using Assistant_Kira.Services.NewsServices;
using Assistant_Kira.Services.WeatherServices;

using Microsoft.AspNetCore.Server.Kestrel.Core;

using Serilog;

using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

//builder.WebHost.ConfigureKestrel((context, options) =>
//{
//    options.ListenAnyIP(5001, listenOptions =>
//    {
//        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
//        listenOptions.UseHttps();
//    });
//});

builder.Services.AddLogging(x => x.ClearProviders().AddSerilog());
builder.Services.AddControllers();
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
        client.DefaultRequestHeaders.Add("apikey", builder.Configuration["ServicesApiKeys:ApilayerCurrency"]);
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
    }
);

builder.Services.AddSingleton<ITelegramBotClient, KiraBot>();
builder.Services.AddSingleton<INewspaperService, LentaNewsService>();
builder.Services.AddTransient<IWeatherService, WeatherService>();
builder.Services.AddTransient<ICurrencyService, ApilayerCurrencyService>();
builder.Services.AddTransient<ServerService>();
builder.Services.AddTransient<Command, HelloCommand>();
builder.Services.AddTransient<Command, WeatherCommand>();
builder.Services.AddTransient<Command, CurrencyCommand>();
builder.Services.AddTransient<Command, ConvertCurrencyCommand>();
builder.Services.AddTransient<Command, NewsCommand>();
builder.Services.AddHostedService<WeatherTimerService>();

var app = builder.Build();
/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();*/
app.MapControllers();
try
{
    app.Services.GetRequiredService<ITelegramBotClient>();
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
