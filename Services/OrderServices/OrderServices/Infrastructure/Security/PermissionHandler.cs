using Microsoft.AspNetCore.Authorization;

namespace OrderServices.Infrastructure.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissions = context.User
                .FindAll("permiso") // 👈 EXACTO como viene del token
                .SelectMany(c => c.Value.Split(','))
                .Select(p => p.Trim());

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
