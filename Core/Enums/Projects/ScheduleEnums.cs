using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.Projects
{
    #region Enums used in DTOs

    public enum ScheduleVersionType
    {
        Draft,
        Working,
        Baseline,
        WhatIf,
        Historical
    }

    public enum ScheduleVersionStatus
    {
        Draft,
        InProgress,
        UnderReview,
        Approved,
        Rejected,
        Archived
    }

    public enum ScheduleTaskType
    {
        Task,
        Summary,
        Milestone,
        WorkPackage,
        PlanningPackage
    }

    public enum ScheduleTaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold,
        Cancelled,
        Delayed
    }

    public enum ProgressMeasurementType
    {
        Percentage,
        Physical,
        Milestone,
        WeightedMilestone,
        LevelOfEffort,
        ApportionedEffort
    }

    public enum DependencyType
    {
        FinishToStart,
        StartToStart,
        FinishToFinish,
        StartToFinish
    }

    public enum ResourceType
    {
        Labor,
        Equipment,
        Material,
        Subcontractor,
        Owner,
        Other
    }

    public enum ResourceAssignmentStatus
    {
        Planned,
        Assigned,
        Active,
        Completed,
        Released
    }

    public enum LevelingStrategy
    {
        MinimizeDelay,
        OptimizeResources,
        PreserveCriticalPath,
        BalanceWorkload
    }

    public enum ProgressUpdateMethod
    {
        Manual,
        Calculated,
        Imported,
        System
    }

    public enum MilestoneStatus
    {
        Pending,
        OnTrack,
        AtRisk,
        Delayed,
        Achieved,
        Missed
    }

    public enum BaselineType
    {
        Initial,
        Revised,
        Current,
        Contract,
        Internal
    }

    public enum BaselineStatus
    {
        Draft,
        Approved,
        Active,
        Superseded,
        Archived
    }

    public enum CalendarType
    {
        Standard,
        Project,
        Resource,
        Task,
        Global
    }

    public enum CalendarExceptionType
    {
        Holiday,
        NonWorkingDay,
        SpecialWorkingDay,
        HalfDay,
        CustomHours
    }

    public enum ConstraintType
    {
        AsSoonAsPossible,
        AsLateAsPossible,
        MustStartOn,
        MustFinishOn,
        StartNoEarlierThan,
        StartNoLaterThan,
        FinishNoEarlierThan,
        FinishNoLaterThan
    }

    public enum CompressionStrategy
    {
        LeastCost,
        MaximumCompression,
        BalancedApproach,
        MinimumRisk
    }

    public enum CompressionTechnique
    {
        Crashing,
        FastTracking,
        ResourceReallocation,
        ScopeReduction,
        Combination
    }

    public enum ScheduleExportFormat
    {
        MSProject,
        PrimaveraP6,
        PrimaveraXML,
        Excel,
        CSV,
        PDF,
        JSON
    }

    public enum ScheduleImportMode
    {
        Replace,
        Merge,
        Append,
        Update
    }

    public enum ExternalSystemType
    {
        MSProject,
        PrimaveraP6,
        ProjectServer,
        Jira,
        Monday,
        Smartsheet,
        Custom
    }

    public enum SyncDirection
    {
        Inbound,
        Outbound,
        TwoWay
    }

    public enum ConflictResolution
    {
        UseLocal,
        UseRemote,
        UseLatest,
        Manual,
        Skip
    }

    public enum ExportDetailLevel
    {
        Summary,
        Standard,
        Detailed,
        Complete
    }

    #endregion
}
