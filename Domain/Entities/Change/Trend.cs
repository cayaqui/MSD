using Core.Enums.Cost;
using Domain.Common;
using Domain.Entities.Projects;
using Domain.Entities.Security;
using System;
using System.Threading;

namespace Domain.Entities.Change;

/// <summary>
/// Trend entity for tracking potential changes that may impact project cost or schedule
/// Based on PMBOK change management practices
/// </summary>
public class Trend : BaseEntity, IAuditable, ISoftDelete, ICodeEntity
{
    // Basic Information
    public string Code { get; set; } = string.Empty; // T-XXX-YYYY format
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Classification
    public Guid ProjectId { get; private set; }
    public Guid? WBSElementId { get; private set; }
    public Guid? RaisedByUserId { get; private set; }
    public TrendCategory Category { get; private set; }
    public TrendType Type { get; private set; }
    public TrendStatus Status { get; private set; }
    public TrendPriority Priority { get; private set; }

    // Dates
    public DateTime IdentifiedDate { get; private set; }
    public DateTime? ReviewDate { get; private set; }
    public DateTime? DecisionDate { get; private set; }
    public DateTime? ImplementationDate { get; private set; }
    public DateTime DueDate { get; private set; }

    // Impact Assessment
    public decimal? EstimatedCostImpact { get; private set; }
    public int? EstimatedScheduleImpactDays { get; private set; }
    public string? QualityImpact { get; private set; }
    public string? ScopeImpact { get; private set; }
    public TrendImpactLevel ImpactLevel { get; private set; }

    // Financial Details
    public decimal? MinCostImpact { get; private set; }
    public decimal? MaxCostImpact { get; private set; }
    public decimal? MostLikelyCostImpact { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal Probability { get; private set; } // 0-100%
    public decimal? ExpectedValue => MostLikelyCostImpact.HasValue
        ? MostLikelyCostImpact.Value * (Probability / 100)
        : null;

    // Decision
    public TrendDecision? Decision { get; private set; }
    public string? DecisionRationale { get; private set; }
    public Guid? DecisionByUserId { get; private set; }
    public string? MitigationStrategy { get; private set; }

    // Change Order Reference (if converted)
    public Guid? ChangeOrderId { get; private set; }
    public bool IsConvertedToChangeOrder { get; private set; }

    // Documentation
    public string? RootCause { get; private set; }
    public string? ProposedSolution { get; private set; }
    public string? AlternativeSolutions { get; private set; }
    public string? Assumptions { get; private set; }
    public string? Risks { get; private set; }

    // Approval Workflow
    public bool RequiresClientApproval { get; private set; }
    public bool? ClientApproved { get; private set; }
    public DateTime? ClientApprovalDate { get; private set; }
    public string? ClientComments { get; private set; }

    // Tracking
    public int DaysOpen => (DateTime.UtcNow - IdentifiedDate).Days;
    public bool IsOverdue => Status != TrendStatus.Closed && DateTime.UtcNow > DueDate;

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public WBSElement? WBSElement { get; private set; }
    public User? RaisedByUser { get; private set; }
    public User? DecisionByUser { get; private set; }
    public ChangeOrder? ChangeOrder { get; private set; }
    public ICollection<TrendAttachment> Attachments { get; private set; } = new List<TrendAttachment>();
    public ICollection<TrendComment> Comments { get; private set; } = new List<TrendComment>();

    // Constructor for EF Core
    private Trend() { }

    public Trend(
        string code,
        string title,
        string description,
        Guid projectId,
        TrendCategory category,
        TrendType type,
        Guid raisedByUserId,
        DateTime dueDate)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        ProjectId = projectId;
        Category = category;
        Type = type;
        RaisedByUserId = raisedByUserId;
        DueDate = dueDate;

        IdentifiedDate = DateTime.UtcNow;
        Status = TrendStatus.Open;
        Priority = TrendPriority.Medium;
        ImpactLevel = TrendImpactLevel.Medium;
        Probability = 50; // Default 50%
        IsConvertedToChangeOrder = false;
        RequiresClientApproval = false;

        CreatedAt = DateTime.UtcNow;
    }

