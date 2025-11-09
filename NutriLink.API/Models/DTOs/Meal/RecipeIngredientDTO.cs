namespace NutriLink.API.Models;

public class RecipeIngredientDTO
{
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}