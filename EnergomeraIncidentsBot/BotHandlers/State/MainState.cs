using System.Configuration;
using BotFramework.Attributes;
using BotFramework.Enums;
using BotFramework.Utils;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.App.Enums;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Db.Repository;
using EnergomeraIncidentsBot.DbExternal.Projections;
using EnergomeraIncidentsBot.Reports;
using EnergomeraIncidentsBot.Resources;
using EnergomeraIncidentsBot.Services.ConfirmationCode;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.ExternalDb;
using EnergomeraIncidentsBot.Services.NotificationService;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnergomeraIncidentsBot.BotHandlers.State;

[BotState(stateName:Name, version: 2.0f)]
public class MainState : BaseIncidentsBotState
{
    public const string Name = "MainState";

    /// <summary>
    /// Сейчас пользователь на шаге комментария отмены инцидента.
    /// </summary>
    private const string cancelMessageStep = "cancelMessage";
    
    private readonly IExternalDbRepository _externalRepository;
    private readonly IConfirmationCodeService _confirmationCodeService;
    private readonly IEmailService _emailService;
    private readonly DbRepository _dbRepository;
    private readonly MainStateResources _r;
    private readonly IReplyMarkup _defaultReply;
    private readonly IReplyMarkup _cancelReply;
    private readonly INotificationService _notificationService;
    
    public MainState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _notificationService = serviceProvider.GetRequiredService<INotificationService>();
        Expected(Telegram.Bot.Types.Enums.UpdateType.Message, Telegram.Bot.Types.Enums.UpdateType.CallbackQuery);
        ExpectedMessage(MessageType.Text);
        NotExpectedMessage = R.InputEmailState.InputEmail;
        _externalRepository = serviceProvider.GetRequiredService<IExternalDbRepository>();
        _confirmationCodeService = serviceProvider.GetRequiredService<IConfirmationCodeService>();
        _emailService = serviceProvider.GetRequiredService<IEmailService>();
        _dbRepository = serviceProvider.GetRequiredService<DbRepository>();
        _r = R.MainState;

        MarkupBuilder<ReplyKeyboardMarkup> mbDefault = new();
        _defaultReply = mbDefault.NewRow().Add(_r.ActiveIncidentsBtn).Build();
        
