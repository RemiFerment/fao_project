namespace NutriLink.API.Models;

public class AchievementPhotoDTO
{
    public int Id { get; set; }
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = "";
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
}