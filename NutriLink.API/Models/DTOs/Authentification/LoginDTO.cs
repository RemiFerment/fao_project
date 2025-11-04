namespace NutriLink.API.Models;

public class LoginDTO
{
    public string Email { get; set; } = string.Empty;
    public string PlainPassword { get; set; } = string.Empty;
}