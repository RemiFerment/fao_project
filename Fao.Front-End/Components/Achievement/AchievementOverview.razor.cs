using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Components.Achievement;

public partial class AchievementOverview : ComponentBase
{
    [Parameter] public AchievementDTO Achievement { get; set; } = new AchievementDTO();
    [Parameter] public EventCallback Display { get; set; }
    [Parameter] public EventCallback<AchievementDTO?> OnAchievementSelected { get; set; }
    [Inject] public AchievementService AchievementService { get; set; } = null!;


    private string ConvertedWeight()
    {
        //Add a comma after the third digit from the right
        string? weight = Achievement.Weight.ToString();
        if (weight != null && weight.Length >= 3)
        {
            weight = weight.Insert(weight.Length - 1, ",");
        }
        return weight!;
    }
    private string ConvertedMeasure(int? measurement)
    {
        //Add a comma after the third digit from the right
        string? measure = measurement.ToString();
        if (measure != null && measure.Length >= 3)
        {
            measure = measure.Insert(measure.Length - 1, ",");
        }
        return measure!;
    }

    private string TruncateDescription()
    {
        string description = Achievement.Description;
        if (description.Length > 100)
        {
            return description.Substring(0, 100) + "...";
        }
        return description;
    }

    public async Task DeleteAchievement(int id)
    {
        await AchievementService.DeleteAchievementAsync(id);
        await Display.InvokeAsync();
    }

    private async Task SelectAchievement()
    {
        if (OnAchievementSelected.HasDelegate)
        {
            await OnAchievementSelected.InvokeAsync(Achievement);
        }
    }
}

