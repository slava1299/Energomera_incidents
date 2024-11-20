using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Services.ConfirmationCode;

/// <inheritdoc />
public class ConfirmationCodeService : IConfirmationCodeService
{
    private readonly AppDbContext _db;
    
    public ConfirmationCodeService(AppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<AppUserConfirmationCode?> GetActiveCode(long telegramUserId)
    {
        var code = await _db.AppUserConfirmationCodes.FirstOrDefaultAsync(c => c.TelegramUserId == telegramUserId);

        if (code is null) return code;

        if ((DateTimeOffset.Now - code.CreatedAt).TotalMinutes > AppConstants.CodeLifetimeMinutes)
        {
            _db.AppUserConfirmationCodes.Remove(code);
            await _db.SaveChangesAsync();
            return null;
        }

        return code;
    }

    /// <inheritdoc />
    public async Task<AppUserConfirmationCode> CreateCode(long telegramUserId)
    {
        var existedCodes = await _db.AppUserConfirmationCodes.Where(c => c.TelegramUserId == telegramUserId).ToListAsync();

        if (existedCodes is not null && existedCodes.Any())
        {
            _db.AppUserConfirmationCodes.RemoveRange(existedCodes);
            await _db.SaveChangesAsync();
        }

        AppUserConfirmationCode code = new()
        {
            TelegramUserId = telegramUserId,
            Code = Generate6DigitCode().ToString()
        };

        _db.AppUserConfirmationCodes.Add(code);
        await _db.SaveChangesAsync();
        return code;
    }

    /// <inheritdoc />
    public async Task<bool> CheckCode(long telegramUserId, string code)
    {
        var codeEntity = await GetActiveCode(telegramUserId);

        if (codeEntity is null) return false;

        if (codeEntity.Code == code) return true;

        return false;
    }
    
    private int Generate6DigitCode()
    {
        Random r = new((int)DateTime.Now.Ticks);
        return r.Next(1000, 9999);
    }
}