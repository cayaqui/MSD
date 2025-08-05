namespace Core.DTOs.Organization.Operation;

/// <summary>
/// DTO for updating operation manager
/// </summary>
public class UpdateOperationManagerDto
{
    public string? ManagerName { get; set; }
    public string? ManagerEmail { get; set; }
}