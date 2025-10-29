namespace NutriLink.API.Models;

public class User
{
    public int Id { get; set; }
    public string UUID { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public int Size {get;set;}
    public int Weight { get; set; }
    public string PhysicalActivity { get; set; } = "";
    public string Job { get; set; } = "";
    public int EnergyRequirement { get; set; }
}
