using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Constants;
using Web.Models;
using Web.Models.Navigation;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    /// <summary>
    /// Implementación del servicio de navegación
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthService _authService;
        private NavigationMenu? _cachedMenu;

        public event EventHandler<NavigationItemChangedEventArgs>? NavigationItemChanged;
        public event EventHandler? NavigationMenuRefreshed;

        public NavigationService(IAuthorizationService authorizationService, IAuthService authService)
        {
            _authorizationService = authorizationService;
            _authService = authService;
        }

        public async Task<NavigationMenu> GetNavigationMenuAsync()
        {
            if (_cachedMenu != null)
                return _cachedMenu;

            var menu = new NavigationMenu
            {
                HomeItem = new NavigationItem
                {
                    Id = "home",
                    Title = "Dashboard",
                    Icon = "fa-light fa-home",
                    Href = "/",
                    Type = NavigationItemType.Link
                },
                Sections = new List<NavigationSection>
                {
                    // Sección de Configuración
                    new NavigationSection
                    {
                        Title = "Configuración",
                        Order = 1,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "companies",
                                Title = "Empresas",
                                Icon = "fa-light fa-building",
                                Href = Routes.Configuration.Companies,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.Company.View
                            },
                            new NavigationItem
                            {
                                Id = "users",
                                Title = "Usuarios",
                                Icon = "fa-light fa-users",
                                Href = Routes.Configuration.Users,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.System.ViewUsers
                            },
                            new NavigationItem
                            {
                                Id = "roles",
                                Title = "Roles y Permisos",
                                Icon = "fa-light fa-shield-check",
                                Href = Routes.Configuration.Roles,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.System.ViewRoles
                            }
                        }
                    },
                    // Sección de Proyectos
                    new NavigationSection
                    {
                        Title = "Gestión de Proyectos",
                        Order = 2,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "projects",
                                Title = "Proyectos",
                                Icon = "fa-light fa-project-diagram",
                                Href = Routes.Projects.List,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.Project.View,
                                Badge = new NavigationBadge { Text = "12", Variant = "primary" }
                            },
                            new NavigationItem
                            {
                                Id = "project-management",
                                Title = "Gestión del Proyecto",
                                Icon = "fa-light fa-tasks",
                                Type = NavigationItemType.Section,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "wbs",
                                        Title = "WBS/EDT",
                                        Icon = "fa-light fa-sitemap",
                                        Href = "/projects/current/wbs",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.WBS.View
                                    },
                                    new NavigationItem
                                    {
                                        Id = "schedule",
                                        Title = "Cronograma",
                                        Icon = "fa-light fa-calendar-alt",
                                        Href = "/projects/current/schedule",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Schedule.View
                                    },
                                    new NavigationItem
                                    {
                                        Id = "gantt",
                                        Title = "Diagrama Gantt",
                                        Icon = "fa-light fa-chart-gantt",
                                        Href = "/projects/current/schedule/gantt",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Schedule.ViewGantt
                                    }
                                }
                            }
                        }
                    },
                    // Sección de Costos
                    new NavigationSection
                    {
                        Title = "Control de Costos",
                        Order = 3,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "costs",
                                Title = "Gestión de Costos",
                                Icon = "fa-light fa-dollar-sign",
                                Type = NavigationItemType.Section,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "budget",
                                        Title = "Presupuesto",
                                        Icon = "fa-light fa-calculator",
                                        Href = "/projects/current/costs/budget",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Cost.ViewBudget
                                    },
                                    new NavigationItem
                                    {
                                        Id = "control-accounts",
                                        Title = "Control Accounts",
                                        Icon = "fa-light fa-folder-tree",
                                        Href = "/projects/current/control-accounts",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Cost.ViewControlAccounts
                                    },
                                    new NavigationItem
                                    {
                                        Id = "cbs",
                                        Title = "CBS",
                                        Icon = "fa-light fa-coins",
                                        Href = "/projects/current/costs/cbs",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Cost.ViewCBS
                                    },
                                    new NavigationItem
                                    {
                                        Id = "evm",
                                        Title = "Valor Ganado (EVM)",
                                        Icon = "fa-light fa-chart-line",
                                        Href = "/projects/current/costs/evm",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Cost.ViewEVM
                                    }
                                }
                            }
                        }
                    },
                    // Sección de Contratos
                    new NavigationSection
                    {
                        Title = "Contratos",
                        Order = 4,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "contracts",
                                Title = "Gestión de Contratos",
                                Icon = "fa-light fa-file-contract",
                                Href = "/projects/current/contracts",
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.Contract.View
                            }
                        }
                    },
                    // Sección de Documentos
                    new NavigationSection
                    {
                        Title = "Documentación",
                        Order = 5,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "documents",
                                Title = "Gestión Documental",
                                Icon = "fa-light fa-folder-open",
                                Href = "/projects/current/documents",
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.Document.View
                            }
                        }
                    },
                    // Sección de Calidad
                    new NavigationSection
                    {
                        Title = "Calidad",
                        Order = 6,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "quality",
                                Title = "Gestión de Calidad",
                                Icon = "fa-light fa-check-circle",
                                Href = "/projects/current/quality",
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.Quality.View
                            }
                        }
                    },
                    // Sección de Riesgos
                    new NavigationSection
                    {
                        Title = "Riesgos",
                        Order = 7,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "risks",
                                Title = "Gestión de Riesgos",
                                Icon = "fa-light fa-exclamation-triangle",
                                Href = "/projects/current/risks",
                                Type = NavigationItemType.Link,
                                RequiredPolicy = PermissionKeys.Risk.View
                            }
                        }
                    },
                    // Sección de Reportes
                    new NavigationSection
                    {
                        Title = "Reportes y Análisis",
                        Order = 8,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "reports",
                                Title = "Reportes",
                                Icon = "fa-light fa-chart-bar",
                                Type = NavigationItemType.Section,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "executive-dashboard",
                                        Title = "Dashboard Ejecutivo",
                                        Icon = "fa-light fa-tachometer-alt",
                                        Href = Routes.Reports.ExecutiveDashboard,
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Report.ViewExecutiveDashboard
                                    },
                                    new NavigationItem
                                    {
                                        Id = "project-reports",
                                        Title = "Reportes de Proyecto",
                                        Icon = "fa-light fa-file-chart-line",
                                        Href = "/reports/projects",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Report.ViewProjectReports
                                    },
                                    new NavigationItem
                                    {
                                        Id = "kpis",
                                        Title = "KPIs",
                                        Icon = "fa-light fa-analytics",
                                        Href = "/reports/kpis",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = PermissionKeys.Report.ViewKPIs
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _cachedMenu = menu;
            return menu;
        }

        public async Task<NavigationMenu> GetNavigationMenuForUserAsync(ClaimsPrincipal user)
        {
            var fullMenu = await GetNavigationMenuAsync();
            var filteredMenu = new NavigationMenu
            {
                HomeItem = fullMenu.HomeItem,
                Sections = new List<NavigationSection>()
            };

            foreach (var section in fullMenu.Sections)
            {
                var filteredSection = new NavigationSection
                {
                    Title = section.Title,
                    Order = section.Order,
                    Items = new List<NavigationItem>()
                };

                foreach (var item in section.Items)
                {
                    var filteredItem = await FilterNavigationItemAsync(item, user);
                    if (filteredItem != null)
                    {
                        filteredSection.Items.Add(filteredItem);
                    }
                }

                if (filteredSection.Items.Any())
                {
                    filteredMenu.Sections.Add(filteredSection);
                }
            }

            return filteredMenu;
        }

        private async Task<NavigationItem?> FilterNavigationItemAsync(NavigationItem item, ClaimsPrincipal user)
        {
            // Verificar acceso al item
            if (!await UserHasAccessToItemAsync(item, user))
            {
                return null;
            }

            // Si tiene hijos, filtrarlos recursivamente
            if (item.Children?.Any() == true)
            {
                var filteredChildren = new List<NavigationItem>();
                foreach (var child in item.Children)
                {
                    var filteredChild = await FilterNavigationItemAsync(child, user);
                    if (filteredChild != null)
                    {
                        filteredChildren.Add(filteredChild);
                    }
                }

                // Si no quedan hijos después del filtrado, no incluir el item
                if (!filteredChildren.Any())
                {
                    return null;
                }

                // Clonar el item con los hijos filtrados
                return new NavigationItem
                {
                    Id = item.Id,
                    Title = item.Title,
                    Icon = item.Icon,
                    Href = item.Href,
                    Type = item.Type,
                    RequiredPolicy = item.RequiredPolicy,
                    RequiredRole = item.RequiredRole,
                    Badge = item.Badge,
                    IsExternal = item.IsExternal,
                    Order = item.Order,
                    Children = filteredChildren
                };
            }

            return item;
        }

        public async Task<bool> UserHasAccessToItemAsync(NavigationItem item, ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return false;
            }

            // Si no requiere política o rol específico, está disponible para todos los usuarios autenticados
            if (string.IsNullOrEmpty(item.RequiredPolicy) && string.IsNullOrEmpty(item.RequiredRole))
            {
                return true;
            }

            // Verificar rol si es requerido
            if (!string.IsNullOrEmpty(item.RequiredRole))
            {
                if (!user.IsInRole(item.RequiredRole))
                {
                    return false;
                }
            }

            // Verificar política/permiso si es requerido
            if (!string.IsNullOrEmpty(item.RequiredPolicy))
            {
                try
                {
                    var authResult = await _authorizationService.AuthorizeAsync(user, item.RequiredPolicy);
                    return authResult.Succeeded;
                }
                catch
                {
                    // Si hay algún error al verificar la autorización, denegar acceso
                    return false;
                }
            }

            return true;
        }

        public Task<NavigationItem?> GetNavigationItemByIdAsync(string itemId)
        {
            // Implementación simplificada
            var menu = _cachedMenu ?? GetNavigationMenuAsync().Result;
            var item = FindItemById(menu, itemId);
            return Task.FromResult(item);
        }

        private NavigationItem? FindItemById(NavigationMenu menu, string itemId)
        {
            if (menu.HomeItem?.Id == itemId)
                return menu.HomeItem;

            foreach (var section in menu.Sections)
            {
                foreach (var item in section.Items)
                {
                    var found = FindItemInTree(item, itemId);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        private NavigationItem? FindItemInTree(NavigationItem item, string itemId)
        {
            if (item.Id == itemId)
                return item;

            if (item.Children != null)
            {
                foreach (var child in item.Children)
                {
                    var found = FindItemInTree(child, itemId);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public Task<List<NavigationItem>> GetNavigationItemsByPathAsync(string path)
        {
            var items = new List<NavigationItem>();
            var menu = _cachedMenu ?? GetNavigationMenuAsync().Result;

            foreach (var section in menu.Sections)
            {
                foreach (var item in section.Items)
                {
                    AddItemsByPath(item, path, items);
                }
            }

            return Task.FromResult(items);
        }

        private void AddItemsByPath(NavigationItem item, string path, List<NavigationItem> items)
        {
            if (!string.IsNullOrEmpty(item.Href) && item.Href.StartsWith(path))
            {
                items.Add(item);
            }

            if (item.Children != null)
            {
                foreach (var child in item.Children)
                {
                    AddItemsByPath(child, path, items);
                }
            }
        }

        public Task UpdateNavigationItemAsync(string itemId, Action<NavigationItem> updateAction)
        {
            var item = GetNavigationItemByIdAsync(itemId).Result;
            if (item != null)
            {
                updateAction(item);
                NavigationItemChanged?.Invoke(this, new NavigationItemChangedEventArgs { Item = item });
            }
            return Task.CompletedTask;
        }

        public Task RefreshMenuAsync()
        {
            _cachedMenu = null;
            NavigationMenuRefreshed?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task<List<NavigationItem>> GetBreadcrumbPathAsync(string currentUrl)
        {
            var breadcrumbs = new List<NavigationItem>();
            var menu = _cachedMenu ?? GetNavigationMenuAsync().Result;

            // Siempre agregar home como primer item
            if (menu.HomeItem != null)
            {
                breadcrumbs.Add(menu.HomeItem);
            }

            // Buscar la ruta completa al item actual
            foreach (var section in menu.Sections)
            {
                foreach (var item in section.Items)
                {
                    if (FindBreadcrumbPath(item, currentUrl, breadcrumbs))
                    {
                        break;
                    }
                }
            }

            return Task.FromResult(breadcrumbs);
        }

        private bool FindBreadcrumbPath(NavigationItem item, string targetUrl, List<NavigationItem> path)
        {
            if (item.Href == targetUrl)
            {
                path.Add(item);
                return true;
            }

            if (item.Children != null)
            {
                foreach (var child in item.Children)
                {
                    if (FindBreadcrumbPath(child, targetUrl, path))
                    {
                        path.Insert(1, item); // Insertar después de home
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class NavigationItemChangedEventArgs : EventArgs
    {
        public NavigationItem Item { get; set; } = null!;
    }
}