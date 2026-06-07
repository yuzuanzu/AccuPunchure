using Microsoft.AspNetCore.Mvc;

namespace AccuPunchure.Web.Controllers;

public class EmployeeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult History() => View();
    public IActionResult Profile() => View();
}
