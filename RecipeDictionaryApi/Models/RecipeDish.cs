using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models
{
    public class RecipeDish
    {
        [Key]
        public int Id { get; set; }
        
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        
        public int DishId { get; set; }
        public Dish Dish { get; set; }
        
        public float Grams { get; set; }
    }
}