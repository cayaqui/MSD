using System.Security.Claims;
using Web.Models;
using Web.Services.Implementation;

namespace Web.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar el menú de navegación
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Obtiene el menú de navegación completo
        /// </summary>
        Task<NavigationMenu> GetNavigationMenuAsync();

        /// <summary>
        /// Obtiene el menú filtrado por los permisos del usuario
        /// </summary>
        Task<NavigationMenu> GetNavigationMenuForUserAsync(ClaimsPrincipal user);

        /// <summary>
        /// Obtiene un item específico por su ID
        /// </summary>
        Task<NavigationItem?> GetNavigationItemByIdAsync(string itemId);

        /// <summary>
        /// Obtiene items por ruta
        /// </summary>
        Task<List<NavigationItem>> GetNavigationItemsByPathAsync(string path);

        /// <summary>
        /// Actualiza un item del menú (ej: badge count)
        /// </summary>
        Task UpdateNavigationItemAsync(string itemId, Action<NavigationItem> updateAction);

        /// <summary>
        /// Refresca el menú desde la fuente de datos
        /// </summary>
        Task RefreshMenuAsync();

        /// <summary>
        /// Verifica si un usuario tiene acceso a un item
        /// </summary>
        Task<bool> UserHasAccessToItemAsync(NavigationItem item, ClaimsPrincipal user);

        /// <summary>
        /// Obtiene la ruta de breadcrumb para una URL
        /// </summary>
        Task<List<NavigationItem>> GetBreadcrumbPathAsync(string currentUrl);

        /// <summary>
        /// Evento cuando cambia un item del menú
        /// </summary>
        event EventHandler<NavigationItemChangedEventArgs>? NavigationItemChanged;

        /// <summary>
        /// Evento cuando se refresca el menú completo
        /// </summary>
        event EventHandler? NavigationMenuRefreshed;
    }
}