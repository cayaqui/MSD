namespace Core.DTOs.EVM;

/// <summary>
/// DTO for EVM Record detail view
/// </summary>
public class EVMRecordDetailDto : EVMRecordDto
{
    public decimal BAC { get; set; }
    public decimal CumulativePV { get; set; }
    public decimal CumulativeEV { get; set; }
    public decimal CumulativeAC { get; set; }
    public decimal CumulativeCV { get; set; }
    public decimal CumulativeSV { get; set; }
    public decimal CumulativeCPI { get; set; }
    public decimal CumulativeSPI { get; set; }
    public decimal EAC { get; set; }
    public decimal ETC { get; set; }
    public decimal VAC { get; set; }
    public decimal TCPI { get; set; }
    public decimal PercentComplete { get; set; }
    public decimal PercentSpent { get; set; }
    public DateTime? EstimatedCompletionDate { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
