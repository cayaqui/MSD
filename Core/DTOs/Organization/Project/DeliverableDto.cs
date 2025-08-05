namespace Core.DTOs.Organization.Project;

/// <summary>
/// Deliverable data transfer object
/// </summary>
public class DeliverableDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? WBSElementId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime PlannedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AcceptanceCriteria { get; set; }
    public string? ResponsibleParty { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
}