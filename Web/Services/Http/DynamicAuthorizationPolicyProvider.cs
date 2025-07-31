using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Web.Services.Interfaces;

namespace Web.Authorization
{
    /// <summary>
    /// Proveedor de políticas de autorización dinámicas para permisos basados en strings
    /// </summary>
    public class DynamicAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Primero intentar obtener una política predefinida
            var fallbackPolicy = _fallbackPolicyProvider.GetPolicyAsync(policyName);
            if (fallbackPolicy.Result != null)
            {
                return fallbackPolicy;
            }

            // Si no existe una política predefinida, crear una dinámicamente
            // basada en el nombre del permiso
            if (IsPermissionPolicy(policyName))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        private bool IsPermissionPolicy(string policyName)
        {
            // Verificar si el nombre de la política sigue el patrón de permisos
            // module.resource.action
            var parts = policyName.Split('.');
            return parts.Length >= 3;
        }
    }

    /// <summary>
    /// Requisito de autorización basado en permisos
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    /// <summary>
    /// Handler para verificar permisos
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IAuthService _authService;

        public PermissionAuthorizationHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                return;
            }

            // Obtener el projectId del contexto si está disponible
            Guid? projectId = null;
            if (context.Resource is Guid projectGuid)
            {
                projectId = projectGuid;
            }

            // Verificar si el usuario tiene el permiso
            var hasPermission = await _authService.HasPermissionAsync(projectId, requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}