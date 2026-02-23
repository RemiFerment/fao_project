using Fao.Front_End.Models;

namespace Fao.Front_End.Pages.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatMessageDTO>> GetChatMessagesAsync(string receiverUuid);
        Task<bool> SendMessageAsync(string receiverUuid, string content);
        Task<IEnumerable<UuidDTO>> GetAllUserFromCoach();
    }
}