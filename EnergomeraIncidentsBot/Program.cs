using BotFramework.Extensions;
using BotFramework.Options;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.App.Options;
using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Db.Repository;
using EnergomeraIncidentsBot.DbExternal;
using EnergomeraIncidentsBot.Extensions;
using EnergomeraIncidentsBot.Resources;
using EnergomeraIncidentsBot.Services.ConfirmationCode;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.EmailQueueService;
using EnergomeraIncidentsBot.Services.ExternalDb;
using EnergomeraIncidentsBot.Services.NotificationService;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;
// Регистрируем конфигурации.
services.Configure<BotConfiguration>(builder.Configuration.GetSection("Bot"));
services.Configure<BotOptions>(builder.Configuration.GetSection("BotOptions"));
var botConfig = builder.Configuration.GetSection("Bot").Get<BotConfiguration>();
BotResources botResources = services.ConfigureBotResources(botConfig.ResourcesFilePath);
services.AddBot(botConfig); // Подключаем бота
services.AddControllers().AddNewtonsoftJson(); //Обязательно подключаем NewtonsoftJson
services.AddHttpContextAccessor();
services.AddCors();

services.AddExceptionHandler<ExceptionHandler>();

// Свои сервисы
services.AddTransient<IExternalDbRepository, ExternalDbRepository>();
services.AddTransient<IConfirmationCodeService, ConfirmationCodeService>();
services.AddTransient<IEmailService, EmailService>();
services.AddTransient<IEmailQueueService, EmailQueueService>();
services.AddTransient<INotificationService, NotificationService>();
services.AddTransient<DbRepository>();

// Регистрируем контексты к базам данных.

services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(botConfig.DbConnection);
});

services.Configure<AppConfig>(builder.Configuration);
var appConfig = builder.Configuration.Get<AppConfig>();
services.AddDbContext<ExternalDbContext>(options =>
{
    options.UseSqlServer(appConfig.DbConnections.ExternalDb);
});

services.AddQuartzJobs(appConfig!.Quartz);
services.AddMapster();

var app = builder.Build();

// Миграция контекста приложения.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
//app.UseHttpsRedirection();
app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();



