namespace NutriLink.API.Models;

public class UserProfileDTO
{
    public string Uuid { get; set; } = "";
    public int Size { get; set; }
    public int Weight { get; set; }
    public string PhysicalActivity { get; set; } = "";
    public string Job { get; set; } = "";
    public int EnergyRequirement { get; set; }
}