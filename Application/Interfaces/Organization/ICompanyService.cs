using Application.Interfaces.Common;
using Core.DTOs.Organization.Company;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Company management
    /// </summary>
    public interface ICompanyService : IBaseService<CompanyDto, CreateCompanyDto, UpdateCompanyDto>
    {
        /// <summary>
        /// Gets a company by its code
        /// </summary>
        Task<CompanyDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates company business information
        /// </summary>
        Task<CompanyDto?> UpdateBusinessInfoAsync(Guid id, UpdateCompanyBusinessInfoDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates company address
        /// </summary>
        Task<CompanyDto?> UpdateAddressAsync(Guid id, UpdateCompanyAddressDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates company contact information
        /// </summary>
        Task<CompanyDto?> UpdateContactInfoAsync(Guid id, UpdateCompanyContactDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates company logo
        /// </summary>
        Task<CompanyDto?> UpdateLogoAsync(Guid id, UpdateCompanyLogoDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates company configuration
        /// </summary>
        Task<CompanyDto?> UpdateConfigurationAsync(Guid id, UpdateCompanyConfigDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets companies with their operations
        /// </summary>
        Task<IEnumerable<CompanyWithOperationsDto>> GetCompaniesWithOperationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a company code is unique
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}