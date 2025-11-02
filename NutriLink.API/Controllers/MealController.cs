namespace NutriLink.API.Controllers;

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

    [HttpGet("{userId}/mealdays")]
    public async Task<IActionResult> GetMealDay(int userId, [FromQuery] DateOnly date)
    {
        var mealDay = await _context.MealDays
            .Include(md => md.Breakfast)
            .Include(md => md.Lunch)
            .Include(md => md.Dinner)
            .FirstOrDefaultAsync(md => md.UserId == userId && md.Date == date);

        if (mealDay == null)
        {
            return NotFound();
        }

        return Ok(mealDay);
    }

    [HttpPost("{userId}/mealdays/{date}/{mealType}")]
    public async Task<IActionResult> UpdateMealDay(int userId, DateOnly date, string mealType, [FromBody] MealDay mealDay)
    {
        if (mealDay == null)
            return BadRequest("MealDay data is required.");

        mealType = mealType.Trim().ToLower();

        var existingMealDay = await _context.MealDays
            .FirstOrDefaultAsync(md => md.UserId == userId && md.Date == date);

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

        mealDay.UserId = userId;
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

        return CreatedAtAction(nameof(GetMealDay), new { userId, date }, mealDay);
    }


}