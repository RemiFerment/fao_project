using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Fao.Front_End.Pages.CoachMessaging
{
    public partial class CoachMessaging : ComponentBase, IDisposable
    {
        [Inject] public ChatService ChatService { get; set; } = default!;
        [Inject] public UserServices UserServices { get; set; } = default!;
        [Inject] public AuthService AuthService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        public List<ChatMessageDTO>? Messages = null;
        public List<UuidDTO>? Customers = null!;
        public string MessageContent = string.Empty;
        public bool isLoading = false;
        private string SenderId = string.Empty;
        private string? CurrentReceiverUuid = null;

        private CancellationTokenSource? _refreshTokenSource;
        private Task? _refreshTask;

        private bool loadUser = true;
        private bool loadMessages = false;


        public DateOnly test = DateOnly.FromDateTime(DateTime.Now);

        protected override async Task OnInitializedAsync()
        {
            SenderId = await AuthService.GetUUIDFromToken() ?? string.Empty;
            if (string.IsNullOrEmpty(SenderId))
            {
                await AuthService.LogoutAsync();
                return;
            }
            Customers = await UserServices.GetCustomersUuidsAsync();
            loadUser = false;
        }

        public async Task LoadMessages()
        {
            Messages = await ChatService.GetChatMessagesAsync(CurrentReceiverUuid!);
        }

        public async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageContent))
                return;

            isLoading = true;
            await ChatService.SendMessageAsync(CurrentReceiverUuid!, MessageContent);
            await LoadMessages();
            await JS.InvokeVoidAsync("scrollChatToBottom");
            MessageContent = string.Empty;
            isLoading = false;
        }

        public async Task SelectReceiver(string uuid)
        {
            loadMessages = true;
            CurrentReceiverUuid = uuid;
            MessageContent = string.Empty;
            await LoadMessages();
            StateHasChanged();
            await JS.InvokeVoidAsync("scrollChatToBottom");
            loadMessages = false;

            ResetRefreshToken();
        }

        private void ResetRefreshToken()
        {
            _refreshTokenSource?.Cancel();
            _refreshTokenSource = new CancellationTokenSource();
            _refreshTask = StartAutoRefresh(_refreshTokenSource.Token);
        }

        private async Task StartAutoRefresh(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (CurrentReceiverUuid != null)
                {
                    await LoadMessages();
                }

                await Task.Delay(3000, token);
            }
        }

        private bool IsSentByCurrentUser(ChatMessageDTO message)
        {
            return message.SenderId == SenderId;
        }

        public void Dispose()
        {
            _refreshTokenSource?.Cancel();
            CurrentReceiverUuid = null;
        }
    }
}