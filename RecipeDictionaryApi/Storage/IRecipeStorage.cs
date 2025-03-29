using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IRecipeStorage
{
    Task<bool> AddNewRecipe(RecipeDto recipeDto, CancellationToken cancellationToken);
    Task<bool> AcceptRecipe(int id, CancellationToken cancellationToken);
    Task<RecipeDto?> GetRecipeById(int id, CancellationToken cancellationToken);
    Task<List<RecipeDto>> GetRecipes(CancellationToken cancellationToken);
    Task<List<RecipeDto>> GetRecipesWithFilters(List<int> plusIds, List<int> minusIds, CancellationToken cancellationToken);
    Task<bool> PatchRecipe(RecipeDto recipe, CancellationToken cancellationToken);
}