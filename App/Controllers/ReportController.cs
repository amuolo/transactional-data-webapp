using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public class ReportController : Controller
{
    public IActionResult Index()
    {
        // TODO: remove these lines
        //var test = new Dictionary<string, TransactionalData>();
        //test["test1"] = new TransactionalData();
        //test["test2"] = new TransactionalData();
        //ViewData["TransactionalData"] = test;

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
