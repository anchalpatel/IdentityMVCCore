using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace IdentityMVCCore.Security
{
    public class SuperAdminHandler : AuthorizationHandler<ManagingAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagingAdminRolesAndClaimsRequirement requirement)
        {
            if (context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
