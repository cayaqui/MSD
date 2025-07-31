using Core.DTOs.Companies;
using Web.Models.Responses;

namespace Web.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<ApiResponse<List<CompanyDto>>> GetCompaniesAsync();
        Task<ApiResponse<CompanyDto>> GetCompanyByIdAsync(Guid id);
        Task<ApiResponse<CompanyDto>> CreateCompanyAsync(CreateCompanyDto dto);
        Task<ApiResponse<CompanyDto>> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto);
        Task<ApiResponse<bool>> DeleteCompanyAsync(Guid id);
    }
}