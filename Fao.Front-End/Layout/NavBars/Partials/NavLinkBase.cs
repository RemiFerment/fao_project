using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Fao.Front_End.Components.Nav;

public abstract class NavLinkBase : ComponentBase, IDisposable
{
    [Parameter] public string Link { get; set; } = "/";
    [Parameter] public string Label { get; set; } = "";
    [Parameter] public string IconClass { get; set; } = "";

    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    protected string GetNavLinkClass()
    {
        var relativeUri = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        return relativeUri.Contains(Link.TrimStart('/'), StringComparison.OrdinalIgnoreCase)
            ? "navButtonActive"
            : string.Empty;
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }
}
