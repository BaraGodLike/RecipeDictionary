using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IStorage
{
    Task<User?> LoginUser(UserDto user);
    Task<User?> Register(UserDto user);
    Task<bool> HasUser(string name);
    Task<bool> HasEmail(string email);
    Task<bool> MakeAdmin(int id);
    Task<List<Dish>> GetDishesLikeName(string name);
    Task<List<Dish>> GetAllDishes();
    Task<bool> AddNewDish(NewDishDto dish);
    Task<bool> AcceptNewDish(int id);
    Task<bool> AddNewRecipe(RecipeDto recipeDto);
    Task<bool> AcceptRecipe(int id);
    Task<RecipeDto?> GetRecipeById(int id);
    Task<List<RecipeDto>> GetRecipes();
    Task<List<RecipeDto>> GetRecipesWithFilters(List<int> plusIds, List<int> minusIds);
}