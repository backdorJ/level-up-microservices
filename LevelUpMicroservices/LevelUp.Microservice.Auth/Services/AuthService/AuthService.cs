using System.Text.Json;
using LevelUp.Microservice.Auth.Db;
using LevelUp.Microservice.Auth.Db.Entities;
using LevelUp.Microservice.Auth.Exceptions;
using LevelUp.Microservice.Auth.Services.AuthService.Models;
using LevelUp.Microservice.Auth.Services.PasswordService;
using LevelUp.Microservice.Auth.Tools;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Microservice.Auth.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IPasswordService _passwordService;
    
    private readonly AppDbContext _dbContext;

    public AuthService(
        IPasswordService passwordService,
        ILogger<AuthService> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        GlobalValidator.ValidateEmail(request.Email);
        GlobalValidator.ValidatePasswordMatch(request.Password, request.ConfirmPassword);
        
        var hasUserWithSameEmail = await _dbContext.Users
            .AnyAsync(x => x.Email == request.Email, cancellationToken);
        
        if (hasUserWithSameEmail)
            throw new ValidateException(message: "Email is already taken", field: nameof(request.Email));

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password)
            };

            var message = new OutboxMessage
            {
                IsSent = false,
                Payload = JsonSerializer.Serialize(new
                {
                    EmailAddress = user.Email,
                    Message = Template.EmailMessage
                }),
            };
            
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}