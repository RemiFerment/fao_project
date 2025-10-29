using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;


var builder = WebApplication.CreateBuilder(args);

// 🔹 Injection du DbContext avec la chaîne de connexion
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/test/connection", async (AppDbContext db) =>
{
    bool canConnect = await db.Database.CanConnectAsync();
    return Results.Ok(new { databaseConnected = canConnect });
});

//CRUD Recipe model
app.MapGet("/api/recipes", async (AppDbContext db) => await db.Recipes.ToListAsync());

app.MapGet("/api/recipes/{id}", async (int id, AppDbContext db) =>
    await db.Recipes.FindAsync(id) is Recipe recipe
        ? Results.Ok(recipe)
        : Results.NotFound());

app.MapPost("/api/recipes", async (Recipe recipe, AppDbContext db) =>
{
    db.Recipes.Add(recipe);
    await db.SaveChangesAsync();
    return Results.Created($"/api/recipes/{recipe.Id}", recipe);
}
);
app.Run();
