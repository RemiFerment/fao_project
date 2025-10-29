using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Recipe> Recipes => Set<Recipe>();
}
