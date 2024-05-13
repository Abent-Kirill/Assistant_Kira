using System.Reflection;

using Assistant_Kira.ExceptionHandlers;
using Assistant_Kira.Models;
using Assistant_Kira.Repositories;
using Assistant_Kira.Services;
using Assistant_Kira.Services.WeatherServices;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;

using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls(@"https://localhost:5001");
    builder.WebHost.ConfigureKestrel((context, options) =>
    {
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            listenOptions.UseHttps();
        });
    }).UseQuic();
}

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls(@"http://localhost:5000");
    builder.WebHost.UseUrls(@"https://localhost:5001");
    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });
}

builder.Services.AddHealthChecks()
    .AddCheck("Sample", () => HealthCheckResult.Healthy("A healthy result."));

builder.Logging.ClearProviders().AddSerilog(Log.Logger, true);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod | HttpLoggingFields.ResponseStatusCode;
    logging.RequestHeaders.Add("Authorization");
    logging.ResponseHeaders.Add("Content-Type");
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers();

builder.Services.ConfigureHttpClientDefaults(httpClientBuilder =>
{
    httpClientBuilder.ConfigureHttpClient(httpClient =>
    {
        httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
    });
});
builder.Services.AddHttpClient
(
    "OpenWeather", httpClient =>
    {
        httpClient.BaseAddress = new(@"https://api.openweathermap.org/data/2.5/", UriKind.Absolute);
        httpClient.Timeout = TimeSpan.FromSeconds(10);
    }
);
builder.Services.AddHttpClient
(
    "Apilayer", httpClient =>
    {
        httpClient.BaseAddress = new(@"https://api.apilayer.com/fixer/", UriKind.Absolute);
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Add("apikey", builder.Configuration["ServicesApiKeys:ApilayerCurrency"]);
    }
);

builder.Services.AddSingleton<ITelegramBotClient, KiraBot>();
builder.Services.AddSingleton<IRepository<NewsContent>, NewsRepository>();
builder.Services.AddSingleton<IRepository<Vacancy>, VacancyRepository>();

builder.Services.AddTransient<ServerService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddHostedService<GoodMorningService>();

var app = builder.Build();
app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    Console.CancelKeyPress += (sender, e) =>
    {
        Console.WriteLine("Получен сигнал SIGTERM. Завершение работы...");
        Log.CloseAndFlush();
        app.StopAsync().Wait();
    };
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseHttpLogging();
app.MapControllers();
app.UseHealthChecks("/hl");
app.Services.GetRequiredService<ITelegramBotClient>();

await app.RunAsync();