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
        public async Task<ActionResult<IEnumerable<RecipeSendDTO>>> GetAll()
        {
            var recipes = await _db.Recipes.Include(r => r.Category).ToListAsync();
            if (recipes == null || recipes.Count == 0)
            {
                return NotFound(new { message = "No recipes found." });
            }
            var recipeDTOs = recipes.Select(r => new RecipeSendDTO
            {
                Id = r.Id,
                Title = r.Title,
                Steps = r.Steps,
                CategoryId = r.CategoryId
            }).ToList();
            return Ok(recipeDTOs);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<FullRecipeDTO>> GetById(int id)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null) return NotFound(new { message = $"Recipe with ID {id} not found." });
            var category = await _db.Categories.FindAsync(recipe.CategoryId);
            var ingredients = _db.Set<RecipeIngredient>()
                .Where(ri => ri.RecipeId == recipe.Id)
                .Include(ri => ri.Ingredient)
                .Select(ri => new FullRecipeIngredientDTO
                {
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.Ingredient.Name,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit
                }).ToList();
            var recipeDTO = new FullRecipeDTO
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Steps = recipe.Steps,
                CategoryName = category!.Name,
                Ingredients = ingredients
            };


            return Ok(recipeDTO);
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FullRecipeDTO>>> SearchByKeyword([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required." });

            // Decode URL encoded keyword (e.g. %20 -> space)
            keyword = System.Net.WebUtility.UrlDecode(keyword).Trim();
            var recipes = await _db.Recipes
                .Where(r => r.Title.Contains(keyword))
                .Include(r => r.Category)
                .ToListAsync();

            if (recipes == null || recipes.Count == 0)
            {
                return NotFound(new { message = "No recipes found matching the keyword." });
            }

            var recipeDTOs = recipes.Select(r => new FullRecipeDTO
            {
                Id = r.Id,
                Title = r.Title,
                Steps = r.Steps,
                CategoryName = r.Category.Name,
                Ingredients = _db.Set<RecipeIngredient>()
                    .Where(ri => ri.RecipeId == r.Id)
                    .Include(ri => ri.Ingredient)
                    .Select(ri => new FullRecipeIngredientDTO
                    {
                        IngredientId = ri.IngredientId,
                        IngredientName = ri.Ingredient.Name,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    }).ToList()
            }).ToList();

            return Ok(recipeDTOs);
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult<RecipeSendDTO>> Create([FromBody] RecipeSendDTO recipe)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await _db.Categories.FindAsync(recipe.CategoryId);
            if (category == null) return NotFound(new { message = "Invalid Category" });
            var newRecipe = new Recipe
            {
                Title = recipe.Title,
                Steps = recipe.Steps,
                CategoryId = recipe.CategoryId,
                Category = category
            };

            _db.Recipes.Add(newRecipe);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, newRecipe);
        }

        [HttpPost("add-ingredient")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> AddIngredientToRecipe([FromBody] RecipeIngredientDTO dto)
        {
            //the checks before added ingredient to a recipe
            var recipe = await _db.Recipes.FindAsync(dto.RecipeId);
            if (recipe == null) return NotFound("Recipe Not Found");

            var ingredient = await _db.Ingredients.FindAsync(dto.IngredientId);
            if (ingredient == null) return NotFound("Ingredient Not Found");

            var checking = await _db.Set<RecipeIngredient>()
            .FirstOrDefaultAsync(c => c.RecipeId == dto.RecipeId && c.IngredientId == dto.IngredientId);
            if (checking != null) return BadRequest("Ingredient already added to this recipe");

            var recipeIngredient = new RecipeIngredient
            {
                RecipeId = dto.RecipeId,
                Recipe = recipe,
                IngredientId = dto.IngredientId,
                Ingredient = ingredient,
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

        [HttpPatch("update")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> Update([FromBody] RecipeSendDTO input)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var recipe = await _db.Recipes.FindAsync(input.Id);
            if (recipe == null) return NotFound();

            recipe.Title = input.Title;
            recipe.Steps = input.Steps;
            recipe.CategoryId = input.CategoryId;

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

            _db.Recipes.Remove(recipe);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("ingredients/remove")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> RemoveIngredientFromRecipe(int recipeId, int ingredientId)
        {
            var recipeIngredient = await _db.Set<RecipeIngredient>()
            .FirstOrDefaultAsync(r => r.IngredientId == ingredientId && r.RecipeId == recipeId);
            if (recipeIngredient == null) return NotFound("This ingredient is not linked to this recipe.");

            _db.Remove(recipeIngredient);
            await _db.SaveChangesAsync();
            return Ok(new { message = "This ingredient was remove from the recipe" });
        }

        [HttpPost("ingredients/update")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> UpdateIngredientInRecipe([FromBody] RecipeIngredientUpdateDTO dto)
        {
            var recipeIngredient = await _db.RecipeIngredients
            .FirstOrDefaultAsync(ri => ri.RecipeId == dto.RecipeId && ri.IngredientId == dto.IngredientId);
            if (recipeIngredient == null) return NotFound("This ingredient link to recipe does not exist.");

            _db.Remove(recipeIngredient);

            var newIngredient = await _db.Ingredients.FindAsync(dto.NewIngredientId);
            if (newIngredient == null) return NotFound("New ingredient not found.");

            var recipe = await _db.Recipes.FindAsync(dto.RecipeId);

            var existingLink = await _db.RecipeIngredients
            .FirstOrDefaultAsync(ri => ri.RecipeId == dto.RecipeId && ri.IngredientId == dto.NewIngredientId);
            if (existingLink != null) return BadRequest("This ingredient is already linked to the recipe.");

            if (dto.Quantity <= 0) return BadRequest("Quantity must be greater than zero.");

            if (string.IsNullOrEmpty(dto.Unit)) return BadRequest("Unit must be provided.");

            var newRecipeIngredient = new RecipeIngredient
            {
                RecipeId = dto.RecipeId,
                Recipe = recipe!,
                IngredientId = dto.NewIngredientId,
                Ingredient = newIngredient,
                Quantity = dto.Quantity,
                Unit = dto.Unit
            };

            _db.RecipeIngredients.Add(newRecipeIngredient);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = $"Ingredient {newIngredient.Name} added to recipe {recipe!.Title} with {dto.Quantity} {dto.Unit}"
            });
        }
    }
}