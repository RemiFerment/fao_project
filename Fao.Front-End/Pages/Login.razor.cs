using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fao.Front_End.Services;
using Fao.Front_End.Models;

namespace Fao.Front_End.Pages;

public partial class Login : ComponentBase
{
    private LoginDTO loginData = new();
    public string? ErrorMessage { get; set; }

    [Inject] private AuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private async Task HandleLogin()
    {
        ErrorMessage = null;

        try
        {
            var token = await AuthService.LoginAsync(loginData.Email, loginData.PlainPassword);

            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"✅ Token: {token}");
                await AuthStateProvider.MarkUserAsAuthenticatedAsync(token);
                Nav.NavigateTo("/test",true);
            }
            else
            {
                ErrorMessage = "Identifiants incorrects.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de connexion : {ex.Message}";
        }
    }
}
