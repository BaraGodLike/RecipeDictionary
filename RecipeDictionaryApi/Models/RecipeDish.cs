using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecipeDictionaryApi.Models
{
    public class RecipeDish
    {
        [Key]
        public int Id { get; set; }
        public int RecipeId { get; set; }
        [JsonIgnore]
        [NotMapped]
        public Recipe Recipe { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; }
        
        public float Grams { get; set; }
    }
}