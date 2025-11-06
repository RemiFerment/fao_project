using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RolesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();

            return Ok(role);
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<Role>> Create([FromBody] Role role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Add(role);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<Role>> Update(int id, [FromBody] Role input)
        {
            if (id != input.Id) return BadRequest(new { message = "ID in route and the body don't match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();

            role.Name = input.Name;
            await _db.SaveChangesAsync();
            return Ok(role);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<Role>> Delete(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();

            _db.Remove(role);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}