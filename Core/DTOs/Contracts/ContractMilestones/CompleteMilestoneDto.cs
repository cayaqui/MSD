namespace Core.DTOs.Contracts.ContractMilestones;

public class CompleteMilestoneDto
{
    public DateTime? ActualDate { get; set; }
    public string CompletionNotes { get; set; } = string.Empty;
}