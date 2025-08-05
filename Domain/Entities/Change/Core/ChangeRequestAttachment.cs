namespace Domain.Entities.Change.Core;

/// <summary>
/// Change Request Attachment
/// </summary>
public class ChangeRequestAttachment : BaseEntity
{
    public Guid ChangeRequestId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string FileType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public AttachmentCategory Category { get; private set; }
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedDate { get; private set; }

    public ChangeRequest ChangeRequest { get; private set; } = null!;
    public User UploadedByUser { get; private set; } = null!;

    private ChangeRequestAttachment() { }

    public ChangeRequestAttachment(
        Guid changeRequestId,
        string fileName,
        string filePath,
        string fileType,
        AttachmentCategory category,
        Guid uploadedBy
    )
    {
        ChangeRequestId = changeRequestId;
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        FileType = fileType ?? throw new ArgumentNullException(nameof(fileType));
        Category = category;
        UploadedBy = uploadedBy;
        UploadedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}
