using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.DbExternal.Projections;

/// <summary>
/// Результат процедуры получения просроченных сотрудниками инцидентов [dbo].[mas_telegram_check_fails].
/// </summary>
[Keyless]
public class ProcMasTelegramCheckFailsProjection
{
    /// <summary>
    /// Статус.
    /// </summary>
    [Column("Статус")]
    public string? Status { get; set; }
    
    /// <summary>
    /// Участок.
    /// </summary>
    [Column("Участок")]
    public string? Area { get; set; }
    
    /// <summary>
    /// Номер инцидента
    /// </summary>
    [Column("Номер инцидента")]
    public string? IncidentNumber { get; set; }
    
    /// <summary>
    /// Дата появления инцидента.
    /// </summary>
    [Column("Дата появления инцидента")]
    public DateTime? IncidentDateTime { get; set; }
    
    /// <summary>
    /// Уровень инцидента.
    /// </summary>
    [Column("Уровень инцидента")]
    public int? IncidentLevel { get; set; }
    
    /// <summary>
    /// Автор (ФИО автора).
    /// </summary>
    [Column("Автор")]
    public string? Author { get; set; }
    
    /// <summary>
    /// Лидер комиссии (ФИО).
    /// </summary>
    [Column("Лидер комиссии")]
    public string? ComissionLeader { get; set; }
    
    /// <summary>
    /// Код изделия.
    /// </summary>
    [Column("Код изделия")]
    public string? ProductCode { get; set; }

    /// <summary>
    /// Наименование изделия.
    /// </summary>
    [Column("Наименование изделия")]
    public string? ProductName { get; set; }
    
    /// <summary>
    /// Код комплектующего изделия.
    /// </summary>
    [Column("Код комплектующего изделия")]
    public string? ComplementaryProductCode { get; set; }
    
    /// <summary>
    /// Наименование комплектующего изделия.
    /// </summary>
    [Column("Наименование комплектующего изделия")]
    public string? ComplementaryProductName { get; set; }

    /// <summary>
    /// Описание несоответствия.
    /// </summary>
    [Column("Описание Несоответствия")]
    public string? ProblemDescription { get; set; }

    /// <summary>
    /// Наименование дефекта.
    /// </summary>
    [Column("Наименование Дефекта")]
    public string? ProblemName { get; set; }

    /// <summary>
    /// Кол-во НП за смену.
    /// </summary>
    [Column("Количество НП За Смену")]
    public int? NPCountForShift { get; set; }

    /// <summary>
    /// Кол-во изделий за смену.
    /// </summary>
    [Column("Количество Изделий За Смену")]
    public int? ComplementaryCountForShift { get; set; }

    /// <summary>
    /// Процент дефектов.
    /// </summary>
    [Column("% дефектов")]
    public decimal? DefectPercent { get; set; }
    
    /// <summary>
    /// ФИО исполнителя.
    /// </summary>
    [Column("fio")]
    public string? Executor { get; set; }
    
    /// <summary>
    /// (Не знаю что значит поле)...
    /// </summary>
    [Column("norm")]
    public int? Norm { get; set; }
    
    /// <summary>
    /// Телеграм исполнителя.
    /// </summary>
    [Column("telegram")]
    public string? ExecutorTelegramUsername { get; set; }
    
    /// <summary>
    /// Почта исполнителя.
    /// </summary>
    [Column("fio_mail")]
    public string? ExecutorEmail { get; set; }

    /// <summary>
    /// ФИО руководителя.
    /// </summary>
    [Column("ruk")]
    public string? Director { get; set; }

    /// <summary>
    /// Почта руководителя.
    /// </summary>
    [Column("ruk_mail")]
    public string? DirectorEmail { get; set; }

    /// <summary>
    /// Телеграм исполнителя.
    /// </summary>
    [Column("ruc_telegram")]
    public string? DirectorTelegramUsername { get; set; }
    
    /// <summary>
    /// Время со старта инцидента в минутах.
    /// </summary>
    [Column("Время со старта инцидента")]
    public int? Time { get; set; }
}