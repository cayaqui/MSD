namespace Domain.Common;

/// <summary>
/// Base entity class with audit properties and soft delete support
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity, ISoftDelete
{
    // Soft Delete properties
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    protected BaseAuditableEntity() : base()
    {
    }
}