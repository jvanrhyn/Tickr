namespace Tickr
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// https://www.robin-gueldenpfennig.de/2018/11/evaluate-multi-value-scope-claim-in-asp-net-core-authorization-policy/
    /// </summary>
    public static class ScopeAuthorizationRequirementExtensions
    {
        public static AuthorizationPolicyBuilder RequireScope(
            this AuthorizationPolicyBuilder authorizationPolicyBuilder,
            params string[] requiredScopes)
        {
            authorizationPolicyBuilder.RequireScope((IEnumerable<string>) requiredScopes);
            return authorizationPolicyBuilder;
        }
 
        public static AuthorizationPolicyBuilder RequireScope(
            this AuthorizationPolicyBuilder authorizationPolicyBuilder,
            IEnumerable<string> requiredScopes)
        {
            authorizationPolicyBuilder.AddRequirements(new ScopeAuthorizationRequirement(requiredScopes));
            return authorizationPolicyBuilder;
        }
    }
}