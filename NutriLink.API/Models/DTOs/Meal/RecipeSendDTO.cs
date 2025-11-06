namespace NutriLink.API.Models;

public class RecipeSendDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Steps { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}