using Core.DTOs.Common;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for phase API operations
/// </summary>
public interface IPhaseApiService
{
    /// <summary>
    /// Get all phases with pagination
    /// </summary>
    Task<PagedResult<Core.DTOs.Organization.Phase.PhaseDto>?> GetPhasesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get phase by ID
    /// </summary>
    Task<Core.DTOs.Organization.Phase.PhaseDto?> GetPhaseByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get phases by project ID
    /// </summary>
    Task<List<Core.DTOs.Organization.Phase.PhaseDto>?> GetPhasesByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get phase with milestones
    /// </summary>
    Task<Core.DTOs.Organization.Phase.PhaseWithMilestonesDto?> GetPhaseWithMilestonesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get phase with deliverables
    /// </summary>
    Task<Core.DTOs.Organization.Phase.PhaseWithDeliverablesDto?> GetPhaseWithDeliverablesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new phase
    /// </summary>
    Task<Core.DTOs.Organization.Phase.PhaseDto?> CreatePhaseAsync(Core.DTOs.Organization.Phase.CreatePhaseDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing phase
    /// </summary>
    Task<Core.DTOs.Organization.Phase.PhaseDto?> UpdatePhaseAsync(Guid id, Core.DTOs.Organization.Phase.UpdatePhaseDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update phase schedule
    /// </summary>
    Task<bool> UpdatePhaseScheduleAsync(Guid id, Core.DTOs.Organization.Phase.UpdatePhaseScheduleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update phase budget
    /// </summary>
    Task<bool> UpdatePhaseBudgetAsync(Guid id, Core.DTOs.Organization.Phase.UpdatePhaseBudgetDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approve phase gate
    /// </summary>
    Task<bool> ApprovePhaseGateAsync(Guid id, Core.DTOs.Organization.Phase.ApprovePhaseGateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a phase (soft delete)
    /// </summary>
    Task<bool> DeletePhaseAsync(Guid id, CancellationToken cancellationToken = default);
}