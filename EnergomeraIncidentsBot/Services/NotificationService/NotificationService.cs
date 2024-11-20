using System.Text.RegularExpressions;
using BotFramework.Utils.ExceptionHandler;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Db.Repository;
using EnergomeraIncidentsBot.Reports;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.EmailQueueService;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EnergomeraIncidentsBot.Services.NotificationService;

public class NotificationService : INotificationService
{
    private readonly DbRepository _dbRepository;
    private readonly ITelegramBotClient _botClient;
    private readonly IEmailService _emailService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEmailQueueService _emailQueueService;
    
    public NotificationService(DbRepository dbRepository, 
        ITelegramBotClient botClient, 
        IEmailService emailService, 
        IServiceProvider serviceProvider, 
        IEmailQueueService emailQueueService)
    {
        _dbRepository = dbRepository;
        _botClient = botClient;
        _emailService = emailService;
        _serviceProvider = serviceProvider;
        _emailQueueService = emailQueueService;
    }
    
    public async Task<bool> NotifyUser(string? email, IReport report)
    {
        if (string.IsNullOrEmpty(email))
        {
            // ToDo Обработать Когда почта == null 
            return false;
        }
        
        if (email.EndsWith(AppConstants.EnergomeraEmailPostfix) == false)
        {
            throw new Exception($"Нельзя отправить письмо на нерабочую почту [{email}].");
        }
        
        AppUser? user = await _dbRepository.GetRegisteredAppUserByEmail(email);
        
        try
        {
            if (user is not null)
            {
                await _botClient.SendTextMessageAsync(user.TelegramChatId, report.GetTelegramReport(), parseMode: ParseMode.Html);
                await _dbRepository.AddTelegramNotificationToHistory(user.TelegramChatId, report.GetTelegramReport());
                // Если получилось отправить уведомление в телеге, то это хорошо, не надо отправлять емайл.
                return true;
            }
        }
        catch (Exception e)
        {
            // Ошибка отправки уведомления в телеграм, обработать. (отправить админу инфу, что не получилось отправить письмо).
            var ex = new Exception(
                $"Не удалось отправить уведомление пользователю [{user.TelegramUserId} : {user.TelegramUsername}]\n" +
                $"{report.GetTelegramReport()}\n\n" +
                $"Проконтролируйте регистрацию пользователя [{user.Fio} : {user.Email}] в системе реагирования Telegram.\n");
            BotExceptionHandler handler = new();
            await handler.Handle(new BotExceptionHandlerArgs(ex, _serviceProvider));
        }
        
        try
        {
            // Добавляем в очередь на отправку письма.
            await _emailQueueService.Enqueue(email, report);

            //await _emailService.SendEmailAsync(email, report.Subject, report.GetEmailReport());
        }
        catch (Exception e)
        {
            // Ошибка отправки письма, обработать. (отправить админу инфу, что не получилось отправить письмо).
            BotExceptionHandler handler = new();
            await handler.Handle(new BotExceptionHandlerArgs(e, _serviceProvider));
            return false;
        }

        return true;
    }
}