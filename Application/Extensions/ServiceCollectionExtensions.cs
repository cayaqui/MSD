namespace Application.Extensions
{

    /// <summary>
    /// Extension methods for IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add a scoped service with its interface
        /// </summary>
        public static IServiceCollection AddScopedService<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            return services.AddScoped<TInterface, TImplementation>();
        }

        /// <summary>
        /// Add multiple scoped services
        /// </summary>
        public static IServiceCollection AddScopedServices(
            this IServiceCollection services,
            params (Type serviceType, Type implementationType)[] serviceTypes)
        {
            foreach (var (serviceType, implementationType) in serviceTypes)
            {
                services.AddScoped(serviceType, implementationType);
            }
            return services;
        }
    }
}