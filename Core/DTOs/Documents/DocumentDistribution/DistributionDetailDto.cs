namespace Core.DTOs.Documents.DocumentDistribution;

public class DistributionDetailDto
{
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientCompany { get; set; } = string.Empty;
    public DateTime DistributionDate { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedDate { get; set; }
    public bool IsDownloaded { get; set; }
    public DateTime? FirstDownloadDate { get; set; }
    public int DownloadCount { get; set; }
}
