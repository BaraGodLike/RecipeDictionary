using System.ComponentModel.DataAnnotations;

namespace RecipeDictionaryApi.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Password { get; set; }    
}