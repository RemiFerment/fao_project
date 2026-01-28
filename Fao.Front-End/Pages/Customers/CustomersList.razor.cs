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
    public IEnumerable<UserDTO> FilteredCustomers =>
    string.IsNullOrWhiteSpace(SearchTerm)
        ? ListCustomers
        : ListCustomers.Where(c =>
            c.FirstName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
            c.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
            (c.FirstName + " " + c.LastName).Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
        );
    protected override async Task OnParametersSetAsync()
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
    public void RemoveCustomer(string uuid)
    {
        ListCustomers.RemoveAll(c => c.Uuid == uuid); // if List<UserDTO>
        StateHasChanged();
    }
}