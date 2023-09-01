using Microsoft.EntityFrameworkCore;
using TaskMagnet.Core.Domain.Entities;

namespace TaskMagnet.Infrastructure.Database;

public class TaskMagnetDBContext : DbContext
{
    public TaskMagnetDBContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskMagnetDBContext).Assembly);
    }
}
