namespace Fao.Front_End.Services;

public class MeasurementAchievementDTO
{
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = string.Empty;
    public int HipsMeasurement { get; set; }
    public int WaistMeasurement { get; set; }
}