using Core.Enums.Documents;

namespace Core.DTOs.Documents.DocumentDistribution;

public class DistributionStatisticsDto
{
    public string Period { get; set; } = string.Empty;
    public int TotalDistributions { get; set; }
    public int UniqueDocuments { get; set; }
    public int UniqueRecipients { get; set; }
    public double AcknowledgmentRate { get; set; }
    public double DownloadRate { get; set; }
    public Dictionary<DistributionMethod, int> MethodBreakdown { get; set; } = new();
    public Dictionary<DistributionPurpose, int> PurposeBreakdown { get; set; } = new();
}
