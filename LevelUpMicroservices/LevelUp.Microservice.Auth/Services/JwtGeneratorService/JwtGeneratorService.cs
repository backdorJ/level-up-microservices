using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LevelUp.Microservice.Auth.Options;
using LevelUp.Microservice.Auth.Services.JwtGeneratorService.Models;
using Microsoft.IdentityModel.Tokens;

namespace LevelUp.Microservice.Auth.Services.JwtGeneratorService;

public class JwtGeneratorService : IJwtGeneratorService
{
    private readonly JwtOptions _jwtOptions;

    public JwtGeneratorService(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    public string GenerateJwtToken(GenerateJwtTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Email, request.Email),
            new(ClaimTypes.Name, request.Username),
            new(ClaimTypes.NameIdentifier, request.Id.ToString())
        };
        
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}