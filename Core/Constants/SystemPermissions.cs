namespace Core.Constants;

/// <summary>
/// Predefined permissions for the system
/// </summary>
public static class SystemPermissions
{
    // Setup Permissions
    public static class Setup
    {
        // Company
        public const string ViewCompany = "Setup.Company.View";
        public const string CreateCompany = "Setup.Company.Create";
        public const string EditCompany = "Setup.Company.Edit";
        public const string DeleteCompany = "Setup.Company.Delete";
        public const string AdminCompany = "Setup.Company.Admin";

        // Operation
        public const string ViewOperation = "Setup.Operation.View";
        public const string CreateOperation = "Setup.Operation.Create";
        public const string EditOperation = "Setup.Operation.Edit";
        public const string DeleteOperation = "Setup.Operation.Delete";

        // Project
        public const string ViewProject = "Setup.Project.View";
        public const string CreateProject = "Setup.Project.Create";
        public const string EditProject = "Setup.Project.Edit";
        public const string DeleteProject = "Setup.Project.Delete";
        public const string ApproveProject = "Setup.Project.Approve";
        public const string ArchiveProject = "Setup.Project.Archive";
        public const string AssignProjectTeam = "Setup.Project.Assign";

        // Phase
        public const string ViewPhase = "Setup.Phase.View";
        public const string CreatePhase = "Setup.Phase.Create";
        public const string EditPhase = "Setup.Phase.Edit";
        public const string DeletePhase = "Setup.Phase.Delete";

        // Work Package
        public const string ViewWorkPackage = "Setup.WorkPackage.View";
        public const string CreateWorkPackage = "Setup.WorkPackage.Create";
        public const string EditWorkPackage = "Setup.WorkPackage.Edit";
        public const string DeleteWorkPackage = "Setup.WorkPackage.Delete";
        public const string ApproveWorkPackage = "Setup.WorkPackage.Approve";

        // Discipline
        public const string ViewDiscipline = "Setup.Discipline.View";
        public const string CreateDiscipline = "Setup.Discipline.Create";
        public const string EditDiscipline = "Setup.Discipline.Edit";
        public const string DeleteDiscipline = "Setup.Discipline.Delete";

        // Contractor
        public const string ViewContractor = "Setup.Contractor.View";
        public const string CreateContractor = "Setup.Contractor.Create";
        public const string EditContractor = "Setup.Contractor.Edit";
        public const string DeleteContractor = "Setup.Contractor.Delete";
        public const string ApproveContractor = "Setup.Contractor.Approve";
    }

    // Cost Permissions
    public static class Cost
    {
        // Budget
        public const string ViewBudget = "Cost.Budget.View";
        public const string CreateBudget = "Cost.Budget.Create";
        public const string EditBudget = "Cost.Budget.Edit";
        public const string DeleteBudget = "Cost.Budget.Delete";
        public const string ApproveBudget = "Cost.Budget.Approve";
        public const string LockBudget = "Cost.Budget.Lock";

        // Trend
        public const string ViewTrend = "Cost.Trend.View";
        public const string CreateTrend = "Cost.Trend.Create";
        public const string EditTrend = "Cost.Trend.Edit";
        public const string DeleteTrend = "Cost.Trend.Delete";
        public const string ApproveTrend = "Cost.Trend.Approve";

        // Commitment
        public const string ViewCommitment = "Cost.Commitment.View";
        public const string CreateCommitment = "Cost.Commitment.Create";
        public const string EditCommitment = "Cost.Commitment.Edit";
        public const string DeleteCommitment = "Cost.Commitment.Delete";
        public const string ApproveCommitment = "Cost.Commitment.Approve";

        // Invoice
        public const string ViewInvoice = "Cost.Invoice.View";
        public const string CreateInvoice = "Cost.Invoice.Create";
        public const string EditInvoice = "Cost.Invoice.Edit";
        public const string DeleteInvoice = "Cost.Invoice.Delete";
        public const string ApproveInvoice = "Cost.Invoice.Approve";
        public const string SubmitInvoice = "Cost.Invoice.Submit";

