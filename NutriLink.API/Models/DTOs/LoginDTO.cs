namespace NutriLink.API.Models;

public class LoginDTO
{
    public string Email { get; set; } = string.Empty;
    public string plainPassword { get; set; } = string.Empty;
}