using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Fao.Front_End.Pages.Core;

public partial class Logout : ComponentBase
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // Clear the authentication state
        CustomAuthStateProvider? authStateProvider = (CustomAuthStateProvider)AuthStateProvider;
        await authStateProvider.MarkUserAsLoggedOutAsync();

        await Task.Delay(500);

        // Redirect to login page
        Nav.NavigateTo(uri: "/login", forceLoad: true);
    }


}