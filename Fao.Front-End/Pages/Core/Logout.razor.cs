using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Pages.Core;

public partial class Logout : ComponentBase
{
    [Inject] private AuthService AuthService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(500);
        await AuthService.LogoutAsync();
    }
}