        // Contingency
        public const string ViewContingency = "Cost.Contingency.View";
        public const string EditContingency = "Cost.Contingency.Edit";
        public const string ApproveContingency = "Cost.Contingency.Approve";
    }

    // Progress Permissions
    public static class Progress
    {
        // Schedule
        public const string ViewSchedule = "Progress.Schedule.View";
        public const string CreateSchedule = "Progress.Schedule.Create";
        public const string EditSchedule = "Progress.Schedule.Edit";
        public const string DeleteSchedule = "Progress.Schedule.Delete";
        public const string ApproveSchedule = "Progress.Schedule.Approve";
        public const string ImportSchedule = "Progress.Schedule.Import";

        // Actual Progress
        public const string ViewActualProgress = "Progress.ActualProgress.View";
        public const string EditActualProgress = "Progress.ActualProgress.Edit";
        public const string ApproveActualProgress = "Progress.ActualProgress.Approve";

        // Deliverables
        public const string ViewDeliverable = "Progress.Deliverable.View";
        public const string CreateDeliverable = "Progress.Deliverable.Create";
        public const string EditDeliverable = "Progress.Deliverable.Edit";
        public const string DeleteDeliverable = "Progress.Deliverable.Delete";
        public const string SubmitDeliverable = "Progress.Deliverable.Submit";
        public const string ApproveDeliverable = "Progress.Deliverable.Approve";

        // RFQ
        public const string ViewRFQ = "Progress.RFQ.View";
        public const string CreateRFQ = "Progress.RFQ.Create";
        public const string EditRFQ = "Progress.RFQ.Edit";
        public const string DeleteRFQ = "Progress.RFQ.Delete";
        public const string SubmitRFQ = "Progress.RFQ.Submit";
        public const string ApproveRFQ = "Progress.RFQ.Approve";

        // Purchase Orders
        public const string ViewPurchaseOrder = "Progress.PurchaseOrder.View";
        public const string CreatePurchaseOrder = "Progress.PurchaseOrder.Create";
        public const string EditPurchaseOrder = "Progress.PurchaseOrder.Edit";
        public const string DeletePurchaseOrder = "Progress.PurchaseOrder.Delete";
        public const string ApprovePurchaseOrder = "Progress.PurchaseOrder.Approve";
    }

    // Reports Permissions
    public static class Reports
    {
        public const string ViewDashboard = "Reports.Dashboard.View";
        public const string ViewMonthlyReport = "Reports.MonthlyReport.View";
        public const string ViewWeeklyReport = "Reports.WeeklyReport.View";
        public const string ViewDailyReport = "Reports.DailyReport.View";
        public const string ExportReports = "Reports.Export.Export";
        public const string ConfigureReports = "Reports.Analytics.Configure";
        public const string ViewAnalytics = "Reports.Analytics.View";
    }

    // Admin Permissions
    public static class Admin
    {
        // Users
        public const string ViewUser = "Admin.User.View";
        public const string CreateUser = "Admin.User.Create";
        public const string EditUser = "Admin.User.Edit";
        public const string DeleteUser = "Admin.User.Delete";
        public const string AssignUserRoles = "Admin.User.Assign";

        // Roles
        public const string ViewRole = "Admin.Role.View";
        public const string CreateRole = "Admin.Role.Create";
        public const string EditRole = "Admin.Role.Edit";
        public const string DeleteRole = "Admin.Role.Delete";
        public const string AssignRolePermissions = "Admin.Role.Assign";

        // System
        public const string ViewAuditLog = "Admin.AuditLog.View";
        public const string ExportAuditLog = "Admin.AuditLog.Export";
        public const string ConfigureSystem = "Admin.SystemSettings.Configure";
        public const string ViewSystemSettings = "Admin.SystemSettings.View";
    }
}
