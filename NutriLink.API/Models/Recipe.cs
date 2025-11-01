using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NutriLink.API.Models;

public class Recipe
{
    public int Id { get; set; }
    [Required, MaxLength(255)]
    public string Title { get; set; } = "";
    [Column(TypeName = "nvarchar(max)")]
    public string Steps { get; set; } = "";


    [Required, Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = default!;
    [JsonIgnore]
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
