namespace AccuPunchure.Web.Models;

public class PunchModel
{
    public int       EmployeeId { get; set; }
    public DateTime  StartTime  { get; set; }
    public DateTime? EndTime    { get; set; }

    // Calculated for display — null while the punch is still open
    public TimeSpan? TotalTime => EndTime.HasValue ? EndTime.Value - StartTime : null;
}
