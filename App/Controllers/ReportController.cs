using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

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

    public string Values()
    {
        return "My values";
    }

    // localhost:{PORT}/TransactionalData/Value?id=Rick
    public string Value(string id)
    {
        return HtmlEncoder.Default.Encode($"Transaction with id {id}");
    }
}
