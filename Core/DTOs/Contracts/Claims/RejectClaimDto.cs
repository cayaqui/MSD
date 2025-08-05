namespace Core.DTOs.Contracts.Claims;

public class RejectClaimDto
{
    public string RejectedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}