using System.Reflection.Metadata;

namespace NutriLink.API.Models;

public class AchievementTypePhoto : AchievementType
{
    public byte[] Photo { get; set; } = [];
}