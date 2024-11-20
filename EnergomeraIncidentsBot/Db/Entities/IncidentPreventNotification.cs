using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// Уведомление о купировании инцидента.
/// </summary>
[Comment("Уведомление о купировании инцидента.")]
public class IncidentPreventNotification: BaseEntity<long>
{
    /// <summary>
    /// Номер инцидента
    /// </summary>
    [Comment("Номер")]
    public string IncidentNumber { get; set; }
    
    /// <summary>
    /// Дата появления инцидента.
    /// </summary>
    [Comment("Дата")]
    public DateTime? IncidentDateTime { get; set; }
    
    /// <summary>
    /// Автор (ФИО автора).
    /// </summary>
    [Comment("Автор")]
    public string? Author { get; set; }
    
    /// <summary>
    /// Емайл автора.
    /// </summary>
    [Comment("Автор_почта")]
    public string? AuthorEmail { get; set; }
    
    /// <summary>
    /// Уровень инцидента.
    /// </summary>
    [Comment("Уровень")]
    public int? IncidentLevel { get; set; }
    
    /// <summary>
    /// Участок.
    /// </summary>
    [Comment("Участок")]
    public string? Area { get; set; }
    
    /// <summary>
    /// Цех.
    /// </summary>
    [Comment("Цех")]
    public string? Manufactory { get; set; }
    
    /// <summary>
    /// Описание.
    /// </summary>
    [Comment("Описание")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Код изделия.
    /// </summary>
    [Comment("Код_изделия")]
    public string? ProductCode { get; set; }
    
    /// <summary>
    /// Наименование изделия.
    /// </summary>
    [Comment("Наименование_изделия")]
    public string? ProductName { get; set; }
    
    /// <summary>
    /// Дефект.
    /// </summary>
    [Comment("Дефект")]
    public string? Defect { get; set; }
    
    /// <summary>
    /// Ответственный от потребителя.
    /// </summary>
    [Comment("Ответственный_от_потребителя")]
    public string? ConsumerResponsiblePerson { get; set; }
    
    /// <summary>
    /// Почта ответственного от потребителя.
    /// </summary>
    [Comment("Ответственный_от_потребителя_почта")]
    public string? ConsumerResponsiblePersonEmail { get; set; }
    
    /// <summary>
    /// Ответственный от поставщика.
    /// </summary>
    [Comment("ОтветственныйОтПоставщика")]
    public string? SupplierResponsiblePerson { get; set; }
    
    /// <summary>
    /// Почта ответственного от поставщика.
    /// </summary>
    [Comment("ОтветственныйОтПоставщика_Почта")]
    public string? SupplierResponsiblePersonEmail { get; set; }
    
    /// <summary>
    /// Норматив.
    /// </summary>
    [Comment("Норматив")]
    public int? Normative { get; set; }
    
    /// <summary>
    /// Время отклика.
    /// </summary>
    [Comment("Время_отклика")]
    public int? ResponseTime { get; set; }
    
    /// <summary>
    /// Лидер комиссии (ФИО).
    /// </summary>
    [Comment("Лидер")]
    public string? ComissionLeader { get; set; }
    
    /// <summary>
    /// Почта лидера комиссии.
    /// </summary>
    [Comment("Лидер_почта")]
    public string? ComissionLeaderEmail { get; set; }
    
    /// <summary>
    /// Заместитель начальника по технологии.
    /// </summary>
    [Comment("Зам_начальник_по_технологии")]
    public string? DeputyTechnologyDirector { get; set; }
    
    /// <summary>
    /// Почта заместителя начальника по технологии.
    /// </summary>
    [Comment("Зам_начальник_по_технологии_почта")]
    public string? DeputyTechnologyDirectorEmail { get; set; }
}