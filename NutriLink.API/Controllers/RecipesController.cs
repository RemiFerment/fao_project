using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
        {
            var recipes = await _db.Recipes.Include(r => r.Category).ToListAsync();
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var recipe = await _db.Recipes.Include(r => r.Category).FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null) return NotFound(new { message = $"Recipe with ID {id} not found." });

            return Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> Create([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoryExist = await _db.Categories.AnyAsync(c => c.Id == recipe.CategoryId);
            if (!categoryExist) return BadRequest(new { message = "Invalid Category" });

            _db.Add(recipe);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
        }

        [HttpPut("{id}")]
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
        public async Task<ActionResult> Delete(int id)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            _db.Remove(recipe);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}