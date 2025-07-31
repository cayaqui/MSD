using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Auth;
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    Task<bool> IsInRoleAsync(string role);
    Task<bool> HasPermissionAsync(string permission);
    Task<bool> HasProjectAccessAsync(Guid projectId, string? requiredRole = null);
    Task<string?> GetProjectRoleAsync(Guid projectId);
    Task<List<Guid>> GetUserProjectIdsAsync();
}
