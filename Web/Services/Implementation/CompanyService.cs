using Core.DTOs.Organization.Company;
using Core.DTOs.Common;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class CompanyService : ICompanyService
{
    private readonly IApiService _apiService;
    
    public CompanyService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task<PagedResult<CompanyDto>> GetCompaniesAsync(int page = 1, int pageSize = 10)
    {
        return await _apiService.GetAsync<PagedResult<CompanyDto>>($"api/companies?page={page}&pageSize={pageSize}") 
            ?? new PagedResult<CompanyDto>();
    }
    
    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid companyId)
    {
        return await _apiService.GetAsync<CompanyDto>($"api/companies/{companyId}");
    }
    
    public async Task<CompanyDto?> CreateCompanyAsync(CreateCompanyDto dto)
    {
        return await _apiService.PostAsync<CreateCompanyDto, CompanyDto>("api/companies", dto);
    }
    
    public async Task<CompanyDto?> UpdateCompanyAsync(Guid companyId, UpdateCompanyDto dto)
    {
        return await _apiService.PutAsync<UpdateCompanyDto, CompanyDto>($"api/companies/{companyId}", dto);
    }
    
    public async Task<bool> DeleteCompanyAsync(Guid companyId)
    {
        return await _apiService.DeleteAsync($"api/companies/{companyId}");
    }
}