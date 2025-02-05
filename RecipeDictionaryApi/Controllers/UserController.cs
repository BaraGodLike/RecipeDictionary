using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Storage;
using RecipeDictionaryApi.Services;

namespace RecipeDictionaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IStorage storage, JwtService jwtService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserDto user)
    {
        var registeredUser = await storage.Register(user);
        if (registeredUser == null)
        {
            return NotFound("Oops...");
        }
        var token = jwtService.GenerateToken(registeredUser.Id.ToString(), registeredUser.Name);
        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(UserDto user)
    {
        var loggedInUser = await storage.LoginUser(user);
        if (loggedInUser == null)
        {
            return Ok(false);
        }
        var token = jwtService.GenerateToken(loggedInUser.Id.ToString(), loggedInUser.Name);
        return Ok(new { Token = token });
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> HasUser(string name)
    {
        return Ok(await storage.HasUser(name));
    }
}