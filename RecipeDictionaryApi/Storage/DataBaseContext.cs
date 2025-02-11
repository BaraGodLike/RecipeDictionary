using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Models;

namespace RecipeDictionaryApi.Storage;

public class DataBaseContext(DbContextOptions<DataBaseContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Dish> DefaultDishes { get; set; }
    public DbSet<NewDishDto> DevDishes { get; set; } 
    public DbSet<MealDay> MealDays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Dish>().HasKey(d => d.Id);
        modelBuilder.Entity<NewDishDto>().HasKey(d => d.Id);
        modelBuilder.Entity<MealDay>().HasKey(day => day.Id);
        
        modelBuilder.Entity<MealDay>()
            .HasOne(day => day.User)
            .WithMany()
            .HasForeignKey(day => day.UserId);
    }
}