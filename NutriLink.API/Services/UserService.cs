using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;

namespace NutriLink.API.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUuidAsync(string uuid)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UUID == uuid);
        }

        public async Task<bool> ExistsAsync(string uuid)
        {
            return await _context.Users.AnyAsync(u => u.UUID == uuid);
        }

        public async Task<int?> GetIdByUuidAsync(string uuid)
        {
            return await _context.Users
                .Where(u => u.UUID == uuid)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync();
        }
        public string GetUUIDByClaims(System.Security.Claims.ClaimsPrincipal user)
        {
            return user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        }
    }
}
