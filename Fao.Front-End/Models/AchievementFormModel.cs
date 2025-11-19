using Microsoft.AspNetCore.Components.Forms;

namespace Fao.Front_End.Models;

public class AchievementFormModel
{
    public string Type { get; set; } = string.Empty;
    public DateOnly DateAchieved { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string Description { get; set; } = string.Empty;
    public int? Weight { get; set; }
    public int? Hips { get; set; }
    public int? Waist { get; set; }
    public byte[]? Photo { get; set; }
}