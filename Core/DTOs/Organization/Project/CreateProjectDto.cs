namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for creating a new project
/// </summary>
public class CreateProjectDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OperationId { get; set; }

    // Dates
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }

    // Financial
    public decimal TotalBudget { get; set; }
    public string Currency { get; set; } = "USD";

    // Additional Information
    public string? Location { get; set; }
    public string? Client { get; set; }
    public string? ContractNumber { get; set; }

    // Optional WBS Code
    public string? WBSCode { get; set; }
}