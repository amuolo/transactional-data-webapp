using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

public class LogsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    public LogsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _dbContext.ActivityLogs.ToListAsync());
    }
}
