namespace NutriLink.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;
[ApiController]
[Route("api/[controller]")]
public class MealController : ControllerBase
{
    private readonly AppDbContext _context;

    public MealController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get MealDay for a specific user and date
    /// <param name="userId"></param>
    /// <param name="date"></param>
    /// </summary>
    [HttpGet("{uuid}/mealdays")]
    [Authorize("SameUser")]
    public async Task<IActionResult> GetMealDay(string uuid, [FromQuery] DateOnly date)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UUID == uuid);
        if (user == null) return NotFound("User not found.");
        var mealDay = await _context.MealDays
            .Include(md => md.Breakfast)
            .Include(md => md.Lunch)
            .Include(md => md.Dinner)
            .FirstOrDefaultAsync(md => md.UserId == user.Id && md.Date == date);

        if (mealDay == null)
        {
            return NotFound();
        }

        return Ok(mealDay);
    }

    [HttpPost("{uuid}/mealdays/{date}/{mealType}")]
    [Authorize("SameUser")]
    public async Task<IActionResult> UpdateMealDay(string uuid, DateOnly date, string mealType, [FromBody] MealDay mealDay)
    {
        if (mealDay == null)
            return BadRequest("MealDay data is required.");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UUID == uuid);
        if (user == null) return NotFound("User not found.");
        mealType = mealType.Trim().ToLower();

        var existingMealDay = await _context.MealDays
            .FirstOrDefaultAsync(md => md.UserId == user.Id && md.Date == date);

        if (existingMealDay != null)
        {
            switch (mealType)
            {
                case "breakfast":
                    existingMealDay.BreakfastId = mealDay.BreakfastId;
                    break;
                case "lunch":
                    existingMealDay.LunchId = mealDay.LunchId;
                    break;
                case "dinner":
                    existingMealDay.DinnerId = mealDay.DinnerId;
                    break;
                default:
                    return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
            }

            await _context.SaveChangesAsync();
            return Ok(existingMealDay);
        }

        mealDay.UserId = user.Id;
        mealDay.Date = date;

        switch (mealType)
        {
            case "breakfast":
                mealDay.LunchId = null;
                mealDay.DinnerId = null;
                break;
            case "lunch":
                mealDay.BreakfastId = null;
                mealDay.DinnerId = null;
                break;
            case "dinner":
                mealDay.BreakfastId = null;
                mealDay.LunchId = null;
                break;
            default:
                return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
        }

        _context.MealDays.Add(mealDay);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMealDay), new { user.Id, date }, mealDay);
    }

    [HttpDelete("{uuid}/mealdays/{date}/{mealType}")]
    [Authorize("SameUser")]
    public async Task<IActionResult> DeleteMealFromMealDay(string uuid, DateOnly date, string mealType)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UUID == uuid);
        if (user == null) return NotFound("User not found.");

        var mealDay = await _context.MealDays
            .FirstOrDefaultAsync(md => md.UserId == user.Id && md.Date == date);

        if (mealDay == null)
        {
            return NotFound();
        }

        mealType = mealType.Trim().ToLower();

        switch (mealType)
        {
            case "breakfast":
                mealDay.BreakfastId = null;
                break;
            case "lunch":
                mealDay.LunchId = null;
                break;
            case "dinner":
                mealDay.DinnerId = null;
                break;
            default:
                return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{uuid}/snackday")]
    public async Task<ActionResult> GetSnackDayFromDate(string uuid, [FromQuery] DateOnly date)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UUID == uuid);
        if (user == null) return NotFound("User not found.");

        var snackDays = await _context.SnackDays
            .Include(sd => sd.Snack)
            .Where(sd => sd.UserId == user.Id && sd.Date == date)
            .ToListAsync();

        if (snackDays == null || snackDays.Count == 0)
        {
            return NotFound();
        }

        return Ok(snackDays);
    }

    [HttpPost("snackday")]
    public async Task<ActionResult> AddSnackDay([FromBody] SnackDay snackDay)
    {
        var user = await _context.Users.FindAsync(snackDay.UserId);
        if (user == null) return NotFound("User doesn't exist.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _context.SnackDays.Add(snackDay);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSnackDayFromDate), new { snackDay.UserId, snackDay.Date }, snackDay);
    }

    [HttpPatch("{uuid}/snack_day/{id}")]
    public async Task<ActionResult> ModifySnackDay(string uuid, int id, [FromBody] SnackDay input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (input.Id != id) return BadRequest("ID mismatch.");

        var snack = await _context.SnackDays.FindAsync(id);
        if (snack == null) return NotFound();

        snack.SnackId = input.SnackId;

        _context.SnackDays.Update(snack);
        await _context.SaveChangesAsync();

        return Ok(snack);
    }

    [HttpDelete("snack_day/{id}")]
    public async Task<ActionResult> DeleteSnackDay(int id)
    {
        var snack = await _context.SnackDays.FindAsync(id);
        if (snack == null) return NotFound();

        _context.Remove(snack);
        await _context.SaveChangesAsync();

        return NoContent();
    }


}