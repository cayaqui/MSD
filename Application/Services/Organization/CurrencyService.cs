    using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.Currency;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization
{
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

        /// <summary>
        /// Gets a currency by its code (e.g., USD, EUR)
        /// </summary>
        public async Task<CurrencyDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var entity = await _unitOfWork.Repository<Currency>()
                .GetAsync(
                    filter: c => c.Code == code && c.IsActive,
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Gets the base currency
        /// </summary>
        public async Task<CurrencyDto?> GetBaseCurrencyAsync(CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetAsync(
                    filter: c => c.IsBaseCurrency && c.IsActive,
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Gets all active currencies
        /// </summary>
        public async Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            var currencies = await _unitOfWork.Repository<Currency>()
                .GetAllAsync(
                    filter: c => c.IsActive,
                    orderBy: q => q.OrderBy(c => c.Code),
                    cancellationToken: cancellationToken);

            var dtos = currencies.Select(c =>
            {
                var dto = _mapper.Map<CurrencyDto>(c);
                dto.IsExchangeRateStale = c.IsExchangeRateStale();
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets currencies enabled for projects
        /// </summary>
        public async Task<IEnumerable<CurrencyDto>> GetProjectCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            var currencies = await _unitOfWork.Repository<Currency>()
                .GetAllAsync(
                    filter: c => c.IsActive && c.IsEnabledForProjects,
                    orderBy: q => q.OrderBy(c => c.Code),
                    cancellationToken: cancellationToken);

            var dtos = currencies.Select(c =>
            {
                var dto = _mapper.Map<CurrencyDto>(c);
                dto.IsExchangeRateStale = c.IsExchangeRateStale();
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets currencies enabled for commitments
        /// </summary>
        public async Task<IEnumerable<CurrencyDto>> GetCommitmentCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            var currencies = await _unitOfWork.Repository<Currency>()
                .GetAllAsync(
                    filter: c => c.IsActive && c.IsEnabledForCommitments,
                    orderBy: q => q.OrderBy(c => c.Code),
                    cancellationToken: cancellationToken);

            var dtos = currencies.Select(c =>
            {
                var dto = _mapper.Map<CurrencyDto>(c);
                dto.IsExchangeRateStale = c.IsExchangeRateStale();
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Updates exchange rate for a currency
        /// </summary>
        public async Task<CurrencyDto?> UpdateExchangeRateAsync(
            Guid id,
            UpdateExchangeRateDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || !entity.IsActive)
                return null;

            entity.UpdateExchangeRate(dto.Rate, dto.Source ?? string.Empty, updatedBy ?? "System");

            _unitOfWork.Repository<Currency>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Currency {CurrencyCode} exchange rate updated to {Rate} by {User}",
                entity.Code, dto.Rate, updatedBy ?? "System");

            var resultDto = _mapper.Map<CurrencyDto>(entity);
            resultDto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return resultDto;
        }

        /// <summary>
        /// Sets a currency as base currency
        /// </summary>
        public async Task<CurrencyDto?> SetAsBaseCurrencyAsync(
            Guid id,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || !entity.IsActive)
                return null;

            // Remove base currency status from current base currency
            var currentBase = await _unitOfWork.Repository<Currency>()
                .GetAsync(
                    filter: c => c.IsBaseCurrency && c.Id != id,
                    cancellationToken: cancellationToken);

            if (currentBase != null)
            {
                currentBase.IsBaseCurrency = false;
                currentBase.UpdatedBy = updatedBy ?? "System";
                currentBase.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Currency>().Update(currentBase);
            }

            // Set new base currency
            entity.SetAsBaseCurrency(updatedBy ?? "System");
            _unitOfWork.Repository<Currency>().Update(entity);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Currency {CurrencyCode} set as base currency by {User}",
                entity.Code, updatedBy ?? "System");

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Updates display settings for a currency
        /// </summary>
        public async Task<CurrencyDto?> UpdateDisplaySettingsAsync(
            Guid id,
            string? decimalSeparator,
            string? thousandsSeparator,
            string? positivePattern,
            string? negativePattern,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || !entity.IsActive)
                return null;

            entity.UpdateDisplaySettings(
                decimalSeparator,
                thousandsSeparator,
                positivePattern,
                negativePattern,
                updatedBy ?? "System");

            _unitOfWork.Repository<Currency>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Enables/disables currency for projects
        /// </summary>
        public async Task<CurrencyDto?> EnableForProjectsAsync(
            Guid id,
            bool enable,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || !entity.IsActive)
                return null;

            entity.EnableForProjects(enable, updatedBy ?? "System");

            _unitOfWork.Repository<Currency>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Enables/disables currency for commitments
        /// </summary>
        public async Task<CurrencyDto?> EnableForCommitmentsAsync(
            Guid id,
            bool enable,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || !entity.IsActive)
                return null;

            entity.EnableForCommitments(enable, updatedBy ?? "System");

            _unitOfWork.Repository<Currency>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Converts amount between currencies
        /// </summary>
        public async Task<CurrencyConversionResultDto> ConvertAmountAsync(
            CurrencyConversionDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = new CurrencyConversionResultDto
            {
                FromCurrency = dto.FromCurrencyCode,
                ToCurrency = dto.ToCurrencyCode,
                OriginalAmount = dto.Amount,
                ConversionDate = DateTime.UtcNow
            };

            // Get source currency
            var fromCurrency = await _unitOfWork.Repository<Currency>()
                .GetAsync(
                    filter: c => c.Code == dto.FromCurrencyCode && c.IsActive,
                    cancellationToken: cancellationToken);

            if (fromCurrency == null)
            {
                // Currency not found
                return result;
            }

            // Get target currency
            var toCurrency = await _unitOfWork.Repository<Currency>()
                .GetAsync(
                    filter: c => c.Code == dto.ToCurrencyCode && c.IsActive,
                    cancellationToken: cancellationToken);

            if (toCurrency == null)
            {
                // Currency not found
                return result;
            }

            try
            {
                result.ConvertedAmount = fromCurrency.ConvertTo(dto.Amount, toCurrency);
                result.ExchangeRate = fromCurrency.IsBaseCurrency 
                    ? 1 / toCurrency.ExchangeRate 
                    : toCurrency.IsBaseCurrency 
                        ? fromCurrency.ExchangeRate 
                        : fromCurrency.ExchangeRate / toCurrency.ExchangeRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting {Amount} from {FromCurrency} to {ToCurrency}",
                    dto.Amount, dto.FromCurrencyCode, dto.ToCurrencyCode);
                
                // Return result with zero converted amount on error
                result.ConvertedAmount = 0;
            }

            return result;
        }

        /// <summary>
        /// Gets currencies with stale exchange rates
        /// </summary>
        public async Task<IEnumerable<CurrencyDto>> GetStaleExchangeRatesAsync(
            int maxDays = 7,
            CancellationToken cancellationToken = default)
        {
            var currencies = await _unitOfWork.Repository<Currency>()
                .GetAllAsync(
                    filter: c => c.IsActive && !c.IsBaseCurrency,
                    cancellationToken: cancellationToken);

            var staleCurrencies = currencies
                .Where(c => c.IsExchangeRateStale(maxDays))
                .OrderBy(c => c.ExchangeRateDate ?? DateTime.MinValue);

            var dtos = staleCurrencies.Select(c =>
            {
                var dto = _mapper.Map<CurrencyDto>(c);
                dto.IsExchangeRateStale = true;
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Checks if currency code is unique
        /// </summary>
        public async Task<bool> IsCodeUniqueAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Currency>()
                .GetAsync(
                    filter: c => c.Code == code && c.Id != excludeId,
                    cancellationToken: cancellationToken);

            return exists == null;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<CurrencyDto> CreateAsync(
            CreateCurrencyDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            // Check if there's already a base currency
            if (createDto.IsBaseCurrency)
            {
                var existingBase = await _unitOfWork.Repository<Currency>()
                    .GetAsync(
                        filter: c => c.IsBaseCurrency,
                        cancellationToken: cancellationToken);

                if (existingBase != null)
                {
                    throw new InvalidOperationException($"Base currency already exists: {existingBase.Code}");
                }
            }

            var entity = new Currency(
                createDto.Code,
                createDto.Name,
                createDto.Symbol,
                createDto.NumericCode,
                createDto.DecimalDigits);

            // Set optional properties
            entity.SymbolNative = createDto.SymbolNative;
            entity.Rounding = createDto.Rounding;
            entity.PluralName = createDto.PluralName;

            // Set display settings
            if (createDto.DecimalSeparator != null || createDto.ThousandsSeparator != null ||
                createDto.PositivePattern != null || createDto.NegativePattern != null)
            {
                entity.UpdateDisplaySettings(
                    createDto.DecimalSeparator,
                    createDto.ThousandsSeparator,
                    createDto.PositivePattern,
                    createDto.NegativePattern,
                    createdBy ?? "System");
            }

            // Set exchange rate if not base currency
            if (!createDto.IsBaseCurrency && createDto.ExchangeRate != 1)
            {
                entity.UpdateExchangeRate(
                    createDto.ExchangeRate,
                    createDto.ExchangeRateSource ?? "Manual",
                    createdBy ?? "System");
            }

            // Set as base currency if needed
            if (createDto.IsBaseCurrency)
            {
                entity.SetAsBaseCurrency(createdBy ?? "System");
            }

            // Enable for projects/commitments
            entity.EnableForProjects(createDto.IsEnabledForProjects, createdBy ?? "System");
            entity.EnableForCommitments(createDto.IsEnabledForCommitments, createdBy ?? "System");

            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Currency>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Currency {CurrencyCode} created by {User}",
                entity.Code, createdBy ?? "System");

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<CurrencyDto?> UpdateAsync(
            Guid id,
            UpdateCurrencyDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Update basic properties that don't have domain methods
            entity.Name = updateDto.Name;
            entity.Symbol = updateDto.Symbol;
            entity.SymbolNative = updateDto.SymbolNative;
            entity.PluralName = updateDto.PluralName;

            // Update display settings if any provided
            if (updateDto.DecimalSeparator != null || updateDto.ThousandsSeparator != null ||
                updateDto.PositivePattern != null || updateDto.NegativePattern != null)
            {
                entity.UpdateDisplaySettings(
                    updateDto.DecimalSeparator ?? entity.DecimalSeparator,
                    updateDto.ThousandsSeparator ?? entity.ThousandsSeparator,
                    updateDto.PositivePattern ?? entity.PositivePattern,
                    updateDto.NegativePattern ?? entity.NegativePattern,
                    updatedBy ?? "System");
            }

            // Update active status
            entity.IsActive = updateDto.IsActive;

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Currency>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Currency {CurrencyCode} updated by {User}",
                entity.Code, updatedBy ?? "System");

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Override GetByIdAsync to include exchange rate staleness
        /// </summary>
        public override async Task<CurrencyDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Currency>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<CurrencyDto>(entity);
            dto.IsExchangeRateStale = entity.IsExchangeRateStale();
            
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include exchange rate staleness
        /// </summary>
        public override async Task<IEnumerable<CurrencyDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Currency>()
                .GetAllAsync(
                    filter: c => c.IsActive,
                    orderBy: q => q.OrderBy(c => c.Code),
                    cancellationToken: cancellationToken);

            var dtos = entities.Select(c =>
            {
                var dto = _mapper.Map<CurrencyDto>(c);
                dto.IsExchangeRateStale = c.IsExchangeRateStale();
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Override base filter to use IsActive instead of IsDeleted
        /// </summary>
        protected override Expression<Func<Currency, bool>>? GetBaseFilter()
        {
            return c => c.IsActive;
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}