// Web/Services/Implementation/NavigationService.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Web.Constants;
using Web.Models;
using Web.Models.Navigation;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    /// <summary>
    /// Implementación del servicio de navegación con verificación de permisos
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthService _authService;
        private readonly ILogger<NavigationService> _logger;
        private NavigationMenu? _cachedMenu;
        private NavigationMenu? _cachedUserMenu;
        private string? _cachedUserId;

        public event EventHandler<NavigationItemChangedEventArgs>? NavigationItemChanged;
        public event EventHandler? NavigationMenuRefreshed;

        public NavigationService(
            IAuthorizationService authorizationService,
            IAuthService authService,
            ILogger<NavigationService> logger)
        {
            _authorizationService = authorizationService;
            _authService = authService;
            _logger = logger;
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
                                RequiredPolicy = "company.view"
                            },
                            new NavigationItem
                            {
                                Id = "users",
                                Title = "Usuarios",
                                Icon = "fa-light fa-users",
                                Href = Routes.Configuration.Users,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = "system.users.view"
                            },
                            new NavigationItem
                            {
                                Id = "roles",
                                Title = "Roles y Permisos",
                                Icon = "fa-light fa-shield-check",
                                Href = Routes.Configuration.Roles,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = "system.roles.view"
                            },
                            new NavigationItem
                            {
                                Id = "project-types",
                                Title = "Tipos de Proyecto",
                                Icon = "fa-light fa-layer-group",
                                Href = Routes.Configuration.ProjectTypes,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = "project.settings.view"
                            }
                        }
                    },

                    // Sección de Proyectos
                    new NavigationSection
                    {
                        Title = "Proyectos",
                        Order = 2,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "projects",
                                Title = "Mis Proyectos",
                                Icon = "fa-light fa-diagram-project",
                                Href = Routes.Projects.List,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = "project.view"
                            },
                            new NavigationItem
                            {
                                Id = "new-project",
                                Title = "Nuevo Proyecto",
                                Icon = "fa-light fa-plus",
                                Href = Routes.Projects.New,
                                Type = NavigationItemType.Link,
                                RequiredPolicy = "project.create"
                            }
                        }
                    },

                    // Sección de Gestión de Proyectos (accordion con hijos)
                    new NavigationSection
                    {
                        Title = "Gestión de Proyectos",
                        Order = 3,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "scope-management",
                                Title = "Gestión de Alcance",
                                Icon = "fa-light fa-bullseye-arrow",
                                Type = NavigationItemType.Accordion,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "wbs",
                                        Title = "EDT/WBS",
                                        Icon = "fa-light fa-sitemap",
                                        Href = "/scope/wbs",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "project.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "deliverables",
                                        Title = "Entregables",
                                        Icon = "fa-light fa-box-check",
                                        Href = "/scope/deliverables",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "project.view"
                                    }
                                }
                            },
                            new NavigationItem
                            {
                                Id = "schedule-management",
                                Title = "Gestión de Cronograma",
                                Icon = "fa-light fa-calendar-days",
                                Type = NavigationItemType.Accordion,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "activities",
                                        Title = "Actividades",
                                        Icon = "fa-light fa-tasks",
                                        Href = "/schedule/activities",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "schedule.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "critical-path",
                                        Title = "Ruta Crítica",
                                        Icon = "fa-light fa-route",
                                        Href = "/schedule/critical-path",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "schedule.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "progress",
                                        Title = "Avance",
                                        Icon = "fa-light fa-chart-line",
                                        Href = "/schedule/progress",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "schedule.view"
                                    }
                                }
                            },
                            new NavigationItem
                            {
                                Id = "cost-management",
                                Title = "Gestión de Costos",
                                Icon = "fa-light fa-coins",
                                Type = NavigationItemType.Accordion,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "budget",
                                        Title = "Presupuesto",
                                        Icon = "fa-light fa-sack-dollar",
                                        Href = "/cost/budget",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "budget.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "costs",
                                        Title = "Control de Costos",
                                        Icon = "fa-light fa-calculator",
                                        Href = "/cost/control",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "cost.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "earned-value",
                                        Title = "Valor Ganado",
                                        Icon = "fa-light fa-chart-mixed",
                                        Href = "/cost/earned-value",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "cost.view"
                                    }
                                }
                            },
                            new NavigationItem
                            {
                                Id = "contract-management",
                                Title = "Gestión de Contratos",
                                Icon = "fa-light fa-file-contract",
                                Type = NavigationItemType.Accordion,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "contracts",
                                        Title = "Contratos",
                                        Icon = "fa-light fa-handshake",
                                        Href = "/contracts",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "contract.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "contractors",
                                        Title = "Contratistas",
                                        Icon = "fa-light fa-hard-hat",
                                        Href = "/contracts/contractors",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "contract.view"
                                    }
                                }
                            }
                        }
                    },

                    // Sección de Documentos
                    new NavigationSection
                    {
                        Title = "Documentos",
                        Order = 4,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "documents",
                                Title = "Repositorio",
                                Icon = "fa-light fa-folder-open",
                                Href = "/documents",
                                Type = NavigationItemType.Link,
                                RequiredPolicy = "document.view"
                            },
                            new NavigationItem
                            {
                                Id = "transmittals",
                                Title = "Transmittals",
                                Icon = "fa-light fa-paper-plane",
                                Href = "/documents/transmittals",
                                Type = NavigationItemType.Link,
                                RequiredPolicy = "document.view"
                            }
                        }
                    },

                    // Sección de Reportes
                    new NavigationSection
                    {
                        Title = "Reportes",
                        Order = 5,
                        Items = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                Id = "reports",
                                Title = "Reportes",
                                Icon = "fa-light fa-chart-pie",
                                Type = NavigationItemType.Accordion,
                                Children = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        Id = "executive-dashboard",
                                        Title = "Dashboard Ejecutivo",
                                        Icon = "fa-light fa-tachometer-alt",
                                        Href = Routes.Reports.ExecutiveDashboard,
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "report.executive.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "project-reports",
                                        Title = "Reportes de Proyecto",
                                        Icon = "fa-light fa-file-chart-line",
                                        Href = "/reports/projects",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "report.project.view"
                                    },
                                    new NavigationItem
                                    {
                                        Id = "kpis",
                                        Title = "KPIs",
                                        Icon = "fa-light fa-analytics",
                                        Href = "/reports/kpis",
                                        Type = NavigationItemType.Link,
                                        RequiredPolicy = "report.kpi.view"
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
            var currentUserId = user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if we have a cached menu for this user
            if (_cachedUserMenu != null && _cachedUserId == currentUserId)
            {
                return _cachedUserMenu;
            }

            var fullMenu = await GetNavigationMenuAsync();
            var filteredMenu = new NavigationMenu
            {
                HomeItem = fullMenu.HomeItem,
                Sections = new List<NavigationSection>()
            };

            // Get user permissions once
            var userPermissions = await _authService.GetUserPermissionsAsync();
            _logger.LogInformation("Filtering menu for user {UserId} with {PermissionCount} permissions",
                currentUserId, userPermissions?.GlobalPermissions.Count ?? 0);

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
                    var filteredItem = await FilterNavigationItemAsync(item, user, userPermissions);
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

            // Cache the filtered menu
            _cachedUserMenu = filteredMenu;
            _cachedUserId = currentUserId;

            return filteredMenu;
        }

        private async Task<NavigationItem?> FilterNavigationItemAsync(
            NavigationItem item,
            ClaimsPrincipal user,
            UserPermissionsDto? userPermissions)
        {
            // Check if user has access to this item
            if (!await UserHasAccessToItemAsync(item, user, userPermissions))
            {
                return null;
            }

            // If item has children, filter them recursively
            if (item.Children != null && item.Children.Any())
            {
                var filteredChildren = new List<NavigationItem>();
                foreach (var child in item.Children)
                {
                    var filteredChild = await FilterNavigationItemAsync(child, user, userPermissions);
                    if (filteredChild != null)
                    {
                        filteredChildren.Add(filteredChild);
                    }
                }

                // Only include parent if it has accessible children
                if (!filteredChildren.Any())
                {
                    return null;
                }

                // Clone the item with filtered children
                return new NavigationItem
                {
                    Id = item.Id,
                    Title = item.Title,
                    Icon = item.Icon,
                    Href = item.Href,
                    Type = item.Type,
                    Badge = new NavigationBadge
                    {
                        Count = item.Badge?.Count,
                        Color = item.Badge.Color
                    },
                    RequiredPolicy = item.RequiredPolicy,
                    RequiredRole = item.RequiredRole
                    Children = filteredChildren
                };
            }

            // Clone the item without children
            return new NavigationItem
            {
                Id = item.Id,
                Title = item.Title,
                Icon = item.Icon,
                Href = item.Href,
                Type = item.Type,
                Badge = new NavigationBadge
                {
                    Count = item.Badge?.Count,
                    Color = item.Badge.Color
                },
                RequiredPolicy = item.RequiredPolicy,
                RequiredRole = item.RequiredRole
            };
        }

        public async Task<bool> UserHasAccessToItemAsync(NavigationItem item, ClaimsPrincipal user)
        {
            var userPermissions = await _authService.GetUserPermissionsAsync();
            return await UserHasAccessToItemAsync(item, user, userPermissions);
        }

        private async Task<bool> UserHasAccessToItemAsync(
            NavigationItem item,
            ClaimsPrincipal user,
            UserPermissionsDto? userPermissions)
        {
            // If no permission required, allow access
            if (string.IsNullOrEmpty(item.RequiredPolicy) && string.IsNullOrEmpty(item.RequiredRole))
            {
                return true;
            }

            // Check policy-based permission
            if (!string.IsNullOrEmpty(item.RequiredPolicy))
            {
                // Quick check against cached permissions
                if (userPermissions != null)
                {
                    var hasPermission = userPermissions.GlobalPermissions.Contains(item.RequiredPolicy) ||
                                      userPermissions.ProjectPermissions.Any(p => p.Permissions.Contains(item.RequiredPolicy));

                    if (!hasPermission)
                    {
                        _logger.LogDebug("User lacks permission {Permission} for menu item {ItemId}",
                            item.RequiredPolicy, item.Id);
                    }

                    return hasPermission;
                }

                // Fallback to authorization service
                var authResult = await _authorizationService.AuthorizeAsync(user, item.RequiredPolicy);
                return authResult.Succeeded;
            }

            // Check role-based permission
            if (!string.IsNullOrEmpty(item.RequiredRole))
            {
                return await _authService.IsInRoleAsync(item.RequiredRole);
            }

            return false;
        }

        public async Task<NavigationItem?> GetNavigationItemByIdAsync(string itemId)
        {
            var menu = await GetNavigationMenuAsync();
            return FindItemById(menu, itemId);
        }

        private NavigationItem? FindItemById(NavigationMenu menu, string itemId)
        {
            if (menu.HomeItem?.Id == itemId)
                return menu.HomeItem;

            foreach (var section in menu.Sections)
            {
                var found = FindItemInList(section.Items, itemId);
                if (found != null)
                    return found;
            }

            return null;
        }

        private NavigationItem? FindItemInList(List<NavigationItem> items, string itemId)
        {
            foreach (var item in items)
            {
                if (item.Id == itemId)
                    return item;

                if (item.Children != null)
                {
                    var found = FindItemInList(item.Children, itemId);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public async Task<List<NavigationItem>> GetNavigationItemsByPathAsync(string path)
        {
            var menu = await GetNavigationMenuAsync();
            var items = new List<NavigationItem>();

            if (menu.HomeItem?.Href == path)
                items.Add(menu.HomeItem);

            foreach (var section in menu.Sections)
            {
                FindItemsByPath(section.Items, path, items);
            }

            return items;
        }

        private void FindItemsByPath(List<NavigationItem> items, string path, List<NavigationItem> result)
        {
            foreach (var item in items)
            {
                if (item.Href == path)
                    result.Add(item);

                if (item.Children != null)
                    FindItemsByPath(item.Children, path, result);
            }
        }

        public async Task UpdateNavigationItemAsync(string itemId, Action<NavigationItem> updateAction)
        {
            var item = await GetNavigationItemByIdAsync(itemId);
            if (item != null)
            {
                updateAction(item);
                NavigationItemChanged?.Invoke(this, new NavigationItemChangedEventArgs { Item = item });
            }
        }

        public async Task RefreshMenuAsync()
        {
            _cachedMenu = null;
            _cachedUserMenu = null;
            _cachedUserId = null;
            await GetNavigationMenuAsync();
            NavigationMenuRefreshed?.Invoke(this, EventArgs.Empty);
        }

        public async Task<List<NavigationItem>> GetBreadcrumbPathAsync(string currentUrl)
        {
            var menu = await GetNavigationMenuAsync();
            var path = new List<NavigationItem>();

            // Always start with home
            if (menu.HomeItem != null)
            {
                path.Add(menu.HomeItem);
            }

            // Find the current item and build path
            foreach (var section in menu.Sections)
            {
                if (BuildBreadcrumbPath(section.Items, currentUrl, path))
                    break;
            }

            return path;
        }

        private bool BuildBreadcrumbPath(List<NavigationItem> items, string targetUrl, List<NavigationItem> path)
        {
            foreach (var item in items)
            {
                if (item.Href == targetUrl)
                {
                    path.Add(item);
                    return true;
                }

                if (item.Children != null)
                {
                    path.Add(item);
                    if (BuildBreadcrumbPath(item.Children, targetUrl, path))
                        return true;
                    path.RemoveAt(path.Count - 1);
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