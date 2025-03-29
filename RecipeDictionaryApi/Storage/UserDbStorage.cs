using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Services;

namespace RecipeDictionaryApi.Storage;

public class UserDbStorage(DataBaseContext context, PasswordHasher hasher) : IUserStorage
{
    public async Task<User?> LoginUser(UserDto user, CancellationToken cancellationToken)
    {
        try
        {
            var dbUser = await context.Users
                .FirstOrDefaultAsync(u => u.Name == user.Name, cancellationToken);

            if (dbUser != null && hasher.VerifyPassword(user.Password, dbUser.Password)) 
                return dbUser;
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<User?> Register(UserDto user, CancellationToken cancellationToken)
    {
        try
        {
            context.Users.Add(new User
            {
                Name = user.Name,
                Password = hasher.HashPassword(user.Password),
            });
            await context.SaveChangesAsync(cancellationToken);
            return (await context.Users
                .Where(u => u.Name == user.Name)
                .FirstOrDefaultAsync(cancellationToken))!;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> HasUser(string name, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Users
                .Where(u => u.Name == name)
                .AnyAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> MakeAdmin(int id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await context.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
                
            if (user == null) 
                return false;
            
            user.IsAdmin = true;
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}