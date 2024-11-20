namespace EnergomeraIncidentsBot.App.Options;

public class AppQuartzOptions
{
    public const string Section = "Email";

    public string MasTelegramAlarmsStartJobCron { get; set; }
    public string ExpiredIncidentTimeNotificationJobCron { get; set; }
    public string PreventIncidentNotificationJobCron { get; set; }
    public string EmailQueueSenderJobCron { get; set; }
}