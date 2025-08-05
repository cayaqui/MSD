using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Claims;

public class ResolveClaimDto
{
    public ClaimResolution Resolution { get; set; }
    public decimal ApprovedAmount { get; set; }
    public int ApprovedTimeExtension { get; set; }
    public string ResolutionDetails { get; set; } = string.Empty;
    public string SettlementTerms { get; set; } = string.Empty;
}
