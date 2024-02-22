using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json;

namespace App.Controllers;

public class LogsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    private async Task<List<ActivityLog>> QueryAsync() 
        => await _dbContext.ActivityLogs.OrderByDescending(x => x.Date).ToListAsync();

    public LogsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: Transactions
    public async Task<IActionResult> Index()
    {
        return View(await QueryAsync());
    }

    // GET: Transactions/List
    public async Task<JsonResult> List()
    {
        return Json(await QueryAsync(), JsonSerializerOptions.Default);
    }
}
