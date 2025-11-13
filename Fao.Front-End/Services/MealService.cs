namespace Fao.Front_End.Services;

using Fao.Front_End.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class MealService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public MealService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<MealOverviewDTO?> GetMealOverviewAsync(DateTime? mealDate)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return null;

        if (mealDate.HasValue)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get,
    $"api/Meal/{uuid}/meal-days?date={mealDate.Value:yyyy-MM-dd}");

            var token = await _authService.GetToken();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var mealOverview = await response.Content.ReadFromJsonAsync<MealOverviewDTO>();
            return mealOverview;

        }
        return null;
    }

    public async Task<RecipeOverviewDTO?> GetRecipeOverviewAsync(int? recipeId)
    {
        if (recipeId == null) return null;
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/Recipes/{recipeId}");

        var token = await _authService.GetToken();
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode) return null;

        var recipeOverview = await response.Content.ReadFromJsonAsync<RecipeOverviewDTO>();
        return recipeOverview;
    }

    public async Task<IEnumerable<RecipeOverviewDTO>?> SearchRecipesAsync(string searchTerm)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Get,
            $"api/Recipes/search?keyword={Uri.EscapeDataString(searchTerm)}");

        var token = await _authService.GetToken();
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode) return null;

        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeOverviewDTO>>();
        return recipes;
    }

    public async Task<bool> AssignRecipeToMealDayAsync(DateOnly mealDate, int recipeId, string mealType)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return false;

        mealType = mealType.ToLower();
        string date = mealDate.ToString("yyyy-MM-dd");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"api/Meal/{uuid}/meal-days/{date}/{mealType}");

        var token = await _authService.GetToken();
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            Id = 0,
            RecipeId = recipeId,
        };

        requestMessage.Content = JsonContent.Create(payload);

        var response = await _httpClient.SendAsync(requestMessage);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveRecipeFromMealDayAsync(DateTime mealDate, string mealType)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return false;

        mealType = mealType.ToLower();
        string date = mealDate.ToString("yyyy-MM-dd");

        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/Meal/{uuid}/meal-days/{date}/{mealType}");

        var token = await _authService.GetToken();
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);
        return response.IsSuccessStatusCode;
    }
}
