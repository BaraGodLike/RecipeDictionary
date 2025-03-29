using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IDishStorage
{
    Task<List<Dish>> GetDishesLikeName(string name, CancellationToken cancellationToken);
    Task<List<Dish>> GetAllDishes(CancellationToken cancellationToken);
    Task<bool> AddNewDish(NewDishDto dish, CancellationToken cancellationToken);
    Task<bool> AcceptNewDish(int id, CancellationToken cancellationToken);
}