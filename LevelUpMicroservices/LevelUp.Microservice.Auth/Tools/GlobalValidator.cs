using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LevelUp.Microservice.Auth.Tools;

public static class GlobalValidator
{
    private static Regex _regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public static void ValidateEmail(string email)
    {
        if (!_regex.IsMatch(email))
            throw new ValidationException("Email");
    }

    public static void ValidatePasswordMatch(string password, string confirmPassword)
    {
        if (password != confirmPassword)
            throw new ValidationException("Password or Email");
    }
}