using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public class ReportsController : Controller
{
    public IActionResult Reports()
    {
        return View();
    }
}
