using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeDictionaryApi.Services;

public class JwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryInMinutes;

    public JwtService(IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        _secretKey = jwtSettings["SecretKey"]!;
        _issuer = jwtSettings["Issuer"]!;
        _audience = jwtSettings["Audience"]!;
        _expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]!);
    }

    public string GenerateToken(string userId, string username, bool isAdmin = false)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        if (isAdmin)
        {
            claims.Add(new Claim("Admin", "true"));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}