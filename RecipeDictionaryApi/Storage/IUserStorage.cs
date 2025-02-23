using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IUserStorage
{
    Task<User?> LoginUser(UserDto user);
    Task<User?> Register(UserDto user);
    Task<bool> HasUser(string name);
    Task<bool> HasEmail(string email);
    Task<bool> MakeAdmin(int id);    
}