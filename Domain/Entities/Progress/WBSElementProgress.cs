using Domain.Common;
using Domain.Entities.Projects;
using Domain.Entities.Security;
using Core.Enums.Progress;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Progress
{
    /// <summary>
    /// WBS Element Progress tracking for historical progress records
    /// Tracks progress updates for any level of WBS (Phase, Summary, Work Package)
    /// </summary>
    public class WBSElementProgress : BaseEntity, IAuditable
    {
        private WBSElementProgress() { }

        public WBSElementProgress(
            Guid wbsElementId,
            DateTime progressDate,
            decimal previousProgress,
            decimal currentProgress,
            ProgressMethod measurementMethod,
            string reportedBy)
            : this()
        {
            WBSElementId = wbsElementId;
            ProgressDate = progressDate;
            PreviousProgress = previousProgress;
            CurrentProgress = currentProgress;
            MeasurementMethod = measurementMethod;
            ReportedBy = reportedBy ?? throw new ArgumentNullException(nameof(reportedBy));

            // Calculate period info
            Year = progressDate.Year;
            Month = progressDate.Month;
            Week = GetIso8601WeekOfYear(progressDate);

            CreatedAt = DateTime.UtcNow;
            ValidateProgress();
        }

        // Foreign Keys
        public Guid WBSElementId { get; private set; }
        public WBSElement WBSElement { get; private set; } = null!;

        // Progress Information
        public DateTime ProgressDate { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Week { get; private set; } // ISO week number

        public decimal PreviousProgress { get; private set; }
        public decimal CurrentProgress { get; private set; }
        public decimal ProgressDelta => CurrentProgress - PreviousProgress;
        public ProgressMethod MeasurementMethod { get; private set; }

        // Physical Progress (for verification)
        public decimal? PhysicalProgress { get; private set; }
        public string? PhysicalProgressDescription { get; private set; }

        // Cost Information at time of progress
        public decimal PreviousActualCost { get; private set; }
        public decimal CurrentActualCost { get; private set; }
        public decimal CostDelta => CurrentActualCost - PreviousActualCost;
        public decimal CommittedCost { get; private set; }
        public decimal ForecastToComplete { get; private set; }
        public decimal EstimateAtCompletion => CurrentActualCost + ForecastToComplete;

        // Schedule Information
        public DateTime? ActualStartDate { get; private set; }
        public DateTime? ForecastEndDate { get; private set; }
        public int? DaysDelayed { get; private set; }
        public string? DelayReason { get; private set; }

        // Earned Value Metrics at time of progress
        public decimal? EarnedValue { get; private set; }
        public decimal? PlannedValue { get; private set; }
        public decimal? ScheduleVariance => EarnedValue.HasValue && PlannedValue.HasValue
            ? EarnedValue.Value - PlannedValue.Value
            : null;
        public decimal? CostVariance => EarnedValue.HasValue
            ? EarnedValue.Value - CurrentActualCost
            : null;
        public decimal? SPI => PlannedValue.HasValue && PlannedValue.Value > 0
            ? EarnedValue / PlannedValue
            : null;
        public decimal? CPI => CurrentActualCost > 0
            ? EarnedValue / CurrentActualCost
            : null;

        // Progress Details
        public string? Comments { get; private set; }
        public string? Issues { get; private set; }
        public string? Risks { get; private set; }
        public string? MitigationActions { get; private set; }

        // Approval Workflow
        public bool IsApproved { get; private set; }
        public DateTime? ApprovalDate { get; private set; }
        public string? ApprovedBy { get; private set; }
        public string? ApprovalComments { get; private set; }
        public bool RequiresReview { get; private set; }
        public string? ReviewReason { get; private set; }

        // Reporting
        public string ReportedBy { get; private set; } = string.Empty;
        public DateTime ReportedAt { get; private set; }
        public string? VerifiedBy { get; private set; }
        public DateTime? VerifiedAt { get; private set; }

        // Supporting Documentation
        public string? PhotoReferences { get; private set; } // JSON array of photo IDs
        public string? DocumentReferences { get; private set; } // JSON array of document IDs
        public string? MilestoneReferences { get; private set; } // JSON array of milestone codes achieved

        // Resource Information
        public decimal? LaborHoursUsed { get; private set; }
        public decimal? EquipmentHoursUsed { get; private set; }
        public decimal? MaterialQuantityUsed { get; private set; }
        public string? ResourceNotes { get; private set; }

        // For summary/phase level progress
        public int? ChildrenCount { get; private set; }
        public int? CompletedChildrenCount { get; private set; }
        public bool IsRollupProgress { get; private set; } // True if calculated from children

        // Business Methods
        public void UpdateCostInformation(
            decimal actualCost,
            decimal committedCost,
            decimal forecastToComplete)
        {
            if (actualCost < 0 || committedCost < 0 || forecastToComplete < 0)
                throw new ArgumentException("Cost values cannot be negative");

            CurrentActualCost = actualCost;
            CommittedCost = committedCost;
            ForecastToComplete = forecastToComplete;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateScheduleInformation(
            DateTime? actualStartDate,
            DateTime? forecastEndDate,
            int? daysDelayed,
            string? delayReason)
        {
            ActualStartDate = actualStartDate;
            ForecastEndDate = forecastEndDate;
            DaysDelayed = daysDelayed;
            DelayReason = delayReason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEarnedValueMetrics(
            decimal earnedValue,
            decimal plannedValue)
        {
            if (earnedValue < 0 || plannedValue < 0)
                throw new ArgumentException("EVM values cannot be negative");

            EarnedValue = earnedValue;
            PlannedValue = plannedValue;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePhysicalProgress(
            decimal physicalProgress,
            string? description)
        {
            if (physicalProgress < 0 || physicalProgress > 100)
                throw new ArgumentException("Physical progress must be between 0 and 100");

            PhysicalProgress = physicalProgress;
            PhysicalProgressDescription = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddComments(string? comments, string? issues, string? risks, string? mitigationActions)
        {
            Comments = comments;
            Issues = issues;
            Risks = risks;
            MitigationActions = mitigationActions;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AttachDocumentation(
            string[]? photoIds,
            string[]? documentIds,
            string[]? milestoneReferences)
        {
            if (photoIds != null)
                PhotoReferences = System.Text.Json.JsonSerializer.Serialize(photoIds);

            if (documentIds != null)
                DocumentReferences = System.Text.Json.JsonSerializer.Serialize(documentIds);

            if (milestoneReferences != null)
                MilestoneReferences = System.Text.Json.JsonSerializer.Serialize(milestoneReferences);

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateResourceUsage(
            decimal? laborHours,
            decimal? equipmentHours,
            decimal? materialQuantity,
            string? resourceNotes)
        {
            if (laborHours.HasValue && laborHours.Value < 0)
                throw new ArgumentException("Labor hours cannot be negative");
            if (equipmentHours.HasValue && equipmentHours.Value < 0)
                throw new ArgumentException("Equipment hours cannot be negative");
            if (materialQuantity.HasValue && materialQuantity.Value < 0)
                throw new ArgumentException("Material quantity cannot be negative");

            LaborHoursUsed = laborHours;
            EquipmentHoursUsed = equipmentHours;
            MaterialQuantityUsed = materialQuantity;
            ResourceNotes = resourceNotes;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Approve(string approvedBy, string? comments = null)
        {
            if (IsApproved)
                throw new InvalidOperationException("Progress already approved");

            IsApproved = true;
            ApprovalDate = DateTime.UtcNow;
            ApprovedBy = approvedBy ?? throw new ArgumentNullException(nameof(approvedBy));
            ApprovalComments = comments;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject(string rejectedBy, string reason)
        {
            IsApproved = false;
            ApprovalDate = DateTime.UtcNow;
            ApprovedBy = rejectedBy ?? throw new ArgumentNullException(nameof(rejectedBy));
            ApprovalComments = $"REJECTED: {reason}";
            RequiresReview = true;
            ReviewReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkForReview(string reason)
        {
            RequiresReview = true;
            ReviewReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Verify(string verifiedBy)
        {
            VerifiedBy = verifiedBy ?? throw new ArgumentNullException(nameof(verifiedBy));
            VerifiedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRollupInformation(int childrenCount, int completedChildrenCount)
        {
            ChildrenCount = childrenCount;
            CompletedChildrenCount = completedChildrenCount;
            IsRollupProgress = true;
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper Methods
        private void ValidateProgress()
        {
            if (CurrentProgress < 0 || CurrentProgress > 100)
                throw new ArgumentException("Progress must be between 0 and 100");

            if (PreviousProgress < 0 || PreviousProgress > 100)
                throw new ArgumentException("Previous progress must be between 0 and 100");

            if (CurrentProgress < PreviousProgress)
                throw new ArgumentException("Current progress cannot be less than previous progress without justification");
        }

        private static int GetIso8601WeekOfYear(DateTime date)
        {
            var day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        // Calculated Properties
        public bool IsDelayed => DaysDelayed.HasValue && DaysDelayed.Value > 0;

        public bool IsOverBudget => CostVariance.HasValue && CostVariance.Value < 0;

        public bool IsBehindSchedule => ScheduleVariance.HasValue && ScheduleVariance.Value < 0;

        public bool RequiresAttention => IsDelayed || IsOverBudget || IsBehindSchedule || RequiresReview;

        public decimal ProgressEfficiency => PreviousProgress > 0 && ProgressDelta > 0
            ? (CostDelta / ProgressDelta)
            : 0;
    }
}