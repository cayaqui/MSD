using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.Progress
{

    /// <summary>
    /// Work Package Status
    /// </summary>
    public enum WorkPackageStatus
    {
        NotStarted = 1,
        InProgress = 2,
        Completed = 3,
        OnHold = 4,
        Cancelled = 5
    }

    /// <summary>
    /// Progress calculation method
    /// </summary>
    public enum ProgressMethod
    {
        Manual = 1,              // Manual entry
        WeightedActivities = 2,  // Based on activity weights
        Milestones = 3,          // Based on milestone completion
        UnitsProduced = 4,       // Based on units completed
        DurationBased = 5        // Based on elapsed time
    }

    /// <summary>
    /// Activity Status
    /// </summary>
    public enum ActivityStatus
    {
        NotStarted = 1,
        InProgress = 2,
        Completed = 3,
        Suspended = 4,
        Cancelled = 5
    }


    /// <summary>
    /// Schedule Status
    /// </summary>
    public enum ScheduleStatus
    {
        Draft = 1,
        UnderReview = 2,
        Approved = 3,
        Baselined = 4,
        InProgress = 5,
        Completed = 6
    }

    /// <summary>
    /// Milestone Type
    /// </summary>
    public enum MilestoneType
    {
        ProjectStart = 1,
        ProjectEnd = 2,
        PhaseGate = 3,
        Deliverable = 4,
        Approval = 5,
        Review = 6,
        ContractMilestone = 7,
        PaymentMilestone = 8,
        Other = 9
    }

    /// <summary>
    /// Dependency Type for activities
    /// </summary>
    public enum DependencyType
    {
        FinishToStart = 1,    // FS - Most common
        StartToStart = 2,     // SS
        FinishToFinish = 3,   // FF
        StartToFinish = 4     // SF - Rare
    }

    /// <summary>
    /// Critical Path status
    /// </summary>
    public enum CriticalPathStatus
    {
        Critical = 1,
        NearCritical = 2,     // Float <= 5 days
        NonCritical = 3
    }

    /// <summary>
    /// Trend Status for cost trends
    /// </summary>
    public enum TrendStatus
    {
        Potential = 1,
        Identified = 2,
        Approved = 3,
        Rejected = 4,
        Incorporated = 5
    }

    /// <summary>
    /// Invoice Status
    /// </summary>
    public enum InvoiceStatus
    {
        Draft = 1,
        Submitted = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        Paid = 6,
        Cancelled = 7
    }

    /// <summary>
    /// Commitment Type
    /// </summary>
    public enum CommitmentType
    {
        PurchaseOrder = 1,
        Contract = 2,
        Subcontract = 3,
        ServiceAgreement = 4,
        LeaseAgreement = 5
    }

    /// <summary>
    /// Report Type
    /// </summary>
    public enum ReportType
    {
        DailyProgress = 1,
        WeeklyStatus = 2,
        MonthlyReport = 3,
        EVMReport = 4,
        CostReport = 5,
        ScheduleReport = 6,
        Dashboard = 7,
        Executive = 8
    }
}
