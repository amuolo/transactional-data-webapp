using Microsoft.AspNetCore.Mvc;

namespace WebApp.wwwroot;

public class TransactionalDataController : Controller
{
    // GET: /TransactionalData/
    public string Index()   // IActionResult
    {
        return "Welcome";
    }

    // GET: /TransactionalData/Dashboard
    public string Dashboard()
    {
        return "This is the dashboard...";
    }
}
