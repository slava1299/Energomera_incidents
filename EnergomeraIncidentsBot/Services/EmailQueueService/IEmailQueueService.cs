using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Reports;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace EnergomeraIncidentsBot.Services.EmailQueueService;

public interface IEmailQueueService
{
    public Task Enqueue(string email, IReport report);
    public Task<EmailQueueItem?> Peek();
    public Task<EmailQueueItem?> Dequeue();
}