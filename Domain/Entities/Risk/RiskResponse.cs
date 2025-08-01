using System;
using Domain.Common;
using Domain.Entities.Security;
using Core.Enums.Risk;

namespace Domain.Entities.Risk;

/// <summary>
/// Risk Response actions and tracking
/// </summary>
public class RiskResponse : BaseEntity, IAuditable
{
    // Basic Information
    public Guid RiskId { get; private set; }
    public string ActionDescription { get; private set; } = string.Empty;
    public ResponseActionType ActionType { get; private set; }

    // Assignment
    public string AssignedToId { get; private set; } = string.Empty;
    public DateTime DueDate { get; private set; }
    public RiskResponseStatus Status { get; private set; }

    // Execution
    public DateTime? CompletedDate { get; private set; }
    public string? CompletionNotes { get; private set; }
    public decimal? ActualCost { get; private set; }

    // Effectiveness
    public bool? WasEffective { get; private set; }
    public string? EffectivenessNotes { get; private set; }

    // Navigation Properties
    public Risk Risk { get; private set; } = null!;
    public User AssignedTo { get; private set; } = null!;

    // Constructor for EF Core
    private RiskResponse() { }

    public RiskResponse(
        Guid riskId,
        string actionDescription,
        ResponseActionType actionType,
        string assignedToId,
        DateTime dueDate)
    {
        RiskId = riskId;
        ActionDescription = actionDescription ?? throw new ArgumentNullException(nameof(actionDescription));
        ActionType = actionType;
        AssignedToId = assignedToId ?? throw new ArgumentNullException(nameof(assignedToId));
        DueDate = dueDate;

        Status = RiskResponseStatus.Planned;
        CreatedAt = DateTime.UtcNow;
    }

    public void StartAction()
    {
        if (Status != RiskResponseStatus.Planned)
            throw new InvalidOperationException("Can only start planned actions");

        Status = RiskResponseStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteAction(string? completionNotes, decimal? actualCost = null)
    {
        if (Status != RiskResponseStatus.InProgress)
            throw new InvalidOperationException("Can only complete in-progress actions");

        Status = RiskResponseStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        CompletionNotes = completionNotes;
        ActualCost = actualCost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EvaluateEffectiveness(bool wasEffective, string? notes)
    {
        if (Status != RiskResponseStatus.Completed)
            throw new InvalidOperationException("Can only evaluate completed actions");

        WasEffective = wasEffective;
        EffectivenessNotes = notes;
        Status = RiskResponseStatus.Evaluated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelAction(string reason)
    {
        Status = RiskResponseStatus.Cancelled;
        CompletionNotes = $"Cancelled: {reason}";
        UpdatedAt = DateTime.UtcNow;
    }
}