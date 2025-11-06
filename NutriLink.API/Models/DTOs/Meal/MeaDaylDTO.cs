namespace NutriLink.API.Models;

public class MealDayDTO
{
    public int Id { get; set; }
    public string UserUUID { get; set; } = default!;
    public DateOnly Date { get; set; }
    public int? BreakfastId { get; set; } = default!;
    public int? LunchId { get; set; } = default!;
    public int? DinnerId { get; set; } = default!;
}