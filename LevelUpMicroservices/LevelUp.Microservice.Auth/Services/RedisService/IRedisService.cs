namespace LevelUp.Microservice.Auth.Services.RedisService;

public interface IRedisService
{
    public Task SetAsync(string key, string value, TimeSpan? expiry = null);
    public Task<string?> GetAsync(string key);
    
    public Task DeleteAsync(string key);
    
    public Task<bool> ExistsAsync(string key);
}