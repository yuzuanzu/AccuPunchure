using AccuPunchure.Business.Enums;
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
        Console.WriteLine($"[PunchPOST] Hit. StartTime={form.StartTime}, EndTime={form.EndTime}, ModelValid={ModelState.IsValid}");
        foreach (var e in ModelState.Values.SelectMany(v => v.Errors))
            Console.WriteLine($"[PunchPOST] ModelState error: {e.ErrorMessage}");

        var employeeIdClaim = User.FindFirstValue("EmployeeId");
        Console.WriteLine($"[PunchPOST] EmployeeId claim={employeeIdClaim}");

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
        Console.WriteLine($"[PunchPOST] Punch saved for EmployeeId={employeeId}");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> History(TimeSpanFilter filter = TimeSpanFilter.OneWeek)
    {
        var employeeIdClaim = User.FindFirstValue("EmployeeId");
        if (!int.TryParse(employeeIdClaim, out var employeeId))
            return RedirectToAction("Login", "Account");

        var businessPunches = await _punchService.GetPunchesByEmployee(employeeId, filter);

        var punches = businessPunches.Select(p => new PunchModel
        {
            EmployeeId = p.EmployeeId,
            StartTime  = p.StartTime,
            EndTime    = p.EndTime
        }).ToList();

        var totalHours = businessPunches
            .Where(p => p.TotalTime.HasValue)
            .Aggregate(TimeSpan.Zero, (sum, p) => sum + p.TotalTime!.Value);

        var daysWorked = businessPunches
            .Select(p => p.StartTime.Date)
            .Distinct()
            .Count();

        var vm = new EmployeeHistoryViewModel
        {
            Initials         = User.FindFirstValue("Initials") ?? "?",
            EmployeeId       = employeeId,
            Punches          = punches,
            TotalHoursWorked = totalHours,
            DaysWorked       = daysWorked,
            SelectedTimeSpan = filter
        };

        return View(vm);
    }
    public IActionResult Profile() => View();
}