    // Domain Methods
    public void UpdateImpactAssessment(
        decimal? costImpact,
        int? scheduleDays,
        string? qualityImpact,
        string? scopeImpact,
        TrendImpactLevel impactLevel)
    {
        EstimatedCostImpact = costImpact;
        EstimatedScheduleImpactDays = scheduleDays;
        QualityImpact = qualityImpact;
        ScopeImpact = scopeImpact;
        ImpactLevel = impactLevel;

        // Auto-adjust priority based on impact
        if (impactLevel == TrendImpactLevel.Critical)
            Priority = TrendPriority.Critical;
        else if (impactLevel == TrendImpactLevel.High && Priority < TrendPriority.High)
            Priority = TrendPriority.High;

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetThreePointEstimate(decimal min, decimal mostLikely, decimal max)
    {
        if (min > max)
            throw new ArgumentException("Minimum cost cannot be greater than maximum");

        MinCostImpact = min;
        MostLikelyCostImpact = mostLikely;
        MaxCostImpact = max;

        // PERT estimate: (Min + 4*MostLikely + Max) / 6
        EstimatedCostImpact = (min + (4 * mostLikely) + max) / 6;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProbability(decimal probability)
    {
        if (probability < 0 || probability > 100)
            throw new ArgumentException("Probability must be between 0 and 100");

        Probability = probability;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SubmitForReview()
    {
        if (Status != TrendStatus.Open)
            throw new InvalidOperationException("Only open trends can be submitted for review");

        Status = TrendStatus.UnderReview;
        ReviewDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MakeDecision(
        TrendDecision decision,
        string rationale,
        Guid decisionByUserId,
        string? mitigationStrategy = null)
    {
        if (Status != TrendStatus.UnderReview)
            throw new InvalidOperationException("Only trends under review can have decisions");

        Decision = decision;
        DecisionRationale = rationale ?? throw new ArgumentNullException(nameof(rationale));
        DecisionByUserId = decisionByUserId;
        DecisionDate = DateTime.UtcNow;
        MitigationStrategy = mitigationStrategy;

        Status = decision == TrendDecision.ConvertToChangeOrder
            ? TrendStatus.Approved
            : TrendStatus.Closed;

        UpdatedAt = DateTime.UtcNow;
    }

    public void ConvertToChangeOrder(Guid changeOrderId)
    {
        if (Decision != TrendDecision.ConvertToChangeOrder)
            throw new InvalidOperationException("Trend must be approved for conversion to change order");

        ChangeOrderId = changeOrderId;
        IsConvertedToChangeOrder = true;
        Status = TrendStatus.Converted;
        ImplementationDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RequireClientApproval()
    {
        RequiresClientApproval = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordClientDecision(bool approved, string? comments)
    {
        if (!RequiresClientApproval)
            throw new InvalidOperationException("This trend does not require client approval");

        ClientApproved = approved;
        ClientApprovalDate = DateTime.UtcNow;
        ClientComments = comments;

        if (!approved)
        {
            Status = TrendStatus.Rejected;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Close(string? reason = null)
    {
        Status = TrendStatus.Closed;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            DecisionRationale = reason;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddComment(string comment, Guid userId)
    {
        var trendComment = new TrendComment(Id, comment, userId);
        Comments.Add(trendComment);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachDocument(string fileName, string filePath, Guid uploadedBy)
    {
        var attachment = new TrendAttachment(Id, fileName, filePath, uploadedBy);
        Attachments.Add(attachment);
        UpdatedAt = DateTime.UtcNow;
    }
}

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

/// <summary>
/// Supporting entity for Trend attachments
/// </summary>
public class TrendAttachment : BaseEntity
{
    public Guid TrendId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string? FileType { get; private set; }
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedDate { get; private set; }

    public Trend Trend { get; private set; } = null!;
    public User UploadedByUser { get; private set; } = null!;

    private TrendAttachment() { }

    public TrendAttachment(Guid trendId, string fileName, string filePath, Guid uploadedBy)
    {
        TrendId = trendId;
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        UploadedBy = uploadedBy;
        UploadedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}