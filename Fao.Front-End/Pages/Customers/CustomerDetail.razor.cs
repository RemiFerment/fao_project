using System.Threading.Tasks;
using Fao.Front_End.Layout;
using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;


namespace Fao.Front_End.Pages.Customers
{
    public partial class CustomerDetail : ComponentBase
    {
        [Parameter] public Guid CustomerUUID { get; set; }
        [Inject] public UserServices UserServices { get; set; } = default!;
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [CascadingParameter] public MainLayout MainLayout { get; set; } = default!;

        public UserDTO? Customer { get; set; }
        public bool IsLoading { get; set; } = true;

        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;
            Customer = await UserServices.GetUserByUuidAsync(CustomerUUID.ToString());

            IsLoading = false;
        }
        private void NavigateBack()
        {
            NavigationManager.NavigateTo("/customers");
        }

        private void NavigateToProfile()
        {
            NavigationManager.NavigateTo($"/customer-profile/{CustomerUUID}");
        }

        private async Task DeletebButton()
        {
            if (MainLayout != null)
            {
                await MainLayout.ShowConfirm(EventCallback.Factory.Create(this, DeleteCustomer),
                "Confirmation", "Êtes-vous sûr de vouloir supprimer ce client ? Cette action est irréversible.");

            }
        }

        private async Task DeleteCustomer()
        {
            if (Customer != null)
            {
                await UserServices.DeleteUserAsync(Customer.Uuid.ToString());
                NavigationManager.NavigateTo("/customers");
            }
        }
    }
}