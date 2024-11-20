using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// Напоминание о незавершении инцидента для пользователя. 
/// </summary>
[Comment("Элемент email рассылки.")]
public class EmailQueueItem : BaseEntity<long>
{
    /// <summary>
    /// Емайл
    /// </summary>
    [Comment("Email адрес.")]
    public string Email { get; set; }
    
    /// <summary>
    /// Заголовок письма.
    /// </summary>
    [Comment("Заголовок письма.")]
    public string? Subject { get; set; }
    
    /// <summary>
    /// Текст письма.
    /// </summary>
    [Comment("Сообщение.")]
    public string Message { get; set; }
}