using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class MessageDTO
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string SenderUuid { get; set; } = string.Empty;
    public string ReceiverUuid { get; set; } = string.Empty;
    public string Content { get; set; } = "";
}