using Fao.Front_End.Models;
using Microsoft.AspNetCore.Components;
using Fao.Front_End.Services;

namespace Fao.Front_End.Pages.Planning;

public partial class MealSelector : ComponentBase
{
    private DateTime MealDate { get; set; }
    private string searchTerm = string.Empty;
    private bool isLoading = false;
    private bool isSubmit = false;
    [Inject] private MealService MealService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    private IEnumerable<RecipeOverviewDTO> Recipes = Enumerable.Empty<RecipeOverviewDTO>();

    public async Task OnSearch()
    {
        Recipes = Enumerable.Empty<RecipeOverviewDTO>();
        isLoading = true;

        var result = await MealService.SearchRecipesAsync(searchTerm);
        if (result == null || !result.Any())
        {
            isLoading = false;
            isSubmit = true;
            return;
        }
        isSubmit = true;
        isLoading = false;
        Recipes = result;
    }

    protected override void OnInitialized()
    {
        var uri = new Uri(NavigationManager.Uri);
        var query = uri.Query.TrimStart('?');
        var dateValue = query
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split('=', 2))
            .Where(p => p.Length == 2 && p[0].Equals("date", StringComparison.OrdinalIgnoreCase))
            .Select(p => Uri.UnescapeDataString(p[1]))
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(dateValue) && DateTime.TryParse(dateValue, out var parsed))
        {
            MealDate = parsed;
        }
        else
        {
            MealDate = DateTime.Today;
        }
    }
}