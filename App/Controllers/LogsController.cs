using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace App.Controllers;

public class LogsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    public LogsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: Transactions
    public async Task<IActionResult> Index()
    {
        return View(await _dbContext.ActivityLogs.ToListAsync());
    }

    // GET: Transactions/List
    public async Task<List<ActivityLog>> List()
    {
        return await _dbContext.ActivityLogs.ToListAsync();
    }
}
