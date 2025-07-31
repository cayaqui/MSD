namespace Web.Models.Search
{

    /// <summary>
    /// Filtros para la búsqueda
    /// </summary>
    public class SearchFilter
    {
        public SearchContentType? ContentType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? ProjectId { get; set; }
        public string? Status { get; set; }
        public List<string>? Tags { get; set; }
    }

    /// <summary>
    /// Tipos de contenido para búsqueda
    /// </summary>
    public enum SearchContentType
    {
        All,
        Projects,
        Documents,
        Activities,
        Costs,
        Risks,
        Companies,
        Users,
        Reports
    }

    /// <summary>
    /// Resultados de búsqueda agrupados
    /// </summary>
    public class SearchResults
    {
        public string Query { get; set; } = "";
        public int TotalResults { get; set; }
        public TimeSpan SearchTime { get; set; }
        public List<SearchResultGroup> Groups { get; set; } = new();
    }

    /// <summary>
    /// Grupo de resultados por tipo
    /// </summary>
    public class SearchResultGroup
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public SearchContentType Type { get; set; }
        public int TotalCount { get; set; }
        public List<SearchResultItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Item individual de resultado
    /// </summary>
    public class SearchResultItem
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string Icon { get; set; } = "";
        public string Url { get; set; } = "";
        public Dictionary<string, string>? Metadata { get; set; }
        public List<string>? Highlights { get; set; }
        public DateTime? LastModified { get; set; }
    }

    /// <summary>
    /// Sugerencia de búsqueda
    /// </summary>
    public class SearchSuggestion
    {
        public string Text { get; set; } = "";
        public string? Category { get; set; }
        public string? Icon { get; set; }
        public int Popularity { get; set; }
    }

    /// <summary>
    /// Búsqueda reciente
    /// </summary>
    public class RecentSearch
    {
        public string Query { get; set; } = "";
        public DateTime SearchedAt { get; set; }
        public int ResultCount { get; set; }
    }
}