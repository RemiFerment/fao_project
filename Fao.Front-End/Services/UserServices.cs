namespace Fao.Front_End.Services;

using System.Net.Http.Headers;
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

        var uuid = await _authService.GetUUIDFromToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var userInfo = await response.Content.ReadFromJsonAsync<UserDTO>();
        if (userInfo == null)
        {
            throw new InvalidOperationException("Failed to retrieve user information.");
        }

        return userInfo;
    }

    public async Task<List<UserDTO>> GetCustomersAsync()
    {
        var customersUuids = await GetCustomersUuidsAsync();
        var customers = new List<UserDTO>();

        foreach (var customerUuid in customersUuids)
        {
            var customerInfo = await GetUserByUuidAsync(customerUuid.Uuid);
            customers.Add(customerInfo);
        }

        return customers;
    }

    public async Task<List<UuidDTO>> GetCustomersUuidsAsync()
    {
        var token = await _authService.GetToken();

        var uuid = await _authService.GetUUIDFromToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}/customers");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var customersUuids = await response.Content.ReadFromJsonAsync<List<UuidDTO>>();
        if (customersUuids == null)
        {
            throw new InvalidOperationException("Failed to retrieve customers UUIDs.");
        }

        return customersUuids;
    }

    public async Task<UserDTO> GetUserByUuidAsync(string uuid)
    {
        var token = await _authService.GetToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var userInfo = await response.Content.ReadFromJsonAsync<UserDTO>();
        if (userInfo == null)
        {
            throw new InvalidOperationException("Failed to retrieve user information.");
        }

        return userInfo;
    }

    public async Task<UuidDTO> GetCoachUuidAsync()
    {
        var token = await _authService.GetToken();

        var uuid = await _authService.GetUUIDFromToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}/coach-user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var coachUuid = await response.Content.ReadFromJsonAsync<UuidDTO>();
        if (coachUuid == null)
        {
            throw new InvalidOperationException("Failed to retrieve coach UUID.");
        }

        return coachUuid;
    }
}