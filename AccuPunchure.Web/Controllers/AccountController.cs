using AccuPunchure.Business.Services;
using AccuPunchure.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AccuPunchure.Web.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _authService;

    public AccountController(AuthService authService)
    {
        _authService = authService;
    }

    // GET /{slug}/login
    [HttpGet]
    public IActionResult Login(string slug)
        => View(new LoginViewModel { Slug = slug });

    // POST /{slug}/login
    [HttpPost]
    public async Task<IActionResult> Login(string slug, LoginViewModel form)
    {
        form.Slug = slug; // re-attach so the view can rebuild the form action on failure

        if (!ModelState.IsValid) return View(form);

        var principal = await _authService.Login(form.Username, form.Password, slug);

        if (principal is null)
        {
            form.ErrorMessage = _authService.LastFailure switch
            {
                AuthService.LoginFailure.OrgNotFound  => $"[DEBUG] No organization found for slug '{slug}'.",
                AuthService.LoginFailure.UserNotFound  => $"[DEBUG] No user found with username '{form.Username}'.",
                AuthService.LoginFailure.OrgMismatch   => "[DEBUG] User exists but belongs to a different organization.",
                AuthService.LoginFailure.BadPassword   => "[DEBUG] Password did not match.",
                _                                      => "Login failed."
            };
            return View(form);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // Use principal (not User) — HttpContext.User hasn't updated yet in this request
        return principal.IsInRole("Admin")
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Employee");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirect back to the org's login page using the OrganizationId claim
        var slug = User.FindFirst("Slug")?.Value ?? string.Empty;
        return RedirectToAction("Login", new { slug });
    }
}
