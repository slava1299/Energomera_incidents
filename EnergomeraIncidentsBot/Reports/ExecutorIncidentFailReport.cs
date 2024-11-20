using System.Text;
using EnergomeraIncidentsBot.App.Enums;
using EnergomeraIncidentsBot.Db.Entities;

namespace EnergomeraIncidentsBot.Reports;

/// <summary>
/// Отчет о том, что сотрудник не пришел на инцидент.
/// Инцидент просрочен.
/// </summary>
public class ExecutorIncidentFailReport : IReport
{
    public string Subject { get; set; }
    private IncidentFailNotification _notification;

    public ExecutorIncidentFailReport(string subject, IncidentFailNotification notification)
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
        sb.AppendLine($"Вы {_notification.Executor} не прибыли в течении {_notification.Time} минут на инцидент {_notification.IncidentNumber} \n" +
                      $"<b>Участок:</b> {_notification.Area}, \n" +
                      $"<b>Уровень:</b> {_notification.IncidentLevel},\n" +
                      $"<b>Автор:</b> {_notification.Author} \n" +
                      $"<b>Лидер:</b> {_notification.ComissionLeader},\n" +
                      $"<b>Изделие:</b> {_notification.ProductCode} {_notification.ProductName},\n" +
                      $"<b>Комплектующее:</b> {_notification.ComplementaryProductCode} {_notification.ComplementaryProductName},\n" +
                      $"<b>Описание несоответствия:</b> {_notification.ProblemDescription}, \n" +
                      $"<b>Наименование дефекта:</b> {_notification.ProblemName},\n" +
                      $"Дефектов {_notification.NPCountForShift} шт./ Всего {_notification.ComplementaryCountForShift} шт., {_notification.DefectPercent}  %.\n" +
                      $"<b>По факту прибытия нажмите на кнопку «Прибыл»</b>\n");

        return sb.ToString();
    }
}