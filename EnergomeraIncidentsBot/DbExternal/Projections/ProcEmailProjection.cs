using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.DbExternal.Projections;

[Keyless]
public class ProcEmailProjection
{
    [Column("mail")]
    public string Mail { get; set; }
}