namespace NutriLink.API.Models;

public class RecipeIngredientUpdateDTO
{
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public int NewIngredientId { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}