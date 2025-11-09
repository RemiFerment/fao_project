namespace NutriLink.API.Models;

public class FullRecipeIngredientDTO
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}