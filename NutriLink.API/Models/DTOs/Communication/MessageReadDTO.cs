using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class MessageReadDTO
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public int? SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public string Content { get; set; } = "";
}