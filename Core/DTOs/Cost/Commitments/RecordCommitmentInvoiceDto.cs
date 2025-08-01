namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for recording invoice against commitment
/// </summary>
public class RecordCommitmentInvoiceDto
{
    public decimal InvoiceAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RetentionAmount { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime? InvoiceDate { get; set; }
}
