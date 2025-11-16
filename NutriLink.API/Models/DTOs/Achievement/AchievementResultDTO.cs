namespace NutriLink.API.Models.DTOs.Achievement;

public class AchievementResultDTO
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public DateOnly DateAchieved { get; set; }
    public string Type { get; set; } = "";
    public object? Data { get; set; }
}