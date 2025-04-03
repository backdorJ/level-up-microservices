using System.Text.Json;
using LevelUp.Microservice.Auth.Exceptions;

namespace LevelUp.Microservice.Auth.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await ErrorHandlingAsync(context, e);
            _logger.LogCritical(e.Message);
        }
    }

    private async Task ErrorHandlingAsync(HttpContext context, Exception exception)
    {
        var (message, statusCode) = exception switch
        {
            ValidateException => (exception.Message, StatusCodes.Status400BadRequest),
            _ => (exception.Message, StatusCodes.Status500InternalServerError)
        };

        var result = JsonSerializer.Serialize(new
        {
            ErrorMessage = message,
        });
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        await context.Response.WriteAsync(result);
    }
}