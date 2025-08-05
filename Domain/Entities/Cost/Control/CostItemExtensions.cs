using Domain.Entities.Cost.Control;
using Domain.Entities.WBS;

namespace Domain.Entities.Cost.Control;

/// <summary>
/// Extension methods for PlanningPackage entity
/// </summary>
public static class PlanningPackageExtensions
{

    /// <summary>
    /// Marks the planning package as converted
    /// </summary>
    public static void MarkAsConverted(this PlanningPackage planningPackage, string userId)
    {
        planningPackage.ConvertToWorkPackage(userId);
    }
}