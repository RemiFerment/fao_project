namespace Fao.Front_End.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private const string TokenKey = "jwtToken";
    public CustomAuthStateProvider(IJSRuntime js)
    {
        _js = js;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        var jwt = handler.ReadJwtToken(token);
        if (!JWTUtilService.IsTokenValid(token))
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }

}
