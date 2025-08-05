using Application.Interfaces.Common;
using Core.DTOs.Organization.Operation;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Operation management
    /// </summary>
    public interface IOperationService : IBaseService<OperationDto, CreateOperationDto, UpdateOperationDto>
    {
        /// <summary>
        /// Gets an operation by its code
        /// </summary>
        Task<OperationDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets operations by company
        /// </summary>
        Task<IEnumerable<OperationDto>> GetByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets operations by country
        /// </summary>
        Task<IEnumerable<OperationDto>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates operation basic information
        /// </summary>
        Task<OperationDto?> UpdateBasicInfoAsync(Guid id, UpdateOperationBasicInfoDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates operation address
        /// </summary>
        Task<OperationDto?> UpdateAddressAsync(Guid id, UpdateOperationAddressDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates operation manager
        /// </summary>
        Task<OperationDto?> UpdateManagerAsync(Guid id, UpdateOperationManagerDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates operation cost center
        /// </summary>
        Task<OperationDto?> UpdateCostCenterAsync(Guid id, string costCenter, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets operation with projects
        /// </summary>
        Task<OperationWithProjectsDto?> GetWithProjectsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if operation code is unique within company
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string code, Guid companyId, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}