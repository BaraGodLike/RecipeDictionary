using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Storage;

namespace RecipeDictionaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DishController(IStorage storage) : ControllerBase
{
    [Authorize]
    [HttpGet("like:{name}")]
    public async Task<IActionResult> GetDishesLikeName(string name)
    {
        var dish = await storage.GetDishesLikeName(name);
        return dish.Count == 0 ? Ok("Dishes not found") : Ok(dish);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddNewDish(NewDishDto dish)
    {
        var result = await storage.AddNewDish(dish);
        return Ok(result ? "Added success" : "Oops..");
    }
}