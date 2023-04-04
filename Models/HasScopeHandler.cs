using Microsoft.AspNetCore.Authorization;

namespace Miskato_Blog.Models
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
                return Task.CompletedTask;
            var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer).Value.Split(" ");
            if (scopes.Any(c => c == requirement.Scope))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}