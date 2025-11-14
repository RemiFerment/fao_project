namespace Fao.Front_End.Models
{
    public class IngredientDTO
    {
        public string IngredientName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}