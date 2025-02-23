using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IRecipeStorage
{
    Task<bool> AddNewRecipe(RecipeDto recipeDto);
    Task<bool> AcceptRecipe(int id);
    Task<RecipeDto?> GetRecipeById(int id);
    Task<List<RecipeDto>> GetRecipes();
    Task<List<RecipeDto>> GetRecipesWithFilters(List<int> plusIds, List<int> minusIds);
}