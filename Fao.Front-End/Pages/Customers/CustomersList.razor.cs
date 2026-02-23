using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Components.Pages.Customers;

public partial class CustomersList : ComponentBase
{
    public List<UserDTO> ListCustomers { get; set; } = new();
    [Inject] public UserServices UserServices { get; set; } = null!;
    public bool IsLoading { get; set; } = true;
    public string SearchTerm { get; set; } = string.Empty;
    public IEnumerable<UserDTO> FilteredCustomers = Enumerable.Empty<UserDTO>();
    protected override async Task OnParametersSetAsync()
    {
        await RefreshCustomersList();
    }
    public async Task RefreshCustomersList()
    {
        await InvokeAsync(async () =>
        {
            IsLoading = true;
            ListCustomers = (await UserServices.GetCustomersAsync() ?? []).Where(c => c != null).ToList()!;
            IsLoading = false;
            FilteredCustomers = ListCustomers;
            StateHasChanged();
        });
    }
    public void RemoveCustomer(string uuid)
    {
        ListCustomers.RemoveAll(c => c.Uuid == uuid);
        StateHasChanged();
    }
}