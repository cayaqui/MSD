using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, CacheEntry> _cache = new();
        private readonly ILogger<CacheService> _logger;

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;
        }

        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.Expiration > DateTime.UtcNow)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<T>(entry.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializando cache para key: {Key}", key);
                        Remove(key);
                    }
                }
                else
                {
                    // Expirado
                    Remove(key);
                }
            }

            return default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var exp = expiration ?? TimeSpan.FromMinutes(30);
                var serialized = JsonSerializer.Serialize(value);

                _cache[key] = new CacheEntry
                {
                    Value = serialized,
                    Expiration = DateTime.UtcNow.Add(exp),
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogDebug("Cache guardado: {Key}, Expira: {Expiration}", key, exp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando cache para key: {Key}", key);
            }
        }

        public void Remove(string key)
        {
            if (_cache.Remove(key))
            {
                _logger.LogDebug("Cache eliminado: {Key}", key);
            }
        }

        public void Clear()
        {
            _cache.Clear();
            _logger.LogInformation("Cache limpiado completamente");
        }

        public bool Exists(string key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                return entry.Expiration > DateTime.UtcNow;
            }
            return false;
        }

        /// <summary>
        /// Limpia entradas expiradas (llamar periódicamente)
        /// </summary>
        public void CleanExpired()
        {
            var expired = _cache.Where(kvp => kvp.Value.Expiration <= DateTime.UtcNow)
                                .Select(kvp => kvp.Key)
                                .ToList();

            foreach (var key in expired)
            {
                Remove(key);
            }

            if (expired.Any())
            {
                _logger.LogDebug("Limpiadas {Count} entradas expiradas del cache", expired.Count);
            }
        }

        private class CacheEntry
        {
            public string Value { get; set; } = string.Empty;
            public DateTime Expiration { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }

    /// <summary>
    /// Helper para construir keys de cache consistentes
    /// </summary>
    public static class CacheKeys
    {
        public const string UserProfile = "user:profile";
        public const string CompanyList = "company:list";
        public const string ActiveCompany = "company:active";
        public const string ProjectList = "project:list";
        public const string ActiveProject = "project:active";
        public const string UserPermissions = "user:permissions";
        public const string SystemConfig = "system:config";

        public static string ProjectDetail(Guid projectId) => $"project:detail:{projectId}";
        public static string CompanyDetail(Guid companyId) => $"company:detail:{companyId}";
        public static string WBSStructure(Guid projectId) => $"wbs:structure:{projectId}";
        public static string ControlAccounts(Guid projectId) => $"control:accounts:{projectId}";
    }
}

