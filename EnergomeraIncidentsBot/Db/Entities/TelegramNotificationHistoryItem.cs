using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// История уведомлений в Telegram
/// </summary>
[Comment("История уведомлений в Telegram.")]
public class TelegramNotificationHistoryItem : BaseEntity<long>
{
    /// <summary>
    /// Чат Telegram.
    /// </summary>
    [Comment("Телеграм чат ИД.")]
    public long TelegramChatId { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    [Comment("Сообщение уведомления.")]
    public string Message { get; set; }
    
}