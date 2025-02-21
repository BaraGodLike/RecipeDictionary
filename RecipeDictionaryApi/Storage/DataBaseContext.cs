using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public class DataBaseContext(DbContextOptions<DataBaseContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Dish> DefaultDishes { get; set; }
    public DbSet<NewDishDto> DevDishes { get; set; } 
    public DbSet<RecipeDish> RecipeDishes { get; set; }
    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Dish>().HasKey(d => d.Id);
        modelBuilder.Entity<NewDishDto>().HasKey(d => d.Id);
        modelBuilder.Entity<RecipeDish>()
            .HasKey(rd => rd.Id);
        
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.User) 
            .WithMany(u => u.Recipes) 
            .HasForeignKey(r => r.IdAuthor);

        modelBuilder.Entity<RecipeDish>()
            .HasOne(rd => rd.Recipe)
            .WithMany(r => r.RecipeDishes)
            .HasForeignKey(rd => rd.RecipeId);
        
    }
}