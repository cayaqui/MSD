namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for commitment-related invoices
/// </summary>
public class CommitmentInvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? VendorInvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal RetentionAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public bool IsApproved => Status == "Approved";
    public bool IsPaid => Status == "Paid";
    public decimal OutstandingAmount => TotalAmount - PaidAmount;
}
