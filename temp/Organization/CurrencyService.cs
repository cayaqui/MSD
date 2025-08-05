using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Organization.Currency;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

/// <summary>
/// Service implementation for Currency management
/// </summary>
public class CurrencyService : BaseService<Currency, CurrencyDto, CreateCurrencyDto, UpdateCurrencyDto>, ICurrencyService
{
    public CurrencyService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CurrencyService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<CurrencyDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Currency>()
            .GetAsync(
                filter: c => c.Code == code && c.IsActive,
                cancellationToken: cancellationToken);

        return entity != null ? _mapper.Map<CurrencyDto>(entity) : null;
    }

    public async Task<CurrencyDto?> GetBaseCurrencyAsync(CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Currency>()
            .GetAsync(
                filter: c => c.IsBaseCurrency && c.IsActive,
                cancellationToken: cancellationToken);

        return entity != null ? _mapper.Map<CurrencyDto>(entity) : null;
    }

    public async Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Currency>()
            .GetAllAsync(
                filter: c => c.IsActive,
                orderBy: q => q.OrderBy(c => c.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<CurrencyDto>>(entities);
    }

    public async Task<IEnumerable<CurrencyDto>> GetProjectCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Currency>()
            .GetAllAsync(
                filter: c => c.IsActive && c.IsEnabledForProjects,
                orderBy: q => q.OrderBy(c => c.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<CurrencyDto>>(entities);
    }

    public async Task<IEnumerable<CurrencyDto>> GetCommitmentCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Currency>()
            .GetAllAsync(
                filter: c => c.IsActive && c.IsEnabledForCommitments,
                orderBy: q => q.OrderBy(c => c.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<CurrencyDto>>(entities);
    }

    public async Task<CurrencyDto?> UpdateExchangeRateAsync(Guid id, UpdateExchangeRateDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Currency>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.UpdateExchangeRate(dto.ExchangeRate, dto.ExchangeRateSource ?? string.Empty, updatedBy ?? "System");

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Currency>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Currency {CurrencyCode} exchange rate updated to {Rate} by {User}", 
            entity.Code, dto.ExchangeRate, updatedBy ?? "System");

        return _mapper.Map<CurrencyDto>(entity);
    }

    public async Task<CurrencyDto?> SetAsBaseCurrencyAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        // First, unset any existing base currency
        var currentBase = await _unitOfWork.Repository<Currency>()
            .GetAsync(c => c.IsBaseCurrency && c.IsActive);
        
        if (currentBase != null)
        {
            currentBase.IsBaseCurrency = false;
            _unitOfWork.Repository<Currency>().Update(currentBase);
        }

        // Set the new base currency
        var entity = await _unitOfWork.Repository<Currency>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.SetAsBaseCurrency();

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Currency>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Currency {CurrencyCode} set as base currency by {User}", 
            entity.Code, updatedBy ?? "System");

        return _mapper.Map<CurrencyDto>(entity);
    }

    public async Task<CurrencyDto?> UpdateDisplaySettingsAsync(Guid id, string? decimalSeparator, string? thousandsSeparator, 
        string? positivePattern, string? negativePattern, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Currency>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.UpdateFormatSettings(
            decimalSeparator ?? entity.DecimalSeparator ?? ".",
            thousandsSeparator ?? entity.ThousandsSeparator ?? ",",
            entity.DecimalDigits);

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Currency>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Currency {CurrencyCode} display settings updated by {User}", 
            entity.Code, updatedBy ?? "System");

        return _mapper.Map<CurrencyDto>(entity);
    }

    public async Task<CurrencyDto?> EnableForProjectsAsync(Guid id, bool enable, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Currency>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.EnableForProjects(enable);

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Currency>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Currency {CurrencyCode} {Action} for projects by {User}", 
            entity.Code, enable ? "enabled" : "disabled", updatedBy ?? "System");

        return _mapper.Map<CurrencyDto>(entity);
    }

    public async Task<CurrencyDto?> EnableForCommitmentsAsync(Guid id, bool enable, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Currency>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.EnableForCommitments(enable);

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Currency>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Currency {CurrencyCode} {Action} for commitments by {User}", 
            entity.Code, enable ? "enabled" : "disabled", updatedBy ?? "System");

        return _mapper.Map<CurrencyDto>(entity);
    }

    public async Task<CurrencyConversionResultDto> ConvertAmountAsync(CurrencyConversionDto dto, CancellationToken cancellationToken = default)
    {
        var fromCurrency = await _unitOfWork.Repository<Currency>()
            .GetAsync(c => c.Code == dto.FromCurrency && c.IsActive);
        
        var toCurrency = await _unitOfWork.Repository<Currency>()
            .GetAsync(c => c.Code == dto.ToCurrency && c.IsActive);

        if (fromCurrency == null || toCurrency == null)
        {
            throw new InvalidOperationException("Invalid currency code(s) provided.");
        }

        // Get base currency for conversion
        var baseCurrency = await _unitOfWork.Repository<Currency>()
            .GetAsync(c => c.IsBaseCurrency && c.IsActive);

        if (baseCurrency == null)
        {
            throw new InvalidOperationException("No base currency configured.");
        }

        decimal convertedAmount;
        
        if (fromCurrency.Id == toCurrency.Id)
        {
            convertedAmount = dto.Amount;
        }
        else if (fromCurrency.IsBaseCurrency)
        {
            convertedAmount = dto.Amount * toCurrency.ExchangeRate;
        }
        else if (toCurrency.IsBaseCurrency)
        {
            convertedAmount = dto.Amount / fromCurrency.ExchangeRate;
        }
        else
        {
            // Convert through base currency
            var baseAmount = dto.Amount / fromCurrency.ExchangeRate;
            convertedAmount = baseAmount * toCurrency.ExchangeRate;
        }

        return new CurrencyConversionResultDto
        {
            FromCurrency = dto.FromCurrency,
            ToCurrency = dto.ToCurrency,
            Amount = dto.Amount,
            ConvertedAmount = Math.Round(convertedAmount, toCurrency.DecimalDigits),
            ExchangeRate = fromCurrency.IsBaseCurrency ? toCurrency.ExchangeRate : 
                          toCurrency.IsBaseCurrency ? (1 / fromCurrency.ExchangeRate) :
                          (toCurrency.ExchangeRate / fromCurrency.ExchangeRate),
            ConversionDate = dto.ConversionDate ?? DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<CurrencyDto>> GetStaleExchangeRatesAsync(int maxDays = 7, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-maxDays);
        
        var allCurrencies = await _unitOfWork.Repository<Currency>()
            .GetAllAsync(
                filter: c => !c.IsBaseCurrency && c.IsActive,
                orderBy: q => q.OrderBy(c => c.ExchangeRateDate),
                cancellationToken: cancellationToken);
        
        var entities = allCurrencies.Where(c => c.IsExchangeRateStale(maxDays));

        return _mapper.Map<IEnumerable<CurrencyDto>>(entities);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.Repository<Currency>()
            .AnyAsync(c => c.Code == code && 
                          (!excludeId.HasValue || c.Id != excludeId.Value) && 
                          c.IsActive,
                      cancellationToken);

        return !exists;
    }

    protected override async Task ValidateEntityAsync(Currency entity, bool isNew)
    {
        // Validate unique code
        if (isNew || entity.Code != null)
        {
            var isUnique = await IsCodeUniqueAsync(entity.Code, isNew ? null : entity.Id);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Currency code '{entity.Code}' already exists.");
            }
        }

        // Validate exchange rate
        if (entity.ExchangeRate <= 0)
        {
            throw new InvalidOperationException("Exchange rate must be greater than zero.");
        }

        // Validate decimal digits
        if (entity.DecimalDigits < 0 || entity.DecimalDigits > 8)
        {
            throw new InvalidOperationException("Decimal digits must be between 0 and 8.");
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}