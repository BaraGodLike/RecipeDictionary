using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class NewDishDto
{
    [Key] 
    public int Id { get; set; }

    public string Name { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbohydrates { get; set; }
    public float Weight { get; set; } = 100;
}