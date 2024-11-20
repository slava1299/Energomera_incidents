namespace EnergomeraIncidentsBot.App.Options;

public class AppConfig
{
    public DbConnectionsOptions DbConnections { get; set; }
    public EmailOptions Email { get; set; }
    public AppQuartzOptions Quartz { get; set; }
}