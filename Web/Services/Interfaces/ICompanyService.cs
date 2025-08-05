using Core.DTOs.Organization.Company;
using Core.DTOs.Common;

namespace Web.Services.Interfaces;

public interface ICompanyService
{
    Task<PagedResult<CompanyDto>> GetCompaniesAsync(int page = 1, int pageSize = 10);
    Task<CompanyDto?> GetCompanyByIdAsync(Guid companyId);
    Task<CompanyDto?> CreateCompanyAsync(CreateCompanyDto dto);
    Task<CompanyDto?> UpdateCompanyAsync(Guid companyId, UpdateCompanyDto dto);
    Task<bool> DeleteCompanyAsync(Guid companyId);
}