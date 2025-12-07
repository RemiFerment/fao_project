namespace Fao.Front_End.Services;

using System.Net.Http.Json;
using Fao.Front_End.Models;

public class UserServices
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public UserServices(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<UserDTO> GetUserInfoAsync()
    {
        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("User is not authenticated.");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "api/user/info");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var userInfo = await response.Content.ReadFromJsonAsync<UserDTO>();
        if (userInfo == null)
        {
            throw new InvalidOperationException("Failed to retrieve user information.");
        }

        return userInfo;
    }

}