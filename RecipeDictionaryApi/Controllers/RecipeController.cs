using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeDictionaryApi.Storage;

namespace RecipeDictionaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController(IStorage storage) : ControllerBase
{
    [Authorize]
    [HttpGet("get:{id:int}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var recipe = await storage.GetRecipeById(id);
        return recipe == null ? Ok(recipe) : NotFound();
    }
}