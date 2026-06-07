using AccuPunchure.Data.Models;
using AccuPunchure.Data.Enums;
namespace AccuPunchure.Data.Seeders;

public static class DevSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Organizations.Any()) return; // already seeded

        var org = new Organization
        {
            Name = "Test Organization",
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
                //Slug = "test-org",
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