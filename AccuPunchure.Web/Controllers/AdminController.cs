using Microsoft.AspNetCore.Mvc;

namespace AccuPunchure.Web.Controllers;

public class AdminController : Controller
{
    public IActionResult Index() => View();
}
