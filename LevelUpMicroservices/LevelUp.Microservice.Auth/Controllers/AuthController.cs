using LevelUp.Microservice.Auth.Middlewares;
using LevelUp.Microservice.Auth.Services.AuthService;
using LevelUp.Microservice.Auth.Services.AuthService.Models;
using LevelUp.Microservice.Auth.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LevelUp.Microservice.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(PolicyNameConstants.FixedWindow)]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(statusCode: 400, type: typeof(ValidationProblemDetails))]
    [ProducesResponseType(statusCode: 200)]
    public async Task Register(RegisterRequest registerRequest, CancellationToken cancellationToken)
        => await _authService.RegisterAsync(registerRequest, cancellationToken);
}