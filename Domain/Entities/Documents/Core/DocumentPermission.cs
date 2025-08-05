namespace Domain.Entities.Documents.Core;

public class DocumentPermission : BaseEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    // Permission can be for a user or a role
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    
    // Permission levels
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
    public DateTime GrantedDate { get; set; }
    public Guid GrantedById { get; set; }
    public string? Comments { get; set; }
    
    // Helper methods
    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        if (ValidFrom.HasValue && now < ValidFrom.Value)
            return false;
        if (ValidTo.HasValue && now > ValidTo.Value)
            return false;
        return true;
    }
    
    public bool HasAnyPermission()
    {
        return CanView || CanDownload || CanEdit || CanDelete || 
               CanComment || CanDistribute || CanManagePermissions;
    }
}