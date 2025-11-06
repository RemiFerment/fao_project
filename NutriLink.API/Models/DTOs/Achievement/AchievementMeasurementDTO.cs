namespace NutriLink.API.Models;

public class AchievementMeasurementDTO
{
    public int Id { get; set; }
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = "";
    public int HipsMeasurement { get; set; } 
    public int WaistMeasurement { get; set; }
}