using AccuPunchure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuPunchure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Punch> Punches { get; set; }
    public DbSet<Organization> Organizations { get; set; }

}