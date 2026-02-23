namespace Fao.Front_End.Services;

using Fao.Front_End.Helpers;
using Fao.Front_End.Models;
using Fao.Front_End.Pages.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ChatService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<ChatMessageDTO>> GetChatMessagesAsync(string receiverUuid)
    {
        var messages = new List<ChatMessageDTO>();

        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return messages;

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.GetAsync($"/api/Message/to/{receiverUuid}");
        if (!response.IsSuccessStatusCode)
            return messages;
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(jsonString).RootElement;

        foreach (var element in json.EnumerateArray())
        {
            var message = new ChatMessageDTO
            {
                Id = element.SafeInt("id") ?? 0,
                RecieverId = element.SafeString("receiverUuid")!,
                SenderId = element.SafeString("senderUuid")!,
                Content = element.SafeString("content")!,
                Timestamp = element.GetProperty("dateTime").GetDateTime()
            };
            messages.Add(message);
        }

        return messages;
    }

    public async Task<bool> SendMessageAsync(string receiverUuid, string content)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) throw new InvalidOperationException("User is not authenticated.");

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        if (string.IsNullOrWhiteSpace(content))
            return false;

        var messageData = new
        {
            receiverUuid,
            content
        };

        var response = await _httpClient.PostAsJsonAsync("/api/Message/send", messageData);
        return response.EnsureSuccessStatusCode() != null;
    }

    public async Task<IEnumerable<UuidDTO>> GetAllUserFromCoach()
    {
        var token = await _authService.GetToken();
        var uuid = await _authService.GetUUIDFromToken();

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.GetAsync($"/api/Users/{uuid}]/customers");
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<IEnumerable<UuidDTO>>();
        if (users == null)
        {
            throw new InvalidOperationException("Failed to retrieve users.");
        }

        return users;
    }
}