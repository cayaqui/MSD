using Core.DTOs.Common;

namespace Core.DTOs.Auth.Permissions;


public class PermissionCategoryDto
{
    public string Resource { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<PermissionActionDto> Actions { get; set; } = new();
}

public class PermissionActionDto
{
    public Guid PermissionId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAssigned { get; set; }
}

public class PermissionFilterDto
{
    public string? Module { get; set; }
    public string? Resource { get; set; }
    public string? Action { get; set; }
    public string? Category { get; set; }
    public bool? IsSystem { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class PermissionMatrixDto
{
    public List<string> Modules { get; set; } = new();
    public List<string> Resources { get; set; } = new();
    public List<string> Actions { get; set; } = new();
    public Dictionary<string, Dictionary<string, Dictionary<string, PermissionDto>>> Matrix { get; set; } = new();
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class BulkPermissionAssignmentDto
{
    public List<Guid> PermissionIds { get; set; } = new();
    public List<Guid> RoleIds { get; set; } = new();
    public bool ReplaceExisting { get; set; } = false;
}

public static class PermissionModules
{
    public const string Projects = "Projects";
    public const string Documents = "Documents";
    public const string Cost = "Cost";
    public const string Schedule = "Schedule";
    public const string Risk = "Risk";
    public const string Reports = "Reports";
    public const string Configuration = "Configuration";
    public const string Users = "Users";
    public const string System = "System";
}

public static class PermissionActions
{
    public const string View = "View";
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string Approve = "Approve";
    public const string Export = "Export";
    public const string Import = "Import";
    public const string Execute = "Execute";
    public const string Manage = "Manage";
    public const string Assign = "Assign";
}