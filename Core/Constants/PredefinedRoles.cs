namespace Core.Constants;

/// <summary>
/// Predefined role configurations
/// </summary>
public static class PredefinedRoles
{
    public static class Admin
    {
        public const string Name = "Admin";
        public const string Description = "System Administrator with full access";

        // Admin has ALL permissions
        public static readonly string[] Permissions = GetAllPermissions();
    }

    public static class Support
    {
        public const string Name = "Soporte";
        public const string Description = "Support team with read access and user management";

        public static readonly string[] Permissions = new[]
        {
            // View all setup
            SystemPermissions.Setup.ViewCompany,
            SystemPermissions.Setup.ViewOperation,
            SystemPermissions.Setup.ViewProject,
            SystemPermissions.Setup.ViewPhase,
            SystemPermissions.Setup.ViewWorkPackage,
            SystemPermissions.Setup.ViewDiscipline,
            SystemPermissions.Setup.ViewContractor,
            
            // View all cost
            SystemPermissions.Cost.ViewBudget,
            SystemPermissions.Cost.ViewTrend,
            SystemPermissions.Cost.ViewCommitment,
            SystemPermissions.Cost.ViewInvoice,
            SystemPermissions.Cost.ViewContingency,
            
            // View all progress
            SystemPermissions.Progress.ViewSchedule,
            SystemPermissions.Progress.ViewActualProgress,
            SystemPermissions.Progress.ViewDeliverable,
            SystemPermissions.Progress.ViewRFQ,
            SystemPermissions.Progress.ViewPurchaseOrder,
            
            // All reports
            SystemPermissions.Reports.ViewDashboard,
            SystemPermissions.Reports.ViewMonthlyReport,
            SystemPermissions.Reports.ViewWeeklyReport,
            SystemPermissions.Reports.ViewDailyReport,
            SystemPermissions.Reports.ExportReports,
            SystemPermissions.Reports.ViewAnalytics,
            
            // User management (no delete)
            SystemPermissions.Admin.ViewUser,
            SystemPermissions.Admin.CreateUser,
            SystemPermissions.Admin.EditUser,
            SystemPermissions.Admin.AssignUserRoles,
            SystemPermissions.Admin.ViewRole,
            SystemPermissions.Admin.ViewAuditLog,
            SystemPermissions.Admin.ViewSystemSettings
        };
    }

    public static class User
    {
        public const string Name = "User";
        public const string Description = "Standard user with basic access";

        public static readonly string[] Permissions = new[]
        {
            // Basic view permissions
            SystemPermissions.Setup.ViewProject,
            SystemPermissions.Setup.ViewPhase,
            SystemPermissions.Setup.ViewWorkPackage,
            SystemPermissions.Setup.ViewDiscipline,
            SystemPermissions.Setup.ViewContractor,
            
            // View cost information
            SystemPermissions.Cost.ViewBudget,
            SystemPermissions.Cost.ViewCommitment,
            SystemPermissions.Cost.ViewInvoice,
            
            // View progress
            SystemPermissions.Progress.ViewSchedule,
            SystemPermissions.Progress.ViewActualProgress,
            SystemPermissions.Progress.ViewDeliverable,
            
            // Reports
            SystemPermissions.Reports.ViewDashboard,
            SystemPermissions.Reports.ViewWeeklyReport,
            SystemPermissions.Reports.ViewDailyReport
        };
    }

    // Additional specialized roles
    public static class ProjectManager
    {
        public const string Name = "ProjectManager";
        public const string Description = "Project Manager with full project control";

