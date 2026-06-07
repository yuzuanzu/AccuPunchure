namespace AccuPunchure.Data.Models;

public class Punch
{
    public int PunchId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public int EmployeeId { get; set; }
    
    public Employee Employee { get; set; } = null!;
}