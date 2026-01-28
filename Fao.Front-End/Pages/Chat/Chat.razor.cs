namespace Fao.Front_End.Pages.Chat;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components.Web;

[Authorize(Roles = "ROLE_USER,ROLE_ADMIN")]
public partial class Chat : ComponentBase
{
    public List<ChatMessageDTO> Messages = new List<ChatMessageDTO>();
    public string MessageContent { get; set; } = string.Empty;
    private string senderId = string.Empty;
    private string recieverUuid = string.Empty;
    private bool isLoading = false;
    private string errorMessage = string.Empty;
    [Inject] public IJSRuntime JS { get; set; } = default!;
    [Inject] public ChatService ChatService { get; set; } = default!;
    [Inject] public UserServices UserServices { get; set; } = default!;

    [Inject] public AuthService AuthService { get; set; } = default!;
    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(MessageContent))
            return;
        isLoading = true;
        await ChatService.SendMessageAsync(recieverUuid, MessageContent);
        var newMessages = await ChatService.GetChatMessagesAsync(recieverUuid);
        Messages = newMessages;
        isLoading = false;
        await JS.InvokeVoidAsync("scrollChatToBottom");
        MessageContent = string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        var currentUserId = await AuthService.GetUUIDFromToken();
        if (currentUserId != null)
        {
            senderId = currentUserId;
        }
        recieverUuid = (await UserServices.GetCoachUuidAsync())!.Uuid;
        if (string.IsNullOrEmpty(recieverUuid))
        {
            errorMessage = "No coach assigned to this user.";
            return;
        }

        _ = Task.Run(async () =>
    {
        while (true)
        {
            await LoadMessages();
            await InvokeAsync(StateHasChanged);
            await Task.Delay(3000);
        }
    });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("scrollChatToBottom");
        }
    }

    private async Task LoadMessages()
    {
        var newMessages = await ChatService.GetChatMessagesAsync(recieverUuid);

        if (newMessages == null || newMessages.Count == 0)
            return;

        foreach (var msg in newMessages)
        {
            if (!Messages.Any(m => m.Id == msg.Id))
                Messages.Add(msg);
        }

        Messages = Messages.OrderBy(m => m.Timestamp).ToList();

        StateHasChanged();
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await SendMessage();
    }

    private bool IsSentByCurrentUser(ChatMessageDTO message)
    {
        return message.SenderId == senderId;
    }


}
