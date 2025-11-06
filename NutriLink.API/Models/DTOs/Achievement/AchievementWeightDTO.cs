namespace NutriLink.API.Models;

public class AchievementWeightDTO
{
    public int Id { get; set; }
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = "";
    public int Weight { get; set; }
}