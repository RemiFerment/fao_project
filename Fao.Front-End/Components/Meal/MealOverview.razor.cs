using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Components.Meal
{
    public partial class MealOverview : ComponentBase
    {
        #region Parameters and Properties
        [Parameter] public DateTime? MealDate { get; set; } = null;
        [Parameter] public MealTypeEnum MealTypeFilter { get; set; } = default!;
        [Parameter] public EventCallback<int> OnRecipeSelected { get; set; }
        public string Uuid { get; set; } = "";

        public string? MealName { get; set; } = "";
        public string? MealType { get; set; } = "";
        public string? ErrorMessage { get; set; } = null;
        private bool IsLoaded { get; set; } = false;
        private int CurrentRecipeId;
        [Inject] public MealService MealService { get; set; } = default!;
        [Inject] public UserServices UserServices { get; set; } = default!;
        [Inject] public UriHelperService UriHelperService { get; set; } = default!;

        #endregion

        #region Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            Uuid = UriHelperService.GetUuidFromUri();
        }
        protected override async Task OnParametersSetAsync()
        {
            await DisplayMeal();
        }
        public async Task ShowRecipeDetails()
        {
            await OnRecipeSelected.InvokeAsync(CurrentRecipeId);
        }

        private async Task DisplayMeal()
        {
            ErrorMessage = null;
            try
            {
                var MealDay = await MealService.GetMealOverviewAsync(MealDate, Uuid);
                switch (MealTypeFilter)
                {
                    case MealTypeEnum.Breakfast:
                        CurrentRecipeId = MealDay?.BreakfastId ?? 0;
                        await LoadMeal(MealDay?.BreakfastId);
                        break;
                    case MealTypeEnum.Lunch:
                        CurrentRecipeId = MealDay?.LunchId ?? 0;
                        await LoadMeal(MealDay?.LunchId);
                        break;
                    case MealTypeEnum.Dinner:
                        CurrentRecipeId = MealDay?.DinnerId ?? 0;
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
                ErrorMessage = "Erreur lors du chargement du repas, si le problème persiste, contactez un administrateur.";
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
                NavigationManager.NavigateTo($"/planning/{Uuid}/meal?date={MealDate.Value:yyyy-MM-dd}&type={MealTypeFilter}");
            }
        }

        public async Task DeleteMeal(DateTime? mealDate, string mealType)
        {
            if (mealDate.HasValue)
            {
                await MealService.RemoveRecipeFromMealDayAsync(mealDate.Value, mealType, Uuid);
                await DisplayMeal();
            }
        }
        #endregion

    }
    public enum MealTypeEnum
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack
    }
}