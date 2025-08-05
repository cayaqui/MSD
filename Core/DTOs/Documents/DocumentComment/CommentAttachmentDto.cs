namespace Core.DTOs.Documents.DocumentComment;

public class CommentAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string BlobStorageUrl { get; set; } = string.Empty;
}
