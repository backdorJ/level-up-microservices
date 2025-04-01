namespace LevelUp.Microservice.Auth.Services.JwtGeneratorService.Models;

public class GenerateJwtTokenRequest
{
    public string Email { get; set; }
    public string Username { get; set; }
    public Guid Id { get; set; }
}