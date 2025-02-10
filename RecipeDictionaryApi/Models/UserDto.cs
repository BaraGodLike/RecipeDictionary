namespace RecipeDictionaryApi.Models;

public class UserDto
{
    public string Name { get; set; }
    public string Password { get; set; }
    public int Proteins { get; set; }
    public int Fats { get; set; }
    public int Carbohydrates { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
}