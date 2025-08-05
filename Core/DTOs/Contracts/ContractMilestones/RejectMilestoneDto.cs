namespace Core.DTOs.Contracts.ContractMilestones;

public class RejectMilestoneDto
{
    public string RejectedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}