using Application.Interfaces.Common;
using Core.DTOs.Security.Roles;

namespace Application.Interfaces.Configuration;

/// <summary>
/// Service for managing roles
/// </summary>
public interface IRoleService : IBaseService<RoleDto, CreateRoleDto, UpdateRoleDto>
{
    /// <summary>
    /// Get role by code
    /// </summary>
    Task<RoleDto?> GetByCodeAsync(string code);
    
    /// <summary>
    /// Get role details with permissions and assignments
    /// </summary>
    Task<RoleDetailDto?> GetDetailsAsync(Guid id);
    
    /// <summary>
    /// Get active roles
    /// </summary>
    Task<IEnumerable<RoleDto>> GetActiveAsync();
    
    /// <summary>
    /// Get roles by type
    /// </summary>
    Task<IEnumerable<RoleDto>> GetByTypeAsync(string type);
    
    /// <summary>
    /// Search roles
    /// </summary>
    Task<IEnumerable<RoleDto>> SearchAsync(RoleFilterDto filter);
    
    /// <summary>
    /// Assign permissions to role
    /// </summary>
    Task<bool> AssignPermissionsAsync(Guid roleId, AssignPermissionsToRoleDto dto);
    
    /// <summary>
    /// Remove permission from role
    /// </summary>
    Task<bool> RemovePermissionAsync(Guid roleId, Guid permissionId);
    
    /// <summary>
    /// Get role permissions
    /// </summary>
    Task<IEnumerable<RolePermissionDto>> GetPermissionsAsync(Guid roleId);
    
    /// <summary>
    /// Get all permissions (including inherited)
    /// </summary>
    Task<IEnumerable<RolePermissionDto>> GetAllPermissionsAsync(Guid roleId);
    
    /// <summary>
    /// Get permission tree
    /// </summary>
    Task<RolePermissionTreeDto> GetPermissionTreeAsync(Guid roleId);
    
    /// <summary>
    /// Assign role to user
    /// </summary>
    Task<bool> AssignToUserAsync(AssignRoleToUserDto dto);
    
    /// <summary>
    /// Remove role from user
    /// </summary>
    Task<bool> RemoveFromUserAsync(Guid userId, Guid roleId, Guid? projectId = null);
    
    /// <summary>
    /// Bulk assign role to users
    /// </summary>
    Task<int> BulkAssignAsync(BulkRoleAssignmentDto dto);
    
    /// <summary>
    /// Get role assignments
    /// </summary>
    Task<IEnumerable<UserRoleAssignmentDto>> GetAssignmentsAsync(Guid roleId);
    
    /// <summary>
    /// Activate role
    /// </summary>
    Task<bool> ActivateAsync(Guid id);
    
    /// <summary>
    /// Deactivate role
    /// </summary>
    Task<bool> DeactivateAsync(Guid id);
    
    /// <summary>
    /// Set parent role
    /// </summary>
    Task<bool> SetParentRoleAsync(Guid roleId, Guid? parentRoleId);
    
    /// <summary>
    /// Get child roles
    /// </summary>
    Task<IEnumerable<RoleDto>> GetChildRolesAsync(Guid parentRoleId);
    
    /// <summary>
    /// Clone role
    /// </summary>
    Task<RoleDto?> CloneAsync(Guid sourceId, string newCode, string newName);
    
    /// <summary>
    /// Check if user has permission
    /// </summary>
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, Guid? projectId = null);
    
    /// <summary>
    /// Get user effective permissions
    /// </summary>
    Task<IEnumerable<RolePermissionDto>> GetUserPermissionsAsync(Guid userId, Guid? projectId = null);
    
    /// <summary>
    /// Check if code is unique
    /// </summary>
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
    
    /// <summary>
    /// Export roles
    /// </summary>
    Task<byte[]> ExportRolesAsync(RoleFilterDto? filter = null);
    
    /// <summary>
    /// Import roles
    /// </summary>
    Task<int> ImportRolesAsync(byte[] data, bool overwriteExisting = false);
}