namespace Core.Constants;

/// <summary>
/// Permission constants for the application
/// </summary>
public static class PermissionConstants
{
    public static class System
    {
        public const string ViewConfiguration = "system.configuration.view";
        public const string EditConfiguration = "system.configuration.edit";
        public const string ViewUsers = "system.users.view";
        public const string ManageUsers = "system.users.manage";
        public const string ViewRoles = "system.roles.view";
        public const string ManageRoles = "system.roles.manage";
        public const string ViewPermissions = "system.permissions.view";
        public const string ManagePermissions = "system.permissions.manage";
        
        public static readonly string[] All = 
        { 
            ViewConfiguration, EditConfiguration, 
            ViewUsers, ManageUsers, 
            ViewRoles, ManageRoles, 
            ViewPermissions, ManagePermissions 
        };
    }
    
    public static class Projects
    {
        public const string View = "project.view";
        public const string Create = "project.create";
        public const string Edit = "project.edit";
        public const string Delete = "project.delete";
        public const string ViewTeam = "project.team.view";
        public const string ManageTeam = "project.team.manage";
        public const string ViewSettings = "project.settings.view";
        public const string ManageSettings = "project.settings.manage";
        
        public static readonly string[] All = 
        { 
            View, Create, Edit, Delete, 
            ViewTeam, ManageTeam, 
            ViewSettings, ManageSettings 
        };
        
        public static readonly string[] ReadOnly = { View, ViewTeam, ViewSettings };
    }
    
    public static class Budget
    {
        public const string View = "budget.view";
        public const string Create = "budget.create";
        public const string Edit = "budget.edit";
        public const string Delete = "budget.delete";
        public const string Approve = "budget.approve";
        public const string Export = "budget.export";
        
        public static readonly string[] All = { View, Create, Edit, Delete, Approve, Export };
        public static readonly string[] ReadOnly = { View, Export };
    }
    
    public static class Cost
    {
        public const string View = "cost.view";
        public const string Create = "cost.create";
        public const string Edit = "cost.edit";
        public const string Delete = "cost.delete";
        public const string Approve = "cost.approve";
        public const string RecordActuals = "cost.actuals.record";
        public const string ViewReports = "cost.reports.view";
        
        public static readonly string[] All = 
        { 
            View, Create, Edit, Delete, Approve, 
            RecordActuals, ViewReports 
        };
        
        public static readonly string[] ReadOnly = { View, ViewReports };
    }
    
    public static class Schedule
    {
        public const string View = "schedule.view";
        public const string Create = "schedule.create";
        public const string Edit = "schedule.edit";
        public const string Delete = "schedule.delete";
        public const string Approve = "schedule.approve";
        public const string UpdateProgress = "schedule.progress.update";
        
        public static readonly string[] All = 
        { 
            View, Create, Edit, Delete, Approve, UpdateProgress 
        };
        
        public static readonly string[] ReadOnly = { View };
    }
    
    public static class Documents
    {
        public const string View = "document.view";
        public const string Create = "document.create";
        public const string Edit = "document.edit";
        public const string Delete = "document.delete";
        public const string Approve = "document.approve";
        public const string Download = "document.download";
        public const string Distribute = "document.distribute";
        
        public static readonly string[] All = 
        { 
            View, Create, Edit, Delete, Approve, 
            Download, Distribute 
        };
        
        public static readonly string[] ReadOnly = { View, Download };
    }
    
    public static class Reports
    {
        public const string ViewDashboard = "report.dashboard.view";
        public const string ViewExecutive = "report.executive.view";
        public const string ViewProject = "report.project.view";
        public const string ViewKPI = "report.kpi.view";
        public const string Create = "report.create";
        public const string Export = "report.export";
        public const string Schedule = "report.schedule";
        
        public static readonly string[] All = 
        { 
            ViewDashboard, ViewExecutive, ViewProject, ViewKPI, 
            Create, Export, Schedule 
        };
        
        public static readonly string[] ReadOnly = 
        { 
            ViewDashboard, ViewProject, Export 
        };
    }
    
    public static class Risk
    {
        public const string View = "risk.view";
        public const string Create = "risk.create";
        public const string Edit = "risk.edit";
        public const string Delete = "risk.delete";
        public const string Assess = "risk.assess";
        public const string MitigationPlan = "risk.mitigation.plan";
        
        public static readonly string[] All = 
        { 
            View, Create, Edit, Delete, Assess, MitigationPlan 
        };
        
        public static readonly string[] ReadOnly = { View };
    }
    
