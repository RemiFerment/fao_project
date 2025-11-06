using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RecipesController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
        {
            var recipes = await _db.Recipes.Include(r => r.Category).ToListAsync();
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var recipe = await _db.Recipes
            .Include(r => r.Category)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null) return NotFound(new { message = $"Recipe with ID {id} not found." });

            return Ok(recipe);
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult<Recipe>> Create([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoryExist = await _db.Categories.AnyAsync(c => c.Id == recipe.CategoryId);
            if (!categoryExist) return BadRequest(new { message = "Invalid Category" });

            _db.Add(recipe);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
        }

        [HttpPost("{recipeId}/igredients")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> AddIngredientToRecipe(int recipeId, [FromBody] RecipeIngredient dto)
        {
            //the checks before added ingredient to a recipe
            var recipe = await _db.Recipes.FindAsync(recipeId);
            if (recipe == null) return NotFound("Recipe Not Found");

            var ingredient = await _db.Ingredients.FindAsync(dto.IngredientId);
            if (ingredient == null) return NotFound("Ingredient Not Found");

            var checking = await _db.Set<RecipeIngredient>()
            .FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.IngredientId == dto.IngredientId);
            if (checking != null) return BadRequest("Ingredient already added to this recipe");

            var recipeIngredient = new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = dto.IngredientId,
                Quantity = dto.Quantity,
                Unit = dto.Unit
            };

            _db.Add(recipeIngredient);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = $"Ingredient {ingredient.Name} added to recipe {recipe.Title} with {dto.Quantity} {dto.Unit}"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> Update(int id, [FromBody] Recipe input)
        {
            if (id != input.Id) return BadRequest(new { message = "ID in route and body don't match." });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            recipe.Title = input.Title;
            recipe.Steps = input.Steps;
            recipe.Category = input.Category;

            await _db.SaveChangesAsync();
            return Ok(recipe);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> Delete(int id)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            var mealDaysWithRecipe = await _db.MealDays
                .Where(md => md.BreakfastId == id || md.LunchId == id || md.DinnerId == id)
                .ToListAsync();

            foreach (var mealDay in mealDaysWithRecipe)
            {
                if (mealDay.BreakfastId == id) mealDay.BreakfastId = null;
                if (mealDay.LunchId == id) mealDay.LunchId = null;
                if (mealDay.DinnerId == id) mealDay.DinnerId = null;
            }

            var snackDaysWithRecipe = await _db.SnackDays
                .Where(sd => sd.SnackId == id)
                .ToListAsync();
            foreach (var snackDay in snackDaysWithRecipe)
            {
                _db.SnackDays.Remove(snackDay);
            }

            _db.Remove(recipe);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{recipeId}/ingredients/{ingredientId}")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> RemoveIngredientFromRecipe(int recipeId, int ingredientId)
        {
            var recipeIngredient = await _db.Set<RecipeIngredient>()
            .FirstOrDefaultAsync(r => r.IngredientId == ingredientId && r.RecipeId == recipeId);
            if (recipeIngredient == null) return NotFound("This ingredient is not linked to this recipe.");

            _db.Remove(recipeIngredient);
            await _db.SaveChangesAsync();
            return Ok(new
            {
                message = "This ingredient was remove from the recipe"
            });
        }
    }
}