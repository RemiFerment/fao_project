using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NutriLink.API.Data;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using NutriLink.API.Models;
using Microsoft.AspNetCore.Authorization;
using NutriLink.API.Services;

var builder = WebApplication.CreateBuilder(args);

var token = builder.Configuration["AppSettings:Token"] ?? throw new InvalidOperationException("AppSettings:Token is not configured.");
var key = Encoding.UTF8.GetBytes(token);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };

});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NutriLink API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT without quotes or line breaks",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "bearer",
            Name = "Bearer",
            In = ParameterLocation.Header
        },
        new List<string>()
    }
});



});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy
            .WithOrigins("https://localhost:7248", "http://localhost:5011", "http://192.168.1.5:5011") // your frontend URLs
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SameUser", policy =>
        policy.RequireAssertion(context =>
        {
            var userUuid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = context.User.FindFirstValue(ClaimTypes.Role);

            var routeUuid = (context.Resource as HttpContext)
                ?.Request.RouteValues["uuid"]?.ToString();

            return role == "ROLE_COACH" || role == "ROLE_ADMIN" || userUuid == routeUuid;
        }));
});

builder.Services.AddSingleton<IAuthorizationHandler, AdminBypassHandler>();
builder.Services.AddScoped<UserService>();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();