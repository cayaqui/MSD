using Application.Interfaces.Common;
using Core.DTOs.Organization.Phase;
using Core.Enums.Projects;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Project Phase management
    /// </summary>
    public interface IPhaseService : IBaseService<PhaseDto, CreatePhaseDto, UpdatePhaseDto>
    {
        /// <summary>
        /// Gets phases by project
        /// </summary>
        Task<IEnumerable<PhaseDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets phases by type
        /// </summary>
        Task<IEnumerable<PhaseDto>> GetByTypeAsync(PhaseType type, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets phases by status
        /// </summary>
        Task<IEnumerable<PhaseDto>> GetByStatusAsync(PhaseStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a phase
        /// </summary>
        Task<PhaseDto?> StartPhaseAsync(Guid id, string? userId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Completes a phase
        /// </summary>
        Task<PhaseDto?> CompletePhaseAsync(Guid id, string? userId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Approves phase gate
        /// </summary>
        Task<PhaseDto?> ApproveGateAsync(Guid id, string? approvedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates phase progress
        /// </summary>
        Task<PhaseDto?> UpdateProgressAsync(Guid id, UpdatePhaseProgressDto dto, string? userId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets phase with details
        /// </summary>
        Task<PhaseDetailDto?> GetPhaseDetailAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets phases requiring gate approval
        /// </summary>
        Task<IEnumerable<PhaseDto>> GetPendingGateApprovalsAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets over budget phases
        /// </summary>
        Task<IEnumerable<PhaseDto>> GetOverBudgetPhasesAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets delayed phases
        /// </summary>
        Task<IEnumerable<PhaseDto>> GetDelayedPhasesAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates phase sequence
        /// </summary>
        Task<IEnumerable<PhaseDto>> UpdateSequenceAsync(Guid projectId, IEnumerable<PhaseSequenceDto> sequences, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates weighted project progress from phases
        /// </summary>
        Task<decimal> CalculateProjectProgressAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}