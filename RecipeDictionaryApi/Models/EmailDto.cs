using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class EmailDto
{
    [EmailAddress]
    public string Email { get; set; }
}