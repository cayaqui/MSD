using Domain.Common;
using Domain.Entities.Auth.Security;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa los permisos de acceso a un documento
/// </summary>
public class DocumentPermission : BaseAuditableEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    // User or Role permission
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    
    public string? RoleName { get; set; }
    
    // Permissions
    public bool CanView { get; set; }
    public bool CanDownload { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanComment { get; set; }
    public bool CanDistribute { get; set; }
    public bool CanManagePermissions { get; set; }
    
    // Validity period
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    
    // Grant information
    public Guid GrantedById { get; set; }
    public virtual User GrantedBy { get; set; } = null!;
    public DateTime GrantedDate { get; set; }
    public string? GrantReason { get; set; }
}