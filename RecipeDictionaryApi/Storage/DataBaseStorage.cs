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

    public async Task<List<Dish>> GetDishesLikeName(string name)
    {
        return await context.DefaultDishes.Where(d => d.Name.Contains(name)).OrderBy(d => d.Name).ToListAsync();
    }

    public async Task<bool> AddNewDish(NewDishDto dish)
    {
        try
        {
            context.DefaultDishes.Add(new Dish
            {
                Name = dish.Name,
                Proteins = dish.Proteins / dish.Weight * 100,
                Fats = dish.Fats / dish.Weight * 100,
                Carbohydrates = dish.Carbohydrates / dish.Weight * 100,
            });
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
