using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class MealDay
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbohydrates { get; set; }
    public DateOnly Date { get; set; }
    
    public User User { get; set; }
}