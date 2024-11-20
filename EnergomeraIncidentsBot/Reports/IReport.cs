namespace EnergomeraIncidentsBot.Reports;

public interface IReport
{
    /// <summary>
    /// Для емайл, заголовок письма.
    /// </summary>
    public string Subject { get; set; }
    
    /// <summary>
    /// Сформировать отчет для емайл.
    /// </summary>
    /// <returns></returns>
    public string GetEmailReport();
    
    /// <summary>
    /// Сформировать отчет для Telegram
    /// </summary>
    /// <returns></returns>
    public string GetTelegramReport();
}