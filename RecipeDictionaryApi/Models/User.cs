using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Password { get; set; } 
    public string Email { get; set; }
    public List<Recipe> Recipes { get; set; }
    public bool IsAdmin { get; set; }
}
