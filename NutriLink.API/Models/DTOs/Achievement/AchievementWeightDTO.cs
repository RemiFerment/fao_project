namespace NutriLink.API.Models.DTOs.Achievement;
    public class AchievementWeightDTO
    {
        public DateOnly DateAchieved { get; set; }
        public string Description { get; set; } = "";
        public int Weight { get; set; }
    }