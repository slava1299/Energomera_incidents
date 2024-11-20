using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;

namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// Сотрудник в боте.
/// </summary>
[Comment("Таблица сотрудников в боте.")]
public class AppUser : BaseEntity<long>
{
    /// <summary>
    /// Telegram идентификатор пользователя.
    /// </summary>
    [Comment("Telegram идентификатор пользователя.")]
    public long TelegramUserId { get; set; }
    
    /// <summary>
    /// Telegram идентификатор чата с пользователем.
    /// </summary>
    [Comment("Telegram идентификатор чата с пользователем.")]
    public long TelegramChatId { get; set; }
    
    /// <summary>
    /// Telegram ник.
    /// </summary>
    [Comment("Telegram ник.")]
    public string? TelegramUsername { get; set; }
    
    /// <summary>
    /// ФИО сотрудника.
    /// </summary>
    [Comment("ФИО сотрудника.")]
    public string? Fio { get; set; }
    
    /// <summary>
    /// Рабочий email сотрудника.
    /// </summary>
    [Comment("Рабочий email сотрудника.")]
    public string? Email { get; set; }

    /// <summary>
    /// Пользователь зарегистрирован в боте?
    /// </summary>
    [Comment("Пользователь зарегистрирован в боте?")]
    public bool IsRegistered { get; set; }
    
    /// <summary>
    /// Активен ли пользователь в боте?
    /// Может ли бот ему писать?
    /// </summary>
    [Comment("Активен ли пользователь в боте?")]
    public bool IsActive { get; set; }
}