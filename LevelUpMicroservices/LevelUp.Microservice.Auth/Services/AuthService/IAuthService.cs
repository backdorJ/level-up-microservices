using LevelUp.Microservice.Auth.Services.AuthService.Models;

namespace LevelUp.Microservice.Auth.Services.AuthService;

public interface IAuthService
{
    public Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);
}