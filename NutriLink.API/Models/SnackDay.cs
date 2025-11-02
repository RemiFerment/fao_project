using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class SnackDay
{
    public int Id { get; set; }
    public DateOnly date;
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;
    public int SnackId { get; set; }
    [ForeignKey(nameof(SnackId))]
    public Recipe Snack { get; set; } = default!;
}