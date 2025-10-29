namespace NutriLink.API.Models;

public record class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Calories { get; set; }
}
