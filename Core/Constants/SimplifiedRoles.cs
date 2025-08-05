namespace Core.Constants;

/// <summary>
/// Simplified role constants for the application
/// Replaces the complex permission system with a straightforward role-based approach
/// </summary>
public static class SimplifiedRoles
{
    /// <summary>
    /// System-wide roles (not project-specific)
    /// </summary>
    public static class System
    {
        /// <summary>
        /// Full system access - can do everything
        /// </summary>
        public const string Admin = "ADMIN";
        
        /// <summary>
        /// Full system access + configuration capabilities
        /// </summary>
        public const string Support = "SUPPORT";
        
        /// <summary>
        /// All system roles
        /// </summary>
        public static readonly string[] All = { Admin, Support };
    }
    
    /// <summary>
    /// Project-specific roles
    /// </summary>
    public static class Project
    {
        /// <summary>
        /// Full control over the project
        /// </summary>
        public const string ProjectManager = "PROJECT_MANAGER";
        
        /// <summary>
        /// Can manage project activities but cannot delete
        /// </summary>
        public const string TeamLead = "TEAM_LEAD";
        
        /// <summary>
        /// Can view and update assigned work
        /// </summary>
        public const string TeamMember = "TEAM_MEMBER";
        
        /// <summary>
        /// Read-only access to project
        /// </summary>
        public const string Viewer = "VIEWER";
        
        /// <summary>
        /// All project roles in hierarchical order
        /// </summary>
        public static readonly string[] All = { ProjectManager, TeamLead, TeamMember, Viewer };
        
        /// <summary>
        /// Roles that can modify project data
        /// </summary>
        public static readonly string[] CanEdit = { ProjectManager, TeamLead, TeamMember };
        
        /// <summary>
        /// Roles that can only view project data
        /// </summary>
        public static readonly string[] ReadOnly = { Viewer };
    }
    
    /// <summary>
    /// Check if a role is a system-wide role
    /// </summary>
    public static bool IsSystemRole(string role) =>
        System.All.Contains(role, StringComparer.OrdinalIgnoreCase);
    
    /// <summary>
    /// Check if a role is a project-specific role
    /// </summary>
    public static bool IsProjectRole(string role) =>
        Project.All.Contains(role, StringComparer.OrdinalIgnoreCase);
    
    /// <summary>
    /// Get the hierarchy level of a project role (higher number = more permissions)
    /// </summary>
    public static int GetProjectRoleLevel(string role) => role?.ToUpperInvariant() switch
    {
        Project.ProjectManager => 4,
        Project.TeamLead => 3,
        Project.TeamMember => 2,
        Project.Viewer => 1,
        _ => 0
    };
    
    /// <summary>
    /// Check if a user role has at least the minimum required role level
    /// </summary>
    public static bool HasMinimumProjectRole(string userRole, string requiredRole)
    {
        return GetProjectRoleLevel(userRole) >= GetProjectRoleLevel(requiredRole);
    }
    
    /// <summary>
    /// Get display name for a role
    /// </summary>
    public static string GetDisplayName(string role) => role?.ToUpperInvariant() switch
    {
        System.Admin => "Administrator",
        System.Support => "Support",
        Project.ProjectManager => "Project Manager",
        Project.TeamLead => "Team Lead",
        Project.TeamMember => "Team Member",
        Project.Viewer => "Viewer",
        _ => role ?? "Unknown"
    };
    
    /// <summary>
    /// Get role description
    /// </summary>
    public static string GetDescription(string role) => role?.ToUpperInvariant() switch
    {
        System.Admin => "Full system access with all permissions",
        System.Support => "Full system access plus configuration capabilities",
        Project.ProjectManager => "Complete control over project including team management and approvals",
        Project.TeamLead => "Can manage project activities, create and edit resources",
        Project.TeamMember => "Can view project and update assigned work",
        Project.Viewer => "Read-only access to project information",
        _ => "No description available"
    };
}