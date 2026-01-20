namespace Fao.Front_End.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js = default!;
    private readonly AuthenticationStateProvider _authStateProvider;

    private readonly NavigationManager _nav;


    public AuthService(HttpClient http, IJSRuntime js, AuthenticationStateProvider authStateProvider, NavigationManager nav)
    {
        _http = http;
        _js = js;
        _authStateProvider = authStateProvider;
        _nav = nav;
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
        var token = await GetToken();

        return JWTUtilService.GetClaim(token!, ClaimTypes.NameIdentifier);
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "jwtToken");
        await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOutAsync();
        _nav.NavigateTo("/login", forceLoad: true);
    }

    public async Task<string?> GetToken()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "jwtToken");
        if (string.IsNullOrEmpty(token))
        {
            await LogoutAsync();
            return null;
        }

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token);

        var exp = jwtToken.Payload.Expiration;

        if (exp == null)
        {
            await LogoutAsync();
            return null;
        }

        var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp.ToString()!));

        if (expirationDate < DateTimeOffset.UtcNow)
        {
            await LogoutAsync();
            return null;
        }

        return token;
    }

}