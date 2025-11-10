using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Models;
using NutriLink.API.Data;
using NutriLink.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace NutriLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null) return BadRequest("Invalid credentials.");
            var passwordHandler = new PasswordHasher<User>();
            var checkPassword = passwordHandler.VerifyHashedPassword(user, user.PasswordHash, login.PlainPassword);
            if (checkPassword == PasswordVerificationResult.Success)
            {
                var jwtServices = new JWTServices(_config);
                var token = jwtServices.GenerateJwtToken(user.UUID, user.Role.Name);
                Console.WriteLine(token);
                return Ok(new { Token = token });
            }
            Console.WriteLine("Password verification failed. Password : " + checkPassword.ToString() + " for user " + user.Email);
            return BadRequest("Invalid credentials.");
        }
    }
}
