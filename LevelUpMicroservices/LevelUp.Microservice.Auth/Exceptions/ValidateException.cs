namespace LevelUp.Microservice.Auth.Exceptions;

public class ValidateException : Exception
{
    public ValidateException(string field)
        : base($"Проверьте корректность ввода поля: {field}")
    {
    }
    
    public ValidateException(string message, string field)
        : base(string.Format(message, field))
    {}
}