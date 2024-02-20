using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public class ReportController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Earnings()
    {
        return View();
    }

    public IActionResult Expenses()
    {
        return View();
    }
}
