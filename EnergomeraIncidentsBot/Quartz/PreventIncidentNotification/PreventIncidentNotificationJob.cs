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

namespace EnergomeraIncidentsBot.Quartz.PreventIncidentNotification;

/// <summary>
/// Задача купирования (срочного предотвращения инцидента).
/// </summary>
public class PreventIncidentNotificationJob : IJob
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

    public PreventIncidentNotificationJob(
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
            
            var preventNotificationProjections = await _externalDbRepository.GetReplyCheck();
            
            if (preventNotificationProjections is not null && preventNotificationProjections.Any())
            {
                // Сохраняем инциденты для купирования себе в БД.
                await _dbRepos.SaveIncidentPreventNotifications(preventNotificationProjections);
            }
            
            List<IncidentPreventNotification> preventNotifications = await _dbRepos.GetIncidentPreventNotifications();
        
            foreach (var notification in preventNotifications)
            {
                // Отправляем уведомление на почту и в телеграм исполнителю.
                IReport reportExecutor = new IncidentPreventReport("Срочно переведите инцидент на купирование", notification);

                List<string?> receivers = new()
                {
                    notification.AuthorEmail,
                    notification.ConsumerResponsiblePersonEmail,
                    notification.SupplierResponsiblePersonEmail, 
                    notification.ComissionLeaderEmail,
                    notification.DeputyTechnologyDirectorEmail
                };

                receivers.RemoveAll(r => string.IsNullOrEmpty(r) == true);
                receivers = receivers.Distinct().ToList();

                foreach (var receiver in receivers)
                {
                    await _notificationService.NotifyUser(receiver!, reportExecutor);
                }
                
                // Уведомление отправлено, теперь его удаляем.
                await _dbRepos.RemoveIncidentPreventNotification(notification);
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