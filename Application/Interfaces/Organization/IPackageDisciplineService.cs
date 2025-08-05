using Application.Interfaces.Common;
using Core.DTOs.Organization.Package;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Package Discipline management
    /// </summary>
    public interface IPackageDisciplineService : IBaseService<PackageDisciplineDto, CreatePackageDisciplineDto, UpdatePackageDisciplineDto>
    {
        /// <summary>
        /// Gets disciplines by package
        /// </summary>
        Task<IEnumerable<PackageDisciplineDto>> GetByPackageAsync(Guid packageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets packages by discipline
        /// </summary>
        Task<IEnumerable<PackageDisciplineDto>> GetByDisciplineAsync(Guid disciplineId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets lead discipline for a package
        /// </summary>
        Task<PackageDisciplineDto?> GetLeadDisciplineAsync(Guid packageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates discipline estimate
        /// </summary>
        Task<PackageDisciplineDto?> UpdateEstimateAsync(Guid id, UpdateDisciplineEstimateDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates discipline actuals
        /// </summary>
        Task<PackageDisciplineDto?> UpdateActualsAsync(Guid id, UpdateDisciplineActualsDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns lead engineer to discipline
        /// </summary>
        Task<PackageDisciplineDto?> AssignLeadEngineerAsync(Guid id, Guid? leadEngineerId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets discipline as lead
        /// </summary>
        Task<PackageDisciplineDto?> SetAsLeadDisciplineAsync(Guid id, bool isLead, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets discipline performance summary
        /// </summary>
        Task<DisciplinePerformanceSummaryDto> GetPerformanceSummaryAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets disciplines by lead engineer
        /// </summary>
        Task<IEnumerable<PackageDisciplineDto>> GetByLeadEngineerAsync(Guid leadEngineerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Bulk creates package disciplines
        /// </summary>
        Task<IEnumerable<PackageDisciplineDto>> BulkCreateAsync(Guid packageId, IEnumerable<CreatePackageDisciplineDto> disciplines, string? createdBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets package discipline allocation summary
        /// </summary>
        Task<PackageDisciplineAllocationDto> GetAllocationSummaryAsync(Guid packageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates discipline assignments (ensures single lead discipline)
        /// </summary>
        Task<bool> ValidateLeadDisciplineAsync(Guid packageId, Guid? excludeDisciplineId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}