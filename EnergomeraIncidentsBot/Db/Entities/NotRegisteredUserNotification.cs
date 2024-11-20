using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// Сущность, помогающая контролировать уведомления о нехарегистрированном пользователе. 
/// </summary>
[Comment("Уведомление о незарегистрированном пользователе. ")]
public class NotRegisteredUserNotification : BaseEntity<long>
{
    /// <summary>
    /// Email пользователя.
    /// </summary>
    [Comment("Email пользователя.")]
    public string? Email { get; set; }
    
    /// <summary>
    /// ФИО пользователя.
    /// </summary>
    [Comment("ФИО пользователя.")]
    public string? Name { get; set; }
}