using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;
using NutriLink.API.Services;

namespace NutriLink.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
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
    [HttpGet("{uuid}/achievements/{achievementId}")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> GetUserAchievementById(string uuid, int achievementId)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });
        var achievement = await _context.Achievements
            .Include(a => a.AchievementType)
            .FirstOrDefaultAsync(a => a.UserId == user.Id && a.Id == achievementId);
        if (achievement == null) return NotFound(new { message = "Achievement not found for this user." });
        return Ok(achievement);
    }

    [HttpPost("{uuid}/achievements/measurement")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreateUserAchievement(string uuid, [FromBody] AchievementMeasurementDTO achievement)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });

        var measurementAchievement = new AchievementTypeMeasurement
        {
            Name = "Mensurations",
            Hips = achievement.HipsMeasurement,
            Waist = achievement.WaistMeasurement
        };

        _context.AchievementTypeMeasurements.Add(measurementAchievement);
        await _context.SaveChangesAsync();
        var newAchievement = new Achievement
        {
            DateAchieved = achievement.DateAchieved,
            Description = achievement.Description,
            UserId = user.Id,
            AchievementTypeId = measurementAchievement.Id,
            AchievementType = measurementAchievement
        };
        _context.Achievements.Add(newAchievement);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserAchievementById), new { uuid, achievementId = newAchievement.Id }, newAchievement);
    }

    [HttpPost("{uuid}/achievements/photo")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreateUserPhotoAchievement(string uuid, [FromBody] AchievementPhotoDTO achievement)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });

        var photoAchievement = new AchievementTypePhoto
        {
            Name = "Photo",
            Photo = achievement.PhotoData
        };

        _context.AchievementTypePhotos.Add(photoAchievement);
        await _context.SaveChangesAsync();
        var newAchievement = new Achievement
        {
            DateAchieved = achievement.DateAchieved,
            Description = achievement.Description,
            UserId = user.Id,
            AchievementTypeId = photoAchievement.Id,
            AchievementType = photoAchievement
        };
        _context.Achievements.Add(newAchievement);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserAchievementById), new { uuid, achievementId = newAchievement.Id }, newAchievement);
    }

    [HttpPost("{uuid}/achievements/weight")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreateUserWeightAchievement(string uuid, [FromBody] AchievementWeightDTO achievement)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });

        var weightAchievement = new AchievementTypeWeight
        {
            Name = "Poids",
            Weight = achievement.Weight
        };

        _context.AchievementTypeWeights.Add(weightAchievement);
        await _context.SaveChangesAsync();
        var newAchievement = new Achievement
        {
            DateAchieved = achievement.DateAchieved,
            Description = achievement.Description,
            UserId = user.Id,
            AchievementTypeId = weightAchievement.Id,
            AchievementType = weightAchievement
        };
        _context.Achievements.Add(newAchievement);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserAchievementById), new { uuid, achievementId = newAchievement.Id }, newAchievement);
    }

    [HttpPost("{uuid}/achievements/free-comment")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreateUserFreeCommentAchievement(string uuid, [FromBody] AchievementFreeCommentDTO achievement)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });

        var newAchievement = new Achievement
        {
            DateAchieved = achievement.DateAchieved,
            Description = achievement.Description,
            UserId = user.Id,
            AchievementTypeId = null,
            AchievementType = null
        };
        _context.Achievements.Add(newAchievement);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserAchievementById), new { uuid, achievementId = newAchievement.Id }, newAchievement);
    }

    //Later, we want to add update methods for achievements

    [HttpDelete("{uuid}/achievements/{achievementId}")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> DeleteUserAchievement(string uuid, int achievementId)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });

        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.UserId == user.Id && a.Id == achievementId);
        if (achievement == null) return NotFound(new { message = "Achievement not found for this user." });

        _context.Achievements.Remove(achievement);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}