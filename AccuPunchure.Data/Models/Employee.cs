namespace AccuPunchure.Data.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool IsManager { get; set; } = false;

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    public IList<Employee> Subordinates { get; set; } = new List<Employee>();
    public IList<Punch> Punches { get; set; } = new List<Punch>();
    public User? User { get; set; }
}