namespace Fao.Front_End.Models;

public class UserDTO
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string? CoachUuid { get; set; }
}