using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Storage;

namespace RecipeDictionaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DishController(IDishStorage storage) : ControllerBase
{
    [Authorize]
    [HttpGet("like/{name}")]
    public async Task<IActionResult> GetDishesLikeName(string name)
    {
        var dishes = await storage.GetDishesLikeName(name);
        return Ok(dishes); 
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddNewDish([FromBody] NewDishDto dish)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await storage.AddNewDish(dish);
        return result 
            ? Ok("Dish added successfully") 
            : StatusCode(StatusCodes.Status500InternalServerError, "Failed to add dish");
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id:int}")]
    public async Task<IActionResult> AcceptNewDish(int id)
    {
        var result = await storage.AcceptNewDish(id);
        return result 
            ? Ok("Dish accepted successfully") 
            : NotFound("Dish not found or already accepted");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetALlDishes()
    {
        return Ok(await storage.GetAllDishes());
    }
}