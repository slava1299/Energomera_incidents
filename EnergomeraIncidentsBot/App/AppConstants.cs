using Quartz;

namespace EnergomeraIncidentsBot.App;

public class AppConstants
{
    /// <summary>
    /// Максимальная длина строки ФИО.
    /// </summary>
    public const int MaxFioLength = 255;
    
    /// <summary>
    /// Ссылка на бота.
    /// </summary>
    public const string TelegramBotLink = "https://t.me/energomera_incident_bot";
    
    /// <summary>
    /// Наименование бота.
    /// </summary>
    public const string TelegramBotName = "Энергомера инциденты";

    /// <summary>
    /// Постфикс для рабочих почт сотрудников энергомеры.
    /// </summary>
    public const string EnergomeraEmailPostfix = "@energomera.ru";
    
    /// <summary>
    /// Время жизни кода подтверждения пользователя (в минутах).
    /// </summary>
    public const int CodeLifetimeMinutes = 15;
    
    
    /// <summary>
    /// Ключи задач и триггеров Quartz.
    /// </summary>
    public static class QuartzKeys
    {
        private const string MasTelegramAlarmsStartGroupName = "mas_telegram_alarms_start_gn";
        public static readonly JobKey MasTelegramAlarmsStartJobKey = new("alarms_start_job", MasTelegramAlarmsStartGroupName);
        public static readonly TriggerKey MasTelegramAlarmsStartTriggerKey = new("alerms_statr_trigger", MasTelegramAlarmsStartGroupName);
        
        private const string ExpiredIncidentTimeNotificationGroupName = "expired_incident_time_gn";
        public static readonly JobKey ExpiredIncidentTimeNotificationJobKey = new("expired_incident_time_job", ExpiredIncidentTimeNotificationGroupName);
        public static readonly TriggerKey ExpiredIncidentTimeNotificationTriggerKey = new("expired_incident_time_trigger", ExpiredIncidentTimeNotificationGroupName);
        
        private const string PreventIncidentNotificationGroupName = "prevent_incident_notification_gn";
        public static readonly JobKey PreventIncidentNotificationJobKey = new("prevent_incident_notification_job", PreventIncidentNotificationGroupName);
        public static readonly TriggerKey PreventIncidentNotificationTriggerKey = new("prevent_incident_notification_trigger", PreventIncidentNotificationGroupName);
        
        private const string EmailQueueSenderGroupName = "email_queue_sender_gn";
        public static readonly JobKey EmailQueueSenderJobKey = new("email_queue_sender_job", EmailQueueSenderGroupName);
        public static readonly TriggerKey EmailQueueSenderTriggerKey = new("email_queue_sender_trigger", EmailQueueSenderGroupName);

        
    }
    
    /// <summary>
    /// Ключи для callback бота.
    /// </summary>
    public static class CallbackKeys
    {
        public static string ArrivedIncidentCallbackKey = "arrived_";
        public static string CancelIncidentCallbackKey = "cancel_";
    }
}