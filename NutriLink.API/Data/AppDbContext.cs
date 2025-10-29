using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Recipe>()
        .HasOne(r => r.Category)
        .WithMany()
        .HasForeignKey(r => r.CategoryId);
}
}
