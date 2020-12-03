namespace Tickr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// https://www.robin-gueldenpfennig.de/2018/11/evaluate-multi-value-scope-claim-in-asp-net-core-authorization-policy/
    /// </summary>
    public class ScopeAuthorizationRequirement : AuthorizationHandler<ScopeAuthorizationRequirement>, IAuthorizationRequirement
    {
        public IEnumerable<string> RequiredScopes { get; }
 
        public ScopeAuthorizationRequirement(IEnumerable<string> requiredScopes)
        {
            if (requiredScopes == null || !requiredScopes.Any())
            {
                throw new ArgumentException($"{nameof(requiredScopes)} must contain at least one value.", nameof(requiredScopes));
            }
 
            RequiredScopes = requiredScopes;
        }
 
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAuthorizationRequirement requirement)
        {
            if (context.User != null)
            {
                var scopeClaim = context.User.Claims.FirstOrDefault(
                    c => string.Equals(c.Type, "scope", StringComparison.OrdinalIgnoreCase));
 
                if (scopeClaim != null)
                {
                    var scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (requirement.RequiredScopes.All(requiredScope => scopes.Contains(requiredScope)))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
 
            return Task.CompletedTask;
        }
    }
}