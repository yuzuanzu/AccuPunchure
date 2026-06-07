namespace AccuPunchure.Business.Models;

public class PunchModel
{
    public int PunchId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? TotalTime => EndTime.HasValue ? EndTime.Value - StartTime : null;
    public bool IsEdited { get; set; }
    public int EmployeeId { get; set; }
}
