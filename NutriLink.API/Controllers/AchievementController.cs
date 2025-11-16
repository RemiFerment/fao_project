using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;
using NutriLink.API.Models.DTOs.Achievement;
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

    // --------------------------
    // GET ALL ACHIEVEMENTS
    // --------------------------
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

        if (achievements.Count == 0)
            return NotFound(new { message = "No achievements found for this user." });

        var dto = achievements.Select(MapToDTO).ToList();

        return Ok(dto);
    }

    // --------------------------
    // GET ONE ACHIEVEMENT
    // --------------------------
    [HttpGet("{uuid}/achievements/{id}")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> GetUserAchievement(string uuid, int id)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });

        var achievement = await _context.Achievements
            .Include(a => a.AchievementType)
            .FirstOrDefaultAsync(a => a.UserId == user.Id && a.Id == id);

        if (achievement == null)
            return NotFound(new { message = "Achievement not found." });

        return Ok(MapToDTO(achievement));
    }

    // --------------------------
    // CREATE MEASUREMENT
    // --------------------------
    [HttpPost("{uuid}/achievements/measurement")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreateMeasurement(string uuid, [FromBody] AchievementMeasurementDTO dto)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound(new { message = "User not found." });

        var type = new AchievementTypeMeasurement
        {
            Name = "Measurement",
            Waist = dto.WaistMeasurement,
            Hips = dto.HipsMeasurement
        };

        _context.AchievementTypeMeasurements.Add(type);
        await _context.SaveChangesAsync();

        var achievement = new Achievement
        {
            UserId = user.Id,
            DateAchieved = dto.DateAchieved,
            Description = dto.Description,
            AchievementTypeId = type.Id
        };

        _context.Achievements.Add(achievement);
        await _context.SaveChangesAsync();

        return Created("", MapToDTO(achievement));
    }

    // --------------------------
    // CREATE PHOTO
    // --------------------------
    [HttpPost("{uuid}/achievements/photo")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreatePhoto(string uuid, [FromBody] AchievementPhotoDTO dto)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound();

        var type = new AchievementTypePhoto
        {
            Name = "Photo",
            Photo = dto.PhotoData
        };

        _context.AchievementTypePhotos.Add(type);
        await _context.SaveChangesAsync();

        var achievement = new Achievement
        {
            UserId = user.Id,
            DateAchieved = dto.DateAchieved,
            Description = dto.Description,
            AchievementTypeId = type.Id
        };

        _context.Achievements.Add(achievement);
        await _context.SaveChangesAsync();

        return Created("", MapToDTO(achievement));
    }

    // --------------------------
    // CREATE WEIGHT
    // --------------------------
    [HttpPost("{uuid}/achievements/weight")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreateWeight(string uuid, [FromBody] AchievementWeightDTO dto)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound();

        var type = new AchievementTypeWeight
        {
            Name = "Weight",
            Weight = dto.Weight
        };

        _context.AchievementTypeWeights.Add(type);
        await _context.SaveChangesAsync();

        var achievement = new Achievement
        {
            UserId = user.Id,
            DateAchieved = dto.DateAchieved,
            Description = dto.Description,
            AchievementTypeId = type.Id
        };

        _context.Achievements.Add(achievement);
        await _context.SaveChangesAsync();

        return Created("", MapToDTO(achievement));
    }

    // --------------------------
    // FREE COMMENT
    // --------------------------
    [HttpPost("{uuid}/achievements/free-comment")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> CreateComment(string uuid, [FromBody] AchievementFreeCommentDTO dto)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound();

        var achievement = new Achievement
        {
            UserId = user.Id,
            DateAchieved = dto.DateAchieved,
            Description = dto.Description,
            AchievementTypeId = null
        };

        _context.Achievements.Add(achievement);
        await _context.SaveChangesAsync();

        return Created("", MapToDTO(achievement));
    }

    // --------------------------
    // DELETE
    // --------------------------
    [HttpDelete("{uuid}/achievements/{id}")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> Delete(string uuid, int id)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound();

        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);

        if (achievement == null)
            return NotFound(new { message = "Achievement not found." });

        _context.Achievements.Remove(achievement);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // --------------------------
    // MAPPING METHOD
    // --------------------------
    private static AchievementDTO MapToDTO(Achievement a)
    {
        var dto = new AchievementDTO
        {
            Id = a.Id,
            DateAchieved = a.DateAchieved,
            Description = a.Description
        };

        if (a.AchievementType is null)
        {
            dto.AchievementType = null;
            return dto;
        }

        var typeDto = new AchievementTypeDTO
        {
            Id = a.AchievementType.Id,
            Name = a.AchievementType.Name
        };

        switch (a.AchievementType)
        {
            case AchievementTypeMeasurement m:
                typeDto.Type = "measurement";
                typeDto.Waist = m.Waist;
                typeDto.Hips = m.Hips;
                break;

            case AchievementTypePhoto p:
                typeDto.Type = "photo";
                typeDto.Photo = p.Photo;
                break;

            case AchievementTypeWeight w:
                typeDto.Type = "weight";
                typeDto.Weight = w.Weight;
                break;

            default:
                typeDto.Type = "none";
                break;
        }

        dto.AchievementType = typeDto;
        return dto;
    }

}
