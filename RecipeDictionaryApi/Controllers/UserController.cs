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
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserDto user,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState); 

        var registeredUser = await storage.Register(user, cancellationToken);
        if (registeredUser == null)
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to register user");
        
        var token = jwtService.GenerateToken(registeredUser.Id.ToString(), registeredUser.Name);
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true, 
            Secure = false, // ПОМЕНЯТЬ 
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(1)
        });
        
        return Ok(new { Message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] UserDto user,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var loggedInUser = await storage.LoginUser(user, cancellationToken);
        if (loggedInUser == null) 
            return Unauthorized(new { Message = "Invalid username or password"});

        var token = jwtService.GenerateToken(
            loggedInUser.Id.ToString(),
            loggedInUser.Name,
            loggedInUser.IsAdmin);
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // ПОМЕНЯТЬ
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(1)
        });

        return Ok(new { Message = "Logged in successfully" });
    }
    
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> HasUser(
        string name,
        CancellationToken cancellationToken)
    {
        return Ok(await storage.HasUser(name, cancellationToken));
    }
    
    [Authorize(Policy = "Admin")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> MakeAdmin(
        int id,
        CancellationToken cancellationToken)
    {
        return await storage.MakeAdmin(id, cancellationToken) 
            ? Ok(new { Message = "User successfully made admin"}) 
            : NotFound(new { Message = "User not found"});
    }
}