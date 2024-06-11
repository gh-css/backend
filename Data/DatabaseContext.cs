using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    
    public DbSet<GitHubUser> GitHubUsers { get; set; }
    public DbSet<Report> Reports { get; set; }
}