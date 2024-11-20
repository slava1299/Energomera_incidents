using EnergomeraIncidentsBot.App.Options;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EnergomeraIncidentsBot.Services.Email;

/// <inheritdoc />
public class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    
    public EmailService(IOptions<AppConfig> options)
    {
        _options = options.Value.Email;
    }

    /// <inheritdoc />
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(_options.Name, _options.EmailAddress));
        
        emailMessage.To.Add(new MailboxAddress("customer", email)); 
        // emailMessage.To.Add(new MailboxAddress("customer", "timonburkush@gmail.com"));
        
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };
         
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_options.EmailAddress, _options.Password);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}