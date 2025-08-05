using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class ValuationDocument : BaseEntity
{
    public Guid ValuationId { get; set; }
    public virtual Valuation Valuation { get; set; } = null!;
    
    public Guid DocumentId { get; set; }
    
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    
    public DateTime UploadedDate { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    
    public DateTime AttachedDate { get; set; }
    public string AttachedBy { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
