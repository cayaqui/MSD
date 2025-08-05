namespace Core.DTOs.Organization.Operation;

/// <summary>
/// DTO for displaying operation information
/// </summary>
public class OperationDto
{
    public Guid Id { get; set; }
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
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    
    // Status
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}