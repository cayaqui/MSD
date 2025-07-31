using Core.DTOs.Companies;

namespace Application.Interfaces.Setup;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllAsync();
    Task<CompanyDto?> GetByIdAsync(Guid id);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto);
    Task<CompanyDto> UpdateAsync(Guid id, UpdateCompanyDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
    Task<byte[]?> GetLogoAsync(Guid id);
    Task UpdateLogoAsync(Guid id, byte[] logo, string contentType);
    Task<CompanyWithOperationsDto> GetWithOperationsAsync(Guid id);
    Task DeleteLogoAsync(Guid id);
}