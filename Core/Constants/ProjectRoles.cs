namespace Core.Constants;

/// <summary>
/// Defines project roles and their associated permissions within the system.
/// Based on simplified project-level access control (Option A).
/// </summary>
public static class ProjectRoles
{
    // Role Constants - Must match database CHECK constraint values
    public const string ProjectManager = "PROJECT_MANAGER";
    public const string ProjectEngineer = "PROJECT_ENGINEER";
    public const string CostController = "COST_CONTROLLER";
    public const string Planner = "PLANNER";
    public const string QaQc = "QA_QC";
    public const string DocumentController = "DOCUMENT_CONTROLLER";
    public const string TeamMember = "TEAM_MEMBER";
    public const string Observer = "OBSERVER";
    
    // Legacy aliases for backward compatibility - DEPRECATED
    // These are kept only for compilation compatibility and should be migrated
    public const string ProjectController = "PROJECT_CONTROLLER_LEGACY"; // Should use ProjectEngineer
    public const string SchedController = "SCHED_CONTROLLER_LEGACY"; // Should use Planner
    public const string TeamLead = "TEAM_LEAD_LEGACY"; // Should use ProjectEngineer
    public const string Viewer = "VIEWER_LEGACY"; // Should use Observer


    /// <summary>
    /// All available roles in hierarchical order (highest to lowest)
    /// </summary>
    public static readonly IReadOnlyList<string> AllRoles = new[]
    {
        ProjectManager,      // PROJECT_MANAGER
        ProjectEngineer,     // PROJECT_ENGINEER
        CostController,      // COST_CONTROLLER
        Planner,            // PLANNER
        QaQc,               // QA_QC
        DocumentController,  // DOCUMENT_CONTROLLER
        TeamMember,         // TEAM_MEMBER
        Observer            // OBSERVER
    };

    /// <summary>
    /// Role hierarchy levels for comparison
    /// </summary>
    public static readonly IReadOnlyDictionary<string, int> RoleHierarchy = new Dictionary<string, int>
    {
        { Observer, 1 },              // OBSERVER - lowest level
        { TeamMember, 2 },            // TEAM_MEMBER
        { DocumentController, 3 },     // DOCUMENT_CONTROLLER
        { QaQc, 3 },                  // QA_QC
        { Planner, 4 },               // PLANNER
        { CostController, 4 },        // COST_CONTROLLER
        { ProjectEngineer, 5 },       // PROJECT_ENGINEER
        { ProjectManager, 6 }         // PROJECT_MANAGER - highest level
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
            ProjectEngineer => Permissions.TeamLeadPermissions, // PROJECT_ENGINEER gets TeamLead permissions
            CostController => Permissions.TeamLeadPermissions,  // Cost Controller gets enhanced permissions
            Planner => Permissions.TeamLeadPermissions,         // Planner gets enhanced permissions
            QaQc => Permissions.TeamMemberPermissions,          // QA/QC gets team member permissions
            DocumentController => Permissions.TeamMemberPermissions, // Doc Controller gets team member permissions
            TeamMember => Permissions.TeamMemberPermissions,
            Observer => Permissions.ViewerPermissions,
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
            ProjectEngineer => "Project Engineer",
            CostController => "Cost Controller",
            Planner => "Planner/Scheduler",
            QaQc => "QA/QC Manager",
            DocumentController => "Document Controller",
            TeamMember => "Team Member",
            Observer => "Observer",
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
            ProjectEngineer => "Technical lead with ability to manage project activities, create budgets and schedules",
            CostController => "Manages project costs, budgets, and financial reporting",
            Planner => "Manages project schedules, timelines, and resource planning",
            QaQc => "Manages quality assurance and quality control processes",
            DocumentController => "Manages project documentation and document control processes",
            TeamMember => "Can view project information and update progress on assigned tasks",
            Observer => "Read-only access to project information",
            _ => "Unknown role"
        };
    }
}