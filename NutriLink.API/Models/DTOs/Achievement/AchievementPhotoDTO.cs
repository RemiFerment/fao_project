namespace NutriLink.API.Models.DTOs.Achievement;

    public class AchievementPhotoDTO
    {
        public DateOnly DateAchieved { get; set; }
        public string Description { get; set; } = "";
        public byte[] PhotoData { get; set; } = [];
    }