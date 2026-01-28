using System.ComponentModel.DataAnnotations;
using Fao.Front_End.Models;
using Fao.Front_End.Services;
using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Components.Pages.Customers;

public partial class CustomersForm : ComponentBase
{
    [Inject] public UserServices UserServices { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    public RegisterDTO Customer { get; set; } = new();
    public bool IsLoading { get; set; } = false;
    public string ErrorMessage { get; set; } = string.Empty;

    [Required]
    private string PasswordConfirmation { get; set; } = string.Empty;

    public async Task HandleValidSubmit()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        if (Customer.PlainPassword != PasswordConfirmation)
        {
            ErrorMessage = "La confirmation du mot de passe ne correspond pas.";
            IsLoading = false;
            return;
        }
        if (Customer != null)
        {
            try
            {
                UserDTO newUser = await UserServices.CreateCustomerAsync(Customer);
                Customer = new RegisterDTO()
                {
                    BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-18))
                };
                PasswordConfirmation = string.Empty;
                NavigationManager.NavigateTo("/customer/" + newUser.Uuid);
                
            }
            catch (Exception ex)
            {
                ErrorMessage = "Une erreur est survenue lors de la création du client : " + ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }
        else
        {
            ErrorMessage = "Veuillez remplir tous les champs obligatoires.";
            IsLoading = false;
        }
    }

    protected override void OnInitialized()
    {
        Customer.BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-18));
    }

}