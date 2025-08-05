namespace Domain.Entities.Change.Core;

/// <summary>
/// Supporting entity for Trend attachments
/// </summary>
public class TrendAttachment : BaseEntity
{
    public Guid TrendId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string? FileType { get; private set; }
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedDate { get; private set; }

    public Trend Trend { get; private set; } = null!;
    public User UploadedByUser { get; private set; } = null!;

    private TrendAttachment() { }

    public TrendAttachment(Guid trendId, string fileName, string filePath, Guid uploadedBy)
    {
        TrendId = trendId;
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        UploadedBy = uploadedBy;
        UploadedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}
