using Microsoft.EntityFrameworkCore;

namespace LevelUp.Microservice.Auth.Db;

public class Migrator
{
    private readonly ILogger<Migrator> _logger;
    private readonly AppDbContext _dbContext;

    public Migrator(ILogger<Migrator> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Migrating database...");
            
            await _dbContext.Database.MigrateAsync(cancellationToken);
            
            _logger.LogInformation("Migrating ended database...");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}