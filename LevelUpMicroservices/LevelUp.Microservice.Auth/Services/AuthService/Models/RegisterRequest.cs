namespace LevelUp.Microservice.Auth.Services.AuthService.Models;

public class RegisterRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}