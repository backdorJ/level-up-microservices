namespace LevelUp.Microservice.Auth.Services.PasswordService;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);
}