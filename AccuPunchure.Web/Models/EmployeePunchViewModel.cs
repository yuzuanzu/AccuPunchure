namespace AccuPunchure.Web.Models;

public class EmployeeDashboardViewModel : BaseViewModel
{
    public bool IsPunchedIn { get; set; }
    public DateTime? CurrentPunchStart { get; set; }
    public PunchModel? CompletedPunch { get; set; }
}