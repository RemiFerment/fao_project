using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class Achievement
{
    public int Id { get; set; }
    public DateOnly DateAchieved { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = default!;
    public int AchievementTypeId { get; set; }
    [ForeignKey("AchievementTypeId")]
    public AchievementType AchievementType { get; set; } = default!;
    public string Description { get; set; } = "";
}