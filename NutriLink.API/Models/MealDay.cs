using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class MealDay
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; } = default!;
    public int? CoachId { get; set; }
    [ForeignKey(nameof(CoachId))]
    public User? Coach { get; set; } = default!;
    public int? BreakfastId { get; set; }
    [ForeignKey(nameof(BreakfastId))]
    public Recipe? Breakfast { get; set; } = default!;
    public int? LunchId { get; set; }
    [ForeignKey(nameof(LunchId))]
    public Recipe? Lunch { get; set; } = default!;
    public int? DinnerId { get; set; }
    [ForeignKey(nameof(DinnerId))]
    public Recipe? Dinner { get; set; } = default!;

}