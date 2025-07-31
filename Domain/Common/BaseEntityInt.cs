namespace Domain.Common;

/// <summary>
/// Base entity with int ID for lookup tables
/// </summary>
public abstract class BaseEntityInt : IAuditable
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    protected BaseEntityInt()
    {
        CreatedAt = DateTime.UtcNow;
    }
}