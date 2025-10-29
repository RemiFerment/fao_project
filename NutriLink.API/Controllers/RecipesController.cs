using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController: ControllerBase
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
    }
}