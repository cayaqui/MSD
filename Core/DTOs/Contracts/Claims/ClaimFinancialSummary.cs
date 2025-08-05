namespace Core.DTOs.Contracts.Claims;

public class ClaimFinancialSummary
{
    public Guid ContractId { get; set; }
    public int TotalClaims { get; set; }
    public int ApprovedClaims { get; set; }
    public int RejectedClaims { get; set; }
    public int PendingClaims { get; set; }
    public decimal TotalClaimedAmount { get; set; }
    public decimal TotalApprovedAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalRejectedAmount { get; set; }
    public decimal PendingAmount { get; set; }
}