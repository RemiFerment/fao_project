namespace NutriLink.API.Models;

public class RegisterDTO
{
    public string Email { get; set; } = string.Empty;
    public string PlainPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public int RoleId { get; set; }
}