namespace Core.DTOs.Cost;

/// <summary>
/// DTO for Planning Package list view
/// </summary>
public class PlanningPackageDto
{
    public Guid Id { get; set; }
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal Budget { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsConverted { get; set; }
    public DateTime? ConversionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