    public static class Quality
    {
        public const string View = "quality.view";
        public const string Create = "quality.create";
        public const string Edit = "quality.edit";
        public const string Delete = "quality.delete";
        public const string Inspect = "quality.inspect";
        public const string ApproveNCR = "quality.ncr.approve";
        
        public static readonly string[] All = 
        { 
            View, Create, Edit, Delete, Inspect, ApproveNCR 
        };
        
        public static readonly string[] ReadOnly = { View };
    }
    
    public static class Contracts
    {
        public const string View = "contract.view";
        public const string Create = "contract.create";
        public const string Edit = "contract.edit";
        public const string Delete = "contract.delete";
        public const string Approve = "contract.approve";
        public const string ManageChangeOrders = "contract.changeorders.manage";
        public const string ApproveClaims = "contract.claims.approve";
        
        public static readonly string[] All = 
        { 
            View, Create, Edit, Delete, Approve, 
            ManageChangeOrders, ApproveClaims 
        };
        
        public static readonly string[] ReadOnly = { View };
    }
    
    /// <summary>
    /// Get permissions for a specific role
    /// </summary>
    public static string[] GetPermissionsForRole(string role)
    {
        return role switch
        {
            ProjectRoles.ProjectManager => GetProjectManagerPermissions(),
            ProjectRoles.ProjectEngineer => GetProjectControllerPermissions(), // PROJECT_ENGINEER uses controller permissions
            ProjectRoles.CostController => GetProjectControllerPermissions(),
            ProjectRoles.Planner => GetTeamLeadPermissions(),
            ProjectRoles.TeamMember => GetTeamMemberPermissions(),
            ProjectRoles.Observer => GetViewerPermissions(),
            _ => GetViewerPermissions()
        };
    }
    
    private static string[] GetProjectManagerPermissions()
    {
        var permissions = new List<string>();
        permissions.AddRange(Projects.All);
        permissions.AddRange(Budget.All);
        permissions.AddRange(Cost.All);
        permissions.AddRange(Schedule.All);
        permissions.AddRange(Documents.All);
        permissions.AddRange(Reports.All);
        permissions.AddRange(Risk.All);
        permissions.AddRange(Quality.All);
        permissions.AddRange(Contracts.All);
        return permissions.ToArray();
    }
    
    private static string[] GetProjectControllerPermissions()
    {
        var permissions = new List<string>();
        permissions.AddRange(Projects.ReadOnly);
        permissions.Add(Projects.ViewTeam);
        permissions.AddRange(Budget.All);
        permissions.AddRange(Cost.All);
        permissions.AddRange(Schedule.ReadOnly);
        permissions.Add(Schedule.UpdateProgress);
        permissions.AddRange(Documents.ReadOnly);
        permissions.AddRange(Reports.All);
        permissions.Add(Risk.View);
        permissions.Add(Risk.Create);
        permissions.Add(Risk.Assess);
        permissions.AddRange(Quality.ReadOnly);
        permissions.AddRange(Contracts.ReadOnly);
        return permissions.ToArray();
    }
    
    private static string[] GetTeamLeadPermissions()
    {
        var permissions = new List<string>();
        permissions.AddRange(Projects.ReadOnly);
        permissions.AddRange(Budget.ReadOnly);
        permissions.AddRange(Cost.ReadOnly);
        permissions.Add(Cost.RecordActuals);
        permissions.AddRange(Schedule.ReadOnly);
        permissions.Add(Schedule.UpdateProgress);
        permissions.AddRange(Documents.All);
        permissions.AddRange(Reports.ReadOnly);
        permissions.Add(Risk.View);
        permissions.Add(Risk.Create);
        permissions.AddRange(Quality.ReadOnly);
        permissions.Add(Quality.Create);
        return permissions.ToArray();
    }
    
    private static string[] GetTeamMemberPermissions()
    {
        var permissions = new List<string>();
        permissions.Add(Projects.View);
        permissions.AddRange(Budget.ReadOnly);
        permissions.AddRange(Cost.ReadOnly);
        permissions.Add(Schedule.View);
        permissions.AddRange(Documents.ReadOnly);
        permissions.Add(Documents.Create);
        permissions.Add(Reports.ViewDashboard);
        permissions.Add(Reports.ViewProject);
        permissions.Add(Risk.View);
        permissions.Add(Quality.View);
        return permissions.ToArray();
    }
    
    private static string[] GetViewerPermissions()
    {
        var permissions = new List<string>();
        permissions.Add(Projects.View);
        permissions.Add(Budget.View);
        permissions.Add(Cost.View);
        permissions.Add(Schedule.View);
        permissions.Add(Documents.View);
        permissions.Add(Reports.ViewDashboard);
        return permissions.ToArray();
    }
}