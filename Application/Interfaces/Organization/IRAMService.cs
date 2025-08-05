using Application.Interfaces.Common;
using Core.DTOs.Organization;
using Core.DTOs.Organization.RAM;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Responsibility Assignment Matrix (RAM/RACI) management
    /// </summary>
    public interface IRAMService : IBaseService<RAMDto, CreateRAMDto, UpdateRAMDto>
    {
        /// <summary>
        /// Gets RAM assignments by project
        /// </summary>
        Task<IEnumerable<RAMDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets RAM assignments by WBS element
        /// </summary>
        Task<IEnumerable<RAMDto>> GetByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets RAM assignments by OBS node
        /// </summary>
        Task<IEnumerable<RAMDto>> GetByOBSNodeAsync(Guid obsNodeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets RAM assignments by responsibility type
        /// </summary>
        Task<IEnumerable<RAMDto>> GetByResponsibilityTypeAsync(Guid projectId, string responsibilityType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates allocation for RAM assignment
        /// </summary>
        Task<RAMDto?> UpdateAllocationAsync(Guid id, UpdateRAMAllocationDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets period for RAM assignment
        /// </summary>
        Task<RAMDto?> SetPeriodAsync(Guid id, DateTime startDate, DateTime endDate, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Links RAM assignment to control account
        /// </summary>
        Task<RAMDto?> LinkToControlAccountAsync(Guid id, Guid controlAccountId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets RAM matrix for project
        /// </summary>
        Task<RAMMatrixDto> GetRAMMatrixAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets accountable assignments for WBS element
        /// </summary>
        Task<IEnumerable<RAMDto>> GetAccountableAssignmentsAsync(Guid wbsElementId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates RACI assignments (ensures single Accountable per WBS)
        /// </summary>
        Task<RAMValidationResult> ValidateAssignmentsAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Bulk creates RAM assignments
        /// </summary>
        Task<IEnumerable<RAMDto>> BulkCreateAsync(IEnumerable<CreateRAMDto> assignments, string? createdBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Copies RAM assignments from template
        /// </summary>
        Task<IEnumerable<RAMDto>> CopyFromTemplateAsync(Guid sourceProjectId, Guid targetProjectId, string? createdBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets resource allocation summary
        /// </summary>
        Task<ResourceAllocationSummaryDto> GetResourceAllocationSummaryAsync(Guid obsNodeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks for conflicts in assignments
        /// </summary>
        Task<IEnumerable<RAMConflictDto>> CheckConflictsAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}