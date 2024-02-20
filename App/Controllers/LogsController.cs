using App.Data;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public class LogsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    public LogsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return View();
    }
}
