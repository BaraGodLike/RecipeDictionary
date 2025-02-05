using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class Dish
{
    [Key] 
    public int Id { get; set; }

    public string Name { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbohydrates { get; set; }
}