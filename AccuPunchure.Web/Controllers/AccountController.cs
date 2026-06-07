using Microsoft.AspNetCore.Mvc;

namespace AccuPunchure.Web.Controllers;

public class AccountController : Controller
{
    public IActionResult Login() => View();
}
