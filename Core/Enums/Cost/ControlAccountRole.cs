namespace Core.Enums.Cost;

/// <summary>
/// Control Account Role
/// </summary>
public enum ControlAccountRole
{
    Manager = 1,      // CAM - Control Account Manager
    Analyst = 2,      // Cost/Schedule Analyst
    Approver = 3,     // Budget Approver
    Viewer = 4        // Read-only access
}
