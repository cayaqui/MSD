using Application.Interfaces.Auth;
using Domain.Entities.Auth.Security;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Authorization;

/// <summary>
/// Handles authorization for system admins or users with specific roles
/// </summary>
public class SystemAdminOrRoleHandler : AuthorizationHandler<SystemAdminOrRoleRequirement>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SystemAdminOrRoleHandler> _logger;

    public SystemAdminOrRoleHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<SystemAdminOrRoleHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SystemAdminOrRoleRequirement requirement)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
        {
            _logger.LogDebug("User is not authenticated");
            return;
        }

        // Get user from database
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

        if (user == null)
        {
            _logger.LogDebug("User not found in database: {UserId}", _currentUserService.UserId);
            return;
        }

        // Check if user is system admin (support user)
        if (user.IsSupport())
        {
            _logger.LogDebug("User {Email} is system admin, granting access", user.Email);
            context.Succeed(requirement);
            return;
        }

        // Check if user has the required role in any active project
        var hasRole = user.ProjectTeamMembers
            .Any(ptm => ptm.IsActive && 
                       (ptm.Role == requirement.RequiredRole || 
                        ptm.Role.Equals(requirement.RequiredRole, StringComparison.OrdinalIgnoreCase)));

        if (hasRole)
        {
            _logger.LogDebug("User has required role {Role}", requirement.RequiredRole);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogDebug("User does not have required role {Role} and is not system admin", requirement.RequiredRole);
        }
    }
}