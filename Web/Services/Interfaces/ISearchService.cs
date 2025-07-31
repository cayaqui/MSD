using Web.Models.Search;

namespace Web.Services.Interfaces
{
    /// <summary>
    /// Servicio para búsqueda global en la aplicación
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Realiza una búsqueda global en todos los tipos de contenido
        /// </summary>
        /// <param name="query">Término de búsqueda</param>
        /// <param name="filter">Filtro opcional por tipo de contenido</param>
        /// <param name="maxResults">Número máximo de resultados por categoría</param>
        /// <returns>Resultados agrupados por categoría</returns>
        Task<SearchResults> SearchAsync(string query, SearchFilter? filter = null, int maxResults = 5);

        /// <summary>
        /// Obtiene sugerencias de búsqueda basadas en el texto ingresado
        /// </summary>
        /// <param name="query">Término parcial de búsqueda</param>
        /// <param name="maxSuggestions">Número máximo de sugerencias</param>
        /// <returns>Lista de sugerencias</returns>
        Task<List<SearchSuggestion>> GetSuggestionsAsync(string query, int maxSuggestions = 10);

        /// <summary>
        /// Obtiene búsquedas recientes del usuario
        /// </summary>
        /// <param name="maxItems">Número máximo de items</param>
        /// <returns>Lista de búsquedas recientes</returns>
        Task<List<RecentSearch>> GetRecentSearchesAsync(int maxItems = 5);

        /// <summary>
        /// Guarda una búsqueda en el historial del usuario
        /// </summary>
        /// <param name="query">Término buscado</param>
        Task SaveSearchHistoryAsync(string query);

        /// <summary>
        /// Limpia el historial de búsquedas del usuario
        /// </summary>
        Task ClearSearchHistoryAsync();
    }
}