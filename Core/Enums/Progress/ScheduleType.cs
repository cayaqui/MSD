namespace Core.Enums.Progress;

/// <summary>
/// Types of project schedules
/// </summary>
public enum ScheduleType
{
    /// <summary>
    /// Master project schedule
    /// </summary>
    Master = 1,

    /// <summary>
    /// Baseline schedule
    /// </summary>
    Baseline = 2,

    /// <summary>
    /// Current/working schedule
    /// </summary>
    Current = 3,

    /// <summary>
    /// Target schedule
    /// </summary>
    Target = 4,

    /// <summary>
    /// What-if scenario schedule
    /// </summary>
    WhatIf = 5,

    /// <summary>
    /// Recovery schedule
    /// </summary>
    Recovery = 6,

    /// <summary>
    /// Milestone schedule
    /// </summary>
    Milestone = 7,

    /// <summary>
    /// Summary schedule
    /// </summary>
    Summary = 8
}