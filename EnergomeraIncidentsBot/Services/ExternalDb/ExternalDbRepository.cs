using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.DbExternal;
using EnergomeraIncidentsBot.DbExternal.Entities;
using EnergomeraIncidentsBot.DbExternal.Projections;
using EnergomeraIncidentsBot.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Services.ExternalDb;

/// <inheritdoc />
public class ExternalDbRepository : IExternalDbRepository
{
    private readonly ExternalDbContext _db;

    public ExternalDbRepository(ExternalDbContext db)
    {
        _db = db;
    }


    /// <inheritdoc />
    public async Task<int> CheckFioDouble(string fio)
    {
        if (fio == null) throw new ArgumentNullException(nameof(fio));
        
        var countProjection = (await _db.ProcCheckFioDouble
            .FromSqlRaw("DECLARE @return_value int,\n" +
                        "@rez int\n" +
                        "EXEC @return_value = [dbo].[mas_telegram_check_fio_double]\n" +
                        $"@fio = N'{fio.ToSqlParam()}',\n" +
                        "@rez = @rez OUTPUT\n" +
                        "SELECT\t@rez as N'Count'")
            .ToListAsync())?.FirstOrDefault();

        if (countProjection is null) return 0;
        return countProjection.Count;
    }

    /// <inheritdoc />
    public async Task<string> GetFioByEmail(string email)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        
        var fioProjection = (await _db.ProcTelegramFioByMail
            .FromSqlRaw("DECLARE @return_value int\n" +
                        "EXEC @return_value = [dbo].[mas_telegram_fio_by_email]\n" +
                        "@TelegramAcc = N'',\n" +
                        $"@mail = N'{email.ToSqlParam()}'")
            .ToListAsync())?.FirstOrDefault();

        if (fioProjection is null) return null;
        return fioProjection.Fio;
    }

    /// <inheritdoc />
    public async Task<string?> GetEmailByFio(string fio)
    {
        if (fio == null) throw new ArgumentNullException(nameof(fio));
        if (fio.Length > 255) throw new Exception("ФИО не должно быть более 255 символов");

        var emailProjection = (await _db.ProcTelegramMailByFio
            .FromSqlRaw("DECLARE @mail varchar(255)\n" + 
                        $"EXECUTE dbo.mas_telegram_mail_by_fio  @fio=N\'{fio.ToSqlParam()}\', @mail = @mail OUTPUT\n" +
                        "SELECT\t@mail as N'mail'")
            .ToListAsync())?.FirstOrDefault();

        if (emailProjection is null) return null;
        return emailProjection.Mail;
    }

    /// <inheritdoc />
    public async Task<List<ProcMasTelegramAlarmsStartProjection>> GetAlarms()
    {
        var alarmProjections = await _db.ProcMasTelegramAlarmsStart
            .FromSqlRaw("DECLARE @return_value int\n" +
                        "EXEC @return_value = [dbo].[mas_telegram_alarms_start]")
            .ToListAsync();
        
        return alarmProjections ?? new();
    }
    
    public async Task<List<ProcMasTelegramAlarmsStartProjection>> GetIncidentFails()
    {
        var alarmProjections = (await _db.ProcMasTelegramAlarmsStart
            .FromSqlRaw("DECLARE @return_value int\n" +
                        "EXEC @return_value = [dbo].[mas_telegram_alarms_start]")
            .ToListAsync());
        
        return alarmProjections;
    }

    /// <inheritdoc />
    public async Task<bool> IsEmailExists(string email)
    {
        var fio = await GetFioByEmail(email);

        if (string.IsNullOrEmpty(fio) == false) return true;

        return false;
    }

    /// <inheritdoc />
    public async Task RemoveTelegramUser(string email)
    {
        var user = await _db.MasTelegramFio.FirstOrDefaultAsync(it => it.Mail == email);
        
        if(user is null) return;

        _db.MasTelegramFio.Remove(user);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<MasTelegramFio> AddTelegramUser(string? fio, string email, string? telegramUsername)
    {
        if (string.IsNullOrEmpty(telegramUsername) == false && telegramUsername.StartsWith("@") == false)
        {
            telegramUsername = "@" + telegramUsername;
        }
        
        var user = await _db.MasTelegramFio.FirstOrDefaultAsync(it => it.Mail == email);

        if (user is null)
        {
            MasTelegramFio newUser = new()
            {
                Fio = fio ?? "",
                Mail = email,
                Telegram = telegramUsername ?? "",
            };
            _db.MasTelegramFio.Add(newUser);
            await _db.SaveChangesAsync();
            return newUser;
        }

        user.Fio = fio ?? "";
        user.Telegram = telegramUsername ?? "";
        _db.MasTelegramFio.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    /// <inheritdoc />
    public async Task<ProcMasTelegramAlarmReplyProjection> ReplyIncident(Incident incident, bool arrivedExecutor, string? comment = null)
    {
        int stat = arrivedExecutor ? 1 : 2;
        
        var result = (await _db.ProcMasTelegramAlarmReply
            .FromSqlRaw($"DECLARE @return_value int,\n" +
                        $"@ruk_fio varchar(252)\n" +
                        $"EXEC @return_value = [dbo].[mas_telegram_alarm_reply]\n" +
                        $"@number = N'{incident.IncidentNumber.ToSqlParam()}',\n" +
                        $"@level = {incident.IncidentLevel},\n" +
                        $"@fio = N'{incident.Executor?.ToSqlParam()}',\n" +
                        $"@komment = N'{comment?.ToSqlParam() ?? "Прибыл"}',\n" +
                        $"@stat = {stat},\n" +
                        $"@ruk_fio = @ruk_fio OUTPUT\n" +
                        $"SELECT @ruk_fio as N'@ruk_fio'")
            .ToListAsync()).First();
        
        return result;
    }

    /// <inheritdoc />
    public async Task<List<ProcMasTelegramCheckFailsProjection>> GetFails()
    {
        List<ProcMasTelegramCheckFailsProjection>? fails = await _db.ProcMasTelegramCheckFails
            .FromSqlRaw($"DECLARE @return_value int\n" +
                        $"EXEC @return_value = [dbo].[mas_telegram_check_fails]")
            .ToListAsync();
        return fails ?? new List<ProcMasTelegramCheckFailsProjection>();
    }

    /// <inheritdoc />
    public async Task<List<ProcMasTelegramReplyCheckProjection>> GetReplyCheck()
    {
        List<ProcMasTelegramReplyCheckProjection>? result = await _db.ProcMasTelegramReplyCheck
            .FromSqlRaw($"DECLARE @return_value int\n" +
                        $"EXEC @return_value = [dbo].[mas_telegram_reply_check]")
            .ToListAsync();
        return result ?? new List<ProcMasTelegramReplyCheckProjection>();
    }
}