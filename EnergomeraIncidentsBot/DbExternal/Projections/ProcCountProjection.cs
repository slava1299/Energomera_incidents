using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.DbExternal.Projections;

[Keyless]
public class ProcCountProjection
{
    [Column("Count")]
    public int Count { get; set; }
}