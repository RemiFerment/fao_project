using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;


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

app.Run();
