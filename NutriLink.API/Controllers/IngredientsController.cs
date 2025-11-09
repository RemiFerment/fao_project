using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public IngredientsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
        {
            var ingredients = await _db.Ingredients.ToListAsync();
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var ingredient = await _db.Ingredients.FindAsync(id);
            if (ingredient == null) return NotFound();

            return Ok(ingredient);
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult<Ingredient>> Create([FromBody] Ingredient ingredient)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var check = await _db.Set<Ingredient>().FirstOrDefaultAsync(i => i.Name == ingredient.Name);
            if (check != null) return BadRequest(new { message = "Ingredient already exists." });

            _db.Add(ingredient);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        [HttpPatch("update")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult<Ingredient>> Update([FromBody] Ingredient input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ingredient = await _db.Ingredients.FindAsync(input.Id);
            if (ingredient == null) return NotFound();
            var check = await _db.Set<Ingredient>().FirstOrDefaultAsync(i => i.Name == input.Name && i.Id != input.Id);
            if (check != null) return Conflict(new { message = "Another ingredient with the same name already exists." });

            ingredient.Name = input.Name;
            await _db.SaveChangesAsync();
            return Ok(ingredient);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult<Ingredient>> Delete(int id)
        {
            var ingredient = await _db.Ingredients.FindAsync(id);
            if (ingredient == null) return NotFound();

            _db.Remove(ingredient);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}