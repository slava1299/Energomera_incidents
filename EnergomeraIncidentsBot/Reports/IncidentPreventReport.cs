using System.Text;
using EnergomeraIncidentsBot.Db.Entities;

namespace EnergomeraIncidentsBot.Reports;

/// <summary>
/// Отчет об инциденте для купирования.
/// </summary>
public class IncidentPreventReport : IReport
{
    public string Subject { get; set; }
    private IncidentPreventNotification _notification;
    
    public IncidentPreventReport(string subject, IncidentPreventNotification notification)
    {
        _notification = notification;
        Subject = subject;
    }
    
    public string GetEmailReport()
    {
        if (_notification == null) throw new ArgumentNullException(nameof(_notification));

        StringBuilder sb = new();

        sb.Append(
                $"<b>Превышено время отклика на следующий инцидент [{_notification.IncidentNumber}]. Срочно переведите инцидент на купирование!!!</b><br>")
            .Append($"<br>")
            .Append($"<b>Номер:</b> {_notification.IncidentNumber}<br>")
            .Append($"<b>Дата:</b> {_notification.IncidentDateTime}<br>")
            .Append($"<b>Автор:</b> {_notification.Author}<br>")
            .Append($"<b>Уровень:</b> {_notification.IncidentLevel}<br>")
            .Append($"<b>Участок:</b> {_notification.Area}<br>")
            .Append($"<b>Цех:</b> {_notification.Manufactory}<br>")
            .Append($"<b>Описание:</b> {_notification.Description}<br>")
            .Append($"<b>Код изделия:</b> {_notification.ProductCode}<br>")
            .Append($"<b>Наименование изделия:</b> {_notification.ProductName}<br>")
            .Append($"<b>Дефект:</b> {_notification.Defect}<br>")
            // .Append($"<b>Ответственный от потребителя:</b> {_notification.ConsumerResponsiblePerson}<br>")
            // .Append($"<b>Ответственный от поставщика:</b> {_notification.SupplierResponsiblePerson}<br>")
            .Append($"<b>Норматив:</b> {_notification.Normative}<br>")
            .Append($"<b>Время отклика:</b> {_notification.ResponseTime}<br>");
            // .Append($"<b>Лидер:</b> {_notification.ComissionLeader}<br>")
            // .Append($"<b>Почта лидера:</b> {_notification.ComissionLeaderEmail}<br>")
            // .Append($"<b>Зам начальника по технологии:</b> {_notification.DeputyTechnologyDirector}<br>");

        return sb.ToString();
    }

    public string GetTelegramReport()
    {
        return GetEmailReport().Replace("<br>", "\n");
    }
}