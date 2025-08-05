namespace Core.DTOs.Documents.DocumentDistribution;

public class DistributionReportDto
{
    public Guid DocumentId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentTitle { get; set; } = string.Empty;
    public int TotalDistributions { get; set; }
    public int AcknowledgedCount { get; set; }
    public int DownloadedCount { get; set; }
    public List<DistributionDetailDto> Details { get; set; } = new();
}
