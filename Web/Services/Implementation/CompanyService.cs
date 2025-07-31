using Core.DTOs.Companies;
using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class CompanyService : ICompanyService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<CompanyService> _logger;
        private const string BaseEndpoint = "api/companies";

        public CompanyService(IApiService apiService, ILogger<CompanyService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CompanyDto>>> GetCompaniesAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<CompanyDto>>(BaseEndpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las empresas");
                return ApiResponse<List<CompanyDto>>.ErrorResponse("Error al cargar las empresas");
            }
        }

        public async Task<ApiResponse<CompanyDto>> GetCompanyByIdAsync(Guid id)
        {
            try
            {
                return await _apiService.GetAsync<CompanyDto>($"{BaseEndpoint}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la empresa {CompanyId}", id);
                return ApiResponse<CompanyDto>.ErrorResponse("Error al cargar la empresa");
            }
        }

        public async Task<ApiResponse<CompanyDto>> CreateCompanyAsync(CreateCompanyDto dto)
        {
            try
            {
                return await _apiService.PostAsync<CompanyDto>(BaseEndpoint, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la empresa");
                return ApiResponse<CompanyDto>.ErrorResponse("Error al crear la empresa");
            }
        }

        public async Task<ApiResponse<CompanyDto>> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto)
        {
            try
            {
                return await _apiService.PutAsync<CompanyDto>($"{BaseEndpoint}/{id}", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la empresa {CompanyId}", id);
                return ApiResponse<CompanyDto>.ErrorResponse("Error al actualizar la empresa");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCompanyAsync(Guid id)
        {
            try
            {
                return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la empresa {CompanyId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar la empresa");
            }
        }
    }
}