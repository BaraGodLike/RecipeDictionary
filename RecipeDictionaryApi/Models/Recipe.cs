using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class Recipe
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Photo { get; set; }
    public string Text { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbohydrates { get; set; }
    public List<RecipeDish> RecipeDishes { get; set; }
    public int IdAuthor { get; set; }
    public User User { get; set; }
    public bool IsConfirmed { get; set; }
}