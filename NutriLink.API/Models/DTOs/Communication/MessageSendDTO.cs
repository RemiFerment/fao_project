using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class MessageSendDTO
{
    public int Id { get; set; }
    public string ReceiverUuid { get; set; } = string.Empty;
    public string Content { get; set; } = "";
}