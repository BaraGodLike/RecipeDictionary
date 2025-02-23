using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Storage;

namespace RecipeDictionaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController(IRecipeStorage storage) : ControllerBase
{
    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var recipe = await storage.GetRecipeById(id);
        return recipe != null ? Ok(recipe) : NotFound();
    }

    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddNewRecipe([FromBody] RecipeDto recipe)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await storage.AddNewRecipe(recipe);
        return result ? Ok() : StatusCode(StatusCodes.Status500InternalServerError, "Failed to add recipe");
    }

    [Authorize(Policy = "Admin")]
    [HttpPatch("accept/{id:int}")]
    public async Task<IActionResult> AcceptRecipe(int id)
    {
        return await storage.AcceptRecipe(id) ? Ok() : NotFound("Recipe not found or already accepted");
    }

    [HttpGet("get_with_filters")]
    public async Task<IActionResult> GetRecipesWithFilters(
        [FromQuery] List<int>? plusIds, 
        [FromQuery] List<int>? minusIds)
    {
        return Ok(await storage.GetRecipesWithFilters(plusIds ?? [], minusIds ?? []));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetRecipes()
    {
        return Ok(await storage.GetRecipes());
    }
}