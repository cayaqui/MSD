namespace Core.DTOs.Documents.Document;

public class DocumentStatisticsDto
{
    public Guid DocumentId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    
    // View Statistics
    public long TotalViews { get; set; }
    public long UniqueViewers { get; set; }
    public DateTime? LastViewDate { get; set; }
    public string? LastViewedBy { get; set; }
    public Dictionary<string, long> ViewsByMonth { get; set; } = new();
    public Dictionary<string, long> ViewsByUser { get; set; } = new();
    
    // Download Statistics
    public long TotalDownloads { get; set; }
    public long UniqueDownloaders { get; set; }
    public DateTime? LastDownloadDate { get; set; }
    public string? LastDownloadedBy { get; set; }
    public Dictionary<string, long> DownloadsByMonth { get; set; } = new();
    public Dictionary<string, long> DownloadsByUser { get; set; } = new();
    
    // Version Statistics
    public int TotalVersions { get; set; }
    public int ApprovedVersions { get; set; }
    public int RejectedVersions { get; set; }
    public double AverageReviewTimeHours { get; set; }
    public DateTime? LastVersionUploadDate { get; set; }
    public string? LastVersionUploadedBy { get; set; }
    
    // Comment Statistics
    public int TotalComments { get; set; }
    public int UnresolvedComments { get; set; }
    public int ResolvedComments { get; set; }
    public Dictionary<string, int> CommentsByType { get; set; } = new();
    public DateTime? LastCommentDate { get; set; }
    public string? LastCommentBy { get; set; }
    
    // Share Statistics
    public int TotalShares { get; set; }
    public int ActiveShares { get; set; }
    public Dictionary<string, int> SharesByPermissionType { get; set; } = new();
    
    // Storage Statistics
    public long TotalFileSizeBytes { get; set; }
    public string FormattedFileSize { get; set; } = string.Empty;
    public long AllVersionsSizeBytes { get; set; }
    public string FormattedAllVersionsSize { get; set; } = string.Empty;
    
    // Activity Summary
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public int DaysSinceCreation { get; set; }
    public int DaysSinceLastActivity { get; set; }
}