using Core.DTOs.Common;
using Core.Enums.Projects;

namespace Core.DTOs.Cost;

/// <summary>
/// Query parameters specific to planning package endpoints
/// </summary>
public class PlanningPackageQueryParameters : QueryParameters
{
    /// <summary>
    /// Filter by planning package status
    /// </summary>
    public PlanningPackageStatus? Status { get; set; }

    /// <summary>
    /// Filter by conversion status
    /// </summary>
    public bool? IsConverted { get; set; }

    /// <summary>
    /// Filter by phase ID
    /// </summary>
    public Guid? PhaseId { get; set; }
}