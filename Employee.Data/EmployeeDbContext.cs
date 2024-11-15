using Employee.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Employee.Data;

public class EmployeeDbContext : DbContext
{ 
    public DbSet<EmployeeEntity> Employers { get; set; } = default!;
    
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    }
}