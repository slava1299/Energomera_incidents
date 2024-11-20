using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.DbExternal.Projections;

/// <summary>
/// Результат процедуры получения инцидентов для купирования [dbo].[mas_telegram_reply_check].
/// </summary>
[Keyless]
public class ProcMasTelegramReplyCheckProjection
{
    /// <summary>
    /// Номер инцидента
    /// </summary>
    [Column("Номер")]
    public string IncidentNumber { get; set; }
    
    /// <summary>
    /// Дата появления инцидента.
    /// </summary>
    [Column("Дата")]
    public DateTime? IncidentDateTime { get; set; }
    
    /// <summary>
    /// Автор (ФИО автора).
    /// </summary>
    [Column("Автор")]
    public string? Author { get; set; }
    
    /// <summary>
    /// Емайл автора.
    /// </summary>
    [Column("Автор_почта")]
    public string? AuthorEmail { get; set; }
    
    /// <summary>
    /// Уровень инцидента.
    /// </summary>
    [Column("Уровень")]
    public int? IncidentLevel { get; set; }
    
    /// <summary>
    /// Участок.
    /// </summary>
    [Column("Участок")]
    public string? Area { get; set; }
    
    /// <summary>
    /// Цех.
    /// </summary>
    [Column("Цех")]
    public string? Manufactory { get; set; }
    
    /// <summary>
    /// Описание.
    /// </summary>
    [Column("Описание")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Код изделия.
    /// </summary>
    [Column("Код_изделия")]
    public string? ProductCode { get; set; }
    
    /// <summary>
    /// Наименование изделия.
    /// </summary>
    [Column("Наименование_изделия")]
    public string? ProductName { get; set; }
    
    /// <summary>
    /// Дефект.
    /// </summary>
    [Column("Дефект")]
    public string? Defect { get; set; }
    
    /// <summary>
    /// Ответственный от потребителя.
    /// </summary>
    [Column("Ответственный_от_потребителя")]
    public string? ConsumerResponsiblePerson { get; set; }
    
    /// <summary>
    /// Почта ответственного от потребителя.
    /// </summary>
    [Column("Ответственный_от_потребителя_почта")]
    public string? ConsumerResponsiblePersonEmail { get; set; }
    
    /// <summary>
    /// Ответственный от поставщика.
    /// </summary>
    [Column("ОтветственныйОтПоставщика")]
    public string? SupplierResponsiblePerson { get; set; }
    
    /// <summary>
    /// Почта ответственного от поставщика.
    /// </summary>
    [Column("ОтветственныйОтПоставщика_Почта")]
    public string? SupplierResponsiblePersonEmail { get; set; }
    
    /// <summary>
    /// Норматив.
    /// </summary>
    [Column("Норматив")]
    public int? Normative { get; set; }
    
    /// <summary>
    /// Время отклика.
    /// </summary>
    [Column("Время_отклика")]
    public int? ResponseTime { get; set; }
    
    /// <summary>
    /// Лидер комиссии (ФИО).
    /// </summary>
    [Column("Лидер")]
    public string? ComissionLeader { get; set; }
    
    /// <summary>
    /// Почта лидера комиссии.
    /// </summary>
    [Column("Лидер_почта")]
    public string? ComissionLeaderEmail { get; set; }
    
    /// <summary>
    /// Заместитель начальника по технологии.
    /// </summary>
    [Column("Зам_начальник_по_технологии")]
    public string? DeputyTechnologyDirector { get; set; }
    
    /// <summary>
    /// Почта заместителя начальника по технологии.
    /// </summary>
    [Column("Зам_начальник_по_технологии_почта")]
    public string? DeputyTechnologyDirectorEmail { get; set; }
}