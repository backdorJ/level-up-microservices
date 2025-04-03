using System.Reflection;
using LevelUp.Microservice.Auth.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Microservice.Auth.Db;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    private AppDbContext()
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}