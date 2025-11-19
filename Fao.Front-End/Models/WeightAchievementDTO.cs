namespace Fao.Front_End.Models;

public class WeightAchievementDTO
{
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Weight { get; set; }
}