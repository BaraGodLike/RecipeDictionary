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
    public async Task<IActionResult> GetRecipeById(int id, CancellationToken cancellationToken)
    {
        var recipe = await storage.GetRecipeById(id, cancellationToken);
        return recipe != null ? Ok(recipe) : NotFound();
    }

    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddNewRecipe([FromBody] RecipeDto recipe, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await storage.AddNewRecipe(recipe, cancellationToken);
        return result ? Ok() : StatusCode(StatusCodes.Status500InternalServerError, "Failed to add recipe");
    }

    [Authorize(Policy = "Admin")]
    [HttpPatch("accept/{id:int}")]
    public async Task<IActionResult> AcceptRecipe(int id, CancellationToken cancellationToken)
    {
        return await storage.AcceptRecipe(id, cancellationToken) ? Ok() : NotFound("Recipe not found or already accepted");
    }

    [HttpGet("get_with_filters")]
    public async Task<IActionResult> GetRecipesWithFilters(
        [FromQuery] List<int>? plusIds, 
        [FromQuery] List<int>? minusIds, 
        CancellationToken cancellationToken)
    {
        return Ok(await storage.GetRecipesWithFilters(plusIds ?? [], minusIds ?? [], cancellationToken));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetRecipes(CancellationToken cancellationToken)
    {
        return Ok(await storage.GetRecipes(cancellationToken));
    }
}