namespace NutriLink.API.Services;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class AdminBypassHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);

        // If the user is an admin, mark ALL requirements as succeeded
        if (role == "ROLE_ADMIN")
        {
            foreach (var requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
