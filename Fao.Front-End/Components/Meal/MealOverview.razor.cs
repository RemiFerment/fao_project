using System.Threading.Tasks;
using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Components.Meal
{
    public partial class MealOverview : ComponentBase
    {
        [Parameter] public DateTime? MealDate { get; set; } = null;
        [Parameter] public MealTypeEnum MealTypeFilter { get; set; } = default!;
        public string? MealName { get; set; } = "";
        public string? MealType { get; set; } = "";
        private bool IsLoaded { get; set; } = false;

        [Inject] public MealService MealService { get; set; } = default!;

        // protected override async Task OnInitializedAsync()
        // {
        //     await DisplayMeal();
        // }

        protected override async Task OnParametersSetAsync()
        {
            await DisplayMeal();
        }

        private async Task DisplayMeal()
        {
            try
            {
                var MealDay = await MealService.GetMealOverviewAsync(MealDate);
                switch (MealTypeFilter)
                {
                    case MealTypeEnum.Breakfast:
                        await LoadMeal(MealDay?.BreakfastId);
                        break;
                    case MealTypeEnum.Lunch:
                        await LoadMeal(MealDay?.LunchId);
                        break;
                    case MealTypeEnum.Dinner:
                        await LoadMeal(MealDay?.DinnerId);
                        break;
                    default:
                        MealName = null;
                        MealType = null;
                        break;
                }
                IsLoaded = true;
            }
            catch (Exception)
            {
                MealName = null;
                MealType = null;
                IsLoaded = false;
            }
            return;
        }
        private async Task LoadMeal(int? recipeId)
        {
            var recipe = await MealService.GetRecipeOverviewAsync(recipeId);
            MealName = recipe?.Title;
            MealType = recipe?.CategoryName;
        }

        public void NavigateToMealSelector()
        {
            if (MealDate.HasValue)
            {
                NavigationManager.NavigateTo($"/planning/meal?date={MealDate.Value:yyyy-MM-dd}&type={MealTypeFilter}");
            }
        }

        public async Task DeleteMeal(DateTime? mealDate, string mealType)
        {
            if (mealDate.HasValue)
            {
                await MealService.RemoveRecipeFromMealDayAsync(mealDate.Value, mealType);
                await DisplayMeal();
            }
        }


    }
    public enum MealTypeEnum
    {
        Breakfast,
        Lunch,
        Dinner
    }
}