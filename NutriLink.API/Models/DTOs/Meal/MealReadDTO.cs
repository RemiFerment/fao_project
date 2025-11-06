namespace NutriLink.API.Models;

public class MealReadDTO
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int? BreakfastId { get; set; } = default!;
    public int? LunchId { get; set; } = default!;
    public int? DinnerId { get; set; } = default!;
}