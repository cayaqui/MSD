namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for commitment audit trail entries
/// </summary>
public class CommitmentAuditDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string? PerformedByName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Comments { get; set; }
    public string EntityType { get; set; } = "Commitment";
    public Guid EntityId { get; set; }
}
