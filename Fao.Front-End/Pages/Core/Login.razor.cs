using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fao.Front_End.Services;
using Fao.Front_End.Models;

namespace Fao.Front_End.Pages.Core;

public partial class Login : ComponentBase
{
    private LoginDTO loginData = new();
    public string? ErrorMessage { get; set; }
    public bool IsLoading { get; set; } = false;

    [Inject] private AuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private async Task HandleLogin()
    {
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            var token = await AuthService.LoginAsync(loginData.Email, loginData.PlainPassword);

            if (!string.IsNullOrEmpty(token))
            {
                await AuthStateProvider.MarkUserAsAuthenticatedAsync(token);
                Nav.NavigateTo("/", true);
            }
            else
            {
                ErrorMessage = "Identifiants incorrects.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de connexion : {ex.Message}, si le problème persiste, contactez un administrateur.";
        }

        IsLoading = false;
    }
}
