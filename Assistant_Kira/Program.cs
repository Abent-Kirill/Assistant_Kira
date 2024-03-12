using Assistant_Kira.Commands;
using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Serilog;

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
//builder.Services.AddSession();
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

builder.Services.AddSingleton<KiraBot>();
builder.Services.AddSingleton<LentaNewsService>();
builder.Services.AddTransient<WeatherService>();
builder.Services.AddTransient<ApilayerCurrencyService>();
builder.Services.AddTransient<ServerService>();
builder.Services.AddTransient<ICommand, HelloCommand>();
builder.Services.AddTransient<ICommand, WeatherCommand>();
builder.Services.AddTransient<ICommand, CurrencyCommand>();
builder.Services.AddTransient<ICommand, ConvertCurrencyCommand>();
builder.Services.AddTransient<ICommand, NewsCommand>();
builder.Services.AddTransient<ICommand, SaveFileToServerCommand>();
builder.Services.AddTransient<ICommand, SavePhotoToSeverCommand>();
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
//app.UseSession();
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
