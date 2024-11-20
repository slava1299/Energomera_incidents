using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.DbExternal.Projections;
using Mapster;

namespace EnergomeraIncidentsBot.Mapster;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProcMasTelegramAlarmsStartProjection, Incident>();
        config.NewConfig<ProcMasTelegramCheckFailsProjection, IncidentFailNotification>();
        config.NewConfig<ProcMasTelegramReplyCheckProjection, IncidentPreventNotification>();
    }
}