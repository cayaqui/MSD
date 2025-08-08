using Core.Enums.Documents;

namespace Core.DTOs.Documents.Document;

public class DocumentPermissionDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }
    public DocumentPermissionType PermissionType { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
    public bool CanComment { get; set; }
    public bool CanApprove { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class CreateDocumentPermissionDto
{
    public Guid DocumentId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public string? DepartmentCode { get; set; }
    public DocumentPermissionType PermissionType { get; set; }
    public bool CanView { get; set; } = true;
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
    public bool CanComment { get; set; } = true;
    public bool CanApprove { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class UpdateDocumentPermissionDto
{
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
    public bool CanComment { get; set; }
    public bool CanApprove { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
}