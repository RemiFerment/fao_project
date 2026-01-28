using System.ComponentModel.DataAnnotations;

namespace Fao.Front_End.Models;

public class RegisterDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 100 caractères.")]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères.")]
    public string LastName { get; set; } = string.Empty;
    [Required]
    [AllowedValues("male", "female")]
    public string Gender { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Date)]
    [Range(typeof(DateOnly), "1920-01-01", "2020-12-31", ErrorMessage = "La date de naissance doit être valide.")]
    public DateOnly BirthDate { get; set; }
    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "Le mot de passe doit contenir au moins une lettre majuscule, une lettre minuscule, un chiffre et un caractère spécial.")]
    [DataType(DataType.Password)]
    public string PlainPassword { get; set; } = string.Empty;


}