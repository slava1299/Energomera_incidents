namespace EnergomeraIncidentsBot.App.Enums;

public enum IncidentStatus
{
    /// <summary>
    /// Новый инцидент.
    /// </summary>
    New = 0,
    
    /// <summary>
    /// В обработке.
    /// </summary>
    InProсess = 20,
    
    /// <summary>
    /// Инцидент отменет, сотрудник не может прийти.
    /// </summary>
    Canceled = 30,
    
    /// <summary>
    /// Инцидент обработан, завершен.
    /// </summary>
    Completed = 100,
}