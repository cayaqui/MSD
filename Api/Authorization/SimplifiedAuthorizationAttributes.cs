using Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

/// <summary>
/// Attribute to require system role (Admin or Support)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireSystemRoleAttribute : AuthorizeAttribute
{
    public RequireSystemRoleAttribute(params string[] allowedRoles)
    {
        if (allowedRoles == null || allowedRoles.Length == 0)
        {
            throw new ArgumentException("At least one role must be specified");
        }

        // Create a policy name based on the roles
        Policy = $"SystemRole_{string.Join("_", allowedRoles.OrderBy(r => r))}";
    }
}

/// <summary>
/// Attribute to require Admin role specifically
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireAdminAttribute : RequireSystemRoleAttribute
{
    public RequireAdminAttribute() : base(SimplifiedRoles.System.Admin)
    {
    }
}

/// <summary>
/// Attribute to require Support role (or Admin, since Admin has all Support permissions)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireSupportAttribute : RequireSystemRoleAttribute
{
    public RequireSupportAttribute() : base(SimplifiedRoles.System.Admin, SimplifiedRoles.System.Support)
    {
    }
}

/// <summary>
/// Attribute to require minimum project role
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireProjectRoleAttribute : AuthorizeAttribute
{
    public string MinimumRole { get; }

    public RequireProjectRoleAttribute(string minimumRole)
    {
        MinimumRole = minimumRole ?? throw new ArgumentNullException(nameof(minimumRole));
        Policy = $"ProjectRole_{minimumRole}";
    }
}

/// <summary>
/// Policy provider to handle dynamic authorization policies
/// </summary>
public class SimplifiedAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _defaultProvider;

    public SimplifiedAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _defaultProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if it's a system role policy
        if (policyName.StartsWith("SystemRole_"))
        {
            var roles = policyName.Substring("SystemRole_".Length).Split('_');
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new SystemRoleRequirement(roles))
                .Build();
            return policy;
        }

        // Check if it's a project role policy
        if (policyName.StartsWith("ProjectRole_"))
        {
            var role = policyName.Substring("ProjectRole_".Length);
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new ProjectRoleRequirement(role))
                .Build();
            return policy;
        }

        // Fall back to default provider
        return await _defaultProvider.GetPolicyAsync(policyName);
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return await _defaultProvider.GetDefaultPolicyAsync();
    }

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return await _defaultProvider.GetFallbackPolicyAsync();
    }
}

/// <summary>
/// Requirement for system roles
/// </summary>
public class SystemRoleRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public SystemRoleRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
    }
}

/// <summary>
/// Requirement for project roles
/// </summary>
public class ProjectRoleRequirement : IAuthorizationRequirement
{
    public string MinimumRole { get; }

    public ProjectRoleRequirement(string minimumRole)
    {
        MinimumRole = minimumRole ?? throw new ArgumentNullException(nameof(minimumRole));
    }
}

/// <summary>
/// Handler for system role authorization
/// </summary>
public class SystemRoleAuthorizationHandler : AuthorizationHandler<SystemRoleRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public SystemRoleAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SystemRoleRequirement requirement)
    {
        using var scope = _serviceProvider.CreateScope();
        var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

        if (!currentUserService.IsAuthenticated)
            return;

        // Check if user has any of the allowed system roles
        if (requirement.AllowedRoles.Contains(currentUserService.SystemRole))
        {
            context.Succeed(requirement);
        }
    }
}