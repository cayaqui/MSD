namespace Core.DTOs.Organization.Operation;

/// <summary>
/// DTO for updating operation information
/// </summary>
public class UpdateOperationDto
{
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
}