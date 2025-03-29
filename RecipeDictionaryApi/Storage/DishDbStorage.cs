using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public class DishDbStorage(DataBaseContext context) : IDishStorage
{
    public async Task<List<Dish>> GetDishesLikeName(string name, CancellationToken cancellationToken)
    {
        try
        {
            return await context.DefaultDishes
                .Where(d => d.Name.Contains(name))
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<Dish>> GetAllDishes(CancellationToken cancellationToken)
    {
        try
        {
            return await context.DefaultDishes
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch
        {
            return [];
        }
    }
    
    public async Task<bool> AddNewDish(NewDishDto dish, CancellationToken cancellationToken)
    {
        try
        {
            context.DevDishes.Add(dish);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AcceptNewDish(int id, CancellationToken cancellationToken)
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
                .FirstAsync(cancellationToken);
            
            context.DefaultDishes.Add(dish);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}