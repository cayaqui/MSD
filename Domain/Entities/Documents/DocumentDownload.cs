using Domain.Common;
using Domain.Entities.Auth.Security;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa una descarga de un documento
/// </summary>
public class DocumentDownload : BaseEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid? DocumentVersionId { get; set; }
    public virtual DocumentVersion? DocumentVersion { get; set; }
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public DateTime DownloadedDate { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Purpose { get; set; }
}