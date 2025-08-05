using Core.DTOs.Auth.Permissions;

namespace Application.Interfaces.Auth;

/// <summary>
/// Service for managing permissions
/// </summary>
public interface IPermissionService:IBaseService<PermissionDto, CreatePermissionDto, UpdatePermissionDto>
{
       
    /// <summary>
    /// Get permission by code
    /// </summary>
    Task<PermissionDto?> GetByCodeAsync(string code);
    
    /// <summary>
    /// Get permissions by module
    /// </summary>
    Task<IEnumerable<PermissionDto>> GetByModuleAsync(string module);
    
    /// <summary>
    /// Get permissions by resource
    /// </summary>
    Task<IEnumerable<PermissionDto>> GetByResourceAsync(string module, string resource);
    
    /// <summary>
    /// Get permission matrix
    /// </summary>
    Task<PermissionMatrixDto> GetMatrixAsync();
    
    /// <summary>
    /// Search permissions
    /// </summary>
    Task<IEnumerable<PermissionDto>> SearchAsync(PermissionFilterDto filter);
    
    /// <summary>
    /// Activate permission
    /// </summary>
    Task<bool> ActivateAsync(Guid id);
    
    /// <summary>
    /// Deactivate permission
    /// </summary>
    Task<bool> DeactivateAsync(Guid id);
    
    /// <summary>
    /// Bulk assign permissions to roles
    /// </summary>
    Task<int> BulkAssignToRolesAsync(BulkPermissionAssignmentDto dto);
    
    
    /// <summary>
    /// Check if permission code is unique
    /// </summary>
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
    
    /// <summary>
    /// Initialize default permissions
    /// </summary>
    Task<int> InitializeDefaultPermissionsAsync();
    
    /// <summary>
    /// Export permissions
    /// </summary>
    Task<byte[]> ExportAsync(PermissionFilterDto? filter = null);
    
    /// <summary>
    /// Import permissions
    /// </summary>
    Task<int> ImportAsync(byte[] data, bool overwriteExisting = false);
    
    /// <summary>
    /// Validate permission structure
    /// </summary>
    Task<PermissionValidationResult> ValidateStructureAsync();
}
