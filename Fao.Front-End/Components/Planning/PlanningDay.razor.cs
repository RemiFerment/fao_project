using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Components.Planning
{
    public partial class PlanningDay
    {
        [Parameter] public DateTime? DayDate { get; set; } = null;
    }
}