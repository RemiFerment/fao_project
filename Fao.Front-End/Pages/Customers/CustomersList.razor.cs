using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Components.Pages.Customers;

public partial class CustomersList : ComponentBase
{
    public List<UserDTO> ListCustomers { get; set; } = new();
    [Inject] public UserServices UserServices { get; set; } = null!;
    public bool IsLoading { get; set; } = true;
    protected override async Task OnInitializedAsync()
    {
        await RefreshCustomersList();
    }
    public async Task RefreshCustomersList()
    {
        await InvokeAsync(async () =>
        {
            IsLoading = true;
            ListCustomers = await UserServices.GetCustomersAsync();
            IsLoading = false;
            StateHasChanged();
        });
    }
}