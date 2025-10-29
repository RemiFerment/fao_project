using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] User input)
        {
            if (id != input.Id) return BadRequest(new { message = "ID in route and the body don't match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.UUID = input.UUID;
            user.Email = input.Email;
            user.PasswordHash = input.PasswordHash;
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.Gender = input.Gender;
            user.BirthDate = input.BirthDate;
            user.Size = input.Size;
            user.Weight = input.Weight;
            user.PhysicalActivity = input.PhysicalActivity;
            user.Job = input.Job;
            user.EnergyRequirement = input.EnergyRequirement;
            await _db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}