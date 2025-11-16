using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;
using NutriLink.API.Services;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserService _userService;
        public UsersController(AppDbContext db, UserService userService)
        {
            _db = db;
            _userService = userService;
        }

        [HttpGet("{uuid}")]
        [Authorize(Roles = "ROLE_COACH")]
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
        // [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(dto.PlainPassword)) return BadRequest(new { message = "Password is required." });

            var passwordHasher = new PasswordHasher<User>();
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == 3);
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
                RoleId = 1,
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
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> GetUserProfile(Guid uuid)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UUID == uuid.ToString());
            if (user == null || user.UserProfileId == null) return NotFound();
            var userProfile = await _db.UserProfiles.FindAsync(user.UserProfileId);
            if (userProfile == null) return NotFound();

            return Ok(userProfile);
        }
        [HttpPost("set-profile")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult> CreateUserProfile([FromBody] UserProfileDTO userProfile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userConcerned = await _db.Users.FirstOrDefaultAsync(u => u.UUID == userProfile.Uuid.ToString());
            if (userConcerned == null) return NotFound(new { message = "User not found." });

            var newUserProfile = new UserProfile
            {
                Job = userProfile.Job,
                EnergyRequirement = userProfile.EnergyRequirement,
                Weight = userProfile.Weight,
                Size = userProfile.Size,
                PhysicalActivity = userProfile.PhysicalActivity
            };
            _db.UserProfiles.Add(newUserProfile);
            await _db.SaveChangesAsync();
            userConcerned.UserProfileId = newUserProfile.Id;
            await _db.SaveChangesAsync();
            return Ok("User profile created successfully.");

        }

        [HttpPost("{userId}/assign-role/{roleId}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult> AssignUserRole(int userId, int roleId)
        {
            var user = await _db.Users.FindAsync(userId);
            var role = await _db.Roles.FindAsync(roleId);

            if (user == null || role == null) { return NotFound("User or Role not found."); }
            if (user.Role.Name == "ROLE_ADMIN") { return BadRequest("Cannot change role of an Admin user."); }
            if (role.Name == "ROLE_ADMIN") { return BadRequest("Cannot assign Admin role through this endpoint."); }

            user.RoleId = roleId;
            await _db.SaveChangesAsync();

            return Ok("Role assigned successfully.");
        }

        /// <summary>
        /// Update user information (except password).
        /// </summary>
        [HttpPatch("update/{uuidUser}")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult<User>> Update(string uuidUser, [FromBody] RegisterDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.GetByUuidAsync(uuidUser);
            if (user == null) return NotFound("User not found.");

            user.Email = input.Email;
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.Gender = input.Gender;
            user.BirthDate = input.BirthDate;

            var readUser = new ReadUserDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                RoleName = (await _db.Roles.FindAsync(user.RoleId))?.Name ?? string.Empty
            };

            await _db.SaveChangesAsync();
            return Ok(readUser);
        }

        [HttpPatch("profile/{userUuid}")]
        [Authorize(Roles = "ROLE_COACH")]
        public async Task<ActionResult<UserProfile>> UpdateProfile(string userUuid, [FromBody] UserProfileDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UUID == userUuid);
            if (user == null || user.UserProfileId == null) return NotFound("User or UserProfile not found.");

            var userProfile = await _db.UserProfiles.FindAsync(user.UserProfileId);
            if (userProfile == null) return NotFound();

            userProfile.Job = input.Job;
            userProfile.EnergyRequirement = input.EnergyRequirement;
            userProfile.Weight = input.Weight;
            userProfile.Size = input.Size;
            userProfile.PhysicalActivity = input.PhysicalActivity;

            await _db.SaveChangesAsync();
            return Ok("User profile updated successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (user.Role.Name == "ROLE_ADMIN")
            {
                return BadRequest("Cannot delete an Admin user.");
            }
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
            var achievements = _db.Achievements.Where(a => a.UserId == id);
            if (achievements.Any())
            {
                _db.RemoveRange(achievements);
            }
            _db.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}