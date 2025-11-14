namespace Fao.Front_End.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js = default!;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<string?> LoginAsync(string email, string plainPassword)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", new { email, plainPassword });
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("token").GetString();
    }

    public async Task<string?> GetUUIDFromToken()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "jwtToken");
        return JWTUtilService.GetClaim(token, ClaimTypes.NameIdentifier);
    }

    public async Task<string?> GetToken()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", "jwtToken");
    }

}