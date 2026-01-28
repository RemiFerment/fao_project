namespace NutriLink.API.Models;

public class ReadUserDTO
{
    public string Uuid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string? CoachUuid { get; set; }
}