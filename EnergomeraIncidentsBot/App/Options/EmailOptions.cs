namespace EnergomeraIncidentsBot.App.Options;

public class EmailOptions
{
    public const string Section = "Email";
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}