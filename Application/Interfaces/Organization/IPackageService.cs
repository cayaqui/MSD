using Application.Interfaces.Common;
using Core.DTOs.Organization.Package;
using Core.Enums.Projects;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Package management
    /// </summary>
    public interface IPackageService : IBaseService<PackageDto, CreatePackageDto, UpdatePackageDto>
    {
        /// <summary>
        /// Gets packages by phase
        /// </summary>
        Task<IEnumerable<PackageDto>> GetByPhaseAsync(Guid phaseId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets packages by WBS element
        /// </summary>
        Task<IEnumerable<PackageDto>> GetByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets packages by contractor
        /// </summary>
        Task<IEnumerable<PackageDto>> GetByContractorAsync(Guid contractorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets packages by type
        /// </summary>
        Task<IEnumerable<PackageDto>> GetByTypeAsync(PackageType packageType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns package to phase
        /// </summary>
        Task<PackageDto?> AssignToPhaseAsync(Guid id, Guid phaseId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns package to work package
        /// </summary>
        Task<PackageDto?> AssignToWorkPackageAsync(Guid id, Guid wbsElementId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns contractor to package
        /// </summary>
        Task<PackageDto?> AssignContractorAsync(Guid id, Guid? contractorId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates package contract information
        /// </summary>
        Task<PackageDto?> UpdateContractInfoAsync(Guid id, UpdatePackageContractDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates package progress
        /// </summary>
        Task<PackageDto?> UpdateProgressAsync(Guid id, decimal progressPercentage, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates package dates
        /// </summary>
        Task<PackageDto?> UpdateDatesAsync(Guid id, UpdatePackageDatesDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a package
        /// </summary>
        Task<PackageDto?> StartPackageAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Completes a package
        /// </summary>
        Task<PackageDto?> CompletePackageAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels a package
        /// </summary>
        Task<PackageDto?> CancelPackageAsync(Guid id, string reason, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets overdue packages
        /// </summary>
        Task<IEnumerable<PackageDto>> GetOverduePackagesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets package with disciplines
        /// </summary>
        Task<PackageWithDisciplinesDto?> GetWithDisciplinesAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets package performance metrics
        /// </summary>
        Task<PackagePerformanceDto?> GetPerformanceMetricsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}