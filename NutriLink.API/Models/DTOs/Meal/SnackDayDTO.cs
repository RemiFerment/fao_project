using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class SnackDayDTO
{
    public int Id { get; set; }
    public DateOnly Date;
    public string UserUUID { get; set; } = default!;
    public int SnackId { get; set; }
}