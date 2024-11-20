using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// Таблица кодов подтверждения для пользователей.
/// </summary>
[Comment("Таблица кодов подтверждения для пользователей.")]
public class AppUserConfirmationCode : BaseEntity<long>
{
    /// <summary>
    /// Telegram ИД пользвоателя.
    /// </summary>
    [Comment("Telegram ИД пользвоателя.")]
    public long TelegramUserId { get; set; }
    
    /// <summary>
    /// Код подтверждения.
    /// </summary>
    [Comment("Код подтверждения.")]
    public string Code { get; set; }
}