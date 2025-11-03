using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
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

app.Run();
