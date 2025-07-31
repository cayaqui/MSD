namespace Core.Constants;

/// <summary>
/// Defines project roles and their associated permissions within the system.
/// Based on simplified project-level access control (Option A).
/// </summary>
public static class ProjectRoles
{
    // Role Constants
    public const string ProjectManager = "ProjectManager";
    public const string ProjectController = "ProjectController";
    public const string CostController = "CostController";
    public const string SchedController = "SchedController";
    public const string TeamLead = "TeamLead";
    public const string TeamMember = "TeamMember";
    public const string Viewer = "Viewer";


    /// <summary>
    /// All available roles in hierarchical order (highest to lowest)
    /// </summary>
    public static readonly IReadOnlyList<string> AllRoles = new[]
    {
        ProjectManager,
        ProjectController,
        CostController,
        SchedController,
        TeamLead,
        TeamMember,
        Viewer
    };

    /// <summary>
    /// Role hierarchy levels for comparison
    /// </summary>
    public static readonly IReadOnlyDictionary<string, int> RoleHierarchy = new Dictionary<string, int>
    {
        { Viewer, 1 },
        { TeamMember, 2 },
        { TeamLead, 3 },
        { ProjectManager, 4 }
    };

    /// <summary>
    /// Permission definitions for each role
    /// </summary>
    public static class Permissions
    {
        // Project Manager permissions (full access)
        public static readonly IReadOnlyList<string> ProjectManagerPermissions = new[]
        {
            // Project Management
            "project.view",
            "project.edit",
            "project.delete",
            "project.close",
            "project.manage_team",
            "project.manage_settings",
            
            // Budget & Cost
            "budget.view",
            "budget.create",
            "budget.edit",
            "budget.delete",
            "budget.approve",
            "cost.view",
            "cost.create",
            "cost.edit",
            "cost.approve",
            
            // Schedule
            "schedule.view",
            "schedule.create",
            "schedule.edit",
            "schedule.delete",
            "schedule.approve",
            "schedule.baseline",
            
            // Contracts & Procurement
            "contract.view",
            "contract.create",
            "contract.edit",
            "contract.approve",
            "purchase_order.create",
            "purchase_order.approve",
            
            // Documents
            "document.view",
            "document.upload",
            "document.edit",
            "document.delete",
            "document.approve",
            
            // Reports
            "report.view",
            "report.generate",
            "report.export",
            
            // Risk Management
            "risk.view",
            "risk.create",
            "risk.edit",
            "risk.delete"
        };

        // Team Lead permissions (management without deletion)
        public static readonly IReadOnlyList<string> TeamLeadPermissions = new[]
        {
            // Project Management
            "project.view",
            "project.edit",
            
            // Budget & Cost
            "budget.view",
            "budget.create",
            "budget.edit",
            "cost.view",
            "cost.create",
            "cost.edit",
            
            // Schedule
            "schedule.view",
            "schedule.create",
            "schedule.edit",
            "schedule.update_progress",
            
            // Contracts & Procurement
            "contract.view",
            "contract.create",
            "contract.edit",
            "purchase_order.create",
            
            // Documents
            "document.view",
            "document.upload",
            "document.edit",
            
            // Reports
            "report.view",
            "report.generate",
            
            // Risk Management
            "risk.view",
            "risk.create",
            "risk.edit"
        };

        // Team Member permissions (operational access)
        public static readonly IReadOnlyList<string> TeamMemberPermissions = new[]
        {
            // Project Management
            "project.view",
            "project.edit",
            
            // Budget & Cost
            "budget.view",
            "cost.view",
            
            // Schedule
            "schedule.view",
            "schedule.update_progress",
            
            // Documents
            "document.view",
            "document.upload",
            
            // Reports
            "report.view"
        };

        // Viewer permissions (read-only access)
        public static readonly IReadOnlyList<string> ViewerPermissions = new[]
        {
            // Project Management
            "project.view",
            
            // Budget & Cost
            "budget.view",
            "cost.view",
            
            // Schedule
            "schedule.view",
            
            // Contracts
            "contract.view",
            
            // Documents
            "document.view",
            
            // Reports
            "report.view"
        };
    }

    /// <summary>
    /// Gets the permissions associated with a specific role
    /// </summary>
    public static IReadOnlyList<string> GetPermissionsForRole(string role)
    {
        return role switch
        {
            ProjectManager => Permissions.ProjectManagerPermissions,
            TeamLead => Permissions.TeamLeadPermissions,
            TeamMember => Permissions.TeamMemberPermissions,
            Viewer => Permissions.ViewerPermissions,
            _ => Array.Empty<string>()
        };
    }

    /// <summary>
    /// Checks if a role has a specific permission
    /// </summary>
    public static bool RoleHasPermission(string role, string permission)
    {
        var permissions = GetPermissionsForRole(role);
        return permissions.Contains(permission);
    }

    /// <summary>
    /// Checks if one role is higher than or equal to another in the hierarchy
    /// </summary>
    public static bool IsRoleHigherOrEqual(string userRole, string requiredRole)
    {
        if (!RoleHierarchy.TryGetValue(userRole, out var userLevel) ||
            !RoleHierarchy.TryGetValue(requiredRole, out var requiredLevel))
        {
            return false;
        }

        return userLevel >= requiredLevel;
    }

    /// <summary>
    /// Gets the display name for a role (for UI)
    /// </summary>
    public static string GetRoleDisplayName(string role)
    {
        return role switch
        {
            ProjectManager => "Project Manager",
            TeamLead => "Team Lead",
            TeamMember => "Team Member",
            Viewer => "Viewer",
            _ => role
        };
    }

    /// <summary>
    /// Gets the description for a role (for UI tooltips/help)
    /// </summary>
    public static string GetRoleDescription(string role)
    {
        return role switch
        {
            ProjectManager => "Full control over the project including team management, budget approval, and project closure",
            TeamLead => "Can manage project activities, create budgets and schedules, but cannot delete or close projects",
            TeamMember => "Can view project information and update progress on assigned tasks",
            Viewer => "Read-only access to project information",
            _ => "Unknown role"
        };
    }
}