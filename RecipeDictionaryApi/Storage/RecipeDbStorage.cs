using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public class RecipeDbStorage(DataBaseContext context) : IRecipeStorage
{
    public async Task<bool> AddNewRecipe(RecipeDto recipeDto)
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
            return await context.SaveChangesAsync() > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AcceptRecipe(int id)
    {
        try
        {
            return await context.Recipes
                .Where(r => r.Id == id)
                .ExecuteUpdateAsync(setters => 
                    setters.SetProperty(r => r.IsConfirmed, true)) > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<RecipeDto?> GetRecipeById(int id)
    {
        try
        {
            return await context.Recipes
                .Where(r => r.Id == id)
                .Select(r => new RecipeDto
                {
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
                .FirstOrDefaultAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<RecipeDto>> GetRecipes()
    {
        try
        {
            return await context.Recipes
                .Select(r => new RecipeDto
                {
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
                .ToListAsync();
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<RecipeDto>> GetRecipesWithFilters(List<int> plusIds, List<int> minusIds)
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
                .ToListAsync();
        }
        catch
        {
            return [];
        }
    }
}