using System.ComponentModel.DataAnnotations.Schema;

namespace NutriLink.API.Models;

public class User
{
    public int Id { get; set; }
    public string UUID { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public int? UserProfileId { get; set; }
    [ForeignKey(nameof(UserProfileId))]
    public UserProfile? UserProfile { get; set; } = default!;
    public int RoleId { get; set; }
    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; } = default!;
}
