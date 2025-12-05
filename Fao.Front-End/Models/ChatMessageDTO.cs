namespace Fao.Front_End.Models;

public class ChatMessageDTO
{
    public int Id;
    public string SenderId = string.Empty;
    public string RecieverId = string.Empty;
    public string Content = string.Empty;
    public DateTime Timestamp;
}
