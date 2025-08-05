using Domain.Common;
using Domain.Entities.Organization.Core;

namespace Domain.Entities.Cost.Core
{
    /// <summary>
    /// Exchange rates and economic indicators for Chilean project cost control
    /// Tracks USD/CLP exchange rates and UF (Unidad de Fomento) values
    /// </summary>
    public class ExchangeRate : BaseEntity
    {
        public DateTime Date { get; private set; }
        public string CurrencyFrom { get; private set; } = "USD";
        public string CurrencyTo { get; private set; } = "CLP";
        public decimal Rate { get; private set; }
        public decimal? UFValue { get; private set; } // Unidad de Fomento value
        public string Source { get; private set; } = string.Empty; // e.g., "Banco Central de Chile"
        public bool IsOfficial { get; private set; }
        
        // For project-specific rates (negotiated rates for large contracts)
        public Guid? ProjectId { get; private set; }
        public virtual Project? Project { get; private set; }
        
        private ExchangeRate() { } // EF Core constructor

        public ExchangeRate(
            DateTime date,
            string currencyFrom,
            string currencyTo,
            decimal rate,
            string source,
            bool isOfficial = true)
        {
            Date = date;
            CurrencyFrom = currencyFrom;
            CurrencyTo = currencyTo;
            Rate = rate;
            Source = source;
            IsOfficial = isOfficial;
        }

        public void SetUFValue(decimal ufValue)
        {
            UFValue = ufValue;
        }

        public void AssignToProject(Guid projectId)
        {
            ProjectId = projectId;
            IsOfficial = false; // Project-specific rates are not official
        }

        public static ExchangeRate CreateUSDtoCLP(DateTime date, decimal rate, string source)
        {
            return new ExchangeRate(date, "USD", "CLP", rate, source, true);
        }

        public static ExchangeRate CreateEURtoCLP(DateTime date, decimal rate, string source)
        {
            return new ExchangeRate(date, "EUR", "CLP", rate, source, true);
        }
    }
}