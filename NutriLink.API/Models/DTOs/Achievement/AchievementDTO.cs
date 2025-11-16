namespace NutriLink.API.Models.DTOs.Achievement;

public class AchievementDTO
{
    public int Id { get; set; }
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = "";
    public AchievementTypeDTO? AchievementType { get; set; }
}