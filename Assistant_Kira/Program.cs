using Assistant_Kira.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<KiraBot>();
//builder.Services.AddDbContext<BotDataContext>(opt =>
//			opt.UseNpgsql(builder.Configuration.GetConnectionString("Db")), ServiceLifetime.Singleton);
//builder.Services.AddSingleton<ICommandExecutor, CommandExecutor>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
