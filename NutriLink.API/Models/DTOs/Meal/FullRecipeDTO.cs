namespace NutriLink.API.Models;

public class FullRecipeDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Steps { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public List<FullRecipeIngredientDTO> Ingredients { get; set; } = new List<FullRecipeIngredientDTO>();
}