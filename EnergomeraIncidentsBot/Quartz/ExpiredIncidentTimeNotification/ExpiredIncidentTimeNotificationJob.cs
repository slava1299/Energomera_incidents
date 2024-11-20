using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Db.Repository;
using EnergomeraIncidentsBot.Reports;
using EnergomeraIncidentsBot.Resources;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.ExternalDb;
using EnergomeraIncidentsBot.Services.NotificationService;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;

namespace EnergomeraIncidentsBot.Quartz.ExpiredIncidentTimeNotification;

/// <summary>
/// Задача уведомления пользователей об истекающих инцидентах.
/// </summary>
public class ExpiredIncidentTimeNotificationJob : IJob
{
    private static bool IsJobWorking = false;

    private readonly IExternalDbRepository _externalDbRepository;
    private readonly IEmailService _emailService;
    private readonly DbRepository _dbRepos;
    private readonly ITelegramBotClient _botClient;
    private readonly BotResources _r;
    private readonly AppDbContext _db;
    private readonly INotificationService _notificationService;
    private readonly IServiceProvider _serviceProvider;
    
    public ExpiredIncidentTimeNotificationJob(
        IExternalDbRepository externalDbRepository, 
        DbRepository dbRepos, 
        IEmailService emailService, 
        ITelegramBotClient botClient, 
        IOptions<BotResources> r, 
        AppDbContext db, 
        INotificationService notificationService, 
        IServiceProvider serviceProvider)
    {
        _externalDbRepository = externalDbRepository;
        _dbRepos = dbRepos;
        _emailService = emailService;
        _botClient = botClient;
        _db = db;
        _notificationService = notificationService;
        _serviceProvider = serviceProvider;
        _r = r!.Value;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        if(IsJobWorking == true) return;

        try
        {
            IsJobWorking = true;
            
            var fails = await _externalDbRepository.GetFails();

            if (fails is not null && fails.Any())
            {
                fails.RemoveAll(f => string.IsNullOrEmpty(f.ExecutorEmail));
            }
            
            if (fails is not null && fails.Any())
            {
                // Сохраняем уведомления по инцидентам себе в БД.
                fails = fails.GroupBy(f => new { f.IncidentNumber, f.IncidentLevel, f.Executor })
                    .Select(g => g.First())
                    .ToList();
                await _dbRepos.SaveIncidentFailNotifications(fails);
            }

            List<IncidentFailNotification> failNotifications = await _dbRepos.GetIncidentFailNotifications();

            foreach (var notification in failNotifications)
            {
                // Отправляем уведомление на почту и в телеграм исполнителю.
                IReport reportExecutor = new ExecutorIncidentFailReport("Вы не прибыли на инцидент", notification);
                await _notificationService.NotifyUser(notification.ExecutorEmail, reportExecutor);
                
                // Отправляем уведомление на почту и в телеграм директору.
                IReport reportDirector = new DirectorIncidentFailReport("Сотрудник не прибыл на инцидент", notification);
                await _notificationService.NotifyUser(notification.DirectorEmail, reportDirector);
                
                // Уведомление отправлено, теперь его удаляем.
                await _dbRepos.RemoveIncidentFailNotification(notification);
            }
        }
        catch (Exception e)
        {
            IsJobWorking = false;
            ExceptionHandler handler = new();
            await handler.HandleBotException(_serviceProvider, e, context.CancellationToken);
            throw;
        }
        finally
        {
            IsJobWorking = false;
        }
    }
}