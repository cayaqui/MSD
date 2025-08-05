namespace Core.DTOs.Contracts.Valuations;

public class ValuationSummaryDto
{
    public Guid ContractId { get; set; }
    public int TotalValuations { get; set; }
    public int ApprovedValuations { get; set; }
    public int PendingValuations { get; set; }
    public int LatestPeriod { get; set; }
    
    public decimal TotalCertified { get; set; }
    public decimal TotalRetention { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalDue { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal OutstandingAmount { get; set; }
    public decimal RetentionHeld { get; set; }
    public decimal MaterialsValue { get; set; }
    
    public decimal ContractValue { get; set; }
    public decimal PercentageCertified { get; set; }
    public decimal PercentagePaid { get; set; }
    
    public DateTime? LastValuationDate { get; set; }
    public DateTime? NextValuationDue { get; set; }
}