        MarkupBuilder<ReplyKeyboardMarkup> mbCancel = new();
        _cancelReply = mbCancel.NewRow().Add(_r.CancelCommentBtn).Build();
    }

    public override async Task HandleBotRequest(Update update)
    {
        // Обрабатываем запрос inline кнопки.
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
        {
            await ProcessCallback(update.CallbackQuery!);
            try
            {
                await BotClient.AnswerCallbackQueryAsync(update.CallbackQuery!.Id);
            }
            catch (Exception e)
            {
                // ничего не делаем
            }
            
            return;
        }

        // Обрабатываем текст.
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            await ProcessMessage(update.Message!);
            return;
        }
    }

    private async Task ProcessCallback(CallbackQuery callback)
    {
        string clbKey = callback.Data!;

        // Нажал на кнопку прибытия на инцидент.
        if (clbKey.StartsWith(AppConstants.CallbackKeys.ArrivedIncidentCallbackKey))
        {
            string incidentIdStr = clbKey.Replace(AppConstants.CallbackKeys.ArrivedIncidentCallbackKey, "");

            long incidentId = long.Parse(incidentIdStr);
            
            if (Chat.Data.Contains(cancelMessageStep))
            {
                Chat.Data.Remove(cancelMessageStep);
            }
            await BotDbContext.SaveChangesAsync();

            await ArriveToIncident(incidentId);
            return;
        }
        
        // Нажал на кнопку отмены инцидента.
        if (clbKey.StartsWith(AppConstants.CallbackKeys.CancelIncidentCallbackKey))
        {
            string incidentIdStr = clbKey.Replace(AppConstants.CallbackKeys.CancelIncidentCallbackKey, "");

            long incidentId = long.Parse(incidentIdStr);
            
            if (Chat.Data.Contains(cancelMessageStep))
            {
                Chat.Data.Remove(cancelMessageStep);
            }
            Chat.Data.Set(cancelMessageStep, incidentId);
            await BotDbContext.SaveChangesAsync();
            
            await Answer(_r.InputCommentToCancelIncident,
                replyMarkup: _cancelReply);
            return;
        }
    }

    private async Task ProcessMessage(Message message)
    {
        string text = message.Text!;
        
        // Пользователь хочет получить список активных инцидентов. Нажал на кнопку.
        if (string.Equals(_r.ActiveIncidentsBtn, text))
        {
            // Здесь бот отправляет последовательно активные инциденты.
            await SendActiveIncidents();
            return;
        }
        
        // Обрабатываем комментирование отмены инцидента.
        if (Chat.Data.Contains(cancelMessageStep) == true)
        {
            long incidentId = Chat.Data.Get<long>(cancelMessageStep);
            
            // Отменяем комментирование отмены инцидента.
            if (string.Equals(_r.CancelCommentBtn, text))
            {
                Chat.Data.Remove(cancelMessageStep);
                await BotDbContext.SaveChangesAsync();
                
                await Answer(_r.CancelCommentIncident, replyMarkup: _defaultReply);
                return;
            }

            await NotArriveToIncident(incidentId, text);
            return;
        }

        await Answer(_r.NotExpectedMessage);
    }

    /// <summary>
    /// Отправляем пользователю активные инциденты.
    /// </summary>
    /// <returns></returns>
    private async Task SendActiveIncidents()
    {
        List<Incident> userIncidents = await _dbRepository.GetUserActiveIncidents(User.TelegramId);

        if (userIncidents is null || userIncidents.Any() == false)
        {
            await Answer(_r.NoActiveIncidents);
            return;
        }
        
        foreach (var incident in userIncidents)
        {
            IReport report = new ExecutorIncidentReport("", incident);
            
            // Отправляем инцидент пользователю.
            MarkupBuilder<InlineKeyboardMarkup> mb = new();
            mb.NewRow()
                .Add(_r.ArrivedIncidentBtn, AppConstants.CallbackKeys.ArrivedIncidentCallbackKey + incident.Id)
                .NewRow()
                .Add(_r.CancelIncidentBtn, AppConstants.CallbackKeys.CancelIncidentCallbackKey + incident.Id);
            await BotClient.SendTextMessageAsync(Chat.ChatId, report.GetTelegramReport(), parseMode: ParseMode.Html,replyMarkup: mb.Build());
        }
    }

    /// <summary>
    /// Обработка прибытия сотрудника на инцидент.
    /// </summary>
    public async Task ArriveToIncident(long incidentId)
    {
        Incident? incident = await _dbRepository.GetIncidentById(incidentId);

        if (incident is null) throw new Exception("Инцидент не найден в БД при обработке процедуры прибытия на инцидент.");

        ProcMasTelegramAlarmReplyProjection res = await _externalRepository.ReplyIncident(incident!, true, null);

        await _dbRepository.ChangeIncidentStatus(incidentId, IncidentStatus.Completed);
        
        await Answer(_r.ArrivalIsRegistered, replyMarkup: _defaultReply);
    }
    
    /// <summary>
    /// Сотрудник не прибыл на инцидент.
    /// </summary>
    /// <param name="incidentId"></param>
    /// <exception cref="Exception"></exception>
    public async Task NotArriveToIncident(long incidentId, string comment)
    {
        Incident? incident = await _dbRepository.GetIncidentById(incidentId);

        if (incident is null) throw new Exception("Инцидент не найден в БД при обработке процедуры не прибытия на инцидент.");

        ProcMasTelegramAlarmReplyProjection res = await _externalRepository.ReplyIncident(incident!, false, comment);
        
        await Answer(_r.NotArrivalIsRegistered);
        await NotifyDirectorNotArrival(incident, comment, res.DirectorEmail);
        
        await _dbRepository.ChangeIncidentStatus(incidentId, IncidentStatus.Canceled);
    }

    /// <summary>
    /// Уведомить директора, что сотрудник не может прийти на инцидент.
    /// </summary>
    /// <param name="directorEmail"></param>
    private async Task NotifyDirectorNotArrival(Incident incident, string comment, string directorEmail)
    {
        IReport report =
            new ExecutorNotArrivedIncidentReport(_r.ExecutorNotArrivedNotificationEmailSubject, comment, incident);

        await _notificationService.NotifyUser(directorEmail, report);
    }
    
    
}