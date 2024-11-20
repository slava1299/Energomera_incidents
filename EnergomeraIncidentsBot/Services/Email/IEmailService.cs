namespace EnergomeraIncidentsBot.Services.Email;

/// <summary>
/// Сервис отправки емайл.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Отправить емайл на почту адресату.
    /// </summary>
    /// <param name="email">Адресат.</param>
    /// <param name="subject">Заголовок письма.</param>
    /// <param name="message">Тело письма.</param>
    /// <returns></returns>
    public Task SendEmailAsync(string email, string subject, string message);
}