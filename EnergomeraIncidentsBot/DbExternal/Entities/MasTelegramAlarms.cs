using System.ComponentModel.DataAnnotations.Schema;

namespace EnergomeraIncidentsBot.DbExternal.Entities;

[Table("mas_telegram_alarms")]
public class MasTelegramAlarms
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("number")]
    public string Number { get; set; }
    
    [Column("level")]
    public int Level { get; set; }
    
    [Column("start_date")]
    public DateTime StartDate { get; set; }
    
    [Column("end_date")]
    public DateTime? EndDate { get; set; }
    
    [Column("stat")]
    public int? Stat { get; set; }
    
    [Column("komment")]
    public string? Comment { get; set; }
    
    [Column("type")]
    public int Type { get; set; }
    
    [Column("fio")]
    public string? Fio { get; set; }
    
    [Column("norm")]
    public int? Norm { get; set; }
}