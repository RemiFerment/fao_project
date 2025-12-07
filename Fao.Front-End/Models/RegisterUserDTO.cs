namespace Fao.Front_End.Models;

public class RegisterUserDTO
{
    public string Email { get; set; } = "";
    public string PlainPassword { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateTime BirthDate { get; set; }
}