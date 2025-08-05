namespace Core.DTOs.Contracts.Valuations;

public class ApproveValuationDto
{
    public string ApprovalComments { get; set; } = string.Empty;
    public decimal? AdjustedAmount { get; set; }
}
