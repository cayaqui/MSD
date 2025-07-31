namespace Core.DTOs.Cost;

/// <summary>
/// DTO for recording commitment
/// </summary>
public class RecordCommitmentDto
{
    public decimal CommittedCost { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? VendorId { get; set; }
    public string? Comments { get; set; }
}
