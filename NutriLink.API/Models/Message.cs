using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class Message
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public int? SenderId { get; set; }
    [ForeignKey(nameof(SenderId))]
    public User? Sender { get; set; } = default!;
    public int? ReceiverId { get; set; }
    [ForeignKey(nameof(ReceiverId))]
    public User? Receiver { get; set; } = default!;
    public string Content { get; set; } = "";
    public bool IsRead { get; set; }
}