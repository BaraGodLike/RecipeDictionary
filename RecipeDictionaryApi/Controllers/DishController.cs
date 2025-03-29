using System.ComponentModel.DataAnnotations;
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
    public async Task<IActionResult> GetDishesLikeName(string name, CancellationToken cancellationToken)
    {
        var dishes = await storage.GetDishesLikeName(name, cancellationToken);
        return Ok(dishes); 
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddNewDish([FromBody] NewDishDto dish, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await storage.AddNewDish(dish, cancellationToken);
        return result 
            ? Ok("Dish added successfully") 
            : StatusCode(StatusCodes.Status500InternalServerError, "Failed to add dish");
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id:int}")]
    public async Task<IActionResult> AcceptNewDish(int id, CancellationToken cancellationToken)
    {
        var result = await storage.AcceptNewDish(id, cancellationToken);
        return result 
            ? Ok("Dish accepted successfully") 
            : NotFound("Dish not found or already accepted");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllDishes(CancellationToken cancellationToken)
    {
        return Ok(await storage.GetAllDishes(cancellationToken));
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("dev_dish")]
    public async Task<IActionResult> DeleteDevDish([FromQuery] [Required] int dishId, CancellationToken cancellationToken)
    {
        var result = await storage.DeleteFromDevDishes(dishId, cancellationToken);
        return result == null ? NotFound("The dish with this id does not exist") : Ok(result);
    }
}