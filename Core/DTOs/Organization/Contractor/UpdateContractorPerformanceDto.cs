namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// DTO for updating contractor performance rating
/// </summary>
public class UpdateContractorPerformanceDto
{
    public decimal Rating { get; set; } // 0-5 scale
    public string? Notes { get; set; }
}