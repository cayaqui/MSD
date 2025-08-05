namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// Evaluate contractor data transfer object
/// </summary>
public class EvaluateContractorDto
{
    public Guid ProjectId { get; set; }
    public decimal QualityRating { get; set; }
    public decimal SafetyRating { get; set; }
    public decimal ScheduleRating { get; set; }
    public decimal CostRating { get; set; }
    public decimal CommunicationRating { get; set; }
    public string? Comments { get; set; }
    public string? ImprovementAreas { get; set; }
    public bool RecommendForFutureWork { get; set; }
}