using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.EmailQueueService;
using Quartz;

namespace EnergomeraIncidentsBot.Quartz.EmailQueueSender;

public class EmailQueueSenderJob : IJob
{
    private static bool IsJobWorking = false;

    private readonly IServiceProvider _serviceProvider;
    private readonly AppDbContext _db;
    private readonly IEmailQueueService _emailQueueService;
    private readonly IEmailService _emailService;
    
    public EmailQueueSenderJob(IServiceProvider serviceProvider, 
        AppDbContext db, IEmailQueueService emailQueueService, 
        IEmailService emailService)
    {
        _serviceProvider = serviceProvider;
        _db = db;
        _emailQueueService = emailQueueService;
        _emailService = emailService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        if(IsJobWorking == true) return;

        try
        {
            IsJobWorking = true;

            var emailToSend = await _emailQueueService.Peek();
            if(emailToSend is null) return;

            await _emailService.SendEmailAsync(emailToSend.Email, emailToSend.Subject ?? "", emailToSend.Message);
            await _emailQueueService.Dequeue();
        }
        catch (Exception e)
        {
            IsJobWorking = false;
            ExceptionHandler handler = new();
            await handler.HandleBotException(_serviceProvider, e, context.CancellationToken);
            throw;
        }
        finally
        {
            IsJobWorking = false;
        }
    }
}