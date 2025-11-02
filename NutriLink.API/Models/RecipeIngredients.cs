using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NutriLink.API.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = default!;

        public int IngredientId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(IngredientId))]
        public Ingredient Ingredient { get; set; } = default!;

        // Extra fields
        public double Quantity { get; set; }
        public string Unit { get; set; } = "";
    }
}

