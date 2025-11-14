namespace Fao.Front_End.Models
{
    public class FullRecipeDTO
    {
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Steps { get; set; } = string.Empty;
        public List<IngredientDTO> Ingredients { get; set; } = new List<IngredientDTO>();
    }
}