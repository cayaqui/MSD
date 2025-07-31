namespace Core.DTOs.Cost;

/// <summary>
/// DTO for Cost Item detail view
/// </summary>
public class CostItemDetailDto : CostItemDto
{
    public string? AccountCode { get; set; }
    public string? CostCenter { get; set; }
    public decimal ExchangeRate { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
