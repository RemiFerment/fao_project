using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
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

        [HttpGet("profile/{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
        {
            var userProfile = await _db.UserProfiles.FindAsync(id);
            if (userProfile == null) return NotFound();

            return Ok(userProfile);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        [HttpPost("profile")]
        public async Task<ActionResult<UserProfile>> CreateUserProfile([FromBody] UserProfile userProfile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Add(userProfile);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserProfile), new { id = userProfile.Id }, userProfile);
        }

        [HttpPost("{userId}/assign-profile/{profileId}")]
        public async Task<ActionResult> AssignUserProfile(int userId, int profileId)
        {
            var user = await _db.Users.FindAsync(userId);
            var profile = await _db.UserProfiles.FindAsync(profileId);

            if (user == null || profile == null)
            {
                return NotFound();
            }

            user.UserProfileId = profileId;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{userId}/assign-role/{roleId}")]
        public async Task<ActionResult> AssignUserRole(int userId, int roleId)
        {
            var user = await _db.Users.FindAsync(userId);
            var role = await _db.Roles.FindAsync(roleId);

            if (user == null || role == null)
            {
                return NotFound();
            }

            user.RoleId = roleId;
            await _db.SaveChangesAsync();

            return NoContent();
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


            await _db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("profile/{id}")]
        public async Task<ActionResult<UserProfile>> UpdateProfile(int id, [FromBody] UserProfile input)
        {
            if (id != input.Id) return BadRequest(new { message = "ID in route and the body don't match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userProfile = await _db.UserProfiles.FindAsync(id);
            if (userProfile == null) return NotFound();

            userProfile.Job = input.Job;
            userProfile.EnergyRequirement = input.EnergyRequirement;
            userProfile.Weight = input.Weight;
            userProfile.Size = input.Size;
            userProfile.PhysicalActivity = input.PhysicalActivity;

            await _db.SaveChangesAsync();
            return Ok(userProfile);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (user.UserProfileId != null)
            {
                var userProfile = await _db.UserProfiles.FindAsync(user.UserProfileId);
                if (userProfile != null)
                {
                    _db.Remove(userProfile);
                }
            }
            _db.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}