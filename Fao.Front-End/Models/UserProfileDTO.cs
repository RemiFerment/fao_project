
using System.ComponentModel.DataAnnotations;

namespace Fao.Front_End.Models;

public class UserProfileDTO
{
    [Required]
    [Range(50, 280, ErrorMessage = "La taille doit être comprise entre 50 et 280 cm.")]
    public int Size { get; set; }
    [Required]
    [Range(20, 500, ErrorMessage = "Le poids doit être compris entre 20 et 500 kg.")]
    public int Weight { get; set; }
    [Required]
    public string PhysicalActivity { get; set; } = string.Empty;
    [Required]
    public string Job { get; set; } = string.Empty;
    [Required]
    [Range(800, 6000, ErrorMessage = "Les besoins énergétiques doivent être compris entre 800 et 6000 kcal.")]
    public int EnergyRequirement { get; set; }
}