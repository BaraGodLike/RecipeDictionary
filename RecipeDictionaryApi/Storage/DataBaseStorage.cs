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
                Password = hasher.HashPassword(user.Password)
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
        return await context.Users.Where(u => u.Name == name).AnyAsync();
    }

    public async Task<bool> PutValuesInUser(UserDto user)
    {
        // await context.Users.Where(u => u.Name == user.Name).SelectMany(u => u.)
        return true;
    }

    public async Task<List<Dish>> GetDishesLikeName(string name)
    {
        return await context.DefaultDishes.Where(d => d.Name.Contains(name)).OrderBy(d => d.Name).ToListAsync();
    }

    public async Task<List<Dish>> GetAllDishes()
    {
        return await context.DefaultDishes.OrderBy(d => d.Name).ToListAsync();
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
    
}
