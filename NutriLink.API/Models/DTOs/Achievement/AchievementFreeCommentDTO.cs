namespace NutriLink.API.Models;

public class AchievementFreeCommentDTO
{
    public int Id { get; set; }
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = "";
}