        public static readonly string[] Permissions = new[]
        {
            // Full project management
            SystemPermissions.Setup.ViewProject,
            SystemPermissions.Setup.EditProject,
            SystemPermissions.Setup.AssignProjectTeam,
            SystemPermissions.Setup.ViewPhase,
            SystemPermissions.Setup.CreatePhase,
            SystemPermissions.Setup.EditPhase,
            SystemPermissions.Setup.DeletePhase,
            SystemPermissions.Setup.ViewWorkPackage,
            SystemPermissions.Setup.CreateWorkPackage,
            SystemPermissions.Setup.EditWorkPackage,
            SystemPermissions.Setup.ApproveWorkPackage,
            
            // Cost management
            SystemPermissions.Cost.ViewBudget,
            SystemPermissions.Cost.CreateBudget,
            SystemPermissions.Cost.EditBudget,
            SystemPermissions.Cost.ApproveBudget,
            SystemPermissions.Cost.ViewTrend,
            SystemPermissions.Cost.CreateTrend,
            SystemPermissions.Cost.ApproveTrend,
            SystemPermissions.Cost.ViewCommitment,
            SystemPermissions.Cost.CreateCommitment,
            SystemPermissions.Cost.ApproveCommitment,
            SystemPermissions.Cost.ViewInvoice,
            SystemPermissions.Cost.ApproveInvoice,
            SystemPermissions.Cost.ViewContingency,
            SystemPermissions.Cost.EditContingency,
            
            // Progress management
            SystemPermissions.Progress.ViewSchedule,
            SystemPermissions.Progress.CreateSchedule,
            SystemPermissions.Progress.EditSchedule,
            SystemPermissions.Progress.ApproveSchedule,
            SystemPermissions.Progress.ImportSchedule,
            SystemPermissions.Progress.ViewActualProgress,
            SystemPermissions.Progress.EditActualProgress,
            SystemPermissions.Progress.ApproveActualProgress,
            SystemPermissions.Progress.ViewDeliverable,
            SystemPermissions.Progress.CreateDeliverable,
            SystemPermissions.Progress.ApproveDeliverable,
            
            // All reports
            SystemPermissions.Reports.ViewDashboard,
            SystemPermissions.Reports.ViewMonthlyReport,
            SystemPermissions.Reports.ViewWeeklyReport,
            SystemPermissions.Reports.ViewDailyReport,
            SystemPermissions.Reports.ExportReports,
            SystemPermissions.Reports.ViewAnalytics
        };
    }

    public static class CostController
    {
        public const string Name = "CostController";
        public const string Description = "Cost Controller with financial management access";

        public static readonly string[] Permissions = new[]
        {
            // View project info
            SystemPermissions.Setup.ViewProject,
            SystemPermissions.Setup.ViewPhase,
            SystemPermissions.Setup.ViewWorkPackage,
            
            // Full cost management
            SystemPermissions.Cost.ViewBudget,
            SystemPermissions.Cost.CreateBudget,
            SystemPermissions.Cost.EditBudget,
            SystemPermissions.Cost.DeleteBudget,
            SystemPermissions.Cost.ApproveBudget,
            SystemPermissions.Cost.LockBudget,
            SystemPermissions.Cost.ViewTrend,
            SystemPermissions.Cost.CreateTrend,
            SystemPermissions.Cost.EditTrend,
            SystemPermissions.Cost.DeleteTrend,
            SystemPermissions.Cost.ApproveTrend,
            SystemPermissions.Cost.ViewCommitment,
            SystemPermissions.Cost.CreateCommitment,
            SystemPermissions.Cost.EditCommitment,
            SystemPermissions.Cost.DeleteCommitment,
            SystemPermissions.Cost.ApproveCommitment,
            SystemPermissions.Cost.ViewInvoice,
            SystemPermissions.Cost.CreateInvoice,
            SystemPermissions.Cost.EditInvoice,
            SystemPermissions.Cost.DeleteInvoice,
            SystemPermissions.Cost.ApproveInvoice,
            SystemPermissions.Cost.ViewContingency,
            SystemPermissions.Cost.EditContingency,
            SystemPermissions.Cost.ApproveContingency,
            
            // Financial reports
            SystemPermissions.Reports.ViewDashboard,
            SystemPermissions.Reports.ViewMonthlyReport,
            SystemPermissions.Reports.ExportReports,
            SystemPermissions.Reports.ViewAnalytics
        };
    }

    public static class Viewer
    {
        public const string Name = "Viewer";
        public const string Description = "Read-only access to project information";

        public static readonly string[] Permissions = new[]
        {
            // View only permissions
            SystemPermissions.Setup.ViewProject,
            SystemPermissions.Setup.ViewPhase,
            SystemPermissions.Setup.ViewWorkPackage,
            SystemPermissions.Cost.ViewBudget,
            SystemPermissions.Progress.ViewSchedule,
            SystemPermissions.Progress.ViewActualProgress,
            SystemPermissions.Reports.ViewDashboard
        };
    }

    // Helper method to get all permissions
    private static string[] GetAllPermissions()
    {
        var allPermissions = new List<string>();

        // Use reflection to get all const string fields from SystemPermissions
        var permissionTypes = typeof(SystemPermissions).GetNestedTypes();

        foreach (var type in permissionTypes)
        {
            var fields = type.GetFields(System.Reflection.BindingFlags.Public |
                                       System.Reflection.BindingFlags.Static |
                                       System.Reflection.BindingFlags.FlattenHierarchy)
                            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = field.GetValue(null) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    allPermissions.Add(value);
                }
            }
        }

        return allPermissions.ToArray();
    }
}
