using System.ComponentModel.DataAnnotations.Schema;
using EnergomeraIncidentsBot.DbExternal.Entities;
using EnergomeraIncidentsBot.DbExternal.Projections;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.DbExternal;


public class ExternalDbContext : DbContext
{
    public ExternalDbContext(DbContextOptions<ExternalDbContext> options) : base(options) { }
    
    // Таблицы 
    
    public DbSet<MasTelegramFio> MasTelegramFio { get; set; }
    public DbSet<MasTelegramAlarms> MasTelegramAlarms { get; set; }
    
    
    // Хранимые процедуры
    public DbSet<ProcEmailProjection> ProcTelegramMailByFio { get; set; }
    public DbSet<ProcFioProjection> ProcTelegramFioByMail { get; set; }
    public DbSet<ProcCountProjection> ProcCheckFioDouble { get; set; }
    public DbSet<ProcMasTelegramAlarmsStartProjection> ProcMasTelegramAlarmsStart { get; set; }
    public DbSet<ProcMasTelegramAlarmReplyProjection> ProcMasTelegramAlarmReply { get; set; }
    public DbSet<ProcMasTelegramCheckFailsProjection> ProcMasTelegramCheckFails { get; set; }
    public DbSet<ProcMasTelegramReplyCheckProjection> ProcMasTelegramReplyCheck { get; set; }
    
}