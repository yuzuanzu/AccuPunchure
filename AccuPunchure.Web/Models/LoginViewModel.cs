using System.ComponentModel.DataAnnotations;

namespace AccuPunchure.Web.Models;

public class LoginViewModel
{
    // Populated from the route — not a form field
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    // Set server-side when credentials fail — not bound from the form
    public string? ErrorMessage { get; set; }
}
