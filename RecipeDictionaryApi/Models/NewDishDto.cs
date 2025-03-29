using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class NewDishDto
{
    [Key] 
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [Length(2, 30, ErrorMessage = "The length of the name can range from 2 to 30")]
    [RegularExpression(@"^[a-zA-Z0-9а-яА-Я ]+$", ErrorMessage = "Name can only contain letters, numbers, and spaces")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Proteins is required")]
    public float Proteins { get; set; }
    
    [Required(ErrorMessage = "Fats is required")]
    public float Fats { get; set; }
    
    [Required(ErrorMessage = "Carbohydrates is required")]
    public float Carbohydrates { get; set; }
    
    [Range(0.01, 5000, ErrorMessage = "Weight must be between 0.01 and 5000")]
    public float Weight { get; set; } = 100;
}