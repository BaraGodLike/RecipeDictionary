using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public class DishDbStorage(DataBaseContext context) : IDishStorage
{
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

    
}