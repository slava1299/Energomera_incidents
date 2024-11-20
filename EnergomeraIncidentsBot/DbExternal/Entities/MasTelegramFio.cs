using System.ComponentModel.DataAnnotations.Schema;

namespace EnergomeraIncidentsBot.DbExternal.Entities;

[Table("mas_telegram_fio")]
public class MasTelegramFio
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("fio")]
    public string Fio { get; set; }
    
    [Column("mail")]
    public string Mail { get; set; }
    
    [Column("telegram")]
    public string Telegram { get; set; }
}