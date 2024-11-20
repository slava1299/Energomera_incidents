using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.DbExternal.Projections;

[Keyless]
public class ProcFioProjection
{
    [Column("Наименование")]
    public string Fio { get; set; }
}