namespace Fao.Front_End.Models;

public class AchievementDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Type { get; set; } = string.Empty;
    public int? Weight { get; set; }
    public int? Hips { get; set; }
    public int? Waist { get; set; }
    public byte[]? Photo { get; set; }
}