using System.Text;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Db.Entities;

namespace EnergomeraIncidentsBot.Reports;

/// <summary>
/// Уведомление для руководства по незарегистрированным сотрудникам.
/// </summary>
public class NotDefinedExecutorsReport : IReport
{
    public string Subject { get; set; }
    private Incident _incident;
    
    public NotDefinedExecutorsReport(string subject, Incident incident)
    {
        Subject = subject;
        _incident = incident;
    }

    public string GetEmailReport()
    {
        if (_incident is null) 
            throw new ArgumentNullException(nameof(_incident));
        
        // Генерируем таблицу.
        StringBuilder sb = new();
        // Текст письма
        sb.AppendLine(
            $"Просим сотрудников, указанных в поле «Исполнитель» пройти регистрацию в телеграмм боте по ссылке {AppConstants.TelegramBotLink}<br>" +
            $"<br>" +
            $"<b>Исполнитель</b> - {_incident.Executor}" +
            $"<b>Руководитель</b> - {_incident.Director}");
        
        // Требование прибыть на участок инцидента.
        sb.AppendLine($"Вам необходимо прибыть на {_incident.Area} в течении 15 минут по инциденту {_incident.IncidentNumber}, <br>" +
                      $"<b>Уровень:</b> {_incident.IncidentLevel},<br>" +
                      $"<b>Автор:</b> {_incident.Author} <br>" +
                      $"<b>Лидер:</b> {_incident.ComissionLeader},<br>" +
                      $"<b>Изделие:</b> {_incident.ProductCode} {_incident.ProductName},<br>" +
                      $"<b>Комплектующее:</b> {_incident.ComplementaryProductCode} {_incident.ComplementaryProductName},<br>" +
                      $"<b>Описание несоответствия:</b> {_incident.ProblemDescription}, <br>" +
                      $"<b>Наименование дефекта:</b> {_incident.ProblemName},<br>" +
                      $"Дефектов {_incident.NPCountForShift} шт./ Всего {_incident.ComplementaryCountForShift} шт., {_incident.DefectPercent} %.<br>" +
                      $"<b>По факту прибытия нажмите кнопку «Прибыл»</b><br>");

        return sb.ToString();
    }

    public string GetTelegramReport()
    {
        return GetEmailReport().Replace("<br>", "\n");
    }
}