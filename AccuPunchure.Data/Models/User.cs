using AccuPunchure.Data.Enums;
namespace AccuPunchure.Data.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Employee;

    public int EmployeeId { get; set; }
}