using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

/// <summary>
/// Extension methods for authorization
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Requires that the user is either a system admin OR has the specified role
    /// </summary>
    public static AuthorizationPolicyBuilder RequireRoleOrAdmin(this AuthorizationPolicyBuilder builder, string role)
    {
        return builder.AddRequirements(new SystemAdminOrRoleRequirement(role));
    }
}