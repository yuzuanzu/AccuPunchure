namespace AccuPunchure.Business.Models;

public class EmployeeModel
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Initials => $"{FirstName[0]}{LastName[0]}".ToUpper();
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int OrganizationId { get; set; }
    public List<PunchModel> Punches { get; set; } = new();
    public TimeSpan TotalHoursWorked => Punches
        .Where(p => p.TotalTime.HasValue)
        .Aggregate(TimeSpan.Zero, (sum, p) => sum + p.TotalTime!.Value);

    public int DaysWorked => Punches
        .Select(p => p.StartTime.Date)
        .Distinct()
        .Count();
}
