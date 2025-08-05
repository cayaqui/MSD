using Core.DTOs.Common;

namespace Core.DTOs.Auth.Permissions
{
    /// <summary>
    /// Detailed DTO for Permission with full configuration and metadata
    /// Combines features from Auth and Configuration DTOs
    /// </summary>
    public class PermissionDetailDto : BaseDto
    {
        // Basic properties
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Module/Resource/Action structure
        public string Module { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        
        // Categorization
        public string? Category { get; set; }
        
        // Status and metadata
        public bool IsSystem { get; set; } // System permissions cannot be deleted
        public bool IsActive { get; set; } = true;
        public int RoleCount { get; set; } // Number of roles using this permission
        
        // Runtime context (when used in user context)
        public bool? IsGranted { get; set; }
        public bool? IsInherited { get; set; }
        public string? Source { get; set; } // Source of the permission (role name if inherited)
    }
}