
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace IdentityMVCCore.Security
{
    public class AdminCanHandleOnlyOtherAdminRolesAndClaims : AuthorizationHandler<ManagingAdminRolesAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public AdminCanHandleOnlyOtherAdminRolesAndClaims(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ManagingAdminRolesAndClaimsRequirement requirement)
        {

            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value.ToString();

            string adminIdBeingEdited = httpContextAccessor.HttpContext.Request.Query["userId"].ToString();

            if (context.User.IsInRole("Admin") &&
            context.User.HasClaim(claim =>
            claim.Type == "Update Role" && claim.Value == "True") && adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
