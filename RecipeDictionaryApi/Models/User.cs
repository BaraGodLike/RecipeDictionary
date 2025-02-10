using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Password { get; set; } 
    public int Proteins { get; set; }
    public int Fats { get; set; }
    public int Carbohydrates { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
}
