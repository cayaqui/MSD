using Application.Interfaces.Common;
using Core.DTOs.Organization.Project;
using Core.Enums.Projects;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Project management
    /// </summary>
    public interface IProjectService : IBaseService<ProjectDto, CreateProjectDto, UpdateProjectDto>
    {
        /// <summary>
        /// Gets a project by its code
        /// </summary>
        Task<ProjectDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets projects by operation
        /// </summary>
        Task<IEnumerable<ProjectDto>> GetByOperationAsync(Guid operationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets projects by status
        /// </summary>
        Task<IEnumerable<ProjectDto>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets active projects
        /// </summary>
        Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates project charter information
        /// </summary>
        Task<ProjectDto?> UpdateProjectCharterAsync(Guid id, UpdateProjectCharterDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates project dates
        /// </summary>
        Task<ProjectDto?> UpdateDatesAsync(Guid id, UpdateProjectDatesDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates project budget
        /// </summary>
        Task<ProjectDto?> UpdateBudgetAsync(Guid id, UpdateProjectBudgetDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates project status
        /// </summary>
        Task<ProjectDto?> UpdateStatusAsync(Guid id, ProjectStatus status, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns project manager
        /// </summary>
        Task<ProjectDto?> AssignProjectManagerAsync(Guid id, AssignProjectManagerDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates project progress
        /// </summary>
        Task<ProjectDto?> UpdateProgressAsync(Guid id, UpdateProjectProgressDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates project costs
        /// </summary>
        Task<ProjectDto?> UpdateCostsAsync(Guid id, UpdateProjectCostsDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets project baseline
        /// </summary>
        Task<ProjectDto?> SetBaselineAsync(Guid id, DateTime baselineDate, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets project summary with financial data
        /// </summary>
        Task<ProjectSummaryDto?> GetProjectSummaryAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets project dashboard data
        /// </summary>
        Task<ProjectDashboardDto?> GetProjectDashboardAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets overdue projects
        /// </summary>
        Task<IEnumerable<ProjectDto>> GetOverdueProjectsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets over budget projects
        /// </summary>
        Task<IEnumerable<ProjectDto>> GetOverBudgetProjectsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if project code is unique
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}