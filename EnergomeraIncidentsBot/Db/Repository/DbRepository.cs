using EnergomeraIncidentsBot.App.Enums;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.DbExternal.Projections;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Repository;

public class DbRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    
    public DbRepository(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Добавить пользователя (если не существует).
    /// </summary>
    /// <returns></returns>
    public async Task<AppUser> AddAppUser(AppUser user)
    {
        AppUser? existed = await GetAppUser(user.TelegramUserId);

        if (existed is null)
        {
            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        existed.IsRegistered = user.IsRegistered;
        existed.Fio = user.Fio;
        existed.Email = user.Email;
        existed.IsActive = user.IsActive;
        _db.AppUsers.Update(existed);
        await _db.SaveChangesAsync();
        return existed;
    }

    /// <summary>
    /// Получить пользователя по телеграм ИД.
    /// </summary>
    /// <param name="telegramUserId"></param>
    /// <returns></returns>
    public async Task<AppUser?> GetAppUser(long telegramUserId)
    {
        return await _db.AppUsers.FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);
    }

    /// <summary>
    /// Получить активного и зарегистрированного пользователя по емайл.
    /// </summary>
    /// <param name="email">Емайл пользователя.</param>
    /// <returns></returns>
    public async Task<AppUser?> GetRegisteredAppUserByEmail(string email)
    {
        return await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == email 
                                                           && u.IsActive == true 
                                                           && u.IsRegistered == true);
    }
    
    /// <summary>
    /// Пометить пользователя неактивным.
    /// </summary>
    /// <param name="telegramUserId"></param>
    public async Task ResetAppUser(long telegramUserId)
    {
        AppUser? appUser = await GetAppUser(telegramUserId);

        if (appUser is null) return;
        
        appUser.IsRegistered = false;
        appUser.IsActive = false;
        _db.AppUsers.Update(appUser);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Метод сохранения инцидентов из внешней БД.
    /// </summary>
    /// <param name="projections"></param>
    public async Task<List<Incident>> SaveIncidentAlarms(List<ProcMasTelegramAlarmsStartProjection> projections)
    {
        List<Incident> newIncidents = projections.Select(p =>
            {
                Incident incident = _mapper.Map<Incident>(p);
                incident.Status = IncidentStatus.New;
                return incident;
            })
        .ToList();
        
        _db.Incidents.AddRange(newIncidents);
        await _db.SaveChangesAsync();
        return newIncidents;
    }
    
    /// <summary>
    /// Метод сохранения уведомлений по неудачным инцидентам.
    /// </summary>
    /// <param name="projections"></param>
    public async Task<List<IncidentFailNotification>> SaveIncidentFailNotifications(List<ProcMasTelegramCheckFailsProjection> projections)
    {
        List<IncidentFailNotification> newFailNotifications = projections.Select(p =>
            {
                IncidentFailNotification fail = _mapper.Map<IncidentFailNotification>(p);
                return fail;
            })
            .ToList();
        
        _db.IncidentFailNotifications.AddRange(newFailNotifications);
        await _db.SaveChangesAsync();
        return newFailNotifications;
    }
    
    /// <summary>
    /// Метод сохранения уведомлений по инцидентам для купирования.
    /// </summary>
    /// <param name="projections"></param>
    public async Task<List<IncidentPreventNotification>> SaveIncidentPreventNotifications(List<ProcMasTelegramReplyCheckProjection> projections)
    {
        List<IncidentPreventNotification> preventNotifications = projections.Select(p =>
            {
                IncidentPreventNotification fail = _mapper.Map<IncidentPreventNotification>(p);
                return fail;
            })
            .ToList();
        
        _db.IncidentPreventNotifications.AddRange(preventNotifications);
        await _db.SaveChangesAsync();
        return preventNotifications;
    }

    /// <summary>
    /// Получить пользователя по инциденту.
    /// </summary>
    /// <param name="incident"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<AppUser?> GetUserForIncident(Incident incident)
    {
        if (incident == null) throw new ArgumentNullException(nameof(incident));
        if(string.IsNullOrEmpty(incident.ExecutorEmail)) throw new ArgumentNullException(nameof(incident.ExecutorEmail));

        return await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == incident.ExecutorEmail);
    }

    /// <summary>
    /// Получить незавершенные инциденты для пользователя.
    /// </summary>
    /// <param name="telegramUserId">Телеграм ИД пользователя.</param>
    /// <returns></returns>
    public async Task<List<Incident>> GetUserActiveIncidents(long telegramUserId)
    {
        AppUser? user = await GetAppUser(telegramUserId);

        if (user is null) return new List<Incident>();

        return await _db.Incidents
            .Where(i => i.ExecutorEmail == user.Email
                        && i.Status != IncidentStatus.Canceled
                        && i.Status != IncidentStatus.Completed)
            .ToListAsync();
    }

    /// <summary>
    /// Получить инцидент по ИД.
    /// </summary>
    /// <param name="incidentId"></param>
    /// <returns></returns>
    public async Task<Incident?> GetIncidentById(long incidentId)
    {
        return await _db.Incidents.FirstOrDefaultAsync(i => i.Id == incidentId);
    }

    /// <summary>
    /// Получить из БД новые инциденты.
    /// </summary>
    /// <returns></returns>
    public async Task<List<Incident>> GetNewIncidents()
    {
        return await _db.Incidents.Where(i => i.Status == IncidentStatus.New).ToListAsync();
    }

    /// <summary>
    /// Получить все активные уведомления.
    /// </summary>
    /// <returns></returns>
    public async Task<List<IncidentFailNotification>> GetIncidentFailNotifications()
    {
        return await _db.IncidentFailNotifications.ToListAsync();
    }
    
    /// <summary>
    /// Получить все активные уведомления по инцидентам для купирования.
    /// </summary>
    /// <returns></returns>
    public async Task<List<IncidentPreventNotification>> GetIncidentPreventNotifications()
    {
        return await _db.IncidentPreventNotifications.ToListAsync();
    }

    /// <summary>
    /// Удалить уведомления о незавершении инцидента из БД.
    /// </summary>
    /// <param name="notifications"></param>
    public async Task RemoveIncidentFailNotification(params IncidentFailNotification[] notifications)
    {
        _db.IncidentFailNotifications.RemoveRange(notifications);
        await _db.SaveChangesAsync();
    }
    
    /// <summary>
    /// Удалить уведомления о купировании инцидента из БД.
    /// </summary>
    /// <param name="notifications"></param>
    public async Task RemoveIncidentPreventNotification(params IncidentPreventNotification[] notifications)
    {
        _db.IncidentPreventNotifications.RemoveRange(notifications);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Поменять статус инциденту.
    /// </summary>
    /// <param name="incidentId">ИД инцидента.</param>
    /// <param name="status">Статус инцидента.</param>
    public async Task ChangeIncidentStatus(long incidentId, IncidentStatus status)
    {
        var incident = await GetIncidentById(incidentId);

        if (incident is null)
            throw new Exception($"Инцидент не найден [{incidentId}] для смены статуса на [{status.ToString()}]");

        incident.Status = status;
        _db.Incidents.Update(incident);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Добавить уведомление в историю уведомлений.
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<TelegramNotificationHistoryItem> AddTelegramNotificationToHistory(long chatId, string message)
    {
        TelegramNotificationHistoryItem item = new()
        {
            TelegramChatId = chatId,
            Message = message,
        };

        _db.TelegramNotificationHistory.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }
}