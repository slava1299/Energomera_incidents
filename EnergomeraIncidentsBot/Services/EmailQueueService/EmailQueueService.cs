using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Reports;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Services.EmailQueueService;

public class EmailQueueService : IEmailQueueService
{
    private readonly AppDbContext _db;

    public EmailQueueService(AppDbContext db)
    {
        _db = db;
    }

    public async Task Enqueue(string email, IReport report)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));

        EmailQueueItem item = new()
        {
            Subject = report.Subject,
            Email = email,
            Message = report.GetEmailReport()
        };

        _db.EmailQueue.Add(item);
        await _db.SaveChangesAsync();
    }

    public async Task<EmailQueueItem?> Peek()
    {
        return await _db.EmailQueue.OrderBy(i => i.CreatedAt).FirstOrDefaultAsync();
    }

    public async Task<EmailQueueItem?> Dequeue()
    {
        var item = await Peek();
        if (item is null) return null;

        _db.EmailQueue.Remove(item);
        await _db.SaveChangesAsync();
        return item;
    }
}