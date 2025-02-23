using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class RecipeDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Photo is required")]
    public byte[] Photo { get; set; }

    [Required(ErrorMessage = "Text is required")]
    public string Text { get; set; }

    [Required(ErrorMessage = "Author ID is required")]
    public int IdAuthor { get; set; }

    [Required(ErrorMessage = "Dishes are required")]
    public List<NewDishDto> Dishes { get; set; }
}