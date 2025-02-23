using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IDishStorage
{
    Task<List<Dish>> GetDishesLikeName(string name);
    Task<List<Dish>> GetAllDishes();
    Task<bool> AddNewDish(NewDishDto dish);
    Task<bool> AcceptNewDish(int id);
}