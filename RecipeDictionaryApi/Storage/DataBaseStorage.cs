using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Services;

namespace RecipeDictionaryApi.Storage;

public class DataBaseStorage(DataBaseContext context, PasswordHasher hasher) : IStorage
{
    public async Task<User?> LoginUser(UserDto user)
    {
        try
        {
            var dbUser = await context.Users
                .FirstOrDefaultAsync(u => u.Name == user.Name);

            if (dbUser != null && hasher.VerifyPassword(user.Password, dbUser.Password)) return dbUser;
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<User?> Register(UserDto user)
    {
        try
        {
            context.Users.Add(new User
            {
                Name = user.Name,
                Password = hasher.HashPassword(user.Password),
                Email = user.Email
            });
            await context.SaveChangesAsync();
            return (await context.Users.Where(u => u.Name == user.Name).FirstOrDefaultAsync())!;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> HasUser(string name)
    {
        try
        {
            return await context.Users.Where(u => u.Name == name).AnyAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HasEmail(string email)
    {
        try
        {
            return await context.Users.Where(u => u.Email == email).AnyAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> MakeAdmin(int id)
    {
        try
        {
            var user = await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return false;
            
            user.IsAdmin = true;
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<List<Dish>> GetDishesLikeName(string name)
    {
        try
        {
            return await context.DefaultDishes.Where(d => d.Name.Contains(name)).OrderBy(d => d.Name).ToListAsync();
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<Dish>> GetAllDishes()
    {
        try
        {
            return await context.DefaultDishes.OrderBy(d => d.Name).ToListAsync();
        }
        catch
        {
            return [];
        }
    }
    
    public async Task<bool> AddNewDish(NewDishDto dish)
    {
        try
        {
            context.DevDishes.Add(dish);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AcceptNewDish(int id)
    {
        try
        {
            var dish = await context.DevDishes
                .Where(d => d.Id == id)
                .Select(d => new Dish
                {
                    Name = d.Name,
                    Proteins = d.Proteins / d.Weight * 100,
                    Fats = d.Fats / d.Weight * 100,
                    Carbohydrates = d.Carbohydrates / d.Weight * 100,
                })
                .FirstAsync();
            
            context.DefaultDishes.Add(dish);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

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

