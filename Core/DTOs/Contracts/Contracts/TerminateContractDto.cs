namespace Core.DTOs.Contracts.Contracts;

public class TerminateContractDto
{
    public string Reason { get; set; } = string.Empty;
    public DateTime? EffectiveDate { get; set; }
}