using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeDictionaryApi.Models;
using RecipeDictionaryApi.Storage;
using RecipeDictionaryApi.Services;

namespace RecipeDictionaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserStorage storage, JwtService jwtService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState); 

        var registeredUser = await storage.Register(user);
        if (registeredUser == null)
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to register user");
        var token = jwtService.GenerateToken(registeredUser.Id.ToString(), registeredUser.Name);
        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserDto user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var loggedInUser = await storage.LoginUser(user);
        if (loggedInUser == null) return Unauthorized("Invalid username or password");

        var token = jwtService.GenerateToken(
            loggedInUser.Id.ToString(),
            loggedInUser.Name,
            loggedInUser.IsAdmin);

        return Ok(new { Token = token });
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> HasUser(string name)
    {
        return Ok(await storage.HasUser(name));
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> HasEmail(string email)
    {
        return Ok(await storage.HasEmail(email));
    }
    

    [Authorize(Policy = "Admin")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> MakeAdmin(int id)
    {
        return await storage.MakeAdmin(id) ? Ok("User successfully made admin") : NotFound("User not found");
    }
}