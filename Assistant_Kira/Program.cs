using System.Reflection;

using Assistant_Kira.DTO;
using Assistant_Kira.ExceptionHandlers;
using Assistant_Kira.Models;
using Assistant_Kira.Options;
using Assistant_Kira.Repositories;
using Assistant_Kira.Services;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;

using Telegram.Bot;

var builder = WebApplication.CreateSlimBuilder();
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
    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });
}

builder.Services.AddHealthChecks()
    .AddCheck("Sample", () => HealthCheckResult.Healthy("A healthy result."));

builder.Logging.AddSerilog(Log.Logger, true);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod | HttpLoggingFields.ResponseStatusCode;
    logging.RequestHeaders.Add("Authorization");
    logging.ResponseHeaders.Add("Content-Type");
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers();
builder.Services.Configure<CalendarOptions>(builder.Configuration.GetSection("GoogleCalendar"));
builder.Services.Configure<BotOptions>(builder.Configuration.GetSection("BotSettings"));
builder.Services.Configure<PathOptions>(builder.Configuration.GetSection("Paths"));
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
        httpClient.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["ServicesApiKeys:Weather"]);
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

builder.Services.AddSingleton<ITelegramBotClient, KiraBot>(provider =>
    new KiraBot(builder.Configuration["BotSettings:Token"],
    new Uri(builder.Configuration["BotSettings:WebhookUrl"], UriKind.Absolute)));

builder.Services.AddSingleton<IRepository<Article>, NewsRepository>();
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
