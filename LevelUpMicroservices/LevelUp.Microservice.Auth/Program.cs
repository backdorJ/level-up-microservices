using System.Threading.RateLimiting;
using LevelUp.Microservice.Auth.Db;
using LevelUp.Microservice.Auth.Middlewares;
using LevelUp.Microservice.Auth.Options;
using LevelUp.Microservice.Auth.Services.AuthService;
using LevelUp.Microservice.Auth.Services.JwtGeneratorService;
using LevelUp.Microservice.Auth.Services.PasswordService;
using LevelUp.Microservice.Auth.Services.RedisService;
using LevelUp.Microservice.Auth.Tools;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddTransient<IdempotencyMiddleware>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtGeneratorService, JwtGeneratorService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration["RedisConnection"]!;
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddTransient<Migrator>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"error\": \"Too many requests. Please try again later.\"}",
            token);
    };
    
    options.AddFixedWindowLimiter(PolicyNameConstants.FixedWindow, config =>
    {
        config.PermitLimit = 1000;
        config.Window = TimeSpan.FromMinutes(60);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var migrator = services.GetRequiredService<Migrator>();
await migrator.MigrateAsync(CancellationToken.None);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter(); 

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();