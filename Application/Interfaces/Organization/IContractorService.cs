using Application.Interfaces.Common;
using Core.DTOs.Organization.Contractor;
using Core.Enums.Projects;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Contractor/Vendor management
    /// </summary>
    public interface IContractorService : IBaseService<ContractorDto, CreateContractorDto, UpdateContractorDto>
    {
        /// <summary>
        /// Gets a contractor by code
        /// </summary>
        Task<ContractorDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets contractors by type
        /// </summary>
        Task<IEnumerable<ContractorDto>> GetByTypeAsync(ContractorType type, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets prequalified contractors
        /// </summary>
        Task<IEnumerable<ContractorDto>> GetPrequalifiedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Prequalifies a contractor
        /// </summary>
        Task<ContractorDto?> PrequalifyAsync(Guid id, PrequalifyContractorDto dto, string? approvedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates contractor contact information
        /// </summary>
        Task<ContractorDto?> UpdateContactInfoAsync(Guid id, string? contactName, string? email, string? phone, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates contractor financial information
        /// </summary>
        Task<ContractorDto?> UpdateFinancialInfoAsync(Guid id, string? bankName, string? bankAccount, string? paymentTerms, decimal creditLimit, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates contractor insurance information
        /// </summary>
        Task<ContractorDto?> UpdateInsuranceAsync(Guid id, UpdateContractorInsuranceDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates contractor status
        /// </summary>
        Task<ContractorDto?> UpdateStatusAsync(Guid id, ContractorStatus status, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates contractor performance rating
        /// </summary>
        Task<ContractorDto?> UpdatePerformanceRatingAsync(Guid id, decimal rating, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets contractors with expired insurance
        /// </summary>
        Task<IEnumerable<ContractorDto>> GetExpiredInsuranceAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets contractors eligible for award
        /// </summary>
        Task<IEnumerable<ContractorDto>> GetEligibleForAwardAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets contractor with commitments summary
        /// </summary>
        Task<ContractorDto?> GetWithCommitmentsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if contractor code is unique
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}