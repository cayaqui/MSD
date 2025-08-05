namespace Domain.Entities.Change.Core;

/// <summary>
/// Change Request Comment tracking
/// </summary>
public class ChangeRequestComment : BaseEntity
{
    public Guid ChangeRequestId { get; private set; }
    public Guid UserId { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public DateTime CommentDate { get; private set; }
    public bool IsInternal { get; private set; }

    public ChangeRequest ChangeRequest { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private ChangeRequestComment() { }

    public ChangeRequestComment(
        Guid changeRequestId,
        Guid userId,
        string comment,
        bool isInternal = false
    )
    {
        ChangeRequestId = changeRequestId;
        UserId = userId;
        Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        IsInternal = isInternal;
        CommentDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}
