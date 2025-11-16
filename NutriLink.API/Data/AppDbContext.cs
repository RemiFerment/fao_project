namespace NutriLink.API.Data;

using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<AchievementType> AchievementTypes => Set<AchievementType>();
    public DbSet<AchievementTypeMeasurement> AchievementTypeMeasurements => Set<AchievementTypeMeasurement>();
    public DbSet<AchievementTypePhoto> AchievementTypePhotos => Set<AchievementTypePhoto>();
    public DbSet<AchievementTypeWeight> AchievementTypeWeights => Set<AchievementTypeWeight>();
    public DbSet<Achievement> Achievements => Set<Achievement>();

    public DbSet<MealDay> MealDays => Set<MealDay>();
    public DbSet<SnackDay> SnackDays => Set<SnackDay>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserProfile)
            .WithMany()
            .HasForeignKey(u => u.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MealDay>()
            .HasOne(md => md.User)
            .WithMany()
            .HasForeignKey(md => md.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<MealDay>()
            .HasOne(md => md.Coach)
            .WithMany()
            .HasForeignKey(md => md.CoachId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<MealDay>()
            .HasOne(md => md.Breakfast)
            .WithMany()
            .HasForeignKey(md => md.BreakfastId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<MealDay>()
            .HasOne(md => md.Lunch)
            .WithMany()
            .HasForeignKey(md => md.LunchId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<MealDay>()
            .HasOne(md => md.Dinner)
            .WithMany()
            .HasForeignKey(md => md.DinnerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SnackDay>()
            .HasOne(sd => sd.User)
            .WithMany()
            .HasForeignKey(sd => sd.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SnackDay>()
            .HasOne(sd => sd.Snack)
            .WithMany()
            .HasForeignKey(sd => sd.SnackId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Achievement>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Achievement>()
            .HasOne(a => a.AchievementType)
            .WithOne(t => t.Achievement)
            .HasForeignKey<Achievement>(a => a.AchievementTypeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<AchievementType>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<AchievementTypeMeasurement>("measurement")
            .HasValue<AchievementTypePhoto>("photo")
            .HasValue<AchievementTypeWeight>("weight");
    }
}
