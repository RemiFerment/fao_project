namespace Fao.Front_End.Services;

using System.IdentityModel.Tokens.Jwt;

public static class JWTUtilService
{
    public static bool IsTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return false;

        var jwt = handler.ReadJwtToken(token);
        return jwt.ValidTo > DateTime.UtcNow;
    }

    public static string? GetClaim(string token, string claimType)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return null;

        var jwt = handler.ReadJwtToken(token);
        return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
