using Fao.Front_End.Models;
using Microsoft.AspNetCore.Components;
using Fao.Front_End.Services;
using Microsoft.JSInterop;
using Fao.Front_End.Pages.Interfaces;
using System.Threading.Tasks;

namespace Fao.Front_End.Pages.Planning;

public partial class MealSelector : ComponentBase, IMealSelector
{
    #region Parameters and Properties
    [Parameter]
    public string Uuid { get; set; } = string.Empty;
    private DateTime MealDate { get; set; }
    private string searchTerm = string.Empty;
    private bool isLoading = false;
    private bool isSubmit = false;
    private IEnumerable<RecipeOverviewDTO> Recipes = Enumerable.Empty<RecipeOverviewDTO>();
    public FullRecipeDTO? SelectedRecipe { get; set; } = null;
    public List<string>? StepsList { get; set; } = null;
    [Inject] private MealService MealService { get; set; } = null!;
    [Inject] private UserServices UserServices { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IJSRuntime JS { get; set; } = default!;
    #endregion

    #region Methods
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

    protected override async Task OnInitializedAsync()
    {


        var uri = new Uri(NavigationManager.Uri);
        var query = uri.Query.TrimStart('?');
        var dateValue = query
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split('=', 2))
            .Where(p => p.Length == 2 && p[0].Equals("date", StringComparison.OrdinalIgnoreCase))
            .Select(p => Uri.UnescapeDataString(p[1]))
            .FirstOrDefault();
        await UserServices.RouteGuardAsync(Uuid, $"/planning/{Uuid}/meal?date={dateValue}");

        if (!string.IsNullOrEmpty(dateValue) && DateTime.TryParse(dateValue, out var parsed))
        {
            MealDate = parsed;
        }
        else
        {
            MealDate = DateTime.Today;
        }
    }

    public async Task HandleRecipeSelected(int recipeId)
    {
        if (recipeId <= 0)
            return;

        SelectedRecipe = await MealService.GetFullRecipeAsync(recipeId);

        StepsList = SelectedRecipe?.Steps?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Select(s => char.ToUpper(s[0]) + s.Substring(1))
            .ToList();

        await InvokeAsync(StateHasChanged);

        await InvokeAsync(async () =>
        {
            await JS.InvokeVoidAsync("showRecipeModal");
        });
    }
    #endregion
}