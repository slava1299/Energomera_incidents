using EnergomeraIncidentsBot.DbExternal;
using EnergomeraIncidentsBot.Services.ExternalDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Controller;

public class TestController : ControllerBase
{
    private readonly ExternalDbContext _externalDb;
    private readonly IExternalDbRepository _repos;

    public TestController(ExternalDbContext externalDb, IExternalDbRepository repos)
    {
        _externalDb = externalDb;
        _repos = repos;
    }

    [HttpGet("/")]
    public async Task<IActionResult> CheckWorkingApp()
    {
        return Ok("Application is working!!!");
    }
    
    [HttpGet("/test")]
    public async Task<IActionResult> TestApp()
    {
        var param = new SqlParameter("fio", "Пьянов");
        var a = await _externalDb.MasTelegramFio.ToListAsync();
        return Ok();
    }
}