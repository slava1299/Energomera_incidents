using System.Text;
using EnergomeraIncidentsBot.App.Enums;
using EnergomeraIncidentsBot.Db.Entities;

namespace EnergomeraIncidentsBot.Reports;

/// <summary>
/// Отчет директору, что сотрудник не пришел на инцидент.
/// Инцидент просрочен.
/// </summary>
public class DirectorIncidentFailReport : IReport
{
    public string Subject { get; set; }
    private IncidentFailNotification _notification;

    public DirectorIncidentFailReport(string subject, IncidentFailNotification notification)
    {
        Subject = subject;
        _notification = notification;
    }

    /// <inheritdoc />
    public string GetEmailReport()
    {
        return GetTelegramReport().Replace("\n", "<br>");
    }

    /// <inheritdoc />
    public string GetTelegramReport()
    {
        if (_notification == null) throw new ArgumentNullException(nameof(_notification));

        StringBuilder sb = new();
        
        // Требование прибыть на участок инцидента.
        sb.AppendLine($"<b>{_notification.Executor ?? "NULL"} не прибыл в течении {_notification.Time} минут на инцидент</b> {_notification.IncidentNumber ?? "NULL"} \n" +
                      $"<b>Участок:</b> {_notification.Area}, \n" +
                      $"<b>Уровень:</b> {_notification.IncidentLevel},\n" +
                      $"<b>Автор:</b> {_notification.Author} \n" +
                      $"<b>Лидер:</b> {_notification.ComissionLeader},\n" +
                      $"<b>Изделие:</b> {_notification.ProductCode} {_notification.ProductName},\n" +
                      $"<b>Комплектующее:</b> {_notification.ComplementaryProductCode} {_notification.ComplementaryProductName},\n" +
                      $"<b>Описание несоответствия:</b> {_notification.ProblemDescription}, \n" +
                      $"<b>Наименование дефекта:</b> {_notification.ProblemName},\n" +
                      $"Дефектов {_notification.NPCountForShift} шт./ Всего {_notification.ComplementaryCountForShift} шт., {_notification.DefectPercent}  %.\n" +
                      $"\n" +
                      $"<b>В рабочем порядке проконтролируйте прибытие сотрудника на участок возникновения инцидента!</b>\n");

        return sb.ToString();
    }
}