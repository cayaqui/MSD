using Application.Interfaces.Common;
using Core.DTOs.Organization.Currency;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Currency management
    /// </summary>
    public interface ICurrencyService : IBaseService<CurrencyDto, CreateCurrencyDto, UpdateCurrencyDto>
    {
        /// <summary>
        /// Gets a currency by its code (e.g., USD, EUR)
        /// </summary>
        Task<CurrencyDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the base currency
        /// </summary>
        Task<CurrencyDto?> GetBaseCurrencyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active currencies
        /// </summary>
        Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets currencies enabled for projects
        /// </summary>
        Task<IEnumerable<CurrencyDto>> GetProjectCurrenciesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets currencies enabled for commitments
        /// </summary>
        Task<IEnumerable<CurrencyDto>> GetCommitmentCurrenciesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates exchange rate for a currency
        /// </summary>
        Task<CurrencyDto?> UpdateExchangeRateAsync(Guid id, UpdateExchangeRateDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a currency as base currency
        /// </summary>
        Task<CurrencyDto?> SetAsBaseCurrencyAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates display settings for a currency
        /// </summary>
        Task<CurrencyDto?> UpdateDisplaySettingsAsync(Guid id, string? decimalSeparator, string? thousandsSeparator, string? positivePattern, string? negativePattern, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables/disables currency for projects
        /// </summary>
        Task<CurrencyDto?> EnableForProjectsAsync(Guid id, bool enable, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables/disables currency for commitments
        /// </summary>
        Task<CurrencyDto?> EnableForCommitmentsAsync(Guid id, bool enable, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Converts amount between currencies
        /// </summary>
        Task<CurrencyConversionResultDto> ConvertAmountAsync(CurrencyConversionDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets currencies with stale exchange rates
        /// </summary>
        Task<IEnumerable<CurrencyDto>> GetStaleExchangeRatesAsync(int maxDays = 7, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if currency code is unique
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}