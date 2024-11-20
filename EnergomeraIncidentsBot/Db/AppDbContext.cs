using BotFramework.Db.Entity;
using BotFramework.Extensions;
using EnergomeraIncidentsBot.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db;

public partial class AppDbContext : DbContext
{
    private const string AppSchema = "app";
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AppUserConfirmationCode> AppUserConfirmationCodes => Set<AppUserConfirmationCode>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentFailNotification> IncidentFailNotifications => Set<IncidentFailNotification>();
    public DbSet<IncidentPreventNotification> IncidentPreventNotifications => Set<IncidentPreventNotification>();
    public DbSet<NotRegisteredUserNotification> NotRegisteredUserNotifications => Set<NotRegisteredUserNotification>();
    public DbSet<EmailQueueItem> EmailQueue => Set<EmailQueueItem>();
    public DbSet<TelegramNotificationHistoryItem> TelegramNotificationHistory => Set<TelegramNotificationHistoryItem>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        SetSchemasToTables(builder);
        SetAllToSnakeCase(builder);
        SetFilters(builder);
        ConfigureEntities(builder);
        base.OnModelCreating(builder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var e in
                 ChangeTracker.Entries<IBaseEntity>())
        {
            switch (e.State)
            {
                case EntityState.Added:
                    e.Entity.CreatedAt = DateTimeOffset.Now;
                    break;
                case EntityState.Modified:
                    e.Entity.UpdatedAt = DateTimeOffset.Now;
                    break;
                case EntityState.Deleted:
                    e.Entity.DeletedAt = DateTimeOffset.Now;
                    e.State = EntityState.Modified;
                    break;
            }
        }

        return base.SaveChangesAsync(ct);
    }
}