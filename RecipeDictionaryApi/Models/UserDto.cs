using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class UserDto
{
    [Required(ErrorMessage = "Name is required")]
    [Length(2, 20, ErrorMessage = "The length of the name can range from 2 to 20")]
    [RegularExpression(@"^[a-zA-Z0-9 _]+$", ErrorMessage = "Name can only contain letters, numbers, and spaces")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}