namespace Fao.Front_End.Services;

public class PhotoAchievementDTO
{
    public DateOnly DateAchieved { get; set; }
    public string Description { get; set; } = string.Empty;
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
}