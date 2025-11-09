using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
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

        ///<summary>
        /// Get all categories.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _db.Categories.ToListAsync();
            return Ok(categories);
        }

        ///<summary> 
        ///Get a category by ID.
        ///</summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return Ok(category);
        }

        ///<summary> 
        ///Create a new category.
        ///</summary>
        [Authorize(Roles = "ROLE_COACH")]
        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existingCategory = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());
            if (existingCategory != null) return Conflict(new { message = "A category with the same name already exists." });
            _db.Add(category);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        ///<summary>
        /// Update an existing category.
        /// </summary>
        [Authorize(Roles = "ROLE_COACH")]
        [HttpPatch("/update")]
        public async Task<ActionResult<Category>> Update([FromBody] Category input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var category = await _db.Categories.FindAsync(input.Id);
            if (category == null) return NotFound("Category not found.");

            category.Name = input.Name;
            var existingCategory = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == input.Name.ToLower() && c.Id != input.Id);
            if (existingCategory != null) return Conflict(new { message = "A category with the same name already exists." });
            await _db.SaveChangesAsync();
            return Ok(category);
        }

        ///<summary>
        /// Delete a category by ID.
        /// </summary>
        [Authorize(Roles = "ROLE_COACH")]
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