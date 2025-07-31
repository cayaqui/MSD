namespace Core.DTOs.Cost;

/// <summary>
/// DTO for recording actual cost
/// </summary>
public class RecordActualCostDto
{
    public decimal ActualCost { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? VendorId { get; set; }
    public string? Comments { get; set; }
}
