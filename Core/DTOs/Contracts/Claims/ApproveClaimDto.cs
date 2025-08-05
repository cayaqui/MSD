namespace Core.DTOs.Contracts.Claims;

public class ApproveClaimDto
{
    public string ApprovedBy { get; set; } = string.Empty;
    public decimal ApprovedAmount { get; set; }
    public string Comments { get; set; } = string.Empty;
}