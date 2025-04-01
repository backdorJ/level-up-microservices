using LevelUp.Microservice.Auth.Services.JwtGeneratorService.Models;

namespace LevelUp.Microservice.Auth.Services.JwtGeneratorService;

public interface IJwtGeneratorService
{
    public string GenerateJwtToken(GenerateJwtTokenRequest request);
}