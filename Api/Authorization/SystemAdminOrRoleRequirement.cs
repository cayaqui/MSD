using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

/// <summary>
/// Authorization requirement that passes if user is system admin OR has the specified role
/// </summary>
public class SystemAdminOrRoleRequirement : IAuthorizationRequirement
{
    public string RequiredRole { get; }

    public SystemAdminOrRoleRequirement(string requiredRole)
    {
        RequiredRole = requiredRole;
    }
}