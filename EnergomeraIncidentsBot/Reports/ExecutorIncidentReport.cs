using System.Text;
using EnergomeraIncidentsBot.Db.Entities;

namespace EnergomeraIncidentsBot.Reports;

/// <summary>
/// Уведомление для сотрудника о новом инциденте для него
/// </summary>
public class ExecutorIncidentReport : IReport
{
    private Incident _incident;
    public string Subject { get; set; }
    
    public ExecutorIncidentReport(string subject, Incident incident)
    {
        Subject = subject;
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
        sb.AppendLine($"Вам необходимо прибыть на {_incident.Area} в течении 15 минут по инциденту {_incident.IncidentNumber}, \n" +
                      $"<b>Уровень:</b> {_incident.IncidentLevel},\n" +
                      $"<b>Создан:</b> {_incident.IncidentDateTime},\n" +
                      $"<b>Автор:</b> {_incident.Author} \n" +
                      $"<b>Лидер:</b> {_incident.ComissionLeader},\n" +
                      $"<b>Изделие:</b> {_incident.ProductCode} {_incident.ProductName},\n" +
                      $"<b>Комплектующее:</b> {_incident.ComplementaryProductCode} {_incident.ComplementaryProductName},\n" +
                      $"<b>Описание несоответствия:</b> {_incident.ProblemDescription}, \n" +
                      $"<b>Наименование дефекта:</b> {_incident.ProblemName},\n" +
                      $"Дефектов {_incident.NPCountForShift} шт./ Всего {_incident.ComplementaryCountForShift} шт., {_incident.DefectPercent} %.\n" +
                      $"<b>По факту прибытия нажмите кнопку «Прибыл»</b>\n");

        return sb.ToString();
    }
}