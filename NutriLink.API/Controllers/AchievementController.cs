using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Services;

namespace NutriLink.API.Controllers;

public class AchievementController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;
    public AchievementController(AppDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet("{uuid}/achievements")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> GetUserAchievements(string uuid)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });
        var achievements = await _context.Achievements
            .Include(a => a.AchievementType)
            .Where(a => a.UserId == user.Id)
            .ToListAsync();
        if (achievements == null || achievements.Count == 0) return NotFound(new { message = "No achievements found for this user." });
        return Ok(achievements);
    }
}