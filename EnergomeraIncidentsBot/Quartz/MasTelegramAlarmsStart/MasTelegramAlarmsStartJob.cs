using System.Text;
using BotFramework.Extensions;
using BotFramework.Utils;
using BotFramework.Utils;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.App.Enums;
using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Db.Repository;
using EnergomeraIncidentsBot.DbExternal;
using EnergomeraIncidentsBot.Reports;
using EnergomeraIncidentsBot.Resources;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.ExternalDb;
using EnergomeraIncidentsBot.Services.NotificationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnergomeraIncidentsBot.Quartz.MasTelegramAlarmsStart;

public class MasTelegramAlarmsStartJob : IJob
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
    
    public MasTelegramAlarmsStartJob(
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
            
            var incidentAlarms = await _externalDbRepository.GetAlarms();
            
            if (incidentAlarms is not null && incidentAlarms.Any())
            {
                //Игнорируем инциденты, где не пришел email исполнителя.
                incidentAlarms.RemoveAll(ia => string.IsNullOrEmpty(ia.ExecutorEmail?.Trim(' ') ?? null));
                
                // Сохраняем инциденты себе в БД.
                await _dbRepos.SaveIncidentAlarms(incidentAlarms);
            }

            // Берем из БД новые инциденты, о которых нужно уведомить пользователей.
            List<Incident> incidents = await _dbRepos.GetNewIncidents();

            // Ищем инциденты, где телеграм исполнителя не определен (то есть исполнитель не зарегистрирован в боте.)
            var definedNotDefinedIncidents = await GetDefinedNotDefinedIncidents(incidents);
            List<Incident> notDefinedExecutorIncidents = definedNotDefinedIncidents.notDefined;
            List<Incident> definedExecutorIncidents = definedNotDefinedIncidents.defined;

            // Отправляем уведомления пользователям, которые есть в телеграм.
            await SendNotificationsToExecutors(definedExecutorIncidents, notDefinedExecutorIncidents);
            
            // Отправляем емайл отчеты по незарегистрированным сотрудникам (уведомления на почту сотрудника и руководителя.).
            await SendDirectorEmailsForNotDefinedExecutors(notDefinedExecutorIncidents);
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

    /// <summary>
    /// Оптправляем уведомления сотрудникам по инцидентам.
    /// </summary>
    /// <param name="incidents">Инциденты, у которых телеграм аккаунт определен.</param>
    /// <param name="notDefined">Список неопределенных инцидентов.</param>
    private async Task SendNotificationsToExecutors(List<Incident> incidents, List<Incident> notDefined)
    {
        if (incidents is null || incidents.Any() == false) return;

        foreach (var incident in incidents)
        {
            IReport report = new ExecutorIncidentReport("", incident);
            AppUser? user = await _dbRepos.GetUserForIncident(incident);
            
            // Не смогли для инцидента подобрать пользователя, добавляем инцидент в неопределенные.
            if (user is null)
            {
                notDefined.Add(incident);
                continue;
            }
            
            // Отправляем уведомление пользователю.
            try
            {
                MarkupBuilder<InlineKeyboardMarkup> mb = new();
                mb.NewRow()
                    .Add(_r.MainState.ArrivedIncidentBtn, AppConstants.CallbackKeys.ArrivedIncidentCallbackKey + incident.Id)
                    .NewRow()
                    .Add(_r.MainState.CancelIncidentBtn, AppConstants.CallbackKeys.CancelIncidentCallbackKey + incident.Id);
                await _botClient.SendTextMessageAsync(user.TelegramChatId, report.GetTelegramReport(), parseMode: ParseMode.Html,replyMarkup: mb.Build());
                
                // Меняем статус инциденту, что он в работе, так как уже уведомили исполнителя.
                incident.Status = IncidentStatus.InProсess;
                _db.Incidents.Update(incident);
                await _db.SaveChangesAsync();
            }
            // Если не смогли отправить уведомление пользователю, значит отправим по емайл.
            catch (Exception e) 
            {
                notDefined.Add(incident);
            }
        }
    }

    private async Task<(List<Incident> defined, List<Incident> notDefined)> GetDefinedNotDefinedIncidents(List<Incident> incidents)
    {
        List<Incident> defined = new();
        List<Incident> notDefined = new();
        
        foreach (var incident in incidents)
        {
            AppUser? user = await _dbRepos.GetUserForIncident(incident);
            if (user is not null)
            {
                defined.Add(incident);
            }
            else
            {
                notDefined.Add(incident);
            }
        }

        return (defined, notDefined);
    }
    
    
    /// <summary>
    /// Отправить письма директорам, для тех исполнителей, которые не зарегистрировались в боте.
    /// </summary>
    /// <param name="incidents">Инциденты где Telegram исполнителя пуст.</param>
    /// <returns></returns>
    public async Task SendDirectorEmailsForNotDefinedExecutors(List<Incident> incidents)
    {
        if(incidents is null || incidents.Any() == false)
            return;
        
        foreach (var incident in incidents)
        {
            // Проверяем, было ли уже уведомление по этому пользователю.
            var notified = await _db.NotRegisteredUserNotifications.FirstOrDefaultAsync(u => u.Email == incident.ExecutorEmail);
            if (notified is not null) return;
            
            IReport report = new NotDefinedExecutorsReport("Выявлены сотрудники, не зарегистрированные в системе реагирования Telegram", incident);

            await _notificationService.NotifyUser(incident.ExecutorEmail, report);
            await _notificationService.NotifyUser(incident.DirectorEmail, report);

            // Добавили запись, что уже уведомили о незарегистрированности этого пользователя.
            _db.NotRegisteredUserNotifications.Add(new()
            {
                Email = incident.ExecutorEmail,
                Name = incident.Executor,
            });
            await _db.SaveChangesAsync();
        }
    }
}