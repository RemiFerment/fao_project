namespace NutriLink.API.Models.DTOs.Achievement;

public class AchievementTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = ""; // "measurement", "photo", "weight", "none"

    // Optional data depending on Type
    public int? Waist { get; set; }
    public int? Hips { get; set; }
    public int? Weight { get; set; }
    public byte[]? Photo { get; set; }
}
