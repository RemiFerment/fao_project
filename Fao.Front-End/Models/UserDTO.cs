namespace Fao.Front_End.Models;

public class UserDTO
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string? CoachUuid { get; set; }

    public string GetIdentity()
    {
        return $"{FirstName} {LastName}";
    }
    public string GetGenderShort()
    {
        return Gender.ToLower() switch
        {
            "male" => "M",
            "female" => "F",
            _ => "O"
        };
    }
    public string GetFormattedBirthDate()
    {
        return BirthDate.ToString("d MMM yyyy");
    }

    public string GetAge()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - BirthDate.Year;
        if (today < BirthDate.AddYears(age))
        {
            age--;
        }
        return age.ToString();
    }
}