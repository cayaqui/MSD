namespace Core.DTOs.Contracts.Claims;

public class SubmitClaimDto
{
    public string SubmittedBy { get; set; } = string.Empty;
    public string SubmissionReference { get; set; } = string.Empty;
}