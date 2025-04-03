namespace LevelUp.Microservice.Auth.Services.PasswordService;

public interface IPasswordService
{
    public string HashPassword(string password);
}