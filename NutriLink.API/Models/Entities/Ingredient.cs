using System.Text.Json.Serialization;

namespace NutriLink.API.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        [JsonIgnore]
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}