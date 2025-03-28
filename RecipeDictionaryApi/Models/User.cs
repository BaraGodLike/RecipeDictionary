using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecipeDictionaryApi.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Password { get; set; } 
    [JsonIgnore]
    [NotMapped]
    public List<Recipe> Recipes { get; set; }
    public bool IsAdmin { get; set; }
}
