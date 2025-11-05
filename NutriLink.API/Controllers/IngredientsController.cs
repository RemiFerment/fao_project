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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
        {
            var ingredients = await _db.Ingredients.ToListAsync();
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var ingredient = await _db.Ingredients.FindAsync(id);
            if (ingredient == null) return NotFound();

            return Ok(ingredient);
        }

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Create([FromBody] Ingredient ingredient)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var check = await _db.Set<Ingredient>().FirstOrDefaultAsync(i => i.Name == ingredient.Name);
            if (check != null) return BadRequest(new { message = "Ingredient already exists." });

            _db.Add(ingredient);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Ingredient>> Update(int id, [FromBody] Ingredient input)
        {
            if (id != input.Id) return BadRequest(new { message = "ID in route and the body don't match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ingredient = await _db.Ingredients.FindAsync(id);
            if (ingredient == null) return NotFound();

            ingredient.Name = input.Name;
            await _db.SaveChangesAsync();
            return Ok(ingredient);
        }

        [HttpDelete("{id}")]
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