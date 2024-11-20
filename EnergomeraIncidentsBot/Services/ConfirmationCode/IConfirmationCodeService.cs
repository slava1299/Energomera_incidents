using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Services.ConfirmationCode;

/// <summary>
/// Сервис для работы с кодом подтверждения пользователя.
/// </summary>
public interface IConfirmationCodeService
{
    /// <summary>
    /// Получить активный код пользователя.
    /// </summary>
    /// <param name="telegramUserId"></param>
    /// <returns></returns>
    public Task<AppUserConfirmationCode?> GetActiveCode(long telegramUserId);

    /// <summary>
    /// Создать код для пользователя.
    /// </summary>
    /// <param name="telegramUserId"></param>
    /// <returns></returns>
    public Task<AppUserConfirmationCode> CreateCode(long telegramUserId);

    /// <summary>
    /// Проверить правильность кода.
    /// </summary>
    /// <param name="telegramUserId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task<bool> CheckCode(long telegramUserId, string code);
    
}