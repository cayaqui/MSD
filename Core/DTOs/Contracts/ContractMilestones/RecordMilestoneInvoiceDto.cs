namespace Core.DTOs.Contracts.ContractMilestones;

public class RecordMilestoneInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal? InvoiceAmount { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
}