using LevelUp.Microservice.Auth.Services.RedisService;

namespace LevelUp.Microservice.Auth.Middlewares;

public class IdempotencyMiddleware : IMiddleware
{
    private readonly IRedisService _redisService;

    public IdempotencyMiddleware(IRedisService redisService)
    {
        _redisService = redisService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await _redisService.DeleteAsync("");
        if (context.Request.Method != HttpMethods.Post ||
            !context.Request.Headers.TryGetValue("X-Idempotency-Key", out var idempotencyKey))
        {
            await next(context);
            return;
        }

        var cachedResponse = await _redisService.GetAsync(idempotencyKey!);
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(cachedResponse);
            return;
        }
        
        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        
        var message = responseBody == string.Empty ? "No response" : responseBody;
        
        await _redisService.SetAsync(idempotencyKey!, message, TimeSpan.FromHours(2));

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBodyStream);
    }
}