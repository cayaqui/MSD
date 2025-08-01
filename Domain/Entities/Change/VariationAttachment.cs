namespace Domain.Entities.Change;

/// <summary>
/// Supporting entity for Variation attachments
/// </summary>
public class VariationAttachment : BaseEntity
{
    public Guid VariationId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string FileType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public AttachmentCategory Category { get; private set; }
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedDate { get; private set; }

    public Variation Variation { get; private set; } = null!;
    public User UploadedByUser { get; private set; } = null!;

    private VariationAttachment() { }

    public VariationAttachment(
        Guid variationId,
        string fileName,
        string filePath,
        string fileType,
        AttachmentCategory category,
        Guid uploadedBy)
    {
        VariationId = variationId;
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        FileType = fileType ?? throw new ArgumentNullException(nameof(fileType));
        Category = category;
        UploadedBy = uploadedBy;
        UploadedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}