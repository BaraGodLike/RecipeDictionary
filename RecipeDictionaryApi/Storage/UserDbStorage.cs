using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Services;

namespace RecipeDictionaryApi.Storage;

public class UserDbStorage(DataBaseContext context, PasswordHasher hasher) : IUserStorage
{
    public async Task<User?> LoginUser(UserDto user)
    {
        try
        {
            var dbUser = await context.Users
                .FirstOrDefaultAsync(u => u.Name == user.Name);

            if (dbUser != null && hasher.VerifyPassword(user.Password, dbUser.Password)) return dbUser;
            
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
                Password = hasher.HashPassword(user.Password),
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
        try
        {
            return await context.Users.Where(u => u.Name == name).AnyAsync();
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> MakeAdmin(int id)
    {
        try
        {
            var user = await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return false;
            
            user.IsAdmin = true;
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}