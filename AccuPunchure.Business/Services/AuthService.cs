using System.Security.Claims;
using AccuPunchure.Data.Repositories;

namespace AccuPunchure.Business.Services;

public class AuthService
{
    private readonly UserRepository         _userRepo;
    private readonly EmployeeRepository     _employeeRepo;
    private readonly OrganizationRepository _orgRepo;

    public AuthService(UserRepository userRepo, EmployeeRepository employeeRepo, OrganizationRepository orgRepo)
    {
        _userRepo     = userRepo;
        _employeeRepo = employeeRepo;
        _orgRepo      = orgRepo;
    }

    /// <summary>
    /// Returns a ClaimsPrincipal on success, null if the slug, username, or password is wrong.
    /// </summary>
    // Separate failure reasons so the controller can show a specific message during debugging.
    // Switch back to a single null return for production to avoid leaking info.
    public enum LoginFailure { OrgNotFound, UserNotFound, OrgMismatch, BadPassword }
    public LoginFailure? LastFailure { get; private set; }

    public async Task<ClaimsPrincipal?> Login(string username, string password, string slug)
    {
        LastFailure = null;

        var org = await _orgRepo.GetBySlug(slug);
        if (org is null) { LastFailure = LoginFailure.OrgNotFound; return null; }

        var user = await _userRepo.GetByUsername(username);

        if (user is null)                                            { LastFailure = LoginFailure.UserNotFound;  return null; }
        if (user.OrganizationId != org.OrganizationId)              { LastFailure = LoginFailure.OrgMismatch;   return null; }
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) { LastFailure = LoginFailure.BadPassword;   return null; }

        // Core claims — always present
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name,           user.Username),
            new("OrganizationId",          org.OrganizationId.ToString()),
            new("Slug",                    org.Slug),
            new(ClaimTypes.Role,           user.Role.ToString())
        };

        // Employee claims — present only when the user has a linked Employee record
        if (user.EmployeeId.HasValue)
        {
            var employee = await _employeeRepo.GetByEmployeeId(user.EmployeeId.Value);
            if (employee is not null)
            {
                claims.Add(new("EmployeeId", employee.EmployeeId.ToString()));
                claims.Add(new("Initials",   $"{employee.FirstName[0]}{employee.LastName[0]}".ToUpper()));
            }
        }

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        return new ClaimsPrincipal(identity);
    }
}
