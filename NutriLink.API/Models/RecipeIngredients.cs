namespace NutriLink.API.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = default!;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = default!;

        // Extra fields
        public double Quantity { get; set; }
        public string Unit { get; set; } = "";
    }
}

