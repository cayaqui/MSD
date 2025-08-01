namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for updating commitment performance
/// </summary>
public class UpdateCommitmentPerformanceDto
{
    public decimal PerformancePercentage { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }
    public string? Notes { get; set; }
}