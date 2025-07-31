using Web.Models;
using Web.Models.Search;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    /// <summary>
    /// Implementación mock del servicio de búsqueda
    /// TODO: Reemplazar con implementación real que conecte al API
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly List<RecentSearch> _recentSearches = new();

        public SearchService(ILogger<SearchService> logger)
        {
            _logger = logger;
        }

        public async Task<SearchResults> SearchAsync(string query, SearchFilter? filter = null, int maxResults = 5)
        {
            // Simular delay de búsqueda
            await Task.Delay(300);

            var startTime = DateTime.Now;
            var results = new SearchResults
            {
                Query = query,
                Groups = new List<SearchResultGroup>()
            };

            // Filtrar por tipo de contenido si se especifica
            var contentTypes = filter?.ContentType == SearchContentType.All || filter?.ContentType == null
                ? Enum.GetValues<SearchContentType>().Where(t => t != SearchContentType.All)
                : new[] { filter.ContentType.Value };

            foreach (var contentType in contentTypes)
            {
                var group = GenerateMockResults(query, contentType, maxResults);
                if (group.Items.Any())
                {
                    results.Groups.Add(group);
                    results.TotalResults += group.TotalCount;
                }
            }

            results.SearchTime = DateTime.Now - startTime;
            return results;
        }

        public async Task<List<SearchSuggestion>> GetSuggestionsAsync(string query, int maxSuggestions = 10)
        {
            await Task.Delay(100);

            var suggestions = new List<SearchSuggestion>();

            // Generar sugerencias basadas en el query
            if (!string.IsNullOrWhiteSpace(query))
            {
                suggestions.Add(new SearchSuggestion
                {
                    Text = query + " en proyectos",
                    Category = "Proyectos",
                    Icon = "fa-light fa-project-diagram",
                    Popularity = 95
                });

                suggestions.Add(new SearchSuggestion
                {
                    Text = query + " en documentos",
                    Category = "Documentos",
                    Icon = "fa-light fa-file-alt",
                    Popularity = 85
                });

                suggestions.Add(new SearchSuggestion
                {
                    Text = query + " actividades",
                    Category = "Actividades",
                    Icon = "fa-light fa-tasks",
                    Popularity = 75
                });
            }

            return suggestions.Take(maxSuggestions).ToList();
        }

        public async Task<List<RecentSearch>> GetRecentSearchesAsync(int maxItems = 5)
        {
            await Task.Delay(50);
            return _recentSearches
                .OrderByDescending(s => s.SearchedAt)
                .Take(maxItems)
                .ToList();
        }

        public async Task SaveSearchHistoryAsync(string query)
        {
            await Task.Delay(50);

            _recentSearches.Add(new RecentSearch
            {
                Query = query,
                SearchedAt = DateTime.Now,
                ResultCount = Random.Shared.Next(5, 50)
            });

            // Mantener solo las últimas 20 búsquedas
            if (_recentSearches.Count > 20)
            {
                _recentSearches.RemoveRange(0, _recentSearches.Count - 20);
            }
        }

        public async Task ClearSearchHistoryAsync()
        {
            await Task.Delay(50);
            _recentSearches.Clear();
        }

        private SearchResultGroup GenerateMockResults(string query, SearchContentType type, int maxResults)
        {
            var group = new SearchResultGroup
            {
                Type = type,
                Title = GetGroupTitle(type),
                Icon = GetGroupIcon(type),
                Items = new List<SearchResultItem>()
            };

            // Generar resultados mock basados en el tipo
            var itemCount = Random.Shared.Next(0, maxResults + 3);
            group.TotalCount = itemCount + Random.Shared.Next(0, 10);

            for (int i = 0; i < Math.Min(itemCount, maxResults); i++)
            {
                group.Items.Add(GenerateMockItem(query, type, i));
            }

            return group;
        }

        private SearchResultItem GenerateMockItem(string query, SearchContentType type, int index)
        {
            return type switch
            {
                SearchContentType.Projects => new SearchResultItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"Proyecto {query} {index + 1}",
                    Subtitle = $"PRJ-2024-{1000 + index}",
                    Description = $"Proyecto de construcción relacionado con {query}",
                    Icon = "fa-light fa-project-diagram",
                    Url = $"/projects/{Guid.NewGuid()}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "Estado", "En Progreso" },
                        { "Progreso", "65%" }
                    },
                    Highlights = new List<string> { $"...encontrado <mark>{query}</mark> en la descripción..." },
                    LastModified = DateTime.Now.AddDays(-Random.Shared.Next(1, 30))
                },

                SearchContentType.Documents => new SearchResultItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"Documento {query} {index + 1}.pdf",
                    Subtitle = "PDF • 2.5 MB",
                    Description = $"Documento técnico sobre {query}",
                    Icon = "fa-light fa-file-pdf",
                    Url = $"/documents/{Guid.NewGuid()}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "Tipo", "Especificación Técnica" },
                        { "Versión", "1.2" }
                    },
                    LastModified = DateTime.Now.AddDays(-Random.Shared.Next(1, 15))
                },

                SearchContentType.Activities => new SearchResultItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"Actividad: {query} - Fase {index + 1}",
                    Subtitle = "Terminal Marítimo Norte",
                    Description = $"Actividad de construcción relacionada con {query}",
                    Icon = "fa-light fa-tasks",
                    Url = $"/activities/{Guid.NewGuid()}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "Duración", "15 días" },
                        { "Responsable", "Juan Pérez" }
                    },
                    LastModified = DateTime.Now.AddDays(-Random.Shared.Next(1, 7))
                },

                _ => new SearchResultItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"{type}: {query} Item {index + 1}",
                    Description = $"Resultado relacionado con {query}",
                    Icon = GetGroupIcon(type),
                    Url = $"/{type.ToString().ToLower()}/{Guid.NewGuid()}"
                }
            };
        }

        private string GetGroupTitle(SearchContentType type)
        {
            return type switch
            {
                SearchContentType.Projects => "Proyectos",
                SearchContentType.Documents => "Documentos",
                SearchContentType.Activities => "Actividades",
                SearchContentType.Costs => "Costos",
                SearchContentType.Risks => "Riesgos",
                SearchContentType.Companies => "Empresas",
                SearchContentType.Users => "Usuarios",
                SearchContentType.Reports => "Reportes",
                _ => type.ToString()
            };
        }

        private string GetGroupIcon(SearchContentType type)
        {
            return type switch
            {
                SearchContentType.Projects => "fa-light fa-project-diagram",
                SearchContentType.Documents => "fa-light fa-file-alt",
                SearchContentType.Activities => "fa-light fa-tasks",
                SearchContentType.Costs => "fa-light fa-dollar-sign",
                SearchContentType.Risks => "fa-light fa-exclamation-triangle",
                SearchContentType.Companies => "fa-light fa-building",
                SearchContentType.Users => "fa-light fa-users",
                SearchContentType.Reports => "fa-light fa-chart-bar",
                _ => "fa-light fa-search"
            };
        }
    }
}