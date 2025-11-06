namespace NutriLink.API.Models;

public class MealSendDTO
{
    public int Id { get; set; }
    public int? RecipeId { get; set; } = default!;
}