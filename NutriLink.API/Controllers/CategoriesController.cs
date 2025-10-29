using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _db.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Add(category);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Update(int id, [FromBody] Category input)
        {
            if (id != input.Id) return BadRequest(new { message = "ID in route and the body don't match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.Name = input.Name;
            await _db.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _db.Remove(category);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}