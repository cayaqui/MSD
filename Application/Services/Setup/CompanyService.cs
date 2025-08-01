using Application.Common.Exceptions;
using Application.Interfaces.Auth;
using Application.Interfaces.Setup;
using Core.DTOs.Companies;

namespace Application.Services.Setup;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CompanyService> _logger;

    // Allowed file types for logo
    private readonly string[] _allowedLogoTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif" };
    private const int MaxLogoSizeInBytes = 5 * 1024 * 1024; // 5MB

    public CompanyService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<CompanyService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllAsync()
    {
        var companies = await _unitOfWork.Repository<Company>()
            .Query()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} companies", companies.Count);

        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<CompanyDto?> GetByIdAsync(Guid id)
    {
        var company = await _unitOfWork.Repository<Company>()
            .Query()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", id);
            return null;
        }

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
    {
        // Validate code uniqueness
        if (!await IsCodeUniqueAsync(dto.Code))
        {
            throw new BadRequestException($"Company code '{dto.Code}' already exists");
        }

        // Create company entity
        var company = new Company(dto.Code, dto.Name, dto.TaxId);

        // Set optional properties
        if (!string.IsNullOrEmpty(dto.Description))
        {
            company.UpdateBusinessInfo(dto.Name, dto.Description, dto.TaxId);
        }

        if (!string.IsNullOrEmpty(dto.DefaultCurrency))
        {
            company.UpdateConfiguration(dto.DefaultCurrency, null);
        }

        // Set audit fields
        company.CreatedBy = _currentUserService.UserId;
        company.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Company>().AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Company {CompanyName} created with ID {CompanyId} by user {UserId}",
            company.Name, company.Id, _currentUserService.UserId);

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> UpdateAsync(Guid id, UpdateCompanyDto dto)
    {
        var company = await _unitOfWork.Repository<Company>()
            .Query()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), id);
        }

        // Update business information
        if (!string.IsNullOrEmpty(dto.Name) || !string.IsNullOrEmpty(dto.TaxId))
        {
            company.UpdateBusinessInfo(
                dto.Name ?? company.Name,
                dto.Description ?? company.Description,
                dto.TaxId ?? company.TaxId
            );
        }

        // Update address if any address field is provided
        if (!string.IsNullOrEmpty(dto.Address) || !string.IsNullOrEmpty(dto.City) ||
            !string.IsNullOrEmpty(dto.State) || !string.IsNullOrEmpty(dto.Country) ||
            !string.IsNullOrEmpty(dto.PostalCode))
        {
            company.UpdateAddress(
                dto.Address ?? company.Address,
                dto.City ?? company.City,
                dto.State ?? company.State,
                dto.Country ?? company.Country,
                dto.PostalCode ?? company.PostalCode
            );
        }

        // Update contact information if any contact field is provided
        if (!string.IsNullOrEmpty(dto.Phone) || !string.IsNullOrEmpty(dto.Email) ||
            !string.IsNullOrEmpty(dto.Website))
        {
            company.UpdateContactInfo(
                dto.Phone ?? company.Phone,
                dto.Email ?? company.Email,
                dto.Website ?? company.Website
            );
        }

        // Update audit fields
        company.UpdatedBy = _currentUserService.UserId;
        company.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Company {CompanyId} updated by user {UserId}", id, _currentUserService.UserId);

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await _unitOfWork.Repository<Company>()
            .Query()
            .Include(c => c.Operations)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), id);
        }

        // Check if company has active operations
        if (company.Operations.Any(o => !o.IsDeleted))
        {
            throw new BadRequestException(
                "Cannot delete company with active operations. Please delete or reassign all operations first.",
                "COMPANY_HAS_OPERATIONS"
            );
        }

        // Soft delete
        company.IsDeleted = true;
        company.DeletedAt = DateTime.UtcNow;
        company.DeletedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Company {CompanyId} deleted by user {UserId}", id, _currentUserService.UserId);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var query = _unitOfWork.Repository<Company>()
            .Query()
            .Where(c => c.Code == code && !c.IsDeleted);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        var exists = await query.AnyAsync();

        return !exists;
    }

    public async Task<byte[]?> GetLogoAsync(Guid id)
    {
        var company = await _unitOfWork.Repository<Company>()
            .Query()
            .Select(c => new { c.Id, c.Logo, c.IsDeleted })
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), id);
        }

        return company.Logo;
    }

    public async Task UpdateLogoAsync(Guid id, byte[] logo, string contentType)
    {
        if (logo == null || logo.Length == 0)
        {
            throw new BadRequestException("Logo file is empty");
        }

        if (logo.Length > MaxLogoSizeInBytes)
        {
            throw new BadRequestException($"Logo file size exceeds maximum allowed size of {MaxLogoSizeInBytes / 1024 / 1024}MB");
        }

        if (!_allowedLogoTypes.Contains(contentType.ToLower()))
        {
            throw new BadRequestException(
                $"Invalid file type. Allowed types are: {string.Join(", ", _allowedLogoTypes)}",
                "INVALID_FILE_TYPE"
            );
        }

        var company = await _unitOfWork.Repository<Company>()
            .Query()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), id);
        }

        // Validate image format
        if (!IsValidImage(logo, contentType))
        {
            throw new BadRequestException("Invalid image format or corrupted file");
        }

        // Update logo
        company.UpdateLogo(logo, contentType);

        // Update audit fields
        company.UpdatedBy = _currentUserService.UserId;
        company.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Logo updated for company {CompanyId} by user {UserId}. Size: {Size} bytes",
            id, _currentUserService.UserId, logo.Length);
    }

    public async Task DeleteLogoAsync(Guid id)
    {
        var company = await _unitOfWork.Repository<Company>()
            .Query()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), id);
        }

        company.Logo = null;
        company.LogoContentType = null;
        company.UpdatedBy = _currentUserService.UserId;
        company.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Logo deleted for company {CompanyId} by user {UserId}",
            id, _currentUserService.UserId);
    }

    public async Task<CompanyWithOperationsDto> GetWithOperationsAsync(Guid id)
    {
        var company = await _unitOfWork.Repository<Company>()
            .Query()
            .Include(c => c.Operations.Where(o => !o.IsDeleted))
                .ThenInclude(o => o.Projects.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), id);
        }

        return _mapper.Map<CompanyWithOperationsDto>(company);
    }

    // Helper method to validate image
    private bool IsValidImage(byte[] imageData, string contentType)
    {
        try
        {
            // Check file signature (magic numbers)
            if (imageData.Length < 4)
                return false;

            // JPEG
            if (contentType.Contains("jpeg") || contentType.Contains("jpg"))
            {
                return imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF;
            }

            // PNG
            if (contentType.Contains("png"))
            {
                return imageData[0] == 0x89 && imageData[1] == 0x50 &&
                       imageData[2] == 0x4E && imageData[3] == 0x47;
            }

            // GIF
            if (contentType.Contains("gif"))
            {
                return (imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}