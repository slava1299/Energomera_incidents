using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.DbExternal.Projections;

/// <summary>
/// Результат процедуры реагирования на инциденты [dbo].[mas_telegram_alarm_reply].
/// </summary>
[Keyless]
public class ProcMasTelegramAlarmReplyProjection
{
    /// <summary>
    /// Почта руководителя.
    /// </summary>
    [Column("ruk_fio")]
    public string DirectorEmail { get; set; }
    
}