namespace Core.DTOs.Documents.TransmittalAttachment;

public class TransmittalAttachmentDto
{
    public Guid Id { get; set; }
    public Guid TransmittalId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BlobStorageUrl { get; set; } = string.Empty;
}
