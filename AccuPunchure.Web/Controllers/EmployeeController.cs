using AccuPunchure.Business.Services;
using AccuPunchure.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccuPunchure.Web.Controllers;

[Authorize]
public class EmployeeController : Controller
{
    private readonly PunchServices _punchService;

    public EmployeeController(PunchServices punchService)
    {
        _punchService = punchService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var vm = new EmployeeDashboardViewModel
        {
            Initials = User.FindFirstValue("Initials") ?? "?"
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Index(PunchModel form)
    {
        // Read EmployeeId from the auth cookie — never trust a value the form submits
        var employeeIdClaim = User.FindFirstValue("EmployeeId");
        if (!int.TryParse(employeeIdClaim, out var employeeId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(new EmployeeDashboardViewModel { Initials = User.FindFirstValue("Initials") ?? "?" });

        var punch = new Business.Models.PunchModel
        {
            EmployeeId = employeeId,
            StartTime  = form.StartTime,
            EndTime    = form.EndTime
        };

        await _punchService.CreatePunch(punch);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult History() => View();
    public IActionResult Profile() => View();
}
