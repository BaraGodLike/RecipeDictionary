using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public interface IUserStorage
{
    Task<User?> Register(UserDto user, CancellationToken cancellationToken);
    Task<User?> LoginUser(UserDto user, CancellationToken cancellationToken);
    Task<bool> HasUser(string name, CancellationToken cancellationToken);
    Task<bool> MakeAdmin(int id, CancellationToken cancellationToken);
}