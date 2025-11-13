namespace Fao.Front_End.Components.Meal;

using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

public partial class RecipeCard : ComponentBase
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string CategoryName { get; set; } = string.Empty;
    [Parameter] public int RecipeId { get; set; }
    [Inject] private MealService MealService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    private string mealType = string.Empty;
    private DateOnly date;

    protected override void OnParametersSet()
    {
        LoadQueryParameters();
    }
    public async Task SelectRecipe(int Id)
    {
        try
        {
            await MealService.AssignRecipeToMealDayAsync(date, Id, mealType);
            NavigationManager.NavigateTo($"/planning?date={date.ToString("yyyy-MM-dd")}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'assignation de la recette : {ex.Message}");
        }
    }

    private void LoadQueryParameters()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("date", out var dateValue))
        {
            if (DateTime.TryParse(dateValue, out var parsedDate))
                date = DateOnly.FromDateTime(parsedDate);
        }

        if (query.TryGetValue("type", out var typeValue))
        {
            mealType = typeValue.ToString();
        }
    }
}