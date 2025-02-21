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
            if (hasher.VerifyPassword(user.Password,
                    (await context.Users.Where(u => u.Name == user.Name)
                        .Select(u => u.Password).FirstOrDefaultAsync())!))
            {
                return (await context.Users.Where(u => u.Name == user.Name).FirstOrDefaultAsync())!;
            }

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

    public async Task<bool> AddNewRecipe(Recipe recipeDto)
    {
        try
        {
            context.Recipes.Add(recipeDto);
            await context.SaveChangesAsync();
            return true;
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
            var rowsAffected = await context.Recipes
                .Where(r => r.Id == id)
                .ExecuteUpdateAsync(setters => 
                    setters.SetProperty(r => r.IsConfirmed, true));
            return rowsAffected > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Recipe?> GetRecipeById(int id)
    {
        try
        {
            return await context.Recipes
                .Include(r => r.RecipeDishes)
                .ThenInclude(rd => rd.Dish)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        catch
        {
            return null;
        }
    }
    
}
