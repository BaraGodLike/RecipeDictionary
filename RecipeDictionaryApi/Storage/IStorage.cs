using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IStorage
{
    Task<User?> LoginUser(UserDto user);
    Task<User?> Register(UserDto user);
    Task<bool> HasUser(string name);
}