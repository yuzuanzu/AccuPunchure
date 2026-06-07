using AccuPunchure.Business.Enums;

namespace AccuPunchure.Web.Models;

public class EmployeeHistoryViewModel : BaseViewModel
{
    public int EmployeeId { get; set; }
    public IList<PunchModel>? Punches { get; set; }
    public TimeSpan TotalHoursWorked { get; set; }
    public TimeSpanFilter SelectedTimeSpan { get; set; } = TimeSpanFilter.OneWeek;

}