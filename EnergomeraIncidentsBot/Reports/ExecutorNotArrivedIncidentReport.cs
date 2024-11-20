using System.Text;
using EnergomeraIncidentsBot.App.Enums;
using EnergomeraIncidentsBot.Db.Entities;

namespace EnergomeraIncidentsBot.Reports;

/// <summary>
/// Генерация уведомления для руководства, что сотрудник не может прийти на инцидент.
/// </summary>
public class ExecutorNotArrivedIncidentReport : IReport
{
    public string Subject { get; set; }
    private string _comment;
    private Incident _incident;

    public ExecutorNotArrivedIncidentReport(string subject, string comment, Incident incident)
    {
        Subject = subject;
        _comment = comment;
        _incident = incident;
    }

    /// <inheritdoc />
    public string GetEmailReport()
    {
        return GetTelegramReport().Replace("\n", "<br>");
    }

    /// <inheritdoc />
    public string GetTelegramReport()
    {
        if (_incident == null) throw new ArgumentNullException(nameof(_incident));

        StringBuilder sb = new();
        
        // Требование прибыть на участок инцидента.
        sb.AppendLine($"{_incident.Executor} не прибыл на инцидент {_incident.IncidentNumber} по причине {_comment}\n" +
                      $"<b>Участок:</b> {_incident.Area}, \n" +
                      $"<b>Уровень:</b> {_incident.IncidentLevel},\n" +
                      $"<b>Автор:</b> {_incident.Author} \n" +
                      $"<b>Лидер:</b> {_incident.ComissionLeader},\n" +
                      $"<b>Изделие:</b> {_incident.ProductCode} {_incident.ProductName},\n" +
                      $"<b>Комплектующее:</b> {_incident.ComplementaryProductCode} {_incident.ComplementaryProductName},\n" +
                      $"<b>Описание несоответствия:</b> {_incident.ProblemDescription}, \n" +
                      $"<b>Наименование дефекта:</b> {_incident.ProblemName},\n" +
                      $"Дефектов {_incident.NPCountForShift} шт./ Всего {_incident.ComplementaryCountForShift} шт., {_incident.DefectPercent}  %.\n" +
                      $"В рабочем порядке назначьте замену сотруднику.\n");

        return sb.ToString();
    }
}