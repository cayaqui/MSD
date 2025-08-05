namespace Core.DTOs.Organization.Operation;

/// <summary>
/// DTO for creating a new operation
/// </summary>
public class CreateOperationDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Location
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    
    // Management
    public string? ManagerName { get; set; }
    public string? ManagerEmail { get; set; }
    public string? CostCenter { get; set; }
    
    // Foreign Key
    public Guid CompanyId { get; set; }
}