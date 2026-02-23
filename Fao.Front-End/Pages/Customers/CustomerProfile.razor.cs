using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Pages.Customers
{
    public partial class CustomerProfile : ComponentBase
    {
        [Parameter]
        public Guid CustomerUuid { get; set; }
        public UserDTO Customer { get; set; } = new UserDTO();
        public UserProfileDTO? CustomerProfileData { get; set; } = null;
        [Inject]
        public UserServices UserServices { get; set; } = default!;
        public bool IsLoading { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        public bool NewProfile { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var customer = await UserServices.GetUserByUuidAsync(CustomerUuid.ToString());
            if (customer == null)
            {
                NavigationManager.NavigateTo("/customers");
                return;
            }
            CustomerProfileData = await UserServices.GetUserProfileAsync(CustomerUuid.ToString());
            if (CustomerProfileData == null)
            {
                NewProfile = true;
                CustomerProfileData = new UserProfileDTO();
            }

        }

        public async Task HandleValidSubmit()
        {
            ErrorMessage = string.Empty;
            IsLoading = true;
            if (NewProfile)
            {
                await UserServices.SetUserProfileAsync(CustomerUuid.ToString(), CustomerProfileData!);
                NavigationManager.NavigateTo("/customer/" + CustomerUuid);
            }
            else
            {
                await UserServices.UpdateUserProfileAsync(CustomerUuid.ToString(), CustomerProfileData!);
                NavigationManager.NavigateTo("/customer/" + CustomerUuid);
            }
            IsLoading = false;
        }

        public void CalculateEnergyNeeds()
        {
            //Formule à modifier + NAP à modifier
            CustomerProfileData!.EnergyRequirement = Customer.Gender switch
            {
                "male" => Convert.ToInt32(Math.Round(88.36 + (13.4 * CustomerProfileData.Weight) + (4.8 * CustomerProfileData.Size) - (5.7 * Customer.GetAgeInt()))),
                "female" => Convert.ToInt32(Math.Round(447.6 + (9.2 * CustomerProfileData.Weight) + (3.1 * CustomerProfileData.Size) - (4.3 * Customer.GetAgeInt()))),
                _ => 0,
            };
        }
        public void NavigateBack()
        {
            NavigationManager.NavigateTo("/customer/" + CustomerUuid);
        }
    }
}