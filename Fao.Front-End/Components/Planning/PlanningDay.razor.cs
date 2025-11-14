using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Fao.Front_End.Components.Planning
{
    public partial class PlanningDay
    {
        [Parameter] public DateTime? DayDate { get; set; } = null;
        public FullRecipeDTO? SelectedRecipe { get; set; } = null;
        public List<string>? StepsList { get; set; } = null;

        [Inject] public MealService MealService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

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

    }
}