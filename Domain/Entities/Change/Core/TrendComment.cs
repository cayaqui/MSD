namespace Domain.Entities.Change.Core;

/// <summary>
/// Supporting entity for Trend comments
/// </summary>
public class TrendComment : BaseEntity
{
    public Guid TrendId { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime CommentDate { get; private set; }

    public Trend Trend { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TrendComment() { }

    public TrendComment(Guid trendId, string comment, Guid userId)
    {
        TrendId = trendId;
        Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        UserId = userId;
        CommentDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}
