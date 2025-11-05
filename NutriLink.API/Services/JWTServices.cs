namespace NutriLink.API.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    public class JWTServices
    {
        private readonly IConfiguration _config;

        public JWTServices(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(string UUID, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UUID),
                new Claim(ClaimTypes.Role, role)
            };

            var secretToken = _config["AppSettings:Token"] ?? throw new InvalidOperationException("AppSettings:Token is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretToken));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}