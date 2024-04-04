using Microsoft.AspNetCore.Mvc;

namespace Pustok.Controllers;

public class HomeController : Controller
{
 
    public IActionResult Index()
    {
        return View();
    }

}

