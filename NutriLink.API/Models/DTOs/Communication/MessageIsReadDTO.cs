using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class MessageIsReadDTO
{
    public int Id { get; set; }
    public bool IsRead { get; set; }
}