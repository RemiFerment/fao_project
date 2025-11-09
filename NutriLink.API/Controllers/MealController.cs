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

        if (mealDay == null) return NotFound("Meal not found for the specified date.");

        var mealDayDTO = new MealReadDTO
        {
            Id = mealDay.Id,
            Date = date,
            BreakfastId = mealDay.BreakfastId,
            LunchId = mealDay.LunchId,
            DinnerId = mealDay.DinnerId
        };

        return Ok(mealDayDTO);
    }

    [HttpPost("{uuid}/meal-days/{date}/{mealType}")]
    [Authorize("SameUser")]
    public async Task<IActionResult> UpdateMealDay(string uuid, DateOnly date, string mealType, [FromBody] MealSendDTO mealDayDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (mealDayDTO == null) return BadRequest("MealDay data is required.");

        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");

        mealType = mealType.Trim().ToLower();

        var recipe = await _context.Recipes.FindAsync(mealDayDTO.RecipeId);
        if (recipe == null) return NotFound("Recipe not found.");

        var existingMealDay = await _context.MealDays.FirstOrDefaultAsync(md => md.UserId == user.Id && md.Date == date);

        if (existingMealDay != null)
        {
            switch (mealType)
            {
                case "breakfast":
                    existingMealDay.BreakfastId = mealDayDTO.RecipeId;
                    existingMealDay.Breakfast = await _context.Recipes.FindAsync(mealDayDTO.RecipeId);
                    break;

                case "lunch":
                    existingMealDay.LunchId = mealDayDTO.RecipeId;
                    existingMealDay.Lunch = await _context.Recipes.FindAsync(mealDayDTO.RecipeId);
                    break;

                case "dinner":
                    existingMealDay.DinnerId = mealDayDTO.RecipeId;
                    existingMealDay.Dinner = await _context.Recipes.FindAsync(mealDayDTO.RecipeId);
                    break;

                default:
                    return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
            }

            await _context.SaveChangesAsync();

            var updatedDTO = new MealReadDTO
            {
                Id = existingMealDay.Id,
                Date = existingMealDay.Date,
                BreakfastId = existingMealDay.BreakfastId,
                LunchId = existingMealDay.LunchId,
                DinnerId = existingMealDay.DinnerId
            };

            return Ok(updatedDTO);
        }

        var mealDay = new MealDay
        {
            Date = date,
            UserId = user.Id,
            User = user
        };
        switch (mealType)
        {
            case "breakfast":
                mealDay.BreakfastId = mealDayDTO.RecipeId;
                mealDay.Breakfast = recipe;
                break;

            case "lunch":
                mealDay.LunchId = mealDayDTO.RecipeId;
                mealDay.Lunch = recipe;
                break;

            case "dinner":
                mealDay.DinnerId = mealDayDTO.RecipeId;
                mealDay.Dinner = recipe;
                break;

            default:
                return BadRequest("Invalid meal type. Use 'breakfast', 'lunch', or 'dinner'.");
        }

        _context.MealDays.Add(mealDay);
        await _context.SaveChangesAsync();
        var mealDayDTOResult = new MealReadDTO
        {
            Id = mealDay.Id,
            Date = mealDay.Date,
            BreakfastId = mealDay.BreakfastId,
            LunchId = mealDay.LunchId,
            DinnerId = mealDay.DinnerId
        };

        return CreatedAtAction(nameof(GetMealDay), new { user.UUID, date }, mealDayDTOResult);
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

    [HttpGet("{uuid}/snack")]
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

    [HttpPost("snack/add")]
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

    [HttpPatch("{uuid}/snack/modify")]
    [Authorize("SameUser")]
    public async Task<ActionResult> ModifySnackDay(string uuid, [FromBody] SnackDayDTO input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null) return NotFound("User not found.");

        var snack = await _context.SnackDays.FindAsync(input.SnackId);
        if (snack == null) return NotFound();

        snack.SnackId = input.SnackId;
        var snackRecipe = await _context.Recipes.FindAsync(input.SnackId);
        if (snackRecipe == null) return NotFound("Snack recipe not found.");
        snack.Snack = snackRecipe;

        _context.SnackDays.Update(snack);
        await _context.SaveChangesAsync();

        return Ok(snack);
    }

    [HttpDelete("{uuid}/snack/remove/{id}")]
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