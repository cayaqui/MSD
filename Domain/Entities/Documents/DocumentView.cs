using Domain.Common;
using Domain.Entities.Auth.Security;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa una vista/lectura de un documento
/// </summary>
public class DocumentView : BaseEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid? DocumentVersionId { get; set; }
    public virtual DocumentVersion? DocumentVersion { get; set; }
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public DateTime ViewedDate { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? DurationSeconds { get; set; }
}