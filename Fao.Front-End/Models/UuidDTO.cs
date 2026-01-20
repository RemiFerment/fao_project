namespace Fao.Front_End.Models;

public class UuidDTO
{
    public string Uuid { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    public string FullName => $"{FirstName} {LastName}";
}