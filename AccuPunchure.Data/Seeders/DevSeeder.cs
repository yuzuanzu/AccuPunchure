using AccuPunchure.Data.Models;
using AccuPunchure.Data.Enums;
namespace AccuPunchure.Data.Seeders;

public static class DevSeeder
{
    /// <summary>Wipes all punches for the test org on every app start — dev only.</summary>
    public static void ClearPunches(AppDbContext db)
    {
        var testOrg = db.Organizations.FirstOrDefault(o => o.Slug == "test-org");
        if (testOrg is null) return;

        var employeeIds = db.Employees
            .Where(e => e.OrganizationId == testOrg.OrganizationId)
            .Select(e => e.EmployeeId)
            .ToList();

        var punches = db.Punches
            .Where(p => employeeIds.Contains(p.EmployeeId))
            .ToList();

        db.Punches.RemoveRange(punches);
        db.SaveChanges();
    }

    public static void Seed(AppDbContext db)
    {
        if (db.Organizations.Any()) return; // already seeded

        var org = new Organization
        {
            Name      = "Test Organization",
            Slug      = "test-org",
            CreatedAt = new DateTime(2026, 1, 1)
        };
        db.Organizations.Add(org);
        db.SaveChanges();

        var employee = new Employee
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = "employee@test.com",
            CreatedAt = new DateTime(2026, 1, 1),
            IsActive = true,
            OrganizationId = org.OrganizationId
        };
        db.Employees.Add(employee);
        db.SaveChanges();

        db.Users.AddRange(
            new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = UserRole.Admin,
                OrganizationId = org.OrganizationId,
                EmployeeId = null
            },
            new User
            {
                Username = "testemployee",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123"),
                Role = UserRole.Employee,
                OrganizationId = org.OrganizationId,
                EmployeeId = employee.EmployeeId
            }
        );
        db.SaveChanges();
    }
}