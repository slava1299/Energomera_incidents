using EnergomeraIncidentsBot.App.Enums;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Entities;

[Comment("Таблица инцидентов")]
public class Incident : BaseEntity<long>
{
    /// <summary>
    /// Исполнитель (ФИО сотрудника).
    /// </summary>
    [Comment("Исполнитель")]
    public string? Executor { get; set; }
    
    /// <summary>
    /// Почта исполнителя.
    /// </summary>
    [Comment("Почта исполнителя")]
    public string ExecutorEmail { get; set; }

    /// <summary>
    /// Телеграм исполнителя.
    /// </summary>
    [Comment("Аккаунт telegram")]
    public string? ExecutorTelegramUsername { get; set; }

    /// <summary>
    /// ФИО руководителя.
    /// </summary>
    [Comment("ФИО руководителя")]
    public string? Director { get; set; }

    /// <summary>
    /// Почта руководителя.
    /// </summary>
    [Comment("Почта руководителя")]
    public string? DirectorEmail { get; set; }

    /// <summary>
    /// Участок.
    /// </summary>
    [Comment("Участок")]
    public string? Area { get; set; }

    /// <summary>
    /// Номер инцидента
    /// </summary>
    [Comment("Номер инцидента")]
    public string IncidentNumber { get; set; }

    /// <summary>
    /// Дата появления инцидента.
    /// </summary>
    [Comment("Дата появления инцидента")]
    public DateTime? IncidentDateTime { get; set; }

    /// <summary>
    /// Уровень инцидента.
    /// </summary>
    [Comment("Уровень инцидента")]
    public int? IncidentLevel { get; set; }

    /// <summary>
    /// Автор (ФИО автора).
    /// </summary>
    [Comment("Автор")]
    public string? Author { get; set; }

    /// <summary>
    /// Лидер комиссии (ФИО).
    /// </summary>
    [Comment("Лидер комиссии")]
    public string? ComissionLeader { get; set; }

    /// <summary>
    /// Код изделия.
    /// </summary>
    [Comment("Код изделия")]
    public string? ProductCode { get; set; }

    /// <summary>
    /// Наименование изделия.
    /// </summary>
    [Comment("Наименование изделия")]
    public string? ProductName { get; set; }

    /// <summary>
    /// Код комплектующего изделия.
    /// </summary>
    [Comment("Код комплектующего изделия")]
    public string? ComplementaryProductCode { get; set; }
    
    /// <summary>
    /// Наименование комплектующего изделия.
    /// </summary>
    [Comment("Наименование комплектующего изделия")]
    public string? ComplementaryProductName { get; set; }

    /// <summary>
    /// Описание несоответствия.
    /// </summary>
    [Comment("Описание Несоответствия")]
    public string? ProblemDescription { get; set; }

    /// <summary>
    /// Наименование дефекта.
    /// </summary>
    [Comment("Наименование Дефекта")]
    public string? ProblemName { get; set; }

    /// <summary>
    /// Кол-во НП за смену.
    /// </summary>
    [Comment("Количество НП За Смену")]
    public int? NPCountForShift { get; set; }

    /// <summary>
    /// Кол-во изделий за смену.
    /// </summary>
    [Comment("Количество Изделий За Смену")]
    public int? ComplementaryCountForShift { get; set; }

    /// <summary>
    /// Процент дефектов.
    /// </summary>
    [Comment("% дефектов")]
    public decimal? DefectPercent { get; set; }

    /// <summary>
    /// Статус инцидента.
    /// </summary>
    public IncidentStatus Status { get; set; }
}