using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public class RecipeDbStorage(DataBaseContext context) : IRecipeStorage
{
    public async Task<bool> AddNewRecipe(RecipeDto recipeDto, CancellationToken cancellationToken)
    {
        try
        {
            context.Recipes.Add(new Recipe
            {
                Name = recipeDto.Name,
                Photo = recipeDto.Photo,
                Text = recipeDto.Text,
                IdAuthor = recipeDto.IdAuthor,
                IsConfirmed = false,
                RecipeDishes = recipeDto.Dishes.Select(dish => new RecipeDish
                {
                    DishId = dish.Id,
                    Grams = dish.Weight
                }).ToList()
            });
            return await context.SaveChangesAsync(cancellationToken) > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AcceptRecipe(int id, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Recipes
                .Where(r => r.Id == id)
                .ExecuteUpdateAsync(setters => 
                    setters.SetProperty(r => r.IsConfirmed, true),
                    cancellationToken) > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<RecipeDto?> GetRecipeById(int id, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Recipes
                .Where(r => r.Id == id)
                .Select(r => new RecipeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Photo = r.Photo,
                    Text = r.Text,
                    IdAuthor = r.IdAuthor,
                    Dishes = r.RecipeDishes.Select(rd => new NewDishDto
                    {
                        Id = rd.Dish.Id,
                        Name = rd.Dish.Name,
                        Proteins = rd.Dish.Proteins,
                        Fats = rd.Dish.Fats,
                        Carbohydrates = rd.Dish.Carbohydrates,
                        Weight = rd.Grams
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<RecipeDto>> GetRecipes(CancellationToken cancellationToken)
    {
        try
        {
            return await context.Recipes
                .Select(r => new RecipeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Photo = r.Photo,
                    Text = r.Text,
                    IdAuthor = r.IdAuthor,
                    Dishes = r.RecipeDishes.Select(rd => new NewDishDto
                    {
                        Id = rd.Dish.Id,
                        Name = rd.Dish.Name,
                        Proteins = rd.Dish.Proteins,
                        Fats = rd.Dish.Fats,
                        Carbohydrates = rd.Dish.Carbohydrates,
                        Weight = rd.Grams
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<RecipeDto>> GetRecipesWithFilters(List<int> plusIds, List<int> minusIds, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Recipes
                .Where(r =>
                    plusIds.All(plusId => r.RecipeDishes.Any(rd => rd.DishId == plusId)) &&
                    !minusIds.Any(minusId => r.RecipeDishes.Any(rd => rd.DishId == minusId))
                )
                .Select(r => new RecipeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Photo = r.Photo,
                    Text = r.Text,
                    IdAuthor = r.IdAuthor,
                    Dishes = r.RecipeDishes.Select(rd => new NewDishDto
                    {
                        Id = rd.Dish.Id,
                        Name = rd.Dish.Name,
                        Proteins = rd.Dish.Proteins,
                        Fats = rd.Dish.Fats,
                        Carbohydrates = rd.Dish.Carbohydrates,
                        Weight = rd.Grams
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
        catch
        {
            return [];
        }
    }

    public async Task<bool> PatchRecipe(RecipeDto recipe, CancellationToken cancellationToken)
    {
        try
        {
            var oldRecipe = await context.Recipes
                .Include(r => r.RecipeDishes)
                .FirstOrDefaultAsync(r => r.Id == recipe.Id, cancellationToken);

            if (oldRecipe == null)
            {
                return false;
            }
            
            oldRecipe.Name = recipe.Name;
            oldRecipe.Photo = recipe.Photo;
            oldRecipe.Text = recipe.Text;
            oldRecipe.IdAuthor = recipe.IdAuthor;
            
            context.RecipeDishes.RemoveRange(oldRecipe.RecipeDishes);
            
            oldRecipe.RecipeDishes = recipe.Dishes.Select(dish => new RecipeDish
            {
                DishId = dish.Id,
                Grams = dish.Weight
            }).ToList();
            
            return await context.SaveChangesAsync(cancellationToken) > 0;
        }
        catch
        {
            return false;
        }
    }
}