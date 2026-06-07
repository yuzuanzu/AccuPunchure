namespace AccuPunchure.Web.Models;

public class EmployeeProfileViewModel : BaseViewModel
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Username { get; set; }
}