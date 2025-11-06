namespace NutriLink.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;
using NutriLink.API.Services;

[ApiController]
[Route("api/[controller]")]
public class MealController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;

    public MealController(AppDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <summary>
    /// Get MealDay for a specific user and date
    /// <param name="uuid">User's UUID</param>
    /// <param name="date"></param>
    /// </summary>
    [HttpGet("{uuid}/meal-days")]
    [Authorize("SameUser")]
    public async Task<IActionResult> GetMealDay(string uuid, [FromQuery] DateOnly date)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");
        var mealDay = await _context.MealDays
            .Include(md => md.Breakfast)
            .Include(md => md.Lunch)
            .Include(md => md.Dinner)
            .FirstOrDefaultAsync(md => md.UserId == user.Id && md.Date == date);

        if (mealDay == null)
        {
            return NotFound("Meal not found for the specified date.");
        }

        var mealDayDTO = new MealDayDTO
        {
            Id = mealDay.Id,
            UserUUID = user.UUID,
            Date = mealDay.Date,
            BreakfastId = mealDay.BreakfastId,
            LunchId = mealDay.LunchId,
            DinnerId = mealDay.DinnerId
        };

        return Ok(mealDayDTO);
    }

    [HttpPost("{uuid}/meal-days/{date}/{mealType}")]
    [Authorize("SameUser")]
    public async Task<IActionResult> UpdateMealDay(string uuid, DateOnly date, string mealType, [FromBody] MealDayDTO mealDayDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (mealDayDTO == null) return BadRequest("MealDay data is required.");
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");

        mealType = mealType.Trim().ToLower();

        var existingMealDay = await _context.MealDays
            .FirstOrDefaultAsync(md => md.UserId == user.Id && md.Date == date);

        if (existingMealDay != null)
        {
            switch (mealType)
            {
                case "breakfast":
                    existingMealDay.BreakfastId = mealDayDTO.BreakfastId;
                    existingMealDay.Breakfast = await _context.Recipes.FindAsync(mealDayDTO.BreakfastId);
                    break;

                case "lunch":
                    existingMealDay.LunchId = mealDayDTO.LunchId;
                    existingMealDay.Lunch = await _context.Recipes.FindAsync(mealDayDTO.LunchId);
                    break;

                case "dinner":
                    existingMealDay.DinnerId = mealDayDTO.DinnerId;
                    existingMealDay.Dinner = await _context.Recipes.FindAsync(mealDayDTO.DinnerId);
                    break;

                default:
                    return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
            }

            await _context.SaveChangesAsync();

            var updatedDTO = new MealDayDTO
            {
                Id = existingMealDay.Id,
                UserUUID = user.UUID,
                Date = existingMealDay.Date,
                BreakfastId = existingMealDay.BreakfastId,
                LunchId = existingMealDay.LunchId,
                DinnerId = existingMealDay.DinnerId
            };

            return Ok(updatedDTO);
        }


        switch (mealType)
        {
            case "breakfast":
                mealDayDTO.LunchId = null;
                mealDayDTO.DinnerId = null;
                break;
            case "lunch":
                mealDayDTO.BreakfastId = null;
                mealDayDTO.DinnerId = null;
                break;
            case "dinner":
                mealDayDTO.BreakfastId = null;
                mealDayDTO.LunchId = null;
                break;
            default:
                return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
        }

        var mealDay = new MealDay
        {
            Date = date,
            UserId = user.Id,
            BreakfastId = mealDayDTO.BreakfastId,
            LunchId = mealDayDTO.LunchId,
            DinnerId = mealDayDTO.DinnerId
        };

        _context.MealDays.Add(mealDay);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMealDay), new { user.UUID, date }, mealDayDTO);
    }

    [HttpDelete("{uuid}/meal-days/{date}/{mealType}")]
    [Authorize("SameUser")]
    public async Task<IActionResult> DeleteMealFromMealDay(string uuid, DateOnly date, string mealType)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");

        var mealDay = await _context.MealDays
            .FirstOrDefaultAsync(md => md.UserId == user.Id && md.Date == date);

        if (mealDay == null) return NotFound("MealDay not found for the specified date.");


        mealType = mealType.Trim().ToLower();

        switch (mealType)
        {
            case "breakfast":
                mealDay.BreakfastId = null;
                mealDay.Breakfast = null;
                break;
            case "lunch":
                mealDay.LunchId = null;
                mealDay.Lunch = null;
                break;
            case "dinner":
                mealDay.DinnerId = null;
                mealDay.Dinner = null;
                break;
            default:
                return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{uuid}/snack-days")]
    public async Task<ActionResult> GetSnackDayFromDate(string uuid, [FromQuery] DateOnly date)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");

        var snackDays = await _context.SnackDays
            .Include(sd => sd.Snack)
            .Where(sd => sd.UserId == user.Id && sd.Date == date)
            .ToListAsync();

        if (snackDays == null || snackDays.Count == 0) return NotFound("No snack days found for the specified date.");

        var snackDayDTOs = snackDays.Select(sd => new SnackDayDTO
        {
            Id = sd.Id,
            UserUUID = user.UUID,
            Date = sd.Date,
            SnackId = sd.SnackId
        }).ToList();

        return Ok(snackDayDTOs);
    }

    [HttpPost("snack-days")]
    public async Task<ActionResult> AddSnackDay([FromBody] SnackDayDTO snackDayDTO)
    {
        var user = await _userService.GetByUuidAsync(snackDayDTO.UserUUID);
        if (user == null) return NotFound("User doesn't exist.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var snack = await _context.Recipes.FindAsync(snackDayDTO.SnackId);
        if (snack == null) return NotFound("Snack recipe not found.");

        var snackDay = new SnackDay
        {
            Date = snackDayDTO.Date,
            UserId = user.Id,
            User = user,
            SnackId = snackDayDTO.SnackId,
            Snack = snack
        };

        _context.SnackDays.Add(snackDay);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSnackDayFromDate), new { user.UUID, snackDay.Date }, snackDayDTO);
    }

    [HttpPatch("{uuid}/snack-days/{id}")]
    [Authorize("SameUser")]
    public async Task<ActionResult> ModifySnackDay(string uuid, int id, [FromBody] SnackDayDTO input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");
        if (input.Id != id) return BadRequest("ID mismatch.");

        var snack = await _context.SnackDays.FindAsync(id);
        if (snack == null) return NotFound();

        snack.SnackId = input.SnackId;
        var snackRecipe = await _context.Recipes.FindAsync(input.SnackId);
        if (snackRecipe == null) return NotFound("Snack recipe not found.");
        snack.Snack = snackRecipe;

        _context.SnackDays.Update(snack);
        await _context.SaveChangesAsync();

        return Ok(snack);
    }

    [HttpDelete("{uuid}/snack-days/{id}")]
    [Authorize("SameUser")]
    public async Task<ActionResult> DeleteSnackDay(string uuid, int id)
    {
        var snack = await _context.SnackDays.FindAsync(id);
        if (snack == null) return NotFound();
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");

        _context.Remove(snack);
        await _context.SaveChangesAsync();

        return NoContent();
    }


}