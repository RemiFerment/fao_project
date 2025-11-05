using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        [HttpGet("{uuid}")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
        public async Task<ActionResult> GetById(Guid uuid)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UUID == uuid.ToString());
            if (user == null) return NotFound();

            var dto = new ReadUserDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                RoleName = (await _db.Roles.FindAsync(user.RoleId))?.Name ?? string.Empty
            };

            return Ok(dto);
        }


        [HttpPost("register")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(dto.PlainPassword)) return BadRequest(new { message = "Password is required." });

            var passwordHasher = new PasswordHasher<User>();
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == dto.RoleId);
            if (role == null) return BadRequest("Role don't exist.");
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null) return Conflict(new { message = "Email is already in use." });
            var user = new User
            {
                UUID = Guid.NewGuid().ToString(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                RoleId = dto.RoleId,
                Role = role
            };
            user.PasswordHash = passwordHasher.HashPassword(user, dto.PlainPassword);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var ReadUser = new ReadUserDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                RoleName = role.Name
            };

            return CreatedAtAction(nameof(GetById), new { uuid = user.UUID.ToString() }, ReadUser);
        }

        [HttpGet("{uuid}/profile")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
        public async Task<ActionResult> GetUserProfile(Guid uuid)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UUID == uuid.ToString());
            if (user == null || user.UserProfileId == null) return NotFound();
            var userProfile = await _db.UserProfiles.FindAsync(user.UserProfileId);
            if (userProfile == null) return NotFound();

            return Ok(userProfile);
        }
        [HttpPost("profile")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
        public async Task<ActionResult> CreateUserProfile([FromBody] UserProfile userProfile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Add(userProfile);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserProfile), new { id = userProfile.Id }, userProfile);
        }

        [HttpPost("{userId}/assign-profile/{profileId}")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
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
        [Authorize(Roles = "ROLE_ADMIN")]
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
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
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
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
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
        [Authorize(Roles = "ROLE_ADMIN,ROLE_COACH")]
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
            var mealDays = _db.MealDays.Where(md => md.UserId == id);
            if (mealDays.Any())
            {
                _db.RemoveRange(mealDays);
            }
            var snackDays = _db.SnackDays.Where(sd => sd.UserId == id);
            if (snackDays.Any())
            {
                _db.RemoveRange(snackDays);
            }
            _db.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}