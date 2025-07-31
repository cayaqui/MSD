namespace Core.Constants;

/// <summary>
/// Permission-related constants for the Core/Web layer
/// These mirror the Domain constants but are used in DTOs and Web services
/// </summary>
public static class PermissionModules
{
    public const string Setup = "Setup";
    public const string Cost = "Cost";
    public const string Progress = "Progress";
    public const string Reports = "Reports";
    public const string Admin = "Admin";

    public static readonly string[] All = { Setup, Cost, Progress, Reports, Admin };
}

public static class PermissionResources
{
    // Setup Module Resources
    public const string Company = "Company";
    public const string Operation = "Operation";
    public const string Project = "Project";
    public const string Phase = "Phase";
    public const string WorkPackage = "WorkPackage";
    public const string Package = "Package";
    public const string Discipline = "Discipline";
    public const string Contractor = "Contractor";

    // Cost Module Resources
    public const string Budget = "Budget";
    public const string Trend = "Trend";
    public const string Commitment = "Commitment";
    public const string Invoice = "Invoice";
    public const string Contingency = "Contingency";
    public const string CostCode = "CostCode";
    public const string Currency = "Currency";

    // Progress Module Resources
    public const string Schedule = "Schedule";
    public const string ActualProgress = "ActualProgress";
    public const string PlanProgress = "PlanProgress";
    public const string Deliverable = "Deliverable";
    public const string RFQ = "RFQ";
    public const string PurchaseOrder = "PurchaseOrder";
    public const string WeeklyReport = "WeeklyReport";
    public const string DailyReport = "DailyReport";
    public const string Document = "Document";
    public const string Photo = "Photo";

    // Reports Module Resources
    public const string MonthlyReport = "MonthlyReport";
    public const string Dashboard = "Dashboard";
    public const string Analytics = "Analytics";
    public const string Export = "Export";

    // Admin Module Resources
    public const string User = "User";
    public const string Role = "Role";
    public const string Permission = "Permission";
    public const string AuditLog = "AuditLog";
    public const string SystemSettings = "SystemSettings";
}

public static class PermissionActions
{
    // Basic CRUD
    public const string View = "View";
    public const string Create = "Create";
    public const string Edit = "Edit";
    public const string Delete = "Delete";

    // Advanced Actions
    public const string Approve = "Approve";
    public const string Reject = "Reject";
    public const string Submit = "Submit";
    public const string Cancel = "Cancel";
    public const string Export = "Export";
    public const string Import = "Import";
    public const string Archive = "Archive";
    public const string Restore = "Restore";
    public const string Assign = "Assign";
    public const string Lock = "Lock";
    public const string Unlock = "Unlock";

    // Special Admin Actions
    public const string Admin = "Admin";
    public const string Configure = "Configure";
    public const string Audit = "Audit";

    public static readonly string[] All = {
        View, Create, Edit, Delete,
        Approve, Reject, Submit, Cancel,
        Export, Import, Archive, Restore,
        Assign, Lock, Unlock,
        Admin, Configure, Audit
    };
}