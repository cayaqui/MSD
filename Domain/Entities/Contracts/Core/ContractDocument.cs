using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class ContractDocument : BaseEntity
{
    public Guid ContractId { get; set; }
    public virtual Contract Contract { get; set; } = null!;
    
    public Guid DocumentId { get; set; }
    
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    
    public DateTime UploadedDate { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
