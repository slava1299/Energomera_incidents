using EnergomeraIncidentsBot.Reports;

namespace EnergomeraIncidentsBot.Services.NotificationService;

public interface INotificationService
{
    /// <summary>
    /// Отправить уведомление пользователю.
    /// На почту + в телеграм.
    /// </summary>
    /// <param name="email">Почта пользователя.</param>
    /// <param name="report">Данные уведомления.</param>
    /// <returns>Возвращает true если уведомление успешно отправлено, иначе false.</returns>
    public Task<bool> NotifyUser(string? email, IReport report);
}