using Microsoft.AspNetCore.Mvc;
using NutriLink.API.Data;

namespace NutriLink.API.Controllers;

public class AchievementController : ControllerBase
{
    private readonly AppDbContext _context;
    public AchievementController(AppDbContext context)
    {
        _context = context;
    }

    
}