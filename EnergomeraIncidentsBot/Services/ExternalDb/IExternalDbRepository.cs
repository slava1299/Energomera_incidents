using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.DbExternal.Entities;
using EnergomeraIncidentsBot.DbExternal.Projections;

namespace EnergomeraIncidentsBot.Services.ExternalDb;

/// <summary>
/// Репозиторий взаимодействия с внешней БД (SQL SERVER).
/// </summary>
public interface IExternalDbRepository
{
    /// <summary>
    /// Проверить сотрудника по ФИО.
    /// Показывает кол-во сотрудников удовлетворяющих строке поиска ФИО.
    /// </summary>
    /// <param name="fio"></param>
    /// <returns></returns>
    public Task<int> CheckFioDouble(string fio);

    /// <summary>
    /// Получить ФИО сотрудника по рабочему емайл [dbo].[mas_telegram_fio_by_email].
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<string?> GetFioByEmail(string email);

    /// <summary>
    /// Получаем емайл по ФИО сотрудника [dbo].[mas_telegram_mail_by_fio].
    /// </summary>
    /// <remarks>
    /// Метод возвращает только одну строку, не список.
    /// Не показывает дубликаты, поэтому лучше делать проверку на дубликаты.
    /// </remarks>
    /// <param name="fio"></param>
    /// <returns></returns>
    public Task<string?> GetEmailByFio(string fio);

    /// <summary>
    /// Проверка есть ли такой email в системе.
    /// </summary>
    /// <param name="email">Емайл сотрудника.</param>
    /// <returns></returns>
    public Task<bool> IsEmailExists(string email);

    /// <summary>
    /// Получение инцидентов из БД энергомеры.
    /// </summary>
    /// <returns></returns>
    public Task<List<ProcMasTelegramAlarmsStartProjection>> GetAlarms();
    
    /// <summary>
    /// Удалить сотрудника из таблицы [dbo].[mas_telegram_fio].
    /// </summary>
    /// <param name="email">Рабочий емайл сотрудника.</param>
    public Task RemoveTelegramUser(string email);

    /// <summary>
    /// Добавить сотрудника в таблицу [dbo].[mas_telegram_fio].
    /// </summary>
    /// <param name="fio">ФИО сотрудника.</param>
    /// <param name="email">Емайл сотрудника.</param>
    /// <param name="telegramUsername">Телеграм ник сотрудника "@username".</param>
    /// <returns></returns>
    public Task<MasTelegramFio> AddTelegramUser(string? fio, string email, string? telegramUsername);

    /// <summary>
    /// Ответ сотрудника на инцидент [dbo].[mas_telegram_alarm_reply].
    /// </summary>
    /// <param name="incident">Инцидент.</param>
    /// <param name="arrivedExecutor">Пришел сотрудник на место инцидента или нет?</param>
    /// <param name="comment">Если не пришел сотрудник, тогда оставил комментарий почему.</param>
    /// <returns></returns>
    public Task<ProcMasTelegramAlarmReplyProjection> ReplyIncident(Incident incident, bool arrivedExecutor,
        string? comment = null);
    
    /// <summary>
    /// Получить уведомления по инцидентам [dbo].[mas_telegram_check_fails].
    /// </summary>
    /// <returns></returns>
    public Task<List<ProcMasTelegramCheckFailsProjection>> GetFails();
    
    /// <summary>
    /// Получить уведомления по инцидентам для купирования [dbo].[mas_telegram_reply_check].
    /// </summary>
    /// <returns></returns>
    public Task<List<ProcMasTelegramReplyCheckProjection>> GetReplyCheck();
}