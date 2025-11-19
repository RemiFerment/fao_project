namespace Fao.Front_End.Pages.Achievement;

using System.Threading.Tasks;
using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

[Authorize]
public partial class Achievement : ComponentBase
{
    [Inject] public AchievementService AchievementService { get; set; } = null!;
    [Inject] public IJSRuntime JS { get; set; } = default!;
    public AchievementDTO? SelectedAchievement { get; set; } = null;

    private List<AchievementDTO?> Achievements { get; set; } = new List<AchievementDTO?>();
    bool isLoading = false;

    protected override async Task OnParametersSetAsync()
    {
        await DisplayAchievements();
    }

    protected override async Task OnInitializedAsync()
    {
        await DisplayAchievements();
    }

    public async Task DisplayAchievements()
    {
        isLoading = false;
        Achievements = await AchievementService.GetAllAchievementsAsync() ?? new List<AchievementDTO?>();
        isLoading = true;
    }

    public async Task GetSelectedAchievement(AchievementDTO achievement)
    {
        SelectedAchievement = achievement;
        await InvokeAsync(StateHasChanged);

        await Task.Delay(100); // Small delay to ensure modal element is rendered

        await InvokeAsync(async () =>
        {
            await JS.InvokeVoidAsync("showAchievementModal");
        });
    